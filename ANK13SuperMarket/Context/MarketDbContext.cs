using ANK13SuperMarket.Entities;
using Microsoft.EntityFrameworkCore;

namespace ANK13SuperMarket.Context
{
    public class MarketDbContext : DbContext
    {
        public MarketDbContext(DbContextOptions<MarketDbContext> options) : base(options)
        {

        }

        public DbSet<Product> Products { get; set; }
    }
}
