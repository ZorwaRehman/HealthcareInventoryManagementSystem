using AutoMapper;
using HealthcareInventoryManagementSystem.Application.DTOs.Stocks;
using HealthcareInventoryManagementSystem.Application.DTOs.Suppliers;
using HealthcareInventoryManagementSystem.Domain.Entities;

namespace HealthcareInventoryManagementSystem.Application.Mappings
{
    // Defines mappings between stock entities and DTOs.
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
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

            // Includes batch number, quantity, unit price, and expiry date.
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

            // Maps a Supplier entity to the response DTO.
            CreateMap<Supplier, SupplierDto>();

            // Maps supplier request data to a Supplier entity.
            CreateMap<SupplierRequestDto, Supplier>();
        }
    }
}
