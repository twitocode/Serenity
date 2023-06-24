using System.Reflection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Serenity.Domain.Entities;
using Serenity.Infrastructure.Persistence.Configurations;

namespace Serenity.Infrastructure.Persistence;

public class DataContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}