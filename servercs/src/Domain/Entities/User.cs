using Microsoft.AspNetCore.Identity;
using Serenity.Domain.ValueObjects;

namespace Serenity.Domain.Entities;

public class ApplicationUser : IdentityUser<Guid>
{
    public PersonDetails PersonDetails { get; set; }
    public string AvatarUrl { get; private set; }
    public string BackgroundUrl { get; private set; }
    public string PasswordLock { get; private set; }

    public UserStats Stats { get; private set; }
    public List<Illness> Illnesses { get; private set; }
    public List<JournalEntry> JournalEntries { get; private set; }
}
