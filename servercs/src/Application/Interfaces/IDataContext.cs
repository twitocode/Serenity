using Microsoft.EntityFrameworkCore;
using Serenity.Domain.Entities;

namespace Serenity.Application.Interfaces;

public interface IDataContext {
	DbSet<Feeling> Feelings { get; }
	DbSet<Activity> Activities { get; }
	DbSet<Mood> Moods { get; }
	DbSet<Illness> Illnesses { get; }
	DbSet<JournalEntry> JournalEntries { get; }
	DbSet<UserStats> Stats { get; }
}
