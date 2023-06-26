using NodaTime;

namespace Serenity.Domain.Common;

public class BaseEntity {
	public Guid Id { get; private set; }
	public Instant CreationAt { get; set; }
	public Instant UpdatedAt { get; set; }
}
