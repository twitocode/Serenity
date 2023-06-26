using Serenity.Domain.Common;

namespace Serenity.Domain.Entities;


public class Activity : BaseEntity {
	public string Name { get; private set; }
	public List<JournalEntry> JournalEntries { get; set; } = new List<JournalEntry>();
}
