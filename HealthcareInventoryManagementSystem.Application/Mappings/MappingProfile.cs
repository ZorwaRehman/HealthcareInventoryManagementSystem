using AutoMapper;
using HealthcareInventoryManagementSystem.Application.DTOs.Stocks;
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

            // Maps a stock batch to its detailed response DTO.
            // Includes batch number, quantity, unit price, and expiry date.
            CreateMap<StockBatchEntity, StockBatchDto>();

            // Maps a stock transaction entity to its response DTO.
            CreateMap<StockTransactionEntity, StockTransactionDto>();
        }
    }
}
