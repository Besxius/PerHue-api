using PerHue.Application.IServices;
using PerHue.Infrastructure.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PerHue.Infrastructure.Services;

internal class OtpService : IOtpService
{
	private readonly RedisHelper _redisHelper;
	private readonly EmailService _emailService;
	public OtpService(RedisHelper redisHelper,
		EmailService emailService)
	{
		_emailService = emailService;
		_redisHelper = redisHelper;
	}
	// ✅ Generate OTP
	public string GenerateOTP(int length = 4)
	{
		const string chars = "0123456789";
		var otp = new char[length];
		using var rng = new RNGCryptoServiceProvider();
		byte[] buffer = new byte[length];

		rng.GetBytes(buffer);
		for (int i = 0; i < length; i++)
		{
			otp[i] = chars[buffer[i] % chars.Length];
		}
		return new string(otp);
	}
	// ✅ Send OTP via email
	public async Task<bool> SendOtpToEmailAsync(string email)
	{
		try
		{
			string otp = GenerateOTP();
			Console.WriteLine($"Generated OTP: {otp} for {email}");

			try
			{
				_redisHelper.SetOTP(email, otp);  // Store OTP in Redis
				Console.WriteLine("Successfully stored OTP in Redis");
			}
			catch (Exception redisEx)
			{
				Console.WriteLine($"Redis error: {redisEx.Message}");
				return false;
			}

			string subject = "Your OTP Code";
			string body = $"<h3>Your OTP Code: <strong>{otp}</strong></h3><p>Valid for 5 minutes.</p>";

			bool emailSent = await _emailService.SendEmailAsync(email, subject, body);
			Console.WriteLine($"Email sending result: {emailSent}");
			return emailSent;
		}
		catch (Exception ex)
		{
			Console.WriteLine($"OTP Service Error: {ex.Message}");
			return false;
		}
	}
	// ✅ Verify OTP
	public bool VerifyOtp(string email, string otp)
	{
		string? storedOtp = _redisHelper.GetOTP(email);
		if (storedOtp == otp)
		{
			_redisHelper.DeleteOTP(email);
			return true;
		}
		return false;
	}

	public async Task<string> GenerateRegisterOtpAsync(string email)
	{
		var otp = new Random().Next(100000, 999999).ToString();

		// Lưu OTP vào Cache với Key là Email, thời gian sống 5 phút
		// await _redisHelper.SetAsync($"OTP_{email}", otp, TimeSpan.FromMinutes(5));

		return otp;
	}

	public async Task<bool> ValidateRegisterOtpAsync(string email, string otpInput)
	{
		// Lấy OTP từ cache
		// var cachedOtp = await _redisHelper.GetAsync($"OTP_{email}");

		// Kiểm tra khớp và chưa hết hạn
		// return cachedOtp == otpInput;
		return true; // Code mẫu
	}
}
