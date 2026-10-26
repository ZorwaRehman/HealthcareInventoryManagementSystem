using System;
using System.Collections.Generic;
using HealthcareInventoryManagementSystem.Application.DTOs.Stocks;
using HealthcareInventoryManagementSystem.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace HealthcareInventoryManagementSystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StockController(IStockService stockService) : ControllerBase
    {
        // GET: api/Stock
        // Returns stock summaries for all items.
        [HttpGet]
        public async Task<IActionResult> GetAllStock()
        {
            var stock = await stockService.GetAllStockAsync();
            return Ok(stock);
        }

        // GET: api/Stock/item/1
        // Returns the stock summary for a specific item.
        [HttpGet("item/{itemId:int}")]
        public async Task<IActionResult> GetStockByItemId(int itemId)
        {
            var stock = await stockService.GetStockByItemIdAsync(itemId);

            if (stock is null)
            {
                return NotFound(new
                {
                    message = $"Item with ID {itemId} was not found."
                });
            }

            return Ok(stock);
        }

        // GET: api/Stock/item/1/batches
        // Returns all batches belonging to an item.
        [HttpGet("item/{itemId:int}/batches")]
        public async Task<IActionResult> GetBatchesByItemId(int itemId)
        {
            var batches = await stockService.GetBatchesByItemIdAsync(itemId);
            return Ok(batches);
        }

        // POST: api/Stock/batches
        // Creates a new stock batch.
        [HttpPost("batches")]
        public async Task<IActionResult> CreateStockBatch(
            [FromBody] CreateStockBatchDto dto)
        {
            var batch = await stockService.CreateStockBatchAsync(dto);

            return CreatedAtAction(
                nameof(GetStockByItemId),
                new { itemId = batch.ItemId },
                batch);
        }

        // POST: api/Stock/adjust
        // Adds, removes, or adjusts stock.

        [HttpPost("adjust")]
        public async Task<IActionResult> AdjustStock(
            [FromBody] StockAdjustmentDto dto)
        {
            try
            {
                await stockService.AdjustStockAsync(dto);

                return Ok(new
                {
                    message = "Stock adjusted successfully."
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }


        // GET: api/Stock/batch/1/transactions
        // Returns transaction history for a stock batch.
        [HttpGet("batch/{stockBatchId:int}/transactions")]
        public async Task<IActionResult> GetTransactionsByBatchId(
            int stockBatchId)
        {
            var transactions =
                await stockService.GetTransactionsByBatchIdAsync(stockBatchId);

            return Ok(transactions);
        }
    }
}
