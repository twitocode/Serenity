using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NodaTime;
using Serenity.Domain.Common;
using Serenity.Domain.Entities;

namespace Serenity.Infrastructure.Persistence.Configurations;

public class FeelingConfiguration : IEntityTypeConfiguration<Feeling>
{
    public void Configure(EntityTypeBuilder<Feeling> config)
    {
        config.Property<Instant>("CreatedAt").HasDefaultValue(new Instant());
        config.Property<Instant>("UpdatedAt").HasDefaultValue(new Instant());
    }
}