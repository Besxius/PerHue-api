using Microsoft.EntityFrameworkCore.Storage;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerHue.Infrastructure.Utils;

public class RedisHelper
{
	private readonly StackExchange.Redis.IDatabase _db;

	public RedisHelper(string connectionString)
	{
		ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(connectionString);
		_db = redis.GetDatabase();
	}

	// ✅ Save OTP with expiration (e.g., 5 minutes)
	public void SetOTP(string email, string otp, int expiryMinutes = 5)
	{
		_db.StringSet($"OTP:{email}", otp, TimeSpan.FromMinutes(expiryMinutes));
	}

	// ✅ Get OTP
	public string? GetOTP(string email)
	{
		return _db.StringGet($"OTP:{email}");
	}

	// ✅ Delete OTP after verification
	public void DeleteOTP(string email)
	{
		_db.KeyDelete($"OTP:{email}");
	}
}
