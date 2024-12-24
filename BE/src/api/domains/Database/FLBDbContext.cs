using BE.src.api.domains.Model;
using Microsoft.EntityFrameworkCore;
using DotNetEnv;

namespace BE.src.api.domains.Database
{
	public class FLBDbContext : DbContext
	{
		private readonly string _connectionString;
		public FLBDbContext(DbContextOptions<FLBDbContext> options)
				: base(options)
		{
			DotNetEnv.Env.Load();
			_connectionString = Environment.GetEnvironmentVariable("DEFAULT_CONNECTION")
				?? throw new InvalidOperationException("Connection string not found in environment variables.");
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.EnableSensitiveDataLogging();
			if (!optionsBuilder.IsConfigured)
			{
				optionsBuilder.UseMySql(_connectionString,
							new MySqlServerVersion(new Version(8, 0, 27)));
			}
		}

		public DbSet<User> Users { get; set; } = null!;
		public DbSet<Membership> Memberships { get; set; } = null!;
		public DbSet<MembershipUser> MemberUsers { get; set; } = null!;
		public DbSet<PostJob> PostJobs { get; set; } = null!;
		public DbSet<UserApply> UserApplies { get; set; } = null!;
		public DbSet<Specialty> Specialties { get; set; } = null!;
		public DbSet<Attachment> Attachments { get; set; } = null!;
		public DbSet<ImageVideo> ImageVideos { get; set; } = null!;
		public DbSet<Shot> Shots { get; set; } = null!;
		public DbSet<ViewAnalyst> ViewAnalysts { get; set; } = null!;
		public DbSet<Like> Likes { get; set; } = null!;
		public DbSet<Save> Saves { get; set; } = null!;
		public DbSet<Follow> Follows { get; set; } = null!;
		public DbSet<Comment> Comments { get; set; } = null!;
		public DbSet<Communication> Communications { get; set; } = null!;
		public DbSet<Message> Messages { get; set; } = null!;
		public DbSet<SocialProfile> SocialProfiles { get; set; } = null!;

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);

			builder.Entity<Attachment>(entity =>
			{
				entity.HasKey(a => a.Id);

				entity.HasOne(a => a.Post)
					.WithMany(p => p.Attachments)
					.HasForeignKey(p => p.PostId)
					.OnDelete(DeleteBehavior.Cascade);

				entity.Property(p => p.File)
				.IsRequired();
			});

			builder.Entity<Comment>(entity =>
			{
				entity.HasKey(c => c.Id);

				entity.HasOne(c => c.Shot)
					.WithMany(s => s.Comments)
					.HasForeignKey(c => c.ShotId)
					.OnDelete(DeleteBehavior.Cascade);

				entity.HasOne(c => c.User)
					.WithMany(u => u.Comments)
					.HasForeignKey(c => c.UserId)
					.OnDelete(DeleteBehavior.Cascade);

				entity.Property(c => c.Description)
					.IsRequired()
					.HasMaxLength(1000);
			});

			builder.Entity<Communication>(entity =>
			{
				entity.HasKey(c => c.Id);

				entity.HasOne(c => c.Zero)
					.WithMany(u => u.Communications)
					.HasForeignKey(c => c.ZeroId)
					.OnDelete(DeleteBehavior.Restrict);

				entity.HasOne(c => c.First)
					.WithMany(u => u.Communications)
					.HasForeignKey(c => c.FirstId)
					.OnDelete(DeleteBehavior.Restrict);
			});

			builder.Entity<Follow>(entity =>
			{
				entity.HasKey(f => f.Id);

				entity.HasOne(f => f.Following)
					.WithMany(u => u.Followers)
					.HasForeignKey(f => f.FollowingId)
					.OnDelete(DeleteBehavior.Restrict);

				entity.HasOne(f => f.Followed)
					.WithMany(u => u.Followings)
					.HasForeignKey(f => f.FollowedId)
					.OnDelete(DeleteBehavior.Restrict);
			});

			builder.Entity<ImageVideo>(entity =>
			{
				entity.HasKey(iv => iv.Id);

				entity.Property(iv => iv.Type)
					.IsRequired();

				entity.Property(iv => iv.Url)
					.IsRequired();

				entity.HasOne(iv => iv.Shot)
					.WithMany()
					.HasForeignKey(iv => iv.ShotId)
					.OnDelete(DeleteBehavior.Restrict);

				entity.HasOne(iv => iv.User)
					.WithMany()
					.HasForeignKey(iv => iv.UserId)
					.OnDelete(DeleteBehavior.Restrict);

				entity.HasOne(iv => iv.Post)
					.WithOne(p => p.CompanyLogo)
					.HasForeignKey<PostJob>(p => p.CompanyLogoId);
			});

			builder.Entity<Like>(entity =>
			{
				entity.HasKey(l => l.Id);

				entity.HasOne(l => l.Shot)
					.WithMany(s => s.Likes)
					.HasForeignKey(l => l.ShotId)
					.OnDelete(DeleteBehavior.Restrict);

				entity.HasOne(l => l.User)
					.WithMany(u => u.Likes)
					.HasForeignKey(l => l.UserId)
					.OnDelete(DeleteBehavior.Restrict);
			});

			builder.Entity<Membership>(entity =>
			{
				entity.HasKey(m => m.Id);

				entity.Property(m => m.Name)
					.IsRequired();

				entity.Property(m => m.Price)
					.IsRequired();

				entity.Property(m => m.ExpireTime)
					.IsRequired();

				entity.Property(m => m.Description)
					.IsRequired();
			});

