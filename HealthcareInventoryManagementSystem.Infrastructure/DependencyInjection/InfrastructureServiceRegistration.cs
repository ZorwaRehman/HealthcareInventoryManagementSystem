
using HealthcareInventoryManagementSystem.Application.Interfaces;
using HealthcareInventoryManagementSystem.Application.Interfaces.Repositories;
using HealthcareInventoryManagementSystem.Application.Interfaces.Services;
using HealthcareInventoryManagementSystem.Application.Services;
using HealthcareInventoryManagementSystem.Infrastructure.Persistence;
using HealthcareInventoryManagementSystem.Infrastructure.Repositories;
using HealthcareInventoryManagementSystem.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

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

            //// Register AutoMapper profiles.
            //services.AddAutoMapper(
            //    configuration => { },
            //    typeof(MappingProfile).Assembly);

            // Register the generic repository.
            services.AddScoped(
                typeof(IGenericRepository<>),
                typeof(GenericRepository<>));

            // Register Item service.
            services.AddScoped<IItemService, ItemService>();

            // Register Item Image Storage service.
            services.AddScoped<
                IItemImageStorageService,
                ItemImageStorageService>();

            // Register authentication services.
            services.AddScoped<IPasswordHasherService, PasswordHasherService>();
            services.AddScoped<ITokenService, JwtTokenService>();
            services.AddScoped<IAuthService, AuthService>();

            return services;
        }
    }
}
