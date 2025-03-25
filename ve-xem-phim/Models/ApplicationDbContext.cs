using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ve_xem_phim.Models
{
    public class ApplicationDbContext: IdentityDbContext<User>
    {
        private readonly IConfiguration _configuration;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IConfiguration configuration) : base(options)
        {
            _configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(_configuration.GetConnectionString("DefaultConnection"));
            }
        }

        public DbSet<Movie> Movies { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Promotion> Promotions { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
    }
}
