using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.JSInterop;
using Moq;
using NuciAPI.Client;
using NUnit.Framework;
using PersonalLogManagerClient.Configuration;
using PersonalLogManagerClient.Services;

namespace PersonalLogManagerClient.IntegrationTests
{
    [TestFixture]
    public sealed class HostAndCompositionIntegrationTests
    {
        private WebApplicationFactory<Program> applicationFactory = null!;

        [SetUp]
        public void SetUp() => applicationFactory = new WebApplicationFactory<Program>();

        [TearDown]
        public void TearDown() => applicationFactory.Dispose();

        [Test]
        public void GivenConfiguredSections_WhenAddingConfigurations_ThenValuesAreBoundAndRegistered()
        {
            Dictionary<string, string?> values = new()
            {
                ["server:pathBase"] = "/journal",
                ["personalLogManager:baseUrl"] = "https://test.nucilandia.ro"
            };
            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(values)
                .Build();
            ServiceCollection services = new();

            services.AddConfigurations(configuration);
            using ServiceProvider serviceProvider = services.BuildServiceProvider();

            Assert.That(serviceProvider.GetRequiredService<ServerSettings>().PathBase, Is.EqualTo("/journal"));
            Assert.That(
                serviceProvider.GetRequiredService<PersonalLogManagerSettings>().BaseUrl,
                Is.EqualTo("https://test.nucilandia.ro"));
        }

        [Test]
        public void GivenMissingConfigurationSections_WhenAddingConfigurations_ThenSettingsStillResolve()
        {
            IConfiguration configuration = new ConfigurationBuilder().Build();
            ServiceCollection services = new();

            services.AddConfigurations(configuration);
            using ServiceProvider serviceProvider = services.BuildServiceProvider();

            Assert.That(serviceProvider.GetRequiredService<ServerSettings>(), Is.Not.Null);
            Assert.That(serviceProvider.GetRequiredService<PersonalLogManagerSettings>(), Is.Not.Null);
        }

        [Test]
        public void GivenTheProductionRegistrations_WhenResolvingOneScope_ThenEveryServiceResolves()
        {
            ServiceProvider serviceProvider = BuildProductionServiceProvider();
            using IServiceScope scope = serviceProvider.CreateScope();

            Assert.That(scope.ServiceProvider.GetRequiredService<INuciApiClient>(), Is.Not.Null);
            Assert.That(scope.ServiceProvider.GetRequiredService<ApiKeyService>(), Is.Not.Null);
            Assert.That(scope.ServiceProvider.GetRequiredService<ApiKeyRateLimitService>(), Is.Not.Null);
            Assert.That(scope.ServiceProvider.GetRequiredService<LocaleService>(), Is.Not.Null);
            Assert.That(scope.ServiceProvider.GetRequiredService<PageTitleService>(), Is.Not.Null);
            Assert.That(scope.ServiceProvider.GetRequiredService<PersonalLogService>(), Is.Not.Null);

            serviceProvider.Dispose();
        }

        [TestCase(typeof(INuciApiClient))]
        [TestCase(typeof(ApiKeyService))]
        [TestCase(typeof(ApiKeyRateLimitService))]
        [TestCase(typeof(LocaleService))]
        [TestCase(typeof(PageTitleService))]
        [TestCase(typeof(PersonalLogService))]
        public void GivenAScopedRegistration_WhenResolvingTwiceInOneScope_ThenTheSameInstanceReturns(Type serviceType)
        {
            ServiceProvider serviceProvider = BuildProductionServiceProvider();
            using IServiceScope scope = serviceProvider.CreateScope();

            object first = scope.ServiceProvider.GetRequiredService(serviceType);
            object second = scope.ServiceProvider.GetRequiredService(serviceType);

            Assert.That(second, Is.SameAs(first));
            serviceProvider.Dispose();
        }

        [TestCase(typeof(INuciApiClient))]
        [TestCase(typeof(ApiKeyService))]
        [TestCase(typeof(ApiKeyRateLimitService))]
        [TestCase(typeof(LocaleService))]
        [TestCase(typeof(PageTitleService))]
        [TestCase(typeof(PersonalLogService))]
        public void GivenAScopedRegistration_WhenResolvingSeparateScopes_ThenDifferentInstancesReturn(Type serviceType)
        {
            ServiceProvider serviceProvider = BuildProductionServiceProvider();
            using IServiceScope firstScope = serviceProvider.CreateScope();
            using IServiceScope secondScope = serviceProvider.CreateScope();

            object first = firstScope.ServiceProvider.GetRequiredService(serviceType);
            object second = secondScope.ServiceProvider.GetRequiredService(serviceType);

            Assert.That(second, Is.Not.SameAs(first));
            serviceProvider.Dispose();
        }

        [Test]
        public async Task GivenTheProductionHost_WhenRequestingRoot_ThenTheBlazorDocumentShellIsReturned()
        {
            using HttpClient httpClient = applicationFactory.CreateClient();

            HttpResponseMessage response = await httpClient.GetAsync("/");
            string html = await response.Content.ReadAsStringAsync();

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.Content.Headers.ContentType?.MediaType, Is.EqualTo("text/html"));
            Assert.That(html, Does.Contain("<!DOCTYPE html>"));
            Assert.That(html, Does.Contain("<base href=\"/\""));
            Assert.That(html, Does.Contain("_framework/blazor.web.js"));
        }

        [Test]
        public async Task GivenTheProductionHost_WhenRequestingCss_ThenTheStaticAssetAndContentTypeAreReturned()
        {
            using HttpClient httpClient = applicationFactory.CreateClient();

            HttpResponseMessage response = await httpClient.GetAsync("/css/app.css");
            string css = await response.Content.ReadAsStringAsync();

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.Content.Headers.ContentType?.MediaType, Is.EqualTo("text/css"));
            Assert.That(css, Does.Contain("body"));
        }

        [Test]
        public async Task GivenTheProductionHost_WhenRequestingAFont_ThenTheCustomFontContentTypeIsReturned()
        {
            using HttpClient httpClient = applicationFactory.CreateClient();

            HttpResponseMessage response = await httpClient.GetAsync("/lib/fontawesome/webfonts/fa-solid-900.woff2");

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.Content.Headers.ContentType?.MediaType, Is.EqualTo("font/woff2"));
            Assert.That(response.Content.Headers.ContentLength, Is.GreaterThan(0));
        }

        [Test]
        public async Task GivenAConfiguredPathBase_WhenRequestingThatBase_ThenTheShellUsesTheBaseHref()
        {
            using WebApplicationFactory<Program> pathBaseFactory = applicationFactory.WithWebHostBuilder(builder =>
                builder.ConfigureServices(services =>
                    services.Replace(ServiceDescriptor.Singleton(new ServerSettings
                    {
                        PathBase = "/journal"
                    }))));
            using HttpClient httpClient = pathBaseFactory.CreateClient();

            HttpResponseMessage response = await httpClient.GetAsync("/journal/");
            string html = await response.Content.ReadAsStringAsync();

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(html, Does.Contain("<base href=\"/journal/\""));
        }

        private static ServiceProvider BuildProductionServiceProvider()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["personalLogManager:baseUrl"] = "https://test.nucilandia.ro"
                })
                .Build();
            ServiceCollection services = new();
            services.AddConfigurations(configuration);
            services.AddCustomServices();
            services.AddSingleton(new Mock<IJSRuntime>().Object);

            return services.BuildServiceProvider(new ServiceProviderOptions
            {
                ValidateOnBuild = true,
                ValidateScopes = true
            });
        }
    }
}
