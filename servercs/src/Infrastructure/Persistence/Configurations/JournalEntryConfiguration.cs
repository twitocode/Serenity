using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NodaTime;
using Serenity.Domain.Common;
using Serenity.Domain.Entities;

namespace Serenity.Infrastructure.Persistence.Configurations;

public class JournalEntryConfiguration : IEntityTypeConfiguration<JournalEntry>
{
    public void Configure(EntityTypeBuilder<JournalEntry> config)
    {
        config.Property<Instant>("CreatedAt").HasDefaultValue(new Instant());
        config.Property<Instant>("UpdatedAt").HasDefaultValue(new Instant());
    }
}