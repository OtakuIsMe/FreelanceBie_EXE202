using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using BE.src.api.domains.DTOs.Payment;

[Route("api/v1/payment")]
[ApiController]
public class PaymentController : ControllerBase
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public PaymentController(IHttpClientFactory httpClientFactory, string apiKey)
    {
        _httpClient = httpClientFactory.CreateClient();
        _apiKey = apiKey;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreatePayment([FromBody] Payment request)
    {
        var paymentData = new
        {
            amount = request.Amount,
            orderCode = request.OrderId,
            description = $"Thanh toán đơn hàng {request.OrderId}",
            returnUrl = "http://localhost:5173/pro/check-out",
            cancelUrl = "http://localhost:5173/pro/check-out"
        };

        var content = new StringContent(JsonSerializer.Serialize(paymentData), Encoding.UTF8, "application/json");
        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _apiKey);

        var response = await _httpClient.PostAsync("https://api.payos.vn/v2/payment-requests", content);
        if (!response.IsSuccessStatusCode)
        {
            return BadRequest(await response.Content.ReadAsStringAsync());
        }

        var responseData = await response.Content.ReadAsStringAsync();
        return Ok(responseData);
    }
}
