using BE.src.api.domains.DTOs.Membership;
using BE.src.api.services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BE.src.api.controllers
{
    [ApiController]
    [Route("api/v1/transaction")]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionServ _transactionServ;
        private readonly ILogger<TransactionController> _logger;
        public TransactionController(ITransactionServ transactionServ, ILogger<TransactionController> logger)
        {
            _transactionServ = transactionServ;
            _logger = logger;
        }
        [Authorize(Policy = "Customer")]
        [HttpPost("buy-membership")]
        public async Task<IActionResult> BuyMembership([FromForm] Guid membershipId)
        {
            _logger.LogInformation("BuyMembership");
            Guid userId = Guid.Parse(User.Claims.First(u => u.Type == "userId").Value);
            return await _transactionServ.BuyMembership(membershipId, userId);
        }
        [Authorize(Policy = "Customer")]
        [HttpGet("payment-membership-success")]
        public async Task<IActionResult> PaymentMembershipSuccess([FromQuery] Guid membershipId, [FromQuery] string paymentId, [FromQuery] string PayerID)
        {
            _logger.LogInformation("PaymentMembershipSuccess");
            Guid userId = Guid.Parse(User.Claims.First(u => u.Type == "userId").Value);
            return await _transactionServ.PaymentMembershipSuccess(membershipId, userId, paymentId, PayerID);
        }
        [Authorize(Policy = "Customer")]
        [HttpGet("view-transactions")]
        public async Task<IActionResult> ViewTransactions()
        {
            _logger.LogInformation("ViewTransactions");
            Guid userId = Guid.Parse(User.Claims.First(u => u.Type == "userId").Value);
            return await _transactionServ.ViewTransactions(userId);
        }
        [HttpGet("Check-Payment")]
        public async Task<IActionResult> CheckPayment([FromQuery] string code)
        {
            return await _transactionServ.CheckPayment(code);
        }
    }
}
