using System.Reflection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Serenity.Application.Interfaces;
using Serenity.Domain.Entities;

namespace Serenity.Infrastructure.Persistence;

public class DataContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>, IDataContext {
	public DbSet<Feeling> Feelings => Set<Feeling>();
	public DbSet<Activity> Activities => Set<Activity>();
	public DbSet<Mood> Moods => Set<Mood>();
	public DbSet<Illness> Illnesses => Set<Illness>();
	public DbSet<JournalEntry> JournalEntries => Set<JournalEntry>();
	public DbSet<UserStats> Stats => Set<UserStats>();

	public DataContext(DbContextOptions<DataContext> options) : base(options) {

	}

	protected override void OnModelCreating(ModelBuilder modelBuilder) {
		base.OnModelCreating(modelBuilder);
		modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
	}
}
