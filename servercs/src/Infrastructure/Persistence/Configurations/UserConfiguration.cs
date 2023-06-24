using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NodaTime;
using Serenity.Domain.Entities;

namespace Serenity.Infrastructure.Persistence.Configurations;

public class UserConfiguration
{
    public void Configure(EntityTypeBuilder<ApplicationUser> config)
    {
        config.OwnsOne(o => o.PersonDetails).Property(o => o.Gender).HasColumnName("Gender");
        config.OwnsOne(o => o.PersonDetails).Property(o => o.Pronouns).HasColumnName("Pronouns");


        config.Property<Instant>("CreatedAt").HasDefaultValue(new Instant());
        config.Property<Instant>("UpdatedAt").HasDefaultValue(new Instant());
    }
}