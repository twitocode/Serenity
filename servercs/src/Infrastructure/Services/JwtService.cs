using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Serenity.Application.Interfaces;
using Serenity.Domain.Entities;

namespace Serenity.Infrastructure.Services;

public class JwtService : IJwtService {
	private const int ExpirationMinutes = 30;
	private readonly IConfiguration configuration;

	public JwtService(IConfiguration configuration) {
		this.configuration = configuration;
	}

	public string CreateToken(ApplicationUser user) {
		var expiration = DateTime.UtcNow.AddMinutes(ExpirationMinutes);
		var token = CreateJwtToken(
			CreateClaims(user),
			CreateSigningCredentials(),
			expiration
		);
		var tokenHandler = new JwtSecurityTokenHandler();
		return tokenHandler.WriteToken(token);
	}

	public ApplicationUser ReadToken(string token) {
		var tokenHandler = new JwtSecurityTokenHandler();
		JwtSecurityToken securityToken = tokenHandler.ReadJwtToken(token);

		return new ApplicationUser {
			UserName = securityToken.Claims.First(claim => claim.Type == ClaimTypes.Name).Value,
			Email = securityToken.Claims.First(claim => claim.Type == ClaimTypes.Email).Value,
			Id = new Guid(securityToken.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value),
		};
	}

	private JwtSecurityToken CreateJwtToken(List<Claim> claims, SigningCredentials credentials,
		DateTime expiration) =>
		new(
			configuration["Authentication:JwtSettings:Issuer"],
			configuration["Authentication:JwtSettings:Audience"],
			claims,
			expires: expiration,
			signingCredentials: credentials
		);

	private static List<Claim> CreateClaims(ApplicationUser user) {
		try {
			var claims = new List<Claim>
				{
					new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
					new Claim(ClaimTypes.Name, user.UserName ?? ""),
					new Claim(ClaimTypes.Email, user.Email ?? "")
				};
			return claims;
		} catch (Exception e) {
			Console.WriteLine(e);
			throw;
		}
	}
	private SigningCredentials CreateSigningCredentials() {
		return new SigningCredentials(
			new SymmetricSecurityKey(
				Encoding.UTF8.GetBytes(configuration["Authentication:JwtSettings:Key"] ?? "")
			),
			SecurityAlgorithms.HmacSha256
		);
	}
}
