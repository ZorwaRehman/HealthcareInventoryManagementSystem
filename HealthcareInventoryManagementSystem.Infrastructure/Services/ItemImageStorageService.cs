using HealthcareInventoryManagementSystem.Application.Interfaces.Services;
using Microsoft.AspNetCore.Hosting;

namespace HealthcareInventoryManagementSystem.Infrastructure.Services
{
    public class ItemImageStorageService : IItemImageStorageService
    {
        private readonly IWebHostEnvironment _environment;

        public ItemImageStorageService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        // Save an uploaded image inside wwwroot/Images/Item.
        public async Task<string> SaveImageAsync(
            Stream imageStream,
            string fileName)
        {
            // Get the wwwroot folder path.
            var webRootPath = _environment.WebRootPath;

            // Use ContentRootPath if wwwroot is not configured.
            if (string.IsNullOrWhiteSpace(webRootPath))
            {
                webRootPath = Path.Combine(
                    _environment.ContentRootPath,
                    "wwwroot");
            }

            // Get the existing Images/Item folder path.
            var uploadsFolder = Path.Combine(
                webRootPath,
                "Images",
                "Item");

            // Create the folder if it does not exist.
            Directory.CreateDirectory(uploadsFolder);

            // Generate a unique filename to avoid overwriting images.
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            var uniqueFileName = $"{Guid.NewGuid()}{extension}";

            // Get the complete image file path.
            var filePath = Path.Combine(
                uploadsFolder,
                uniqueFileName);

            // Save the uploaded image.
            using (var fileStream = new FileStream(
                filePath,
                FileMode.Create,
                FileAccess.Write))
            {
                await imageStream.CopyToAsync(fileStream);
            }

            // Return the relative URL to store in the database.
            return $"/Images/Item/{uniqueFileName}";
        }

        // Delete an existing uploaded image.
        public Task DeleteImageAsync(string? imageUrl)
        {
            // Do nothing if the image URL is empty.
            if (string.IsNullOrWhiteSpace(imageUrl))
            {
                return Task.CompletedTask;
            }

            // Extract only the filename.
            var fileName = Path.GetFileName(imageUrl);

            if (string.IsNullOrWhiteSpace(fileName))
            {
                return Task.CompletedTask;
            }

            // Get the wwwroot folder path.
            var webRootPath = _environment.WebRootPath;

            if (string.IsNullOrWhiteSpace(webRootPath))
            {
                webRootPath = Path.Combine(
                    _environment.ContentRootPath,
                    "wwwroot");
            }

            // Get the image file path.
            var filePath = Path.Combine(
                webRootPath,
                "Images",
                "Item",
                fileName);

            // Delete the image if it exists.
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            return Task.CompletedTask;
        }
    }
}