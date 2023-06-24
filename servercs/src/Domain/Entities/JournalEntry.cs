using Serenity.Domain.Common;

namespace Serenity.Domain.Entities;

public class JournalEntry : BaseEntity
{
    public string QuickNote { get; private set; }
    public Mood Mood { get; private set; }
    public List<Feeling> Feelings { get; private set; }
    public List<Activity> Activities { get; private set; }
    public ApplicationUser User { get; set; }

}