using BE.src.api.domains.Model.Base;

namespace BE.src.api.domains.Model
{
    public class Notification : BaseEntity
    {
        public required string Title { get; set; }
        public required string Message { get; set; }
        public required Guid UserId { get; set; }      
        public User User { get; set; } = null!;
    }
}