using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PerHue.Application.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks;

namespace PerHue.Infrastructure.Utils
{
	public class CloudinaryService : IImageUploadService
	{
		private readonly Cloudinary _cloudinary;
		private readonly ILogger<CloudinaryService> _logger;
		private static readonly HttpClient _httpClient = new HttpClient();

		public CloudinaryService(IConfiguration configuration, ILogger<CloudinaryService> logger)
		{
			var account = new Account(
				configuration["Cloudinary:CloudName"],
				configuration["Cloudinary:ApiKey"],
				configuration["Cloudinary:ApiSecret"]
			);
			_cloudinary = new Cloudinary(account);
			_logger = logger;
		}

		public async Task<string> UploadImageAsync(IFormFile file)
		{
			if (file == null || file.Length == 0)
			{
				return null;
			}

			await using var stream = file.OpenReadStream();
			var uploadParams = new ImageUploadParams()
			{
				File = new FileDescription(file.FileName, stream),
				// Optional: You can add transformations or folders
				// PublicId = $"perhue_app/{Guid.NewGuid()}" 
			};

			var uploadResult = await _cloudinary.UploadAsync(uploadParams);

			if (uploadResult.Error != null)
			{
				throw new Exception(uploadResult.Error.Message);
			}

			return uploadResult.SecureUrl.ToString();
		}
	}
}