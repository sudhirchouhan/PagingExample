using Microsoft.EntityFrameworkCore;

namespace PagingExample.Models
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions options):base(options)
        {
            
        }
        public DbSet<Item> Items { get; set; }
    }
}
