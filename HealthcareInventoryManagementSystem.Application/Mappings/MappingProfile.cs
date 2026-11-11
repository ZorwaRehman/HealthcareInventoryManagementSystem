using HealthcareInventoryManagementSystem.Application.DTOs.GoodsReceived;
using AutoMapper;
using HealthcareInventoryManagementSystem.Application.DTOs.Stocks;
using HealthcareInventoryManagementSystem.Application.DTOs.Suppliers;
using HealthcareInventoryManagementSystem.Application.DTOs.PurchaseOrders;
using HealthcareInventoryManagementSystem.Domain.Entities;

namespace HealthcareInventoryManagementSystem.Application.Mappings
{
    // Defines mappings between entities and DTOs.
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // ---------------- STOCK MAPPINGS ----------------

            // Maps a stock batch to a transaction-independent stock summary.
            CreateMap<StockBatchEntity, StockDto>()
                .ForMember(
                    destination => destination.ItemId,
                    options => options.MapFrom(source => source.ItemId))
                .ForMember(
                    destination => destination.ItemName,
                    options => options.MapFrom(source => source.Item.Name))
                .ForMember(
                    destination => destination.TotalQuantity,
                    options => options.MapFrom(source => source.QuantityInStock))
                .ForMember(
                    destination => destination.ReorderLevel,
                    options => options.MapFrom(source => source.Item.ReorderLevel));

            // Maps a stock batch to its detailed response DTO.
            CreateMap<StockBatchEntity, StockBatchDto>()
                .ForMember(
                    destination => destination.SupplierName,
                    options => options.MapFrom(
                        source => source.Supplier != null
                            ? source.Supplier.Name
                            : null));

            // Maps a stock transaction entity to its response DTO.
            CreateMap<StockTransactionEntity, StockTransactionDto>();

            // ---------------- SUPPLIER MAPPINGS ----------------

            // Maps a Supplier entity to the response DTO.
            CreateMap<Supplier, SupplierDto>();

            // Maps supplier request data to a Supplier entity.
            CreateMap<SupplierRequestDto, Supplier>();

            // ---------------- PURCHASE ORDER MAPPINGS ----------------

            // Maps a purchase order entity to its response DTO.
            CreateMap<PurchaseOrderEntity, PurchaseOrderDto>()
                .ForMember(
                    destination => destination.SupplierName,
                    options => options.MapFrom(
                        source => source.Supplier.Name))
                .ForMember(
                    destination => destination.Status,
                    options => options.MapFrom(
                        source => source.Status.ToString()));

            // Maps a purchase order item entity to its response DTO.
            CreateMap<PurchaseOrderItemEntity, PurchaseOrderItemDto>()
                .ForMember(
                    destination => destination.ItemName,
                    options => options.MapFrom(
                        source => source.Item.Name))
                .ForMember(
                    destination => destination.SKU,
                    options => options.MapFrom(
                        source => source.Item.SKU))
                .ForMember(
                    destination => destination.RemainingQuantity,
                    options => options.MapFrom(
                        source => source.OrderedQuantity - source.ReceivedQuantity));

            // ---------------- GOODS RECEIVED MAPPINGS ----------------

            // Maps a Goods Received entity to its response DTO.
            CreateMap<GoodsReceivedEntity, GoodsReceivedDto>()
                .ForMember(
                    destination => destination.PurchaseOrderNumber,
                    options => options.MapFrom(
                        source => source.PurchaseOrder.OrderNumber))
                .ForMember(
                    destination => destination.SupplierName,
                    options => options.MapFrom(
                        source => source.PurchaseOrder.Supplier.Name));

            // Maps a Goods Received item entity to its response DTO.
            CreateMap<GoodsReceivedItemEntity, GoodsReceivedItemDto>()
                .ForMember(
                    destination => destination.ItemId,
                    options => options.MapFrom(
                        source => source.PurchaseOrderItem.ItemId))
                .ForMember(
                    destination => destination.ItemName,
                    options => options.MapFrom(
                        source => source.PurchaseOrderItem.Item.Name))
                .ForMember(
                    destination => destination.SKU,
                    options => options.MapFrom(
                        source => source.PurchaseOrderItem.Item.SKU));
        }
    }
}
