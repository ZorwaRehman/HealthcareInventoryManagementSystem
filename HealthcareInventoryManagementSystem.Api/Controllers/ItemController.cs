
using HealthcareInventoryManagementSystem.Application.Dtos;
using HealthcareInventoryManagementSystem.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HealthcareInventoryManagementSystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemsController : ControllerBase
    {
        private readonly IItemService _itemService;

        // Maximum image size: 5 MB.
        private const long MaxImageSize = 5 * 1024 * 1024;

        // Allowed image extensions.
        private static readonly string[] AllowedExtensions =
        {
            ".jpg",
            ".jpeg",
            ".png",
            ".webp"
        };

        public ItemsController(IItemService itemService)
        {
            _itemService = itemService;
        }

        // GET: api/Items
        [HttpGet]
        public async Task<IActionResult> GetAllItems()
        {
            var items = await _itemService.GetAllItemsAsync();

            return Ok(items);
        }

        // GET: api/Items/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetItemById(int id)
        {
            var item = await _itemService.GetItemByIdAsync(id);

            if (item == null)
            {
                return NotFound(new
                {
                    message = $"Item with ID {id} was not found."
                });
            }

            return Ok(item);
        }

        // GET: api/Items/search?name=gloves
        [HttpGet("search")]
        public async Task<IActionResult> SearchItems(
            [FromQuery] string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest(new
                {
                    message = "Please provide an item name to search."
                });
            }

            var items = await _itemService.SearchItemsAsync(name);

            return Ok(items);
        }

        // POST: api/Items
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreateItem(
            [FromForm] ItemRequestDto request,
            IFormFile? image)
        {
            try
            {
                // Validate the uploaded image.
                var validationError = ValidateImage(image);

                if (validationError != null)
                {
                    return BadRequest(new
                    {
                        message = validationError
                    });
                }

                // Open the image stream if an image was selected.
                using var imageStream = image?.OpenReadStream();

                var item = await _itemService.CreateItemAsync(
                    request,
                    imageStream,
                    image?.FileName);

                return CreatedAtAction(
                    nameof(GetItemById),
                    new { id = item.Id },
                    item);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        // PUT: api/Items/5
        [HttpPut("{id:int}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateItem(
            int id,
            [FromForm] ItemRequestDto request,
            IFormFile? image)
        {
            try
            {
                // Validate the uploaded image.
                var validationError = ValidateImage(image);

                if (validationError != null)
                {
                    return BadRequest(new
                    {
                        message = validationError
                    });
                }

                // Open the image stream if a new image was selected.
                using var imageStream = image?.OpenReadStream();

                var item = await _itemService.UpdateItemAsync(
                    id,
                    request,
                    imageStream,
                    image?.FileName);

                if (item == null)
                {
                    return NotFound(new
                    {
                        message = $"Item with ID {id} was not found."
                    });
                }

                return Ok(item);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        // DELETE: api/Items/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteItem(int id)
        {
            var deleted = await _itemService.DeleteItemAsync(id);

            if (!deleted)
            {
                return NotFound(new
                {
                    message = $"Item with ID {id} was not found."
                });
            }

            return NoContent();
        }

        // Validate an optional uploaded image.
        private static string? ValidateImage(IFormFile? image)
        {
            // Image is optional.
            if (image == null)
            {
                return null;
            }

            if (image.Length == 0)
            {
                return "The selected image is empty.";
            }

            // Check image size.
            if (image.Length > MaxImageSize)
            {
                return "Image size cannot exceed 5 MB.";
            }

            // Check file extension.
            var extension = Path.GetExtension(image.FileName)
                .ToLowerInvariant();

            if (!AllowedExtensions.Contains(extension))
            {
                return "Only JPG, JPEG, PNG, and WEBP images are allowed.";
            }

            // Check the declared content type.
            var allowedContentTypes = new[]
            {
                "image/jpeg",
                "image/png",
                "image/webp"
            };

            if (!allowedContentTypes.Contains(
                image.ContentType.ToLowerInvariant()))
            {
                return "Invalid image content type.";
            }

            return null;
        }
    }
}
