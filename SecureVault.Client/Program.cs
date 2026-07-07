using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SecureVault.Client;

namespace SecureVault.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            // Configure HttpClient to use API base URL
            var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "https://localhost:7061";
            builder.Services.AddScoped(sp => new HttpClient 
            { 
                BaseAddress = new Uri(apiBaseUrl) 
            });

            await builder.Build().RunAsync();
        }
    }
}
