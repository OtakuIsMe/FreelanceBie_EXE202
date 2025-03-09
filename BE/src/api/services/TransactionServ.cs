using BE.src.api.domains.Model;
using BE.src.api.repositories;
using BE.src.api.shared.Constant;
using BE.src.api.shared.Type;
using Microsoft.AspNetCore.Mvc;
using PayPal.Api;
using PaypalTransaction = PayPal.Api.Transaction;
using MyTransaction = BE.src.api.domains.Model.Transaction;
using BE.src.api.domains.Enum;
using System.Net.Http;
using System.Text.Json;
using BE.src.api.domains.DTOs.Transaction;
using System.Text;

namespace BE.src.api.services
{
	public interface ITransactionServ
	{
		Task<IActionResult> BuyMembership(Guid membershipId, Guid userId);
		Task<IActionResult> PaymentMembershipSuccess(Guid membershipId, Guid userId, string paymentId, string PayerID);
		Task<IActionResult> ViewTransactions(Guid userId);
		Task<IActionResult> CheckPayment(string code);
	}
	public class TransactionServ : ITransactionServ
	{
		private readonly ITransactionRepo _transactionRepo;
		private readonly IMembershipRepo _membershipRepo;
		private readonly ICacheService _cacheService;
		public TransactionServ(ITransactionRepo transactionRepo, IMembershipRepo membershipRepo, ICacheService cacheService)
		{
			_transactionRepo = transactionRepo;
			_membershipRepo = membershipRepo;
			_cacheService = cacheService;
		}

		private APIContext GetAPIContext()
		{
			var config = new Dictionary<string, string>
			{
				{ "mode", Paypal.Mode }
			};

			var clientId = Paypal.ClientId;
			var clientSecret = Paypal.Secret;

			try
			{
				var accessToken = new OAuthTokenCredential(clientId, clientSecret, config).GetAccessToken();
				return new APIContext(accessToken) { Config = config };
			}
			catch (Exception ex)
			{
				throw new Exception($"PayPal API initialization failed: {ex.Message}", ex);
			}
		}

		private Payment CreatePayment(float total, string returnUrl, string cancelUrl)
		{
			var apiContext = GetAPIContext();

			decimal newTotal = Math.Round((decimal)total / 24850, 2);

			var payment = new Payment
			{
				intent = "sale",
				payer = new Payer { payment_method = "paypal" },
				redirect_urls = new RedirectUrls
				{
					cancel_url = cancelUrl,
					return_url = returnUrl
				},
				transactions = new List<PaypalTransaction>
				{
					new PaypalTransaction
					{
						description = "Transaction description",
						invoice_number = Guid.NewGuid().ToString(),
						amount = new Amount
						{
							currency = "USD",
							total = newTotal.ToString("F2")
						}
					}
				}
			};

			return payment.Create(apiContext);
		}

		public async Task<IActionResult> BuyMembership(Guid membershipId, Guid userId)
		{
			try
			{
				var membership = await _membershipRepo.GetMembershipById(membershipId);
				if (membership == null)
				{
					return ErrorResp.NotFound("Membership not found");
				}

				string return_url = $"http://localhost:5147/transaction/payment-membership-success?membershipId={membershipId}&userId={userId}";
				string cancel_url = "http://localhost:5173/";
				var payment = CreatePayment(membership.Price, return_url, cancel_url);
				var approvalUrl = payment.links.FirstOrDefault(lnk => lnk.rel.Equals("approval_url", StringComparison.OrdinalIgnoreCase))?.href;
				return SuccessResp.Ok(approvalUrl);
			}
			catch (System.Exception ex)
			{
				return ErrorResp.BadRequest(ex.Message);
			}
		}

