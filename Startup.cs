using System;
using System.Net.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PersonalLogManagerClient.Services;

namespace PersonalLogManagerClient
{
    public class Startup(IConfiguration configuration)
    {
        public IConfiguration Configuration => configuration;

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorComponents()
                .AddInteractiveServerComponents();

            string baseUrl = Configuration["PersonalLogManager:BaseUrl"] ?? "http://localhost:5000";
            services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(baseUrl) });
            services.AddScoped<ApiKeyService>();
            services.AddScoped<PersonalLogService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseStaticFiles();
            app.UseAntiforgery();
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorComponents<App>()
                    .AddInteractiveServerRenderMode();
            });
        }
    }
}
