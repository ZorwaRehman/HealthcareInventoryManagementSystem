using System;
using System.Collections.Generic;
using System.Text;
using HealthcareInventoryManagementSystem.Application.Dtos;
using HealthcareInventoryManagementSystem.Application.Interfaces;
using HealthcareInventoryManagementSystem.Application.Interfaces.Repositories;
using HealthcareInventoryManagementSystem.Domain.Entities;

namespace HealthcareInventoryManagementSystem.Application.Services
{
    public class ItemService : IItemService
    {
        private readonly IGenericRepository<ItemEntity> _itemRepository;

        public ItemService(
            IGenericRepository<ItemEntity> itemRepository)
        {
            _itemRepository = itemRepository;
        }

        // Retrieve all items.
        public async Task<IEnumerable<ItemDto>> GetAllItemsAsync()
        {
            var items = await _itemRepository.GetAllAsync();

            return items.Select(MapToDto).ToList();
        }

        // Retrieve one item by ID.
        public async Task<ItemDto?> GetItemByIdAsync(int id)
        {
            var item = await _itemRepository.GetByIdAsync(id);

            if (item == null)
            {
                return null;
            }

            return MapToDto(item);
        }

        // Search for all items matching the name.
        public async Task<IEnumerable<ItemDto>> SearchItemsAsync(
            string name)
        {
            var items = await _itemRepository.FindAllAsync(
                item => item.Name.Contains(name));

            return items.Select(MapToDto).ToList();
        }

        // Create a new item.
        public async Task<ItemDto> CreateItemAsync(
            ItemRequestDto request)
        {
            // Check whether the SKU already exists.
            var skuExists = await _itemRepository.ExistsAsync(
                item => item.SKU == request.SKU);

            if (skuExists)
            {
                throw new InvalidOperationException(
                    "An item with this SKU already exists.");
            }

            var item = new ItemEntity
            {
                Name = request.Name,
                Description = request.Description,
                Category = request.Category,
                SKU = request.SKU,
                Unit = request.Unit,
                ReorderLevel = request.ReorderLevel,
                ImageUrl = request.ImageUrl,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _itemRepository.AddAsync(item);
            await _itemRepository.SaveChangesAsync();

            return MapToDto(item);
        }

        // Update an existing item.
        public async Task<ItemDto?> UpdateItemAsync(
            int id,
            ItemRequestDto request)
        {
            var item = await _itemRepository.GetByIdAsync(id);

            if (item == null)
            {
                return null;
            }

            // Prevent another item from using the same SKU.
            var skuExists = await _itemRepository.ExistsAsync(
                existingItem =>
                    existingItem.SKU == request.SKU &&
                    existingItem.Id != id);

            if (skuExists)
            {
                throw new InvalidOperationException(
                    "Another item already uses this SKU.");
            }

            item.Name = request.Name;
            item.Description = request.Description;
            item.Category = request.Category;
            item.SKU = request.SKU;
            item.Unit = request.Unit;
            item.ReorderLevel = request.ReorderLevel;
            item.ImageUrl = request.ImageUrl;
            item.UpdatedAt = DateTime.UtcNow;

            _itemRepository.Update(item);
            await _itemRepository.SaveChangesAsync();

            return MapToDto(item);
        }

        // Delete an existing item.
        public async Task<bool> DeleteItemAsync(int id)
        {
            var item = await _itemRepository.GetByIdAsync(id);

            if (item == null)
            {
                return false;
            }

            _itemRepository.Delete(item);
            await _itemRepository.SaveChangesAsync();

            return true;
        }

        // Convert ItemEntity into ItemDto.
        private static ItemDto MapToDto(ItemEntity item)
        {
            return new ItemDto
            {
                Id = item.Id,
                Name = item.Name,
                Description = item.Description,
                Category = item.Category,
                SKU = item.SKU,
                Unit = item.Unit,
                ReorderLevel = item.ReorderLevel,
                ImageUrl = item.ImageUrl,
                IsActive = item.IsActive,
                CreatedAt = item.CreatedAt,
                UpdatedAt = item.UpdatedAt
            };
        }
    }
}

