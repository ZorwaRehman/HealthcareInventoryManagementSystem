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

            // Register the generic repository for all entity types.
            services.AddScoped(
                typeof(IGenericRepository<>),
                typeof(GenericRepository<>));

            // Register the item service.
            services.AddScoped<IItemService, ItemService>();

            // Register the password-hashing service.
            services.AddScoped<IPasswordHasherService, PasswordHasherService>();


            // Register the JWT Service
            services.AddScoped<ITokenService, JwtTokenService>();

            // Register the authentication service.
            services.AddScoped<IAuthService, AuthService>();
            return services;
        }
    }
}
