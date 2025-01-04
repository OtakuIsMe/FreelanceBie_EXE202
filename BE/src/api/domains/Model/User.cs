using System.Collections;
using System.ComponentModel.DataAnnotations;
using Azure.Storage.Blobs.Models;
using BE.src.api.domains.Enum;
using BE.src.api.domains.Model.Base;

namespace BE.src.api.domains.Model
{
	public class User : BaseEntity
	{
		public string? Name { get; set; } = null!;
		public required string Username { get; set; }
		public required string Email { get; set; }
		public required string Password { get; set; }
		public required RoleEnum Role { get; set; }
		public string? Phone { get; set; } = null!;
		public string? City { get; set; } = null!;
		public string? Education { get; set; } = null!;
		public string? Description { get; set; } = null!;
		public DateOnly? DOB { get; set; } = null;

		public ICollection<Experience> Experiences { get; set; } = null!;
		public ICollection<SocialProfile> SocialProfiles { get; set; } = null!;
		public ICollection<Comment> Comments { get; set; } = null!;
		public ICollection<Communication> Zeros { get; set; } = null!;
		public ICollection<Communication> Firsts { get; set; } = null!;
		public ICollection<PostJob> Posts { get; set; } = null!;
		public ICollection<Like> Likes { get; set; } = null!;
		public ICollection<MembershipUser> MembershipUsers { get; set; } = null!;
		public ICollection<UserApply> UserApplies { get; set; } = null!;
		public ICollection<Save> Saves { get; set; } = null!;
		public ICollection<Shot> Shots { get; set; } = null!;
		public ICollection<Follow> Followers { get; set; } = null!;
		public ICollection<Follow> Followings { get; set; } = null!;
		public ICollection<ImageVideo> ImageVideos { get; set; } = null!;
		public ICollection<Notification> Notifications { get; set; } = null!;
	}
}
