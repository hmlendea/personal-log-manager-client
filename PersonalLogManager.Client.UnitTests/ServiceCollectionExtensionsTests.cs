using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NuciAPI.Client;
using NUnit.Framework;
using PersonalLogManagerClient.Configuration;
using PersonalLogManagerClient.Services;

namespace PersonalLogManagerClient.UnitTests
{
    [TestFixture]
    public sealed class ServiceCollectionExtensionsTests
    {
        [Test]
        public void GivenConfigurationValues_WhenAddingConfigurations_ThenBothSettingsObjectsAreBound()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["server:pathBase"] = "/logs",
                    ["personalLogManager:baseUrl"] = "https://localhost:5001"
                })
                .Build();
            ServiceProvider provider = new ServiceCollection()
                .AddConfigurations(configuration)
                .BuildServiceProvider();

            Assert.That(provider.GetRequiredService<ServerSettings>().PathBase, Is.EqualTo("/logs"));
            Assert.That(provider.GetRequiredService<PersonalLogManagerSettings>().BaseUrl, Is.EqualTo("https://localhost:5001"));
        }

        [Test]
        public void GivenAServiceCollection_WhenAddingCustomServices_ThenAllCustomServiceDescriptorsAreRegistered()
        {
            IServiceCollection services = new ServiceCollection();

            services.AddCustomServices();

            Assert.That(services.Any(descriptor => descriptor.ServiceType == typeof(INuciApiClient)), Is.True);
            Assert.That(services.Any(descriptor => descriptor.ServiceType == typeof(ApiKeyService)), Is.True);
            Assert.That(services.Any(descriptor => descriptor.ServiceType == typeof(ApiKeyRateLimitService)), Is.True);
            Assert.That(services.Any(descriptor => descriptor.ServiceType == typeof(LocaleService)), Is.True);
            Assert.That(services.Any(descriptor => descriptor.ServiceType == typeof(PageTitleService)), Is.True);
            Assert.That(services.Any(descriptor => descriptor.ServiceType == typeof(PersonalLogService)), Is.True);
        }
    }
}
