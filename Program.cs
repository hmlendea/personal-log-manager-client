using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using PersonalLogManagerClient.Services;

namespace PersonalLogManagerClient
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            WebAssemblyHostBuilder builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            string baseUrl = builder.Configuration["PersonalLogManager:BaseUrl"] ?? "http://localhost:5000";
            builder.Services.AddScoped(sp => new HttpClient
            {
                BaseAddress = new Uri(baseUrl)
            });

            builder.Services.AddScoped<ApiKeyService>();
            builder.Services.AddScoped<PersonalLogService>();

            await builder.Build().RunAsync();
        }
    }
}