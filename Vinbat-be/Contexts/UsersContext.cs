using Microsoft.EntityFrameworkCore;
using Vinbat_be.Models;

namespace Vinbat_be.Contexts;

public class UsersContext : DbContext
{
    public UsersContext(DbContextOptions<UsersContext> options) : base(options)
    {
    }
    public DbSet<User> Users { get; set; } = null!;
}
