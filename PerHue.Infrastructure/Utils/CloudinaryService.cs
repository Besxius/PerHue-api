using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
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
		private static readonly HttpClient _httpClient = new HttpClient();

		public CloudinaryService(IConfiguration configuration)
		{
			var account = new Account(
				configuration["Cloudinary:CloudName"],
				configuration["Cloudinary:ApiKey"],
				configuration["Cloudinary:ApiSecret"]
			);
			_cloudinary = new Cloudinary(account);
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

		public async Task<IFormFile> DownloadImageAsFormFileAsync(string imageUrl)
		{
			if (string.IsNullOrEmpty(imageUrl))
				throw new ArgumentException("Image URL cannot be null or empty", nameof(imageUrl));

			try
			{
				// Download image từ URL
				var response = await _httpClient.GetAsync(imageUrl);
				response.EnsureSuccessStatusCode();

				var imageBytes = await response.Content.ReadAsByteArrayAsync();
				var contentType = response.Content.Headers.ContentType?.ToString() ?? "image/jpeg";

				// Extract filename từ URL
				var uri = new Uri(imageUrl);
				var fileName = Path.GetFileName(uri.LocalPath);

				// Nếu không có extension, thêm mặc định
				if (!Path.HasExtension(fileName))
				{
					fileName = $"{fileName}.jpg";
				}

				// Convert sang IFormFile
				return new FormFileFromBytes(imageBytes, fileName, contentType);
			}
			catch (HttpRequestException ex)
			{
				throw new InvalidOperationException($"Failed to download image from URL: {imageUrl}", ex);
			}
		}

	}
}