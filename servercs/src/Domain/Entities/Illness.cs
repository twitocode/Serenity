using Serenity.Domain.Common;

namespace Serenity.Domain.Entities;


public class Illness : BaseEntity {
	public string Name { get; private set; } = "";
	public List<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
}
