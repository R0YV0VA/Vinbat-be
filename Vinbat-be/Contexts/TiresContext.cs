using Microsoft.EntityFrameworkCore;
using Vinbat_be.Models;

namespace Vinbat_be.Contexts;

public class TiresContext : DbContext
{
    public TiresContext(DbContextOptions<TiresContext> options) : base(options)
    {
    }
    public DbSet<Tires> Tires { get; set; } = null!;
}
