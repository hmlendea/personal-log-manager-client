using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NuciAPI.Client;
using PersonalLogManagerClient.Configuration;
using PersonalLogManagerClient.Services;

namespace PersonalLogManagerClient
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddConfigurations(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            ServerSettings serverSettings = new();
            PersonalLogManagerSettings personalLogManagerSettings = new();

            configuration.Bind("server", serverSettings);
            configuration.Bind("personalLogManager", personalLogManagerSettings);

            return services
                .AddSingleton(serverSettings)
                .AddSingleton(personalLogManagerSettings);
        }

        public static IServiceCollection AddCustomServices(this IServiceCollection services)
        {
            return services
                .AddScoped<INuciApiClient>(sp => new NuciApiClient(
                    sp.GetRequiredService<PersonalLogManagerSettings>().BaseUrl ?? "http://localhost:5000"))
                .AddScoped<ApiKeyService>()
                .AddScoped<PersonalLogService>();
        }
    }
}
