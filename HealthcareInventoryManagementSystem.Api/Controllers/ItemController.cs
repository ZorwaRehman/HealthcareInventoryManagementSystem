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
        public async Task<IActionResult> CreateItem(
            [FromBody] ItemRequestDto request)
        {
            try
            {
                var item = await _itemService.CreateItemAsync(request);

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
        public async Task<IActionResult> UpdateItem(
            int id,
            [FromBody] ItemRequestDto request)
        {
            try
            {
                var item = await _itemService.UpdateItemAsync(id, request);

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
    }
}

