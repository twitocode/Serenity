using Microsoft.AspNetCore.Identity;
using Serenity.Domain.ValueObjects;

namespace Serenity.Domain.Entities;

public class ApplicationUser : IdentityUser<Guid> {
	public PersonDetails PersonDetails { get; set; } = null!;
	public string AvatarUrl { get; init; }
	public string BackgroundUrl { get; init; }
	public string PasswordLock { get; init; }

	public UserStats Stats { get; private set; } = null!;
	public List<Illness> Illnesses { get; private set; } = new List<Illness>();
	public List<JournalEntry> JournalEntries { get; private set; } = new List<JournalEntry>();
}
