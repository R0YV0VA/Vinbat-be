using Microsoft.EntityFrameworkCore;
using Vinbat_be.Models;

namespace Vinbat_be.Contexts;

public class BatteriesContext : DbContext
{
    public BatteriesContext(DbContextOptions<BatteriesContext> options) : base(options)
    {
    }
    public DbSet<Battery> Batteries { get; set; } = null!;
}
