using BE.src.api.domains.Enum;
using BE.src.api.domains.Model.Base;

namespace BE.src.api.domains.Model
{
	public class MembershipUser : BaseEntity
	{
		public required Guid UserId { get; set; }
		public User User { get; set; } = null!;

		public required Guid MembershipId { get; set; }
		public Membership Membership { get; set; } = null!;

		public ICollection<Transaction> Transactions { get; set; } = null!;
	}
}
