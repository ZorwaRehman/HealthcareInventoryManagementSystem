namespace HealthcareInventoryManagementSystem.Application.Interfaces.Services
{
    public interface IItemImageStorageService
    {
        // Save an uploaded image and return its relative URL.
        Task<string> SaveImageAsync(
            Stream imageStream,
            string fileName);

        // Delete an existing image using its relative URL.
        Task DeleteImageAsync(string? imageUrl);
    }
}