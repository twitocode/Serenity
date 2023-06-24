using NodaTime;
using Serenity.Domain.Common;

namespace Serenity.Domain.Entities;

public class UserStats : BaseEntity
{
    public Instant LastBreathingCheckup { get; private set; }
    public Guid UserId { get; set; }
    public ApplicationUser User { get; set; } = null!;
}