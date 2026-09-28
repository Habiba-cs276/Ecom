using Ecom.Core.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Infrastructure.Repositries.Services
{
    public class ImageManagementService : IImageManagementService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ImageManagementService( IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;   
        }
        public async Task<List<string>> AddImageAsync(IFormFileCollection files, string categoryFolder)
        {
            var savedPaths = new List<string>();
            if (files == null || !files.Any()) return savedPaths;

            string webRoot = _webHostEnvironment.WebRootPath ?? Path.Combine(_webHostEnvironment.ContentRootPath, "wwwroot");

            string FolderPath = Path.Combine(webRoot,"images", categoryFolder);

            if (!Directory.Exists(FolderPath))
            {
                Directory.CreateDirectory(FolderPath);
            }
            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string fullPath = Path.Combine(FolderPath, uniqueFileName);

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                    string relativePath = Path.Combine("images",categoryFolder,uniqueFileName)
                        .Replace("\\", "/");    
                    savedPaths.Add(relativePath);
                }
            }
            return savedPaths;
        }

        public Task DeleteAsync(string src)
        {
            if (string.IsNullOrEmpty(src))
                return Task.CompletedTask;

            var relativePath = src.TrimStart('/', '\\');
            string webRoot = _webHostEnvironment.WebRootPath ?? Path.Combine(_webHostEnvironment.ContentRootPath, "wwwroot");

            var fullPath = Path.Combine(webRoot, relativePath);

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }

            return Task.CompletedTask;

        }

    }
}
