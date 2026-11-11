
using System.Threading.Tasks;
using HealthcareInventoryManagementSystem.Application.DTOs.PurchaseOrders;
using HealthcareInventoryManagementSystem.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace HealthcareInventoryManagementSystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PurchaseOrderController(
        IPurchaseOrderService purchaseOrderService) : ControllerBase
    {
        // GET: api/PurchaseOrder
        // Returns all purchase orders.
        [HttpGet]
        public async Task<IActionResult> GetAllPurchaseOrders()
        {
            var purchaseOrders =
                await purchaseOrderService.GetAllPurchaseOrdersAsync();

            return Ok(purchaseOrders);
        }

        // GET: api/PurchaseOrder/1
        // Returns details of a specific purchase order.
        [HttpGet("{purchaseOrderId:int}")]
        public async Task<IActionResult> GetPurchaseOrderById(
            int purchaseOrderId)
        {
            var purchaseOrder =
                await purchaseOrderService.GetPurchaseOrderByIdAsync(
                    purchaseOrderId);

            if (purchaseOrder == null)
            {
                return NotFound(new
                {
                    message = $"Purchase order with ID {purchaseOrderId} was not found."
                });
            }

            return Ok(purchaseOrder);
        }

        // POST: api/PurchaseOrder
        // Creates a new purchase order without authentication.
        [HttpPost]
        public async Task<IActionResult> CreatePurchaseOrder(
            [FromBody] PurchaseOrderRequestDto request)
        {
            var purchaseOrder =
                await purchaseOrderService.CreatePurchaseOrderAsync(request);

            return CreatedAtAction(
                nameof(GetPurchaseOrderById),
                new { purchaseOrderId = purchaseOrder.PurchaseOrderId },
                purchaseOrder);
        }

        // PUT: api/PurchaseOrder/1
        // Updates an existing draft purchase order.
        [HttpPut("{purchaseOrderId:int}")]
        public async Task<IActionResult> UpdatePurchaseOrder(
            int purchaseOrderId,
            [FromBody] PurchaseOrderRequestDto request)
        {
            var purchaseOrder =
                await purchaseOrderService.UpdatePurchaseOrderAsync(
                    purchaseOrderId, request);

            if (purchaseOrder == null)
            {
                return NotFound(new
                {
                    message = $"Purchase order with ID {purchaseOrderId} was not found."
                });
            }

            return Ok(purchaseOrder);
        }

        // PATCH: api/PurchaseOrder/1/submit
        // Submits a draft purchase order for approval.
        [HttpPatch("{purchaseOrderId:int}/submit")]
        public async Task<IActionResult> SubmitPurchaseOrder(
            int purchaseOrderId)
        {
            var submitted =
                await purchaseOrderService.SubmitPurchaseOrderAsync(
                    purchaseOrderId);

            if (!submitted)
            {
                return NotFound(new
                {
                    message = $"Purchase order with ID {purchaseOrderId} was not found."
                });
            }

            return Ok(new
            {
                message = "Purchase order submitted successfully."
            });
        }

        // PATCH: api/PurchaseOrder/1/approve
        // Approves a submitted purchase order without authentication.
        [HttpPatch("{purchaseOrderId:int}/approve")]
        public async Task<IActionResult> ApprovePurchaseOrder(
            int purchaseOrderId)
        {
            var approved =
                await purchaseOrderService.ApprovePurchaseOrderAsync(
                    purchaseOrderId);

            if (!approved)
            {
                return NotFound(new
                {
                    message = $"Purchase order with ID {purchaseOrderId} was not found."
                });
            }

            return Ok(new
            {
                message = "Purchase order approved successfully."
            });
        }

        // PATCH: api/PurchaseOrder/1/order
        // Marks an approved purchase order as ordered.
        [HttpPatch("{purchaseOrderId:int}/order")]
        public async Task<IActionResult> MarkPurchaseOrderAsOrdered(
            int purchaseOrderId)
        {
            var ordered =
                await purchaseOrderService.MarkPurchaseOrderAsOrderedAsync(
                    purchaseOrderId);

            if (!ordered)
            {
                return NotFound(new
                {
                    message = $"Purchase order with ID {purchaseOrderId} was not found."
                });
            }

            return Ok(new
            {
                message = "Purchase order marked as ordered successfully."
            });
        }

        // PATCH: api/PurchaseOrder/1/cancel
        // Cancels a purchase order if cancellation is allowed.
        [HttpPatch("{purchaseOrderId:int}/cancel")]
        public async Task<IActionResult> CancelPurchaseOrder(
            int purchaseOrderId)
        {
            var cancelled =
                await purchaseOrderService.CancelPurchaseOrderAsync(
                    purchaseOrderId);

            if (!cancelled)
            {
                return NotFound(new
                {
                    message = $"Purchase order with ID {purchaseOrderId} was not found."
                });
            }

            return Ok(new
            {
                message = "Purchase order cancelled successfully."
            });
        }
    }
}
