using APiTurboSetup.Interfaces;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Linq;

namespace APiTurboSetup.Services
{
    public class FirebaseStorageService : IFirebaseStorageService
    {
        private readonly StorageClient _storageClient;
        private readonly string _bucketName;
        private readonly IConfiguration _configuration;
        private readonly long _maxFileSize = 5 * 1024 * 1024; // 5 MB
        private readonly HashSet<string> _allowedExtensions = new() { ".jpg", ".jpeg", ".png", ".webp" };

        public FirebaseStorageService(IConfiguration configuration)
        {
            _configuration = configuration;
            var credentialPath = _configuration["Firebase:AdminSdkPath"];
            var credential = GoogleCredential.FromFile(credentialPath);
            _storageClient = StorageClient.Create(credential);
            _bucketName = _configuration["Firebase:BucketName"];
        }

        private bool IsValidImage(IFormFile file)
        {
            if (file == null || file.Length == 0) return false;
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            return _allowedExtensions.Contains(extension) && file.Length <= _maxFileSize;
        }

        private string GetPublicUrl(string objectName)
        {
            return $"https://firebasestorage.googleapis.com/v0/b/{_bucketName}/o/{Uri.EscapeDataString(objectName)}?alt=media";
        }

        public async Task<string> GetSignedUrlAsync(string objectName)
        {
            return GetPublicUrl(objectName);
        }

        public async Task<string> UploadImageAsync(IFormFile imageFile, string folder)
        {
            if (!IsValidImage(imageFile))
            {
                throw new ArgumentException("Arquivo inválido. Verifique o tipo (JPG, PNG, WEBP) e o tamanho (máx 5MB).", nameof(imageFile));
            }

            var imageName = $"{Guid.NewGuid()}_{Path.GetExtension(imageFile.FileName)}";
            var objectName = $"{folder}/{imageName}";

            using (var memoryStream = new MemoryStream())
            {
                await imageFile.CopyToAsync(memoryStream);
                memoryStream.Position = 0;

                await _storageClient.UploadObjectAsync(
                    bucket: _bucketName,
                    objectName: objectName,
                    contentType: imageFile.ContentType,
                    source: memoryStream
                );
                return GetPublicUrl(objectName);
            }
        }

        public async Task<List<string>> UploadImagesAsync(List<IFormFile> imageFiles, string destinationPath)
        {
            if (imageFiles == null || !imageFiles.Any())
            {
                throw new ArgumentException("Nenhum arquivo de imagem fornecido.", nameof(imageFiles));
            }

            var uploadedImageUrls = new List<string>();
            var validationErrors = new List<string>();

            foreach (var imageFile in imageFiles)
            {
                if (!IsValidImage(imageFile))
                {
                    validationErrors.Add($"Arquivo '{imageFile?.FileName}' é inválido. Verifique o tipo (JPG, PNG, WEBP) e o tamanho (máx 5MB).");
                    continue;
                }

                var imageName = $"{Guid.NewGuid()}_{Path.GetExtension(imageFile.FileName)}";
                var objectName = $"{destinationPath}/{imageName}";

                using (var memoryStream = new MemoryStream())
                {
                    await imageFile.CopyToAsync(memoryStream);
                    memoryStream.Position = 0;

                    await _storageClient.UploadObjectAsync(
                        bucket: _bucketName,
                        objectName: objectName,
                        contentType: imageFile.ContentType,
                        source: memoryStream
                    );
                    
                    uploadedImageUrls.Add(GetPublicUrl(objectName));
                }
            }
            
            if (validationErrors.Any() && !uploadedImageUrls.Any())
            {
                throw new ArgumentException($"Falha na validação de todos os arquivos: {string.Join("; ", validationErrors)}", nameof(imageFiles));
            }

            return uploadedImageUrls;
        }
    }
}
