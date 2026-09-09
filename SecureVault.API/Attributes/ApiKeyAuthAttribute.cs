using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SecureVault.API.Data;
using SecureVault.API.Services;
using Microsoft.EntityFrameworkCore;

namespace SecureVault.API.Attributes
{
    public class ApiKeyAuthAttribute : Attribute, IAsyncActionFilter
    {
        private const string ApiKeyHeaderName = "X-Api-Key";

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.HttpContext.Request.Headers.TryGetValue(ApiKeyHeaderName, out var extractedApiKey))
            {
                context.Result = new UnauthorizedObjectResult("API Key is missing.");
                return;
            }

            var dbContext = context.HttpContext.RequestServices.GetRequiredService<SecureVaultDbContext>();
            var apiKeyService = context.HttpContext.RequestServices.GetRequiredService<ApiKeyService>();

            var sensors = await dbContext.Sensors.ToListAsync();
            var matchingSensor = sensors.FirstOrDefault(s => apiKeyService.VerifyApiKey(extractedApiKey!, s.ApiKeyHash));

            if (matchingSensor == null)
            {
                context.Result = new UnauthorizedObjectResult("Invalid API Key.");
                return;
            }

            // Store the sensor in HttpContext for use in the controller
            context.HttpContext.Items["Sensor"] = matchingSensor;

            await next();
        }
    }
}
