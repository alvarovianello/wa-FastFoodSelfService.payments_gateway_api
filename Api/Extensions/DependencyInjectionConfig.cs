using Application.Interfaces.ExternalServices;
using Application.Interfaces.Repositories;
using Application.Interfaces.UseCases;
using Application.UseCases;
using CrossCutting.Helpers;
using Infrastructure.ExternalServices;
using Infrastructure.Repositories;

namespace Api.Extensions
{
    public static class DependencyInjectionConfig
    {
        public static IServiceCollection AddResolveDependencies(this WebApplicationBuilder builder)
        {
            IServiceCollection services = builder.Services;

            services.AddScoped<IPaymentRepository, PaymentRepository>();
            services.AddScoped<IMercadoPagoService, MercadoPagoService>();
            services.AddScoped<IOrderService, OrderService>();

            //Product
            services.AddScoped<ICreatePayment, CreatePayment>();
            services.AddScoped<IGetPaymentStatus, GetPaymentStatus>();
            services.AddScoped<IGetApprovedPayments, GetApprovedPayments>();
            services.AddScoped<IHandlePaymentWebhook, HandlePaymentWebhook>();

            services.AddScoped<HmacVerifierHelper>();

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                               .AllowAnyMethod()
                               .AllowAnyHeader();
                    });
            });

            services.AddRouting(options => options.LowercaseUrls = true);
            services.AddHttpClient();

            return services;
        }
    }
}
