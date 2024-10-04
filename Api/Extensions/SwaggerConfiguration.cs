using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Api.Extensions
{
    public static class SwaggerConfiguration
    {
        /// <summary>
        ///     Apply the default Swagger configuration on the container
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <param name="applicationName">The service name</param>
        /// <param name="applicationDescription">The service description</param>
        /// <returns>The updated service collection</returns>
        public static IServiceCollection ApplySwaggerConfiguration(this IServiceCollection services, string applicationName, string applicationDescription)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc(
                    name: "v1",
                    info: new OpenApiInfo
                    {
                        Version = "v1",
                        Title = applicationName,
                        Description = applicationDescription,
                    });
                options.DocumentFilter<HealthChecksFilter>();
            });

            services.AddHealthChecks();

            return services;
        }
    }

    public class HealthChecksFilter : IDocumentFilter
    {
        public void Apply(OpenApiDocument openApiDocument, DocumentFilterContext context)
        {
            var operation = new OpenApiOperation { Summary = "Endpoint para consultar o status do serviço." };
            operation.Tags.Add(new OpenApiTag { Name = "HealthCheck" });

            var healthyResponse = new OpenApiResponse();
            var healtySchema = new OpenApiSchema
            { Title = nameof(HealthStatus), Example = new OpenApiString(nameof(HealthStatus.Healthy)) };
            healthyResponse.Content.Add("text/plain", new OpenApiMediaType { Schema = healtySchema });
            healthyResponse.Description = "API está disponível.";
            operation.Responses.Add("200", healthyResponse);

            var unhealtySchema = new OpenApiSchema
            { Title = nameof(HealthStatus), Example = new OpenApiString(nameof(HealthStatus.Unhealthy)) };
            var unhealthyResponse = new OpenApiResponse();
            unhealthyResponse.Content.Add("text/plain", new OpenApiMediaType { Schema = unhealtySchema });
            unhealthyResponse.Description = "API está indisponível.";
            operation.Responses.Add("503", unhealthyResponse);

            var pathItem = new OpenApiPathItem();
            pathItem.AddOperation(OperationType.Get, operation);

            openApiDocument?.Paths.Add("/api/v1/health", pathItem);
        }
    }
}
