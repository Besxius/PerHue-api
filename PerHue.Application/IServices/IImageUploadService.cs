using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerHue.Application.IServices
{
	public interface IImageUploadService
	{
		Task<string> UploadImageAsync(IFormFile file);
		Task<IFormFile> DownloadImageAsFormFileAsync(string imageUrl);
	}
}