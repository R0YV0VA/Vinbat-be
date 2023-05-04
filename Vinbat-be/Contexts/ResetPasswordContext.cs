using Microsoft.EntityFrameworkCore;
using Vinbat_be.Models;

namespace Vinbat_be.Contexts;

public class ResetPasswordContext : DbContext
{
    public ResetPasswordContext(DbContextOptions<ResetPasswordContext> options) : base(options)
    {
    }

    public DbSet<ResetPassword> ResetPassword { get; set; }
}
