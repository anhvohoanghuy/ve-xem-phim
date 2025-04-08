using System.Reflection.Emit;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ve_xem_phim.Models
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        private readonly IConfiguration _configuration;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IConfiguration configuration) : base(options)
        {
            _configuration = configuration;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<User>()
                .HasOne(c => c.Cart)
                .WithOne(c => c.User)
                .HasForeignKey<Cart>(c => c.UserId)
                .IsRequired();
            builder.Entity<Ticket>()
               .HasOne(t => t.Movie)
               .WithMany(m => m.Tickets)
               .HasForeignKey(t => t.MovieId)
               .OnDelete(DeleteBehavior.Cascade); // Có thể giữ cascade nếu chỉ 1 quan hệ

            builder.Entity<Ticket>()
                .HasOne(t => t.Promotion)
                .WithMany(p => p.Tickets)
                .HasForeignKey(t => t.PromotionId)
                .OnDelete(DeleteBehavior.SetNull); // ✅ Tránh gây multiple cascade path
        }

        public DbSet<Movie> Movies { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Promotion> Promotions { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<Category> Categories { get; set; }
    }
}
