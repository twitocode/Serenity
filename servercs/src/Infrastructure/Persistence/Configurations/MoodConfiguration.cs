using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NodaTime;
using Serenity.Domain.Common;
using Serenity.Domain.Entities;

namespace Serenity.Infrastructure.Persistence.Configurations;

public class MoodConfiguration : IEntityTypeConfiguration<Mood>
{
    public void Configure(EntityTypeBuilder<Mood> config)
    {
        config.Property<Instant>("CreatedAt").HasDefaultValue(new Instant());
        config.Property<Instant>("UpdatedAt").HasDefaultValue(new Instant());
    }
}