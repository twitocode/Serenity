using Serenity.Domain.Entities;

namespace Serenity.Application.Interfaces;

public interface IJwtService {
	string CreateToken(ApplicationUser user);
	ApplicationUser ReadToken(string token);
}
