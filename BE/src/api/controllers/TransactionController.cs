using BE.src.api.domains.DTOs.Membership;
using BE.src.api.services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BE.src.api.controllers
{
    [ApiController]
    [Route("api/transaction")]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionServ _transactionServ;
        public TransactionController(ITransactionServ transactionServ)
        {
            _transactionServ = transactionServ;
        }
        [Authorize(Policy = "Customer")]
        [HttpPost("buy-membership")]
        public async Task<IActionResult> BuyMembership([FromForm] Guid membershipId)
        {
            Guid userId = Guid.Parse(User.Claims.First(u => u.Type == "userId").Value);
            return await _transactionServ.BuyMembership(membershipId, userId);
        }
        [Authorize(Policy = "Customer")]
        [HttpGet("payment-membership-success")]
        public async Task<IActionResult> PaymentMembershipSuccess([FromQuery] Guid membershipId, [FromQuery] string paymentId, [FromQuery] string PayerID)
        {
            Guid userId = Guid.Parse(User.Claims.First(u => u.Type == "userId").Value);
            return await _transactionServ.PaymentMembershipSuccess(membershipId, userId, paymentId, PayerID);
        }
        [Authorize(Policy = "Customer")]
        [HttpGet("view-transactions")]
        public async Task<IActionResult> ViewTransactions()
        {
            Guid userId = Guid.Parse(User.Claims.First(u => u.Type == "userId").Value);
            return await _transactionServ.ViewTransactions(userId);
        }
    }
}