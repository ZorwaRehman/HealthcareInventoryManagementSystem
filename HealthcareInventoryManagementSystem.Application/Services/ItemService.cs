
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using HealthcareInventoryManagementSystem.Application.Dtos;
using HealthcareInventoryManagementSystem.Application.Interfaces;
using HealthcareInventoryManagementSystem.Application.Interfaces.Repositories;
using HealthcareInventoryManagementSystem.Application.Interfaces.Services;
using HealthcareInventoryManagementSystem.Domain.Entities;

namespace HealthcareInventoryManagementSystem.Application.Services
{
    public class ItemService : IItemService
    {
        private readonly IGenericRepository<ItemEntity> _itemRepository;
        private readonly IItemImageStorageService _imageStorageService;

        public ItemService(
            IGenericRepository<ItemEntity> itemRepository,
            IItemImageStorageService imageStorageService)
        {
            _itemRepository = itemRepository;
            _imageStorageService = imageStorageService;
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
        public async Task<IEnumerable<ItemDto>> SearchItemsAsync(string name)
        {
            var items = await _itemRepository.FindAllAsync(
                item => item.Name.Contains(name));

            return items.Select(MapToDto).ToList();
        }

        // Create a new item with an optional image.
        public async Task<ItemDto> CreateItemAsync(
            ItemRequestDto request,
            Stream? imageStream = null,
            string? imageFileName = null)
        {
            // Check whether the SKU already exists.
            var skuExists = await _itemRepository.ExistsAsync(
                item => item.SKU == request.SKU);

            if (skuExists)
            {
                throw new InvalidOperationException(
                    "An item with this SKU already exists.");
            }

            string? savedImageUrl = null;

            try
            {
                // Save the uploaded image if one was selected.
                if (imageStream != null)
                {
                    savedImageUrl = await _imageStorageService.SaveImageAsync(
                        imageStream,
                        imageFileName ?? "item-image");
                }

                var item = new ItemEntity
                {
                    Name = request.Name,
                    Description = request.Description,
                    Category = request.Category,
                    SKU = request.SKU,
                    Unit = request.Unit,
                    ReorderLevel = request.ReorderLevel,
                    ImageUrl = savedImageUrl ?? request.ImageUrl,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                await _itemRepository.AddAsync(item);
                await _itemRepository.SaveChangesAsync();

                return MapToDto(item);
            }
            catch
            {
                // Remove the newly uploaded image if item creation fails.
                if (savedImageUrl != null)
                {
                    await _imageStorageService.DeleteImageAsync(savedImageUrl);
                }

                throw;
            }
        }

        // Update an existing item with an optional replacement image.
        public async Task<ItemDto?> UpdateItemAsync(
            int id,
            ItemRequestDto request,
            Stream? imageStream = null,
            string? imageFileName = null)
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

            // Keep the existing image path for possible cleanup.
            var oldImageUrl = item.ImageUrl;
            string? newImageUrl = null;

            try
            {
                // Upload a replacement only if a new image was selected.
                if (imageStream != null)
                {
                    newImageUrl = await _imageStorageService.SaveImageAsync(
                        imageStream,
                        imageFileName ?? "item-image");
                }

                item.Name = request.Name;
                item.Description = request.Description;
                item.Category = request.Category;
                item.SKU = request.SKU;
                item.Unit = request.Unit;
                item.ReorderLevel = request.ReorderLevel;

                // If no replacement was selected, keep the old image.
                if (newImageUrl != null)
                {
                    item.ImageUrl = newImageUrl;
                }

                item.UpdatedAt = DateTime.UtcNow;

                _itemRepository.Update(item);
                await _itemRepository.SaveChangesAsync();

                // Delete the old image only after the database update succeeds.
                if (newImageUrl != null && !string.IsNullOrWhiteSpace(oldImageUrl))
                {
                    await _imageStorageService.DeleteImageAsync(oldImageUrl);
                }

                return MapToDto(item);
            }
            catch
            {
                // Remove the new image if the update fails.
                if (newImageUrl != null)
                {
                    await _imageStorageService.DeleteImageAsync(newImageUrl);
                }

                throw;
            }
        }

        // Delete an existing item and its associated image.
        public async Task<bool> DeleteItemAsync(int id)
        {
            var item = await _itemRepository.GetByIdAsync(id);

            if (item == null)
            {
                return false;
            }

            var imageUrl = item.ImageUrl;

            _itemRepository.Delete(item);
            await _itemRepository.SaveChangesAsync();

            // Delete the image after the database record is removed.
            await _imageStorageService.DeleteImageAsync(imageUrl);

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
