using System;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
            string baseUrl = services
                .BuildServiceProvider()
                .GetRequiredService<PersonalLogManagerSettings>()
                .BaseUrl ?? "http://localhost:5000";

            return services
                .AddScoped(_ => new HttpClient { BaseAddress = new Uri(baseUrl) })
                .AddScoped<ApiKeyService>()
                .AddScoped<PersonalLogService>();
        }
    }
}
