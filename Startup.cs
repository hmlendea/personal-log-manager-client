using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PersonalLogManagerClient.Configuration;

namespace PersonalLogManagerClient
{
    public static class Startup
    {
        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddRazorComponents()
                .AddInteractiveServerComponents();

            services
                .AddConfigurations(configuration)
                .AddCustomServices();
        }

        public static void Configure(WebApplication app)
        {
            ServerSettings serverSettings = app.Services.GetRequiredService<ServerSettings>();

            if (!string.IsNullOrEmpty(serverSettings.PathBase))
                app.UsePathBase(serverSettings.PathBase);

            app.UseStaticFiles();
            app.UseRouting();
            app.UseAntiforgery();

            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();
        }
    }
}
