namespace Serenity.Domain.Entities.Redis;

public class RedisUser : ApplicationUser {
	public DateTimeOffset ExpirationTime { get; set; }
	public string Password { get; set; }
	public string StringId { get; set; }
}