			builder.Entity<MembershipUser>(entity =>
			{
				entity.HasKey(mu => mu.Id);

				entity.HasOne(mu => mu.User)
					.WithMany(u => u.MembershipUsers)
					.HasForeignKey(mu => mu.UserId)
					.OnDelete(DeleteBehavior.Restrict);

				entity.HasOne(mu => mu.Membership)
					.WithMany(m => m.MembershipUsers)
					.HasForeignKey(mu => mu.MembershipId)
					.OnDelete(DeleteBehavior.Restrict);
			});

			builder.Entity<Message>(entity =>
			{
				entity.HasKey(m => m.Id);

				entity.Property(m => m.PersonIndex)
					.IsRequired();

				entity.Property(m => m.Content)
					.IsRequired();

				entity.Property(m => m.Index)
					.IsRequired();

				entity.HasOne(m => m.Communication)
					.WithMany(c => c.Messages)
					.HasForeignKey(m => m.CommunicationId)
					.OnDelete(DeleteBehavior.Restrict);
			});

			builder.Entity<PostJob>(entity =>
			{
				entity.HasKey(p => p.Id);

				entity.Property(p => p.Title)
					.IsRequired();

				entity.Property(p => p.Description)
					.IsRequired();

				entity.Property(p => p.WorkLocation)
					.IsRequired();

				entity.Property(p => p.CompanyName)
					.IsRequired();

				entity.Property(p => p.Experience)
					.IsRequired();

				entity.Property(p => p.WorkType)
					.IsRequired();

				entity.Property(p => p.EmploymentType)
					.IsRequired();

				entity.HasOne(p => p.User)
					.WithMany(u => u.Posts)
					.HasForeignKey(p => p.UserId)
					.OnDelete(DeleteBehavior.Restrict);

				entity.HasOne(p => p.CompanyLogo)
					.WithMany()
					.HasForeignKey(p => p.CompanyLogoId)
					.OnDelete(DeleteBehavior.Restrict);

				entity.HasOne(p => p.Specialty)
					.WithMany()
					.HasForeignKey(p => p.SpecialtyId)
					.OnDelete(DeleteBehavior.Restrict);
			});

			builder.Entity<Save>(entity =>
			{
				entity.HasKey(s => s.Id);

				entity.Property(s => s.UserId)
					.IsRequired();

				entity.HasOne(s => s.User)
					.WithMany(u => u.Saves)
					.HasForeignKey(s => s.UserId)
					.OnDelete(DeleteBehavior.Restrict);

				entity.HasOne(s => s.Post)
					.WithMany(p => p.Saves)
					.HasForeignKey(s => s.PostId)
					.OnDelete(DeleteBehavior.Restrict);

				entity.HasOne(s => s.Shot)
					.WithMany()
					.HasForeignKey(s => s.ShotId)
					.OnDelete(DeleteBehavior.Restrict);
			});

			builder.Entity<Shot>(entity =>
			{
				entity.HasKey(s => s.Id);

				entity.Property(s => s.UserId)
					.IsRequired();

				entity.Property(s => s.SpecialtyId)
					.IsRequired();

				entity.Property(s => s.Html)
					.IsRequired()
					.HasMaxLength(5000);

				entity.Property(s => s.Css)
					.IsRequired()
					.HasMaxLength(5000);

				entity.Property(s => s.View)
					.IsRequired();

				entity.HasOne(s => s.User)
					.WithMany(u => u.Shots)
					.HasForeignKey(s => s.UserId)
					.OnDelete(DeleteBehavior.Restrict);

				entity.HasOne(s => s.Specialty)
					.WithMany()
					.HasForeignKey(s => s.SpecialtyId)
					.OnDelete(DeleteBehavior.Restrict);
			});

			builder.Entity<SocialProfile>(entity =>
			{
				entity.HasKey(sp => sp.Id);

				entity.Property(sp => sp.Type)
					.IsRequired();

				entity.Property(sp => sp.Linked)
					.IsRequired()
					.HasMaxLength(500);

				entity.Property(sp => sp.UserId)
					.IsRequired();

				entity.HasOne(sp => sp.User)
					.WithMany(u => u.SocialProfiles)
					.HasForeignKey(sp => sp.UserId)
					.OnDelete(DeleteBehavior.Restrict);
			});

			builder.Entity<Specialty>(entity =>
			{
				entity.HasKey(s => s.Id);

				entity.Property(s => s.Name)
					.IsRequired()
					.HasMaxLength(100);
			});

			builder.Entity<User>(entity =>
			{
				entity.HasKey(u => u.Id);

				entity.Property(u => u.Name)
					.IsRequired()
					.HasMaxLength(100);

				entity.Property(u => u.Email)
					.IsRequired()
					.HasMaxLength(200);

				entity.Property(u => u.Password)
					.IsRequired()
					.HasMaxLength(100);

				entity.Property(u => u.Phone)
					.HasMaxLength(15);

				entity.Property(u => u.City)
					.HasMaxLength(100);

				entity.Property(u => u.Education)
					.HasMaxLength(100);

				entity.Property(u => u.Description)
					.HasMaxLength(500);
			});

			builder.Entity<UserApply>(entity =>
			{
				entity.HasKey(ua => ua.Id);

				entity.HasOne(ua => ua.User)
					.WithMany(u => u.UserApplies)
					.HasForeignKey(ua => ua.UserId)
					.OnDelete(DeleteBehavior.Cascade);

				entity.HasOne(ua => ua.Post)
					.WithMany(p => p.UserApplies)
					.HasForeignKey(ua => ua.PostId)
					.OnDelete(DeleteBehavior.Cascade);
			});

			builder.Entity<ViewAnalyst>(entity =>
			{
				entity.HasKey(va => va.Id);

				entity.HasOne(va => va.Shot)
					.WithMany(s => s.ViewAnalysts)
					.HasForeignKey(va => va.ShotId)
					.OnDelete(DeleteBehavior.Cascade);
			});
		}
	}
}