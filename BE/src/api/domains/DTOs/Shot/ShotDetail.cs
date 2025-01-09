namespace BE.src.api.domains.DTOs.Shot
{
    public class ShotDetail
    {
        public required string Title { get; set; }
        public required string Html { get; set; }
        public required string Css { get; set; }
        public required ShotOwner Owner { get; set; }
        public ShotUser? User { get; set; }
    }

    public class ShotOwner
    {
        public required string Image { get; set; }
        public required string Name { get; set; }
        public required string Status { get; set; }
        public required string Slogan { get; set; }
    }

    public class ShotUser
    {
        public required bool IsLiked { get; set; }
        public required bool IsSaved { get; set; }
    }
}