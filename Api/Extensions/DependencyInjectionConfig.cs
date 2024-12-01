using Application.Interfaces;
using Application.Interfaces.ExternalServices;
using Application.Interfaces.Repositories;
using Application.Interfaces.UseCases;
using Application.UseCases;
using CrossCutting.Helpers;
using Infrastructure.Configurations.Database;
using Infrastructure.Data.Initializer;
using Infrastructure.ExternalServices;
using Infrastructure.Repositories;

namespace Api.Extensions
{
    public static class DependencyInjectionConfig
    {
        public static IServiceCollection AddResolveDependencies(this WebApplicationBuilder builder)
        {
            return AddResolveDependencies(builder.Services, builder.Configuration);
        }

        public static IServiceCollection AddResolveDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IDbConnectionFactory, PostgreSqlConnectionFactory>();

            // Inicializador do banco de dados
            services.AddSingleton<DatabaseInitializer>();

            services.AddScoped<IPaymentRepository, PaymentRepository>();
            services.AddScoped<IMercadoPagoService, MercadoPagoService>();
            services.AddScoped<IOrderService, OrderService>();

            //Product
            services.AddScoped<ICreatePayment, CreatePayment>();
            services.AddScoped<IGetPaymentStatus, GetPaymentStatus>();
            services.AddScoped<IGetApprovedPayments, GetApprovedPayments>();
            services.AddScoped<IHandlePaymentWebhook, HandlePaymentWebhook>();

            services.AddScoped<HmacVerifierHelper>();

            services.AddRouting(options => options.LowercaseUrls = true);
            services.AddHttpClient();

            return services;
        }
    }
}
