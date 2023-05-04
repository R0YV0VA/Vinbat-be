using Microsoft.EntityFrameworkCore;
using Vinbat_be.Models;

namespace Vinbat_be.Contexts;

public class ConfirmUsersContext : DbContext
{
    public ConfirmUsersContext(DbContextOptions<ConfirmUsersContext> options) : base(options)
    { 
    }
    public DbSet<ConfirmUsers> ConfirmUsers { get; set; } = null!;
}
