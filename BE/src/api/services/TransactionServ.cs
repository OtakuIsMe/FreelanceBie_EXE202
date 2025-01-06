using BE.src.api.domains.Model;
using BE.src.api.repositories;
using BE.src.api.shared.Constant;
using BE.src.api.shared.Type;
using Microsoft.AspNetCore.Mvc;
using PayPal.Api;
using PaypalTransaction = PayPal.Api.Transaction;
using MyTransaction = BE.src.api.domains.Model.Transaction;
using BE.src.api.domains.Enum;

namespace BE.src.api.services
{
    public interface ITransactionServ
    {
        Task<IActionResult> BuyMembership(Guid membershipId, Guid userId);
        Task<IActionResult> PaymentMembershipSuccess(Guid membershipId, Guid userId, string paymentId, string PayerID);
        Task<IActionResult> ViewTransactions(Guid userId);
    }
    public class TransactionServ : ITransactionServ
    {
        private readonly ITransactionRepo _transactionRepo;
        private readonly IMembershipRepo _membershipRepo;
        public TransactionServ(ITransactionRepo transactionRepo, IMembershipRepo membershipRepo)
        {
            _transactionRepo = transactionRepo;
            _membershipRepo = membershipRepo;
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
                
                var newMembershipUser = new MembershipUser
                {
                    MembershipId = membershipId,
                    UserId = userId,
                    CreateAt = DateTime.Now,
                    UpdateAt = DateTime.Now
                };

                var isPurchase = await _membershipRepo.AddMembershipUser(newMembershipUser);
                if (!isPurchase)
                {
                    return ErrorResp.BadRequest("Purchase failed");
                }

                var transaction = new MyTransaction
                {
                    MemberUserId = newMembershipUser.Id,
                    Total = membership.Price,
                    Status = TransactionStatusEnum.Completed
                };

                var isCreatedTrasaction = await _transactionRepo.CreateTransaction(transaction);
                if (!isCreatedTrasaction)
                {
                    return ErrorResp.BadRequest("Cant create transaction");
                }
                return SuccessResp.Ok(new {Redirect = "http://localhost:5173/"});
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
                var transactions = await _transactionRepo.GetTransactions(userId);
                if(transactions.Count == 0)
                {
                    return ErrorResp.NotFound("No transaction found");
                }
                return SuccessResp.Ok(transactions);
            }
            catch (System.Exception ex)
            {
                return ErrorResp.BadRequest(ex.Message);
            }
		}
	}
}