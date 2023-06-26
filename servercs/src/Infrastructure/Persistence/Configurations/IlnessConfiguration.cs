using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NodaTime;
using Serenity.Domain.Entities;

namespace Serenity.Infrastructure.Persistence.Configurations;

public class IllnessConfiguration : IEntityTypeConfiguration<Illness> {
	public void Configure(EntityTypeBuilder<Illness> config) {
		config.Property<Instant>("CreatedAt").HasDefaultValue(new Instant());
		config.Property<Instant>("UpdatedAt").HasDefaultValue(new Instant());
	}
}
