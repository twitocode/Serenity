using Microsoft.AspNetCore.Identity;
using Serenity.Domain.ValueObjects;

namespace Serenity.Domain.Entities;

public class ApplicationUser : IdentityUser<Guid> {
	public PersonDetails PersonDetails { get; set; } = null!;
	public string AvatarUrl { get; set; }
	public string BackgroundUrl { get; set; }
	public string PasswordLock { get; set; }

	public UserStats Stats { get; set; } = null!;
	public List<Illness> Illnesses { get; set; } = new List<Illness>();
	public List<JournalEntry> JournalEntries { get; set; } = new List<JournalEntry>();
}
