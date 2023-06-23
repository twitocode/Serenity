using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Serenity.Api.Database.Entities;

namespace Serenity.Api.Database;

public class DataContext : IdentityDbContext<AppUser>
{

    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
    }
}