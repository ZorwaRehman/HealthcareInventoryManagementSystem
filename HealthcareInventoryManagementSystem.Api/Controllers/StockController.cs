
using System;
using System.Collections.Generic;
using HealthcareInventoryManagementSystem.Application.DTOs.Stocks;
using HealthcareInventoryManagementSystem.Application.Dtos.Pagination;
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

        // GET: api/Stock/paged?pageNumber=1&pageSize=10&search=gloves
        // Returns paginated stock summaries with optional search.
        [HttpGet("paged")]
        public async Task<IActionResult> GetPagedStock(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null)
        {
            if (pageNumber < 1 || pageSize < 1)
            {
                return BadRequest(new
                {
                    message = "Page number and page size must be greater than zero."
                });
            }

            var request = new PaginationRequestDto
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var result = await stockService.GetPagedStockAsync(
                request,
                search);

            return Ok(result);
        }

        // GET: api/Stock/batches
        // Returns all stock batches across every inventory item.
        [HttpGet("batches")]
        public async Task<IActionResult> GetAllBatches()
        {
            var batches = await stockService.GetAllBatchesAsync();
            return Ok(batches);
        }


        // GET: api/Stock/batches/paged?pageNumber=1&pageSize=10&search=gloves&itemId=1
        // Returns paginated stock batches with optional search and item filtering.
        [HttpGet("batches/paged")]
        public async Task<IActionResult> GetPagedBatches(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] int? itemId = null)
        {
            if (pageNumber < 1 || pageSize < 1)
            {
                return BadRequest(new
                {
                    message = "Page number and page size must be greater than zero."
                });
            }

            if (itemId.HasValue && itemId.Value <= 0)
            {
                return BadRequest(new
                {
                    message = "Item ID must be greater than zero."
                });
            }

            var request = new PaginationRequestDto
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var result = await stockService.GetPagedBatchesAsync(
                request,
                search,
                itemId);

            return Ok(result);
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

        // PUT: api/Stock/batches/1/batch-number
        // Updates the batch number of an existing stock batch.
        [HttpPut("batches/{stockBatchId:int}/batch-number")]
        public async Task<IActionResult> UpdateStockBatchNumber(
            int stockBatchId,
            [FromBody] UpdateStockBatchNumberDto dto)
        {
            try
            {
                await stockService.UpdateStockBatchNumberAsync(
                    stockBatchId,
                    dto.BatchNumber);

                return Ok(new
                {
                    message = "Stock batch number updated successfully."
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
