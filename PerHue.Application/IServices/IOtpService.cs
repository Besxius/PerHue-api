using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerHue.Application.IServices
{
	public interface IOtpService 
	{
		/// <summary>
		/// Generate a one-time password (OTP).
		/// </summary>
		/// <param name="length">Length of the OTP, default is 4.</param>
		/// <returns>The generated OTP as a string.</returns>
		string GenerateOTP(int length = 4);

		/// <summary>
		/// Send an OTP to the given email address and store it in Redis.
		/// </summary>
		/// <param name="email">Recipient email.</param>
		/// <returns>True if sent successfully, otherwise false.</returns>
		Task<bool> SendOtpToEmailAsync(string email);

		/// <summary>
		/// Verify if the provided OTP matches the stored one.
		/// </summary>
		/// <param name="email">Recipient email.</param>
		/// <param name="otp">OTP to verify.</param>
		/// <returns>True if OTP is valid, otherwise false.</returns>
		bool VerifyOtp(string email, string otp);
	}
}
