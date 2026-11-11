using HealthcareInventoryManagementSystem.Application.DTOs.PurchaseOrders;
using HealthcareInventoryManagementSystem.Application.Validators.PurchaseOrders;
using FluentValidation;
using HealthcareInventoryManagementSystem.Application.DTOs.Suppliers;
using HealthcareInventoryManagementSystem.Application.Mappings;
using HealthcareInventoryManagementSystem.Application.Interfaces;
using HealthcareInventoryManagementSystem.Application.Interfaces.Repositories;
using HealthcareInventoryManagementSystem.Application.Interfaces.Services;
using HealthcareInventoryManagementSystem.Application.Services;
using HealthcareInventoryManagementSystem.Application.Validators.Suppliers;
using HealthcareInventoryManagementSystem.Infrastructure.Persistence;
using HealthcareInventoryManagementSystem.Infrastructure.Repositories;
using HealthcareInventoryManagementSystem.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using HealthcareInventoryManagementSystem.Application.DTOs.GoodsReceived;
using HealthcareInventoryManagementSystem.Application.Validators;

namespace HealthcareInventoryManagementSystem.Infrastructure.DependencyInjection
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var connectionString =
                configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException(
                    "DefaultConnection is missing from configuration.");

            // Register the SQL Server database context.
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            // Register AutoMapper profiles.
            services.AddAutoMapper(
                configuration => { },
                typeof(MappingProfile).Assembly);

            // Register the generic repository for all entity types.
            services.AddScoped(
                typeof(IGenericRepository<>),
                typeof(GenericRepository<>));

            // Register the item service.
            services.AddScoped<IItemService, ItemService>();

            // Register the item image storage service.
            services.AddScoped<
                IItemImageStorageService,
                ItemImageStorageService>();

            // Register the stock management service.
            services.AddScoped<IStockService, StockService>();

            // Register the alert service.
            services.AddScoped<IAlertService, AlertService>();

            // Register the supplier management service.
            services.AddScoped<ISupplierService, SupplierService>();

            // Register the Purchase Order management service.
            services.AddScoped<IPurchaseOrderService, PurchaseOrderService>();

            // Register the Goods Received management service.
            services.AddScoped<IGoodsReceivedService, GoodsReceivedService>();

            // Register the Supplier request validator.
            services.AddScoped<
                IValidator<SupplierRequestDto>,
                SupplierDtoValidator>();

            // Register the Purchase Order request validator.
            services.AddScoped<
                IValidator<PurchaseOrderRequestDto>,
                PurchaseOrderRequestDtoValidator>();

            // Register the Goods Received request validator.
            services.AddScoped<
                IValidator<CreateGoodsReceivedRequestDto>,
                GoodsReceivedRequestValidator>();

            // Register the database transaction manager.
            services.AddScoped<
                IStockTransactionManager,
                StockTransactionManager>();

            // Register the password-hashing service.
            services.AddScoped<IPasswordHasherService, PasswordHasherService>();

            // Register the JWT service.
            services.AddScoped<ITokenService, JwtTokenService>();

            // Register the authentication service.
            services.AddScoped<IAuthService, AuthService>();

            return services;
        }
    }
}
