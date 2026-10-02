using System.Linq;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using NUnit.Framework;
using PersonalLogManagerClient.Layout;
using PersonalLogManagerClient.Services;

namespace PersonalLogManagerClient.UnitTests
{
    [TestFixture]
    public sealed class ApiKeyWidgetTests
    {
        private BunitContext testContext = null!;

        [SetUp]
        public void SetUp()
        {
            testContext = new BunitContext();
            testContext.JSInterop.Mode = JSRuntimeMode.Loose;
            testContext.Services.AddSingleton<ApiKeyService>(serviceProvider =>
                new ApiKeyService(serviceProvider.GetRequiredService<IJSRuntime>()));
            testContext.Services.AddSingleton<LocaleService>(serviceProvider =>
                new LocaleService(serviceProvider.GetRequiredService<IJSRuntime>()));
        }

        [TearDown]
        public void TearDown() => testContext.Dispose();

        [Test]
        public void GivenNoApiKey_WhenOpeningAndCancellingAuthentication_ThenTheDialogIsClosed()
        {
            IRenderedComponent<ApiKeyWidget> component = RenderWidget("");

            component.Find(".btn-authenticate").Click();
            Assert.That(component.FindAll("[role='dialog']"), Has.Count.EqualTo(1));

            component.Find(".btn-auth-cancel").Click();
            Assert.That(component.FindAll("[role='dialog']"), Is.Empty);
        }

        [Test]
        public void GivenNoApiKey_WhenSavingAnEmptyValue_ThenAValidationMessageIsDisplayed()
        {
            IRenderedComponent<ApiKeyWidget> component = RenderWidget("");
            component.Find(".btn-authenticate").Click();

            component.Find(".btn-auth-confirm").Click();

            Assert.That(component.Find("[role='alert']").TextContent, Is.EqualTo(LocalisationStrings.Romanian.ApiKeyRequired));
            Assert.That(GetStorageInvocations("localStorage.setItem", "plm_api_key"), Is.Empty);
        }

        [Test]
        public void GivenAnApiKey_WhenAuthenticating_ThenTheTrimmedKeyIsStoredAndTheDialogIsClosed()
        {
            IRenderedComponent<ApiKeyWidget> component = RenderWidget("");
            component.Find(".btn-authenticate").Click();
            component.Find(".auth-popup-input").Input("  NucileRullz!  ");

            component.Find(".btn-auth-confirm").Click();

            JSRuntimeInvocation invocation = GetStorageInvocations("localStorage.setItem", "plm_api_key").Single();
            Assert.That(invocation.Arguments[1], Is.EqualTo("NucileRullz!"));
            Assert.That(component.FindAll("[role='dialog']"), Is.Empty);
            Assert.That(component.FindAll(".btn-clear-key"), Has.Count.EqualTo(1));
        }

        [Test]
        public void GivenAStoredApiKey_WhenDeauthenticating_ThenTheKeyIsRemoved()
        {
            IRenderedComponent<ApiKeyWidget> component = RenderWidget("NucileRullz!");

            component.Find(".btn-clear-key").Click();

            Assert.That(GetStorageInvocations("localStorage.removeItem", "plm_api_key").Count(), Is.EqualTo(1));
            Assert.That(component.FindAll(".btn-authenticate"), Has.Count.EqualTo(1));
        }

        private IRenderedComponent<ApiKeyWidget> RenderWidget(string apiKey)
        {
            testContext.JSInterop.Setup<string>("localStorage.getItem", "plm_api_key").SetResult(apiKey);

            return testContext.Render<ApiKeyWidget>();
        }

        private System.Collections.Generic.IEnumerable<JSRuntimeInvocation> GetStorageInvocations(
            string identifier,
            string storageKey)
            => testContext.JSInterop.Invocations.Where(invocation =>
                string.Equals(invocation.Identifier, identifier) &&
                invocation.Arguments.Count > 0 &&
                string.Equals(invocation.Arguments[0]?.ToString(), storageKey));
    }
}
