using BE.src.api.repositories;
using BE.src.api.services;
using DotNetEnv;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Moq.Protected;
using System.Net;
using System.Text;
using System.Text.Json;

namespace Service.UnitTests.Transactions;
public class CheckPaymentServiceTests
{
	private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
	private readonly HttpClient _httpClient;
	private readonly Mock<ITransactionRepo> _transactionRepoMock;
	private readonly Mock<IMembershipRepo> _membershipRepoMock;
	private readonly Mock<ICacheService> _cacheServiceMock;
	private readonly TransactionServ _transactionServ;
	private record ResponseDto(object Transaction, bool Status);
	public CheckPaymentServiceTests()
	{
		_httpMessageHandlerMock = new Mock<HttpMessageHandler>();
		_httpClient = new HttpClient(_httpMessageHandlerMock.Object);
		string envPath = Path.GetFullPath(Path.Combine(
			AppDomain.CurrentDomain.BaseDirectory, "../../../../../BE/.env"));

		Env.Load(envPath);

		_transactionRepoMock = new Mock<ITransactionRepo>();
		_membershipRepoMock = new Mock<IMembershipRepo>();
		_cacheServiceMock = new Mock<ICacheService>();
		_transactionServ = new TransactionServ(
			_transactionRepoMock.Object,
			_membershipRepoMock.Object,
			_cacheServiceMock.Object);
	}

	private void SetupHttpResponse(string url, HttpStatusCode statusCode, object responseContent)
	{
		var responseJson = JsonSerializer.Serialize(responseContent);
		_httpMessageHandlerMock
			.Protected()
			.Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(req => req.RequestUri.ToString() == url), ItExpr.IsAny<CancellationToken>())
			.ReturnsAsync(new HttpResponseMessage
			{
				StatusCode = statusCode,
				Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
			});
	}

	[Fact]
	public async Task CheckPaymentAsync_Should_ReturnSuccess_WhenTransactionContainsCode()
	{
		// Arrange
		var expectedCode = "TEST123";
		var url1 = "https://oauth.casso.vn/v2/transactions?sort=DESC&pageSize=3&page=1";
		var url2 = "https://oauth.casso.vn/v2/sync";

		SetupHttpResponse(url2, HttpStatusCode.OK, new { success = true });
		SetupHttpResponse(url1, HttpStatusCode.OK, new
		{
			Error = 0,
			Data = new
			{
				Records = new[]
				{
					new { Description = "Payment for TEST123" }
				}
			}
		});

		// Act
		var result = await _transactionServ.CheckPayment(expectedCode);

		// Assert
		var jsonResult = Assert.IsType<JsonResult>(result);
		Assert.NotNull(result);
		Assert.Equal(200, jsonResult.StatusCode);
	}

	[Fact]
	public async Task CheckPaymentAsync_Should_ReturnFailure_WhenTransactionDoesNotContainCode()
	{
		// Arrange
		var expectedCode = "TEST123";
		var url1 = "https://oauth.casso.vn/v2/transactions?sort=DESC&pageSize=3&page=1";
		var url2 = "https://oauth.casso.vn/v2/sync";

		SetupHttpResponse(url2, HttpStatusCode.OK, new { success = true });
		SetupHttpResponse(url1, HttpStatusCode.OK, new
		{
			Error = 0,
			Data = new
			{
				Records = new[]
				{
					new { Description = "Payment for OTHER" }
				}
			}
		});

		// Act
		var result = await _transactionServ.CheckPayment(expectedCode);

		// Assert
		Assert.NotNull(result);

		var jsonResult = Assert.IsType<JsonResult>(result);
		var json = JsonSerializer.Serialize(jsonResult.Value);
		var response = JsonSerializer.Deserialize<ResponseDto>(json);

		Assert.NotNull(response);
		Assert.False(response.Status);
	}

	[Fact]
	public async Task CheckPaymentAsync_Should_ReturnFailure_WhenNoTransactionsFound()
	{
		// Arrange
		var expectedCode = "TEST123";
		var url1 = "https://oauth.casso.vn/v2/transactions?sort=DESC&pageSize=3&page=1";
		var url2 = "https://oauth.casso.vn/v2/sync";

		SetupHttpResponse(url2, HttpStatusCode.OK, new { success = true });
		SetupHttpResponse(url1, HttpStatusCode.OK, new { Error = 0, Data = new { Records = new object[0] } });

		// Act
		var result = await _transactionServ.CheckPayment(expectedCode);

		// Assert
		Assert.NotNull(result);

		var jsonResult = Assert.IsType<JsonResult>(result);
		var json = JsonSerializer.Serialize(jsonResult.Value);
		var response = JsonSerializer.Deserialize<ResponseDto>(json);

		Assert.NotNull(response);
		Assert.False(response.Status);
	}

	[Fact]
	public async Task CheckPaymentAsync_Should_ReturnError_WhenApiFails()
	{
		// Arrange
		var expectedCode = "TEST123";
		var url1 = "https://oauth.casso.vn/v2/transactions?sort=DESC&pageSize=3&page=1";
		var url2 = "https://oauth.casso.vn/v2/sync";

		SetupHttpResponse(url2, HttpStatusCode.InternalServerError, new { error = "Sync failed" });
		SetupHttpResponse(url1, HttpStatusCode.InternalServerError, new { error = "API error" });

		// Act
		var result = await _transactionServ.CheckPayment(expectedCode);

		// Assert
		Assert.NotNull(result);

		var jsonResult = Assert.IsType<JsonResult>(result);
		var json = JsonSerializer.Serialize(jsonResult.Value);
		var response = JsonSerializer.Deserialize<ResponseDto>(json);

		Assert.NotNull(response);
		Assert.False(response.Status);
	}
}
