using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PerHue.Domain.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PerHue.Infrastructure.Authentication
{
	public class JwtProvider(IOptionsMonitor<AppSetting> optionsMonitor)
	{
		private readonly AppSetting _appSetting = optionsMonitor.CurrentValue;

		public string GenerateToken(UserAccount user)
		{
			var jwtTokenHandler = new JwtSecurityTokenHandler();

			var secretKeyBytes = Encoding.UTF8.GetBytes(_appSetting.SecretKey);

			var tokenDescription = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(new[]{
				new Claim("TokenId", Guid.NewGuid().ToString()),
				new Claim(ClaimTypes.Email, user.Email),
				new Claim("UserId", user.Id.ToString()),
				new Claim("UserName", user.Username!),
				new Claim("FullName", user.Fullname ?? string.Empty),
				new Claim(ClaimTypes.Role, user.Role.Name),
			}),
				Expires = DateTime.Now.AddMinutes(30),
				SigningCredentials = new SigningCredentials(
					new SymmetricSecurityKey(secretKeyBytes), SecurityAlgorithms.HmacSha256Signature)
			};

			var token = jwtTokenHandler.CreateToken(tokenDescription);

			var accessToken = jwtTokenHandler.WriteToken(token);

			return accessToken;
		}
	}
}
