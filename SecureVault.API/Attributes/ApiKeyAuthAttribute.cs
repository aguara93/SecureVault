using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SecureVault.API.Data;
using SecureVault.API.Services;
using Microsoft.EntityFrameworkCore;

namespace SecureVault.API.Attributes
{
    /// <summary>
    /// An action filter that authenticates incoming requests using a
    /// sensor-specific API key instead of a JWT. The key is expected
    /// in the X-Api-Key request header and is verified against the
    /// hashed keys stored for each sensor in the database.
    /// </summary>
    public class ApiKeyAuthAttribute : Attribute, IAsyncActionFilter
    {
        private const string ApiKeyHeaderName = "X-Api-Key";

        /// <summary>
        /// Validates the API key present in the request header before
        /// allowing the action to execute. If the key is missing or
        /// does not match any registered sensor, the request is
        /// rejected with a 401 Unauthorized response.
        /// </summary>
        /// <param name="context">The context for the executing action,
        /// used to inspect the request and short-circuit the pipeline
        /// if authentication fails.</param>
        /// <param name="next">The delegate that executes the next
        /// action filter or the action itself.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
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
            var matchingSensor = sensors.FirstOrDefault(s =>
                !string.IsNullOrEmpty(s.ApiKeyHash) &&
                apiKeyService.VerifyApiKey(extractedApiKey!, s.ApiKeyHash));

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
