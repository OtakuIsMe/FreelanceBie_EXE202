using BE.src.api.domains.Enum;
using BE.src.api.domains.Model.Base;

namespace BE.src.api.domains.Model
{
    public class Transaction : BaseEntity
    {
        public required Guid MemberUserId { get; set; }
        public MembershipUser MemberUser { get; set; } = null!;

        public float Total { get; set; }
        public TransactionStatusEnum Status { get; set; }

        public string PaymentId { get; set; } = null!;
    }
}