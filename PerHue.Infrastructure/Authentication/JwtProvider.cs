using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PerHue.Domain.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography;

namespace PerHue.Infrastructure.Authentication
{
	public class JwtProvider(IOptionsMonitor<JwtSetting> optionsMonitor)
	{
		private readonly JwtSetting _jwtSetting = optionsMonitor.CurrentValue;

		public string GenerateToken(UserAccount user)
		{
			var jwtTokenHandler = new JwtSecurityTokenHandler();

			var secretKeyBytes = Encoding.UTF8.GetBytes(_jwtSetting.Key);

			var claimsIdentity = new ClaimsIdentity(
				new[]
				{
					new Claim("TokenId", Guid.NewGuid().ToString()),
					new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
					new Claim(ClaimTypes.Email, user.Email),
					new Claim(ClaimTypes.Name, user.Username!),
					new Claim(ClaimTypes.Role, user.Role.Name),
				},
				JwtBearerDefaults.AuthenticationScheme
			);

			var tokenDescription = new SecurityTokenDescriptor
			{
				Subject = claimsIdentity,
				Expires = DateTime.Now.AddMinutes(_jwtSetting.DurationInMinutes),
				Issuer = _jwtSetting.Issuer,
				Audience = _jwtSetting.Audience,
				SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKeyBytes), SecurityAlgorithms.HmacSha256)
			};

			var token = jwtTokenHandler.CreateToken(tokenDescription);

			var accessToken = jwtTokenHandler.WriteToken(token);

			return accessToken;
		}

		public string GenerateRefreshToken()
		{
			var randomNumber = new byte[64];
			using var rng = RandomNumberGenerator.Create();
			rng.GetBytes(randomNumber);
			return Convert.ToBase64String(randomNumber);
		}

		public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
		{
			var secretKeyBytes = Encoding.UTF8.GetBytes(_jwtSetting.Key);

			var tokenValidationParameters = new TokenValidationParameters
			{
				ValidateIssuer = true,
				ValidateAudience = true,
				ValidateLifetime = false, // <-- This is the key: we don't validate the token's expiry date
				ValidateIssuerSigningKey = true,
				ValidIssuer = _jwtSetting.Issuer,
				ValidAudience = _jwtSetting.Audience,
				IssuerSigningKey = new SymmetricSecurityKey(secretKeyBytes),
				ClockSkew = TimeSpan.Zero
			};

			var tokenHandler = new JwtSecurityTokenHandler();
			var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

			if (securityToken is not JwtSecurityToken jwtSecurityToken ||
				!jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
			{
				throw new SecurityTokenException("Invalid token");
			}

			return principal;
		}

	}
}
