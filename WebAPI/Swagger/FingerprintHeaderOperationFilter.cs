using Microsoft.OpenApi.Models;
using SessionSentinel.WebApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace WebAPI.Swagger;

public sealed class FingerprintHeaderOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (!operation.Parameters.Any(parameter =>
                string.Equals(parameter.Name, "X-Sentinel-Fingerprint", StringComparison.OrdinalIgnoreCase)))
        {
            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "X-Sentinel-Fingerprint",
                In = ParameterLocation.Header,
                Required = false,
                Description = "Session-Sentinel fingerprint header. Login ve korumali isteklerde ayni deger kullanilmali.",
                Schema = new OpenApiSchema
                {
                    Type = "string"
                }
            });
        }

        if (!operation.Parameters.Any(parameter =>
                string.Equals(parameter.Name, ClientIpResolver.ForwardedForHeaderName, StringComparison.OrdinalIgnoreCase)))
        {
            operation.Parameters.Add(new OpenApiParameter
            {
                Name = ClientIpResolver.ForwardedForHeaderName,
                In = ParameterLocation.Header,
                Required = false,
                Description = "Localhost testinde istemci IP'sini simule etmek icin kullanilir. Ornek: 1.1.1.1",
                Schema = new OpenApiSchema
                {
                    Type = "string"
                }
            });
        }
    }
}
