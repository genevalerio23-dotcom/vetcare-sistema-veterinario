using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace VetCare.Infrastructure.Data;

public class VetCareIdentityDbContext
    : IdentityDbContext<IdentityUser>
{
    public VetCareIdentityDbContext(
        DbContextOptions<VetCareIdentityDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
    }
}
