using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using YongXing_Project.Models;

namespace YongXing_Project.Data
{
    public class BookingContext : IdentityDbContext<IdentityUser>
    {
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Comment> Comments { get; set; }

        public BookingContext(DbContextOptions<BookingContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

        }
    }
}
