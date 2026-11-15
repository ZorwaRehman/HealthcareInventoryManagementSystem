
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using HealthcareInventoryManagementSystem.Application.DTOs.PurchaseOrders;
using HealthcareInventoryManagementSystem.Application.Interfaces.Repositories;
using HealthcareInventoryManagementSystem.Application.Interfaces.Services;
using HealthcareInventoryManagementSystem.Domain.Entities;
using HealthcareInventoryManagementSystem.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace HealthcareInventoryManagementSystem.Application.Services
{
    public class PurchaseOrderService : IPurchaseOrderService
    {
        private readonly IGenericRepository<PurchaseOrderEntity> _purchaseOrderRepository;
        private readonly IGenericRepository<PurchaseOrderItemEntity> _purchaseOrderItemRepository;
        private readonly IGenericRepository<Supplier> _supplierRepository;
        private readonly IGenericRepository<ItemEntity> _itemRepository;

        private readonly IMapper _mapper;
        private readonly IValidator<PurchaseOrderRequestDto> _validator;
        private readonly ILogger<PurchaseOrderService> _logger;

        public PurchaseOrderService(
            IGenericRepository<PurchaseOrderEntity> purchaseOrderRepository,
            IGenericRepository<PurchaseOrderItemEntity> purchaseOrderItemRepository,
            IGenericRepository<Supplier> supplierRepository,
            IGenericRepository<ItemEntity> itemRepository,
            IMapper mapper,
            IValidator<PurchaseOrderRequestDto> validator,
            ILogger<PurchaseOrderService> logger)
        {
            _purchaseOrderRepository = purchaseOrderRepository;
            _purchaseOrderItemRepository = purchaseOrderItemRepository;
            _supplierRepository = supplierRepository;
            _itemRepository = itemRepository;
            _mapper = mapper;
            _validator = validator;
            _logger = logger;
        }

        // Retrieve all purchase orders with their related information.
        public async Task<IEnumerable<PurchaseOrderDto>> GetAllPurchaseOrdersAsync()
        {
            var purchaseOrders =
                await _purchaseOrderRepository.FindAllWithNestedIncludesAsync(
                    order => true,
                    "Supplier",
                    "Items.Item");

            return _mapper.Map<IEnumerable<PurchaseOrderDto>>(purchaseOrders);
        }

        // Retrieve one purchase order with its complete details.
        public async Task<PurchaseOrderDto?> GetPurchaseOrderByIdAsync(
            int purchaseOrderId)
        {
            var purchaseOrders =
                await _purchaseOrderRepository.FindAllWithNestedIncludesAsync(
                    order => order.PurchaseOrderId == purchaseOrderId,
                    "Supplier",
                    "Items.Item");

            var purchaseOrder = purchaseOrders.FirstOrDefault();

            if (purchaseOrder == null)
            {
                _logger.LogWarning(
                    "Purchase order with ID {PurchaseOrderId} was not found.",
                    purchaseOrderId);

                return null;
            }

            return _mapper.Map<PurchaseOrderDto>(purchaseOrder);
        }

        // Create a new draft purchase order without requiring a user ID.
        public async Task<PurchaseOrderDto> CreatePurchaseOrderAsync(
            PurchaseOrderRequestDto request)
        {
            // Validate the incoming request.
            await _validator.ValidateAndThrowAsync(request);

            // Verify that the selected supplier exists and is active.
            var supplier = await _supplierRepository.FindAsync(
                supplier => supplier.SupplierId == request.SupplierId
                    && supplier.IsActive);

            if (supplier == null)
            {
                throw new ValidationException(
                    "The selected supplier does not exist or is inactive.");
            }

            // Create the purchase order entity.
            var purchaseOrder = new PurchaseOrderEntity
            {
                OrderNumber = GenerateOrderNumber(),
                SupplierId = request.SupplierId,
                Supplier = supplier,
                OrderDate = request.OrderDate,
                ExpectedDeliveryDate = request.ExpectedDeliveryDate,
                TaxRate = request.TaxRate,
                Notes = request.Notes,
                CreatedAt = DateTime.UtcNow,
                Status = PurchaseOrderStatus.Draft
            };

            // Add the requested purchase order items.
            foreach (var itemRequest in request.Items)
            {
                var item = await _itemRepository.FindAsync(
                    inventoryItem =>
                        inventoryItem.Id == itemRequest.ItemId
                        && inventoryItem.IsActive);

                if (item == null)
                {
                    throw new ValidationException(
                        $"Inventory item with ID {itemRequest.ItemId} does not exist or is inactive.");
                }

                var orderItem = new PurchaseOrderItemEntity
                {
                    ItemId = item.Id,
                    Item = item,
                    OrderedQuantity = itemRequest.OrderedQuantity,
                    ReceivedQuantity = 0,
                    UnitPrice = itemRequest.UnitPrice,
                    LineTotal = Math.Round(
                        itemRequest.OrderedQuantity * itemRequest.UnitPrice,
                        2,
                        MidpointRounding.AwayFromZero)
                };

                purchaseOrder.Items.Add(orderItem);
            }

            // Calculate the order amounts on the backend.
            CalculateOrderTotals(purchaseOrder);

            // Add the purchase order and its items.
            await _purchaseOrderRepository.AddAsync(purchaseOrder);

            // Save all changes.
            await _purchaseOrderRepository.SaveChangesAsync();

            _logger.LogInformation(
                "Purchase order {OrderNumber} was created.",
                purchaseOrder.OrderNumber);

            // Return the complete purchase order with related details.
            var result = await GetPurchaseOrderByIdAsync(
                purchaseOrder.PurchaseOrderId);

            if (result == null)
            {
                throw new InvalidOperationException(
                    "The purchase order was created but could not be retrieved.");
            }

            return result;
        }

        // Update an existing draft purchase order.
        public async Task<PurchaseOrderDto?> UpdatePurchaseOrderAsync(
            int purchaseOrderId,
            PurchaseOrderRequestDto request)
        {
            // Validate the incoming request.
            await _validator.ValidateAndThrowAsync(request);

            // Retrieve the purchase order and its existing items.
            var purchaseOrders =
                await _purchaseOrderRepository.FindAllWithNestedIncludesAsync(
                    order => order.PurchaseOrderId == purchaseOrderId,
                    "Supplier",
                    "Items.Item");

            var purchaseOrder = purchaseOrders.FirstOrDefault();

            if (purchaseOrder == null)
            {
                _logger.LogWarning(
                    "Purchase order with ID {PurchaseOrderId} was not found.",
                    purchaseOrderId);

                return null;
            }

            // Only draft purchase orders can be edited.
            if (purchaseOrder.Status != PurchaseOrderStatus.Draft)
            {
                throw new ValidationException(
                    "Only draft purchase orders can be updated.");
            }

            // Verify that the selected supplier exists and is active.
            var supplier = await _supplierRepository.FindAsync(
                supplier => supplier.SupplierId == request.SupplierId
                    && supplier.IsActive);

            if (supplier == null)
            {
                throw new ValidationException(
                    "The selected supplier does not exist or is inactive.");
            }

            // Remove the existing purchase order items.
            foreach (var existingItem in purchaseOrder.Items.ToList())
            {
                _purchaseOrderItemRepository.Delete(existingItem);
            }

            purchaseOrder.Items.Clear();

            // Update purchase order information.
            purchaseOrder.SupplierId = request.SupplierId;
            purchaseOrder.Supplier = supplier;
            purchaseOrder.OrderDate = request.OrderDate;
            purchaseOrder.ExpectedDeliveryDate = request.ExpectedDeliveryDate;
            purchaseOrder.TaxRate = request.TaxRate;
            purchaseOrder.Notes = request.Notes;
            purchaseOrder.UpdatedAt = DateTime.UtcNow;

            // Add the updated purchase order items.
            foreach (var itemRequest in request.Items)
            {
                var item = await _itemRepository.FindAsync(
                    inventoryItem =>
                        inventoryItem.Id == itemRequest.ItemId
                        && inventoryItem.IsActive);

                if (item == null)
                {
                    throw new ValidationException(
                        $"Inventory item with ID {itemRequest.ItemId} does not exist or is inactive.");
                }

                var orderItem = new PurchaseOrderItemEntity
                {
                    PurchaseOrderId = purchaseOrder.PurchaseOrderId,
                    PurchaseOrder = purchaseOrder,
                    ItemId = item.Id,
                    Item = item,
                    OrderedQuantity = itemRequest.OrderedQuantity,
                    ReceivedQuantity = 0,
                    UnitPrice = itemRequest.UnitPrice,
                    LineTotal = Math.Round(
                        itemRequest.OrderedQuantity * itemRequest.UnitPrice,
                        2,
                        MidpointRounding.AwayFromZero)
                };

                purchaseOrder.Items.Add(orderItem);
            }

            // Recalculate the purchase order amounts.
            CalculateOrderTotals(purchaseOrder);

            // Save the tracked entity changes.
            await _purchaseOrderRepository.SaveChangesAsync();

            _logger.LogInformation(
                "Purchase order {PurchaseOrderId} was updated.",
                purchaseOrderId);

            return await GetPurchaseOrderByIdAsync(purchaseOrderId);
        }

        // Submit a draft purchase order for approval.
        public async Task<bool> SubmitPurchaseOrderAsync(int purchaseOrderId)
        {
            var purchaseOrder = await _purchaseOrderRepository.FindAsync(
                order => order.PurchaseOrderId == purchaseOrderId);

            if (purchaseOrder == null)
            {
                return false;
            }

            if (purchaseOrder.Status != PurchaseOrderStatus.Draft)
            {
                throw new ValidationException(
                    "Only draft purchase orders can be submitted.");
            }

            purchaseOrder.Status = PurchaseOrderStatus.Submitted;
            purchaseOrder.UpdatedAt = DateTime.UtcNow;

            await _purchaseOrderRepository.SaveChangesAsync();

            _logger.LogInformation(
                "Purchase order {PurchaseOrderId} was submitted.",
                purchaseOrderId);

            return true;
        }

        // Approve a submitted purchase order without requiring a user ID.
        public async Task<bool> ApprovePurchaseOrderAsync(int purchaseOrderId)
        {
            var purchaseOrder = await _purchaseOrderRepository.FindAsync(
                order => order.PurchaseOrderId == purchaseOrderId);

            if (purchaseOrder == null)
            {
                return false;
            }

            if (purchaseOrder.Status != PurchaseOrderStatus.Submitted)
            {
                throw new ValidationException(
                    "Only submitted purchase orders can be approved.");
            }

            purchaseOrder.Status = PurchaseOrderStatus.Approved;
            purchaseOrder.ApprovedAt = DateTime.UtcNow;
            purchaseOrder.UpdatedAt = DateTime.UtcNow;

            await _purchaseOrderRepository.SaveChangesAsync();

            _logger.LogInformation(
                "Purchase order {PurchaseOrderId} was approved.",
                purchaseOrderId);

            return true;
        }

        // Mark an approved purchase order as ordered with the supplier.
        public async Task<bool> MarkPurchaseOrderAsOrderedAsync(
            int purchaseOrderId)
        {
            var purchaseOrder = await _purchaseOrderRepository.FindAsync(
                order => order.PurchaseOrderId == purchaseOrderId);

            if (purchaseOrder == null)
            {
                return false;
            }

            if (purchaseOrder.Status != PurchaseOrderStatus.Approved)
            {
                throw new ValidationException(
                    "Only approved purchase orders can be marked as ordered.");
            }

            purchaseOrder.Status = PurchaseOrderStatus.Ordered;
            purchaseOrder.UpdatedAt = DateTime.UtcNow;

            await _purchaseOrderRepository.SaveChangesAsync();

            _logger.LogInformation(
                "Purchase order {PurchaseOrderId} was marked as ordered.",
                purchaseOrderId);

            return true;
        }

        // Cancel an eligible purchase order.
        public async Task<bool> CancelPurchaseOrderAsync(int purchaseOrderId)
        {
            var purchaseOrders =
                await _purchaseOrderRepository.FindAllWithNestedIncludesAsync(
                    order => order.PurchaseOrderId == purchaseOrderId,
                    "Items");

            var purchaseOrder = purchaseOrders.FirstOrDefault();

            if (purchaseOrder == null)
            {
                return false;
            }

            // Only orders with no received quantities can be cancelled.
            if (purchaseOrder.Items.Any(item => item.ReceivedQuantity > 0))
            {
                throw new ValidationException(
                    "A purchase order with received items cannot be cancelled.");
            }

            // Already received or cancelled orders cannot be cancelled again.
            if (purchaseOrder.Status == PurchaseOrderStatus.Received
                || purchaseOrder.Status == PurchaseOrderStatus.PartiallyReceived
                || purchaseOrder.Status == PurchaseOrderStatus.Cancelled)
            {
                throw new ValidationException(
                    "This purchase order cannot be cancelled.");
            }

            purchaseOrder.Status = PurchaseOrderStatus.Cancelled;
            purchaseOrder.UpdatedAt = DateTime.UtcNow;

            await _purchaseOrderRepository.SaveChangesAsync();

            _logger.LogInformation(
                "Purchase order {PurchaseOrderId} was cancelled.",
                purchaseOrderId);

            return true;
        }

        // Generate a unique purchase order number.
        private static string GenerateOrderNumber()
        {
            return $"PO-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..12].ToUpperInvariant()}";
        }

        // Calculate subtotal, tax amount and total amount on the backend.
        private static void CalculateOrderTotals(PurchaseOrderEntity purchaseOrder)
        {
            purchaseOrder.Subtotal = purchaseOrder.Items.Sum(
                item => item.LineTotal);

            purchaseOrder.TaxAmount = Math.Round(
                purchaseOrder.Subtotal * purchaseOrder.TaxRate / 100m,
                2,
                MidpointRounding.AwayFromZero);

            purchaseOrder.TotalAmount =
                purchaseOrder.Subtotal + purchaseOrder.TaxAmount;
        }
    }
}