		public async Task<IActionResult> PaymentMembershipSuccess(Guid membershipId, Guid userId, string paymentId, string PayerID)
		{
			try
			{
				if (membershipId == Guid.Empty || userId == Guid.Empty ||
					string.IsNullOrEmpty(paymentId) || string.IsNullOrEmpty(PayerID))
				{
					return ErrorResp.BadRequest("Invalid input parameters.");
				}

				var membership = await _membershipRepo.GetMembershipById(membershipId);
				if (membership == null)
				{
					return ErrorResp.NotFound("Cant find membership");
				}

				var paymentExecution = new PaymentExecution { payer_id = PayerID };
				var payment = new PayPal.Api.Payment() { id = paymentId };
				var executedPayment = payment.Execute(GetAPIContext(), paymentExecution);

				if (executedPayment.state.ToLower() != "approved")
				{
					return ErrorResp.BadRequest("Payment was not approved.");
				}

				var membershipUser = await _membershipRepo.GetMembershipUserRegistered(userId);
				bool operationResult;

				if (membershipUser != null)
				{
					membershipUser.MembershipId = membershipId;
					membershipUser.UpdateAt = DateTime.Now;
					operationResult = await _membershipRepo.UpdateMembershipUser(membershipUser);
				}
				else
				{
					membershipUser = new MembershipUser
					{
						MembershipId = membershipId,
						UserId = userId,
						CreateAt = DateTime.Now,
						UpdateAt = DateTime.Now
					};
					operationResult = await _membershipRepo.AddMembershipUser(membershipUser);
				}

				if (!operationResult)
				{
					return ErrorResp.BadRequest("Failed to process membership user.");
				}

				var transaction = new MyTransaction
				{
					MemberUserId = membershipUser.Id,
					Total = membership.Price,
					Status = TransactionStatusEnum.Completed,
					PaymentId = paymentId,
					CreateAt = DateTime.Now
				};

				var isTransactionCreated = await _transactionRepo.CreateTransaction(transaction);
				if (!isTransactionCreated)
				{
					return ErrorResp.BadRequest("Failed to create transaction.");
				}

				return SuccessResp.Ok(new { Redirect = Environment.GetEnvironmentVariable("FRONTEND_REDIRECT_URL") ?? "http://localhost:5173/" });
			}
			catch (System.Exception ex)
			{
				return ErrorResp.BadRequest(ex.Message);
			}
		}

		public async Task<IActionResult> ViewTransactions(Guid userId)
		{
			try
			{
				var key = $"transactions:{userId}";

				var cacheTransactions = await _cacheService.Get<List<MyTransaction>>(key);
				if (cacheTransactions != null)
					return SuccessResp.Ok(cacheTransactions);

				var transactions = await _transactionRepo.GetTransactions(userId);
				if (transactions.Count == 0)
				{
					return ErrorResp.NotFound("No transaction found");
				}

				await _cacheService.Set(key, transactions, TimeSpan.FromMinutes(10));

				return SuccessResp.Ok(transactions);
			}
			catch (System.Exception ex)
			{
				return ErrorResp.BadRequest(ex.Message);
			}
		}

		public async Task<IActionResult> CheckPayment(string code)
		{
			try
			{
				string url1 = "https://oauth.casso.vn/v2/transactions?sort=DESC&pageSize=3&page=1";
				string apiKey = Environment.GetEnvironmentVariable("CASSO_API_KEY");
				string url2 = "https://oauth.casso.vn/v2/sync";

				using (HttpClient httpClient = new HttpClient())
				{
					httpClient.DefaultRequestHeaders.Add("Authorization", $"Apikey {apiKey}");
					var syncBody = new { bank_acc_id = "1026347842" };
					var syncContent = new StringContent(JsonSerializer.Serialize(syncBody), Encoding.UTF8, "application/json");
					HttpResponseMessage syncResponse = await httpClient.PostAsync(url2, syncContent);
					Console.WriteLine(syncResponse);
					await Task.Delay(2000);

					HttpResponseMessage response = await httpClient.GetAsync(url1);
					string jsonData = await response.Content.ReadAsStringAsync();
					var result = JsonSerializer.Deserialize<CassoDto>(jsonData, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

					if (result?.Data?.Records != null && result.Data.Records.Any(item => item.Description.Contains(code)))
					{
						return SuccessResp.Ok(new { trasaction = result, status = true });
					}

					return SuccessResp.Ok(new { trasaction = result, status = false });
				}
			}
			catch (System.Exception ex)
			{
				return ErrorResp.BadRequest(ex.Message);
			}
		}
	}
}
