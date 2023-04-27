using Microsoft.EntityFrameworkCore;
using Vinbat_be.Models;

namespace Vinbat_be.Contexts;

public class CasesContext : DbContext
{
    public CasesContext(DbContextOptions<CasesContext> options) : base(options)
    {
    }
    public DbSet<Case> Cases { get; set; } = null!;
}
