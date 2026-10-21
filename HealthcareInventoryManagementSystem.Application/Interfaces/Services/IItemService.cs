using HealthcareInventoryManagementSystem.Application.Dtos;

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

        // Create a new item.
        Task<ItemDto> CreateItemAsync(ItemRequestDto request);

        // Update an existing item.
        Task<ItemDto?> UpdateItemAsync(int id, ItemRequestDto request);

        // Delete an item.
        Task<bool> DeleteItemAsync(int id);
    }
}

