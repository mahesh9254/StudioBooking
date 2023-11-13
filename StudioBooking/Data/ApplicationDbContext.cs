using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudioBooking.Data.Models;
using System.Reflection.Emit;

namespace StudioBooking.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<ServicePrice> ServicePrices { get; set; }
        public DbSet<ServiceGallery> ServiceGallery { get; set; }
        public DbSet<CategoryDetail> CategoryDetails { get; set; }
        public DbSet<CategoryGear> CategoryGears { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<WebsiteSetting> WebsiteSettings { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Testimonial> Testimonials { get; set; }
        public DbSet<PaymentGatewayResponse> PaymentGatewayResponses { get; set; }
        public DbSet<ScheduleRequest> ScheduleRequests { get; set; }
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<WalletTransaction> WalletTransactions { get; set; }
        public DbSet<PaymentReceipt> PaymentReceipts { get; set; }
        public DbSet<Addon> Addons { get; set; }
        public DbSet<CustomerAddress> CustomerAddresses { get; set; }
        public DbSet<Coupon> Coupons { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            //builder.HasDefaultSchema("StudioBooking");
            builder.Entity<ApplicationUser>(entity =>
            {
                entity.ToTable(name: "User");
            });
            builder.Entity<IdentityRole>(entity =>
            {
                entity.ToTable(name: "Role");
            });
            builder.Entity<IdentityUserRole<string>>(entity =>
            {
                entity.ToTable("UserRoles");
            });
            builder.Entity<IdentityUserClaim<string>>(entity =>
            {
                entity.ToTable("UserClaims");
            });
            builder.Entity<IdentityUserLogin<string>>(entity =>
            {
                entity.ToTable("UserLogins");
            });
            builder.Entity<IdentityRoleClaim<string>>(entity =>
            {
                entity.ToTable("RoleClaims");
            });
            builder.Entity<IdentityUserToken<string>>(entity =>
            {
                entity.ToTable("UserTokens");
            });            
            builder.Entity<Booking>()
                .Property(p => p.RatePerHour)
                .HasColumnType("decimal(18,4)");
            builder.Entity<Booking>()
                .Property(p => p.Total)
                .HasColumnType("decimal(18,4)");
            builder.Entity<Transaction>()
                .Property(p => p.Amount)
                .HasColumnType("decimal(18,4)");
            builder.Entity<Coupon>()
                .Property(p => p.Discount)
                .HasColumnType("decimal(18,4)");
            builder.ApplyConfiguration(new ApplicationUserEntityConfiguration());
        }
    }

    public class ApplicationUserEntityConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.Property(u => u.FirstName).HasMaxLength(200);
            builder.Property(u => u.LastName).HasMaxLength(200);
            builder.Property(u => u.ProfileImageUrl).HasMaxLength(250);
            builder.Property(u => u.CreatedBy).HasMaxLength(50);
            builder.Property(u => u.ModifiedBy).HasMaxLength(50);
        }
    }
}