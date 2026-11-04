using System.IO;
using HealthcareInventoryManagementSystem.Application.Dtos;
using HealthcareInventoryManagementSystem.Application.Interfaces.Services;

namespace HealthcareInventoryManagementSystem.Application.Interfaces
{
    public interface IItemService
    {
        // Retrieve all items.
        Task<IEnumerable<ItemDto>> GetAllItemsAsync();

        // Retrieve one item by ID.
        Task<ItemDto?> GetItemByIdAsync(int id);

        // Search for an item by name.
        Task<IEnumerable<ItemDto>> SearchItemsAsync(string name);

        // Create a new item with an optional image.
        Task<ItemDto> CreateItemAsync(
            ItemRequestDto request,
            Stream? imageStream = null,
            string? imageFileName = null);

        // Update an existing item with an optional replacement image.
        Task<ItemDto?> UpdateItemAsync(
            int id,
            ItemRequestDto request,
            Stream? imageStream = null,
            string? imageFileName = null);

        // Delete an item and its associated image.
        Task<bool> DeleteItemAsync(int id);
    }
}