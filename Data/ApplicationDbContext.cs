using Microsoft.EntityFrameworkCore;
using CoffeeShopMVC.Models;

namespace CoffeeShopMVC.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<TeamMember> TeamMembers { get; set; }
    }
}