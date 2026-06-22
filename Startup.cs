using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PersonalLogManagerClient.Configuration;

namespace PersonalLogManagerClient
{
    public class Startup(IConfiguration configuration)
    {
        public IConfiguration Configuration => configuration;

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorComponents()
                .AddInteractiveServerComponents();

            services
                .AddConfigurations(Configuration)
                .AddCustomServices();
        }

        public void Configure(IApplicationBuilder app)
        {
            ServerSettings serverSettings = app.ApplicationServices.GetRequiredService<ServerSettings>();

            if (!string.IsNullOrEmpty(serverSettings.PathBase))
                app.UsePathBase(serverSettings.PathBase);

            app.UseStaticFiles();
            app.UseRouting();
            app.UseAntiforgery();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorComponents<App>()
                    .AddInteractiveServerRenderMode();
            });
        }
    }
}
