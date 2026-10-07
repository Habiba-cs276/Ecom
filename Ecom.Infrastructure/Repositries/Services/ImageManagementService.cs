using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Ecom.Core.Entites; 
using Ecom.Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ecom.Infrastructure.Repositries.Services
{
    #region ImageServiceIn wwwroot
    //public class ImageManagementService : IImageManagementService
    //{
    //    private readonly IWebHostEnvironment _webHostEnvironment;
    //    public ImageManagementService( IWebHostEnvironment webHostEnvironment)
    //    {
    //        _webHostEnvironment = webHostEnvironment;   
    //    }
    //    public async Task<List<string>> AddImageAsync(IFormFileCollection files, string categoryFolder)
    //    {
    //        var savedPaths = new List<string>();
    //        if (files == null || !files.Any()) return savedPaths;

    //        string webRoot = _webHostEnvironment.WebRootPath ?? Path.Combine(_webHostEnvironment.ContentRootPath, "wwwroot");

    //        string FolderPath = Path.Combine(webRoot,"images", categoryFolder);

    //        if (!Directory.Exists(FolderPath))
    //        {
    //            Directory.CreateDirectory(FolderPath);
    //        }
    //        foreach (var file in files)
    //        {
    //            if (file.Length > 0)
    //            {
    //                string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
    //                string fullPath = Path.Combine(FolderPath, uniqueFileName);

    //                using (var stream = new FileStream(fullPath, FileMode.Create))
    //                {
    //                    await file.CopyToAsync(stream);
    //                }
    //                string relativePath = Path.Combine("images",categoryFolder,uniqueFileName)
    //                    .Replace("\\", "/");    
    //                savedPaths.Add(relativePath);
    //            }
    //        }
    //        return savedPaths;
    //    }

    //    public Task DeleteAsync(string src)
    //    {
    //        if (string.IsNullOrEmpty(src))
    //            return Task.CompletedTask;

    //        var relativePath = src.TrimStart('/', '\\');
    //        string webRoot = _webHostEnvironment.WebRootPath ?? Path.Combine(_webHostEnvironment.ContentRootPath, "wwwroot");

    //        var fullPath = Path.Combine(webRoot, relativePath);

    //        if (File.Exists(fullPath))
    //        {
    //            File.Delete(fullPath);
    //        }

    //        return Task.CompletedTask;

    //    }

    //}
    #endregion

    public class ImageManagementService : IImageManagementService
    {
        private readonly Cloudinary _cloudinary;

        public ImageManagementService(IOptions<CloudinarySettings> config)
        {
            var account = new Account(
                config.Value.CloudName,
                config.Value.ApiKey,
                config.Value.ApiSecret
            );

            _cloudinary = new Cloudinary(account);
        }

        public async Task<List<string>> AddImageAsync(IFormFileCollection files, string categoryFolder)
        {
            var savedPaths = new List<string>();
            if (files == null || !files.Any()) return savedPaths;

            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    using var stream = file.OpenReadStream();

                    var uploadParams = new ImageUploadParams
                    {
                        File = new FileDescription(file.FileName, stream),

                        Folder = $"ecom/{categoryFolder}"
                    };

                    var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                    if (uploadResult.Error != null)
                    {
                        throw new Exception($"Cloudinary Upload Error: {uploadResult.Error.Message}");
                    }

                    savedPaths.Add(uploadResult.SecureUrl.ToString());
                }
            }

            return savedPaths;
        }

        public async Task DeleteAsync(string src)
        {
            if (string.IsNullOrEmpty(src)) return;

            var publicId = GetPublicIdFromUrl(src);
            if (!string.IsNullOrEmpty(publicId))
            {
                var deleteParams = new DeletionParams(publicId);
                await _cloudinary.DestroyAsync(deleteParams);
            }
        }

        private string GetPublicIdFromUrl(string url)
        {
            try
            {
                var uri = new Uri(url);
                var segments = uri.AbsolutePath.Split('/');

                var fileNameWithFolder = string.Join("/", segments.Skip(Array.IndexOf(segments, "upload") + 2));
                var publicId = fileNameWithFolder.Substring(0, fileNameWithFolder.LastIndexOf('.'));
                return publicId;
            }
            catch
            {
                return null;
            }
        }
    }
}
