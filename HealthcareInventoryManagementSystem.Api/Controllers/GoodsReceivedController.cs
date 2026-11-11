
using HealthcareInventoryManagementSystem.Application.DTOs.GoodsReceived;
using HealthcareInventoryManagementSystem.Application.Interfaces;
using HealthcareInventoryManagementSystem.Application.Interfaces.Services;
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
