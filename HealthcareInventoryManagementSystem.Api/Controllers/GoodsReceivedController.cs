
using HealthcareInventoryManagementSystem.Application.Dtos.Pagination;
using HealthcareInventoryManagementSystem.Application.DTOs.GoodsReceived;
using HealthcareInventoryManagementSystem.Application.Interfaces;
using HealthcareInventoryManagementSystem.Application.Interfaces.Services;
using HealthcareInventoryManagementSystem.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace HealthcareInventoryManagementSystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GoodsReceivedController(
        IGoodsReceivedService goodsReceivedService) : ControllerBase
    {
        // GET: api/GoodsReceived
        // Returns all Goods Received documents.
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var receipts = await goodsReceivedService.GetAllAsync();

            return Ok(receipts);
        }

        // GET: api/GoodsReceived/paged?pageNumber=1&pageSize=10
        // Returns paginated Goods Received documents with optional search and status filters.
        [HttpGet("paged")]
        public async Task<IActionResult> GetPaged(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] string? status = null)
        {
            if (pageNumber < 1 || pageSize < 1)
            {
                return BadRequest(new
                {
                    message = "Page number and page size must be greater than zero."
                });
            }

            GoodsReceivedStatus? parsedStatus = null;

            if (!string.IsNullOrWhiteSpace(status))
            {
                if (!Enum.TryParse<GoodsReceivedStatus>(
                        status, true, out var statusValue)
                    || !Enum.IsDefined(statusValue))
                {
                    return BadRequest(new
                    {
                        message = "The selected Goods Received status is invalid."
                    });
                }

                parsedStatus = statusValue;
            }

            var request = new PaginationRequestDto
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var result = await goodsReceivedService.GetPagedAsync(
                request,
                search,
                parsedStatus);

            return Ok(result);
        }

        // GET: api/GoodsReceived/1
        // Returns a specific Goods Received document.
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var receipt = await goodsReceivedService.GetByIdAsync(id);

            if (receipt == null)
            {
                return NotFound(new
                {
                    message = $"Goods Received document with ID {id} was not found."
                });
            }

            return Ok(receipt);
        }

        // GET: api/GoodsReceived/purchase-order/1
        // Returns Purchase Order details for receiving goods.
        [HttpGet("purchase-order/{purchaseOrderId:int}")]
        public async Task<IActionResult> GetPurchaseOrderForReceiving(
            int purchaseOrderId)
        {
            var purchaseOrder =
                await goodsReceivedService.GetPurchaseOrderForReceivingAsync(
                    purchaseOrderId);

            if (purchaseOrder == null)
            {
                return NotFound(new
                {
                    message = $"Purchase Order with ID {purchaseOrderId} was not found."
                });
            }

            return Ok(purchaseOrder);
        }

        // POST: api/GoodsReceived
        // Creates a new Goods Received document.
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateGoodsReceivedRequestDto request)
        {
            var receipt = await goodsReceivedService.CreateAsync(request);

            return CreatedAtAction(
                nameof(GetById),
                new { id = receipt.GoodsReceivedId },
                receipt);
        }

        // PATCH: api/GoodsReceived/1/confirm
        // Confirms a Draft Goods Received document.
        [HttpPatch("{id:int}/confirm")]
        public async Task<IActionResult> Confirm(int id)
        {
            var receipt = await goodsReceivedService.ConfirmAsync(id);

            return Ok(receipt);
        }

        // PATCH: api/GoodsReceived/1/cancel
        // Cancels a Draft Goods Received document.
        [HttpPatch("{id:int}/cancel")]
        public async Task<IActionResult> Cancel(int id)
        {
            await goodsReceivedService.CancelAsync(id);

            return Ok(new
            {
                message = "Goods Received document cancelled successfully."
            });
        }

        // PATCH: api/GoodsReceived/items/1/release-quarantine?quantity=5
        // Releases quarantined quantity after inspection.
        [HttpPatch("items/{goodsReceivedItemId:int}/release-quarantine")]
        public async Task<IActionResult> ReleaseQuarantinedQuantity(
            int goodsReceivedItemId,
            [FromQuery] int quantity)
        {
            var receipt =
                await goodsReceivedService.ReleaseQuarantinedQuantityAsync(
                    goodsReceivedItemId,
                    quantity);

            return Ok(receipt);
        }
    }
}
