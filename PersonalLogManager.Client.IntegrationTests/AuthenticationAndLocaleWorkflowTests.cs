using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Bunit;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using NUnit.Framework;
using PersonalLogManagerClient.Layout;
using PersonalLogManagerClient.Services;

namespace PersonalLogManagerClient.IntegrationTests
{
    [TestFixture]
    public sealed class AuthenticationAndLocaleWorkflowTests
    {
        private BunitContext testContext = null!;
        private LocaleService localeService = null!;

        [SetUp]
        public void SetUp()
        {
            testContext = new BunitContext();
            testContext.JSInterop.Mode = JSRuntimeMode.Loose;
            ConfigureStorage("", LocaleService.English);
            testContext.Services.AddSingleton<ApiKeyService>(serviceProvider =>
                new ApiKeyService(serviceProvider.GetRequiredService<IJSRuntime>()));
            testContext.Services.AddSingleton<LocaleService>(serviceProvider =>
                new LocaleService(serviceProvider.GetRequiredService<IJSRuntime>()));
            localeService = testContext.Services.GetRequiredService<LocaleService>();
        }

        [TearDown]
        public void TearDown() => testContext.Dispose();

        [TestCase(LocaleService.English)]
        [TestCase(LocaleService.Romanian)]
        public void GivenASupportedStoredLocale_WhenRenderingTheSelector_ThenThatLocaleIsSelected(string storedLocale)
        {
            ConfigureStorage("", storedLocale);

            IRenderedComponent<LocaleSelector> component = testContext.Render<LocaleSelector>();

            Assert.That(component.Find(".locale-select").GetAttribute("value"), Is.EqualTo(storedLocale));
            Assert.That(localeService.Current, Is.EqualTo(storedLocale));
        }

        [TestCase("")]
        [TestCase("en-US")]
        [TestCase("invalid")]
        public void GivenAnUnsupportedStoredLocale_WhenRenderingTheSelector_ThenRomanianIsSelected(string storedLocale)
        {
            ConfigureStorage("", storedLocale);

            IRenderedComponent<LocaleSelector> component = testContext.Render<LocaleSelector>();

            Assert.That(component.Find(".locale-select").GetAttribute("value"), Is.EqualTo(LocaleService.Romanian));
            Assert.That(localeService.Current, Is.EqualTo(LocaleService.Romanian));
        }

        [Test]
        public void GivenTheEnglishLocale_WhenSelectingRomanian_ThenTheLocalePersistsAndTheSelectorUpdates()
        {
            IRenderedComponent<LocaleSelector> component = testContext.Render<LocaleSelector>();

            component.Find(".locale-select").Change(LocaleService.Romanian);

            Assert.That(localeService.Current, Is.EqualTo(LocaleService.Romanian));
            Assert.That(component.Find(".locale-select").GetAttribute("value"), Is.EqualTo(LocaleService.Romanian));
            AssertStorageInvocation("localStorage.setItem", "plm_locale", LocaleService.Romanian, 1);
        }

        [Test]
        public void GivenTheSelectedLocale_WhenSelectingItAgain_ThenNoStorageWriteOccurs()
        {
            IRenderedComponent<LocaleSelector> component = testContext.Render<LocaleSelector>();

            component.Find(".locale-select").Change(LocaleService.English);

            AssertStorageInvocation("localStorage.setItem", "plm_locale", LocaleService.English, 0);
        }

        [Test]
        public async Task GivenRenderedLocaleSubscribers_WhenChangingLocale_ThenTheWidgetAndFooterRerender()
        {
            IRenderedComponent<ApiKeyWidget> widget = testContext.Render<ApiKeyWidget>();
            IRenderedComponent<AppFooter> footer = testContext.Render<AppFooter>();
            await localeService.SetLocaleAsync(LocaleService.English);

            Assert.That(widget.Find(".btn-authenticate").GetAttribute("title"), Is.EqualTo(LocalisationStrings.English.Authenticate));
            Assert.That(footer.FindAll(".footer-link").Select(link => link.TextContent), Is.EqualTo(new[]
            {
                LocalisationStrings.English.FooterApiSource,
                LocalisationStrings.English.FooterClientSource
            }));

            await localeService.SetLocaleAsync(LocaleService.Romanian);

            Assert.That(widget.Find(".btn-authenticate").GetAttribute("title"), Is.EqualTo(LocalisationStrings.Romanian.Authenticate));
            Assert.That(footer.FindAll(".footer-link").Select(link => link.TextContent), Is.EqualTo(new[]
            {
                LocalisationStrings.Romanian.FooterApiSource,
                LocalisationStrings.Romanian.FooterClientSource
            }));
        }

        [Test]
        public async Task GivenDisposedLocaleSubscribers_WhenChangingLocale_ThenNoRenderingExceptionOccurs()
        {
            IRenderedComponent<ApiKeyWidget> widget = testContext.Render<ApiKeyWidget>();
            IRenderedComponent<AppFooter> footer = testContext.Render<AppFooter>();
            widget.Dispose();
            footer.Dispose();

            Assert.That(
                async () => await localeService.SetLocaleAsync(LocaleService.Romanian),
                Throws.Nothing);
        }

        [Test]
        public async Task GivenNoApiKey_WhenOpeningTheDialog_ThenLocalizedAccessibleFieldsAreRendered()
        {
            await localeService.SetLocaleAsync(LocaleService.English);
            IRenderedComponent<ApiKeyWidget> component = testContext.Render<ApiKeyWidget>();

            component.Find(".btn-authenticate").Click();

            Assert.That(component.Find("[role='dialog']").GetAttribute("aria-modal"), Is.EqualTo("true"));
            Assert.That(component.Find(".auth-popup-title").TextContent, Is.EqualTo(LocalisationStrings.English.Authenticate));
            Assert.That(component.Find(".auth-popup-label").TextContent, Is.EqualTo(LocalisationStrings.English.ApiKeyPlaceholder));
            Assert.That(component.Find(".auth-popup-input").GetAttribute("type"), Is.EqualTo("password"));
        }

        [Test]
        public void GivenAnOpenAuthenticationDialog_WhenPressingEscape_ThenTheDialogClosesWithoutStorageChanges()
        {
            IRenderedComponent<ApiKeyWidget> component = OpenAuthenticationDialog();

            component.Find("[role='dialog']").KeyDown(new KeyboardEventArgs { Key = "Escape" });

            Assert.That(component.FindAll("[role='dialog']"), Is.Empty);
            AssertStorageInvocation("localStorage.setItem", "plm_api_key", null, 0);
        }

        [Test]
        public void GivenAnOpenAuthenticationDialog_WhenClickingTheBackdrop_ThenTheDialogCloses()
        {
            IRenderedComponent<ApiKeyWidget> component = OpenAuthenticationDialog();

            component.Find(".auth-popup-backdrop").Click();

            Assert.That(component.FindAll("[role='dialog']"), Is.Empty);
        }

        [TestCase("")]
        [TestCase(" ")]
        [TestCase("\t")]
        public async Task GivenBlankApiKeyInput_WhenPressingEnter_ThenLocalizedValidationIsRendered(string apiKey)
        {
            await localeService.SetLocaleAsync(LocaleService.English);
            IRenderedComponent<ApiKeyWidget> component = OpenAuthenticationDialog();
            component.Find(".auth-popup-input").Input(apiKey);

            component.Find("[role='dialog']").KeyDown(new KeyboardEventArgs { Key = "Enter" });

            Assert.That(component.Find("[role='alert']").TextContent, Is.EqualTo(LocalisationStrings.English.ApiKeyRequired));
            AssertStorageInvocation("localStorage.setItem", "plm_api_key", null, 0);
        }

        [Test]
        public void GivenAnApiKeyWithWhitespace_WhenPressingEnter_ThenTheTrimmedKeyPersistsAndTheDialogCloses()
        {
            IRenderedComponent<ApiKeyWidget> component = OpenAuthenticationDialog();
            component.Find(".auth-popup-input").Input("  NucileRullz!  ");

            component.Find("[role='dialog']").KeyDown(new KeyboardEventArgs { Key = "Enter" });

            Assert.That(component.FindAll("[role='dialog']"), Is.Empty);
            Assert.That(component.FindAll(".btn-clear-key"), Has.Count.EqualTo(1));
            AssertStorageInvocation("localStorage.setItem", "plm_api_key", "NucileRullz!", 1);
        }

        [Test]
        public void GivenAValidationError_WhenCancellingAndReopening_ThenTheErrorAndInputAreReset()
        {
            IRenderedComponent<ApiKeyWidget> component = OpenAuthenticationDialog();
            component.Find(".auth-popup-input").Input(" ");
            component.Find(".btn-auth-confirm").Click();
            Assert.That(component.FindAll("[role='alert']"), Has.Count.EqualTo(1));

            component.Find(".btn-auth-cancel").Click();
            component.Find(".btn-authenticate").Click();

            Assert.That(component.FindAll("[role='alert']"), Is.Empty);
            Assert.That(component.Find(".auth-popup-input").GetAttribute("value"), Is.Empty);
        }

        [Test]
        public void GivenAStoredApiKey_WhenClearingIt_ThenStorageIsRemovedAndAuthenticationReturns()
        {
            ConfigureStorage("NucileRullz!", LocaleService.English);
            IRenderedComponent<ApiKeyWidget> component = testContext.Render<ApiKeyWidget>();

            component.Find(".btn-clear-key").Click();

            AssertStorageInvocation("localStorage.removeItem", "plm_api_key", null, 1);
            Assert.That(component.FindAll(".btn-authenticate"), Has.Count.EqualTo(1));
            Assert.That(component.FindAll(".btn-clear-key"), Is.Empty);
        }

        private IRenderedComponent<ApiKeyWidget> OpenAuthenticationDialog()
        {
            IRenderedComponent<ApiKeyWidget> component = testContext.Render<ApiKeyWidget>();
            component.Find(".btn-authenticate").Click();

            return component;
        }

        private void ConfigureStorage(string apiKey, string locale)
        {
            testContext.JSInterop.Setup<string>("localStorage.getItem", "plm_api_key").SetResult(apiKey);
            testContext.JSInterop.Setup<string>("localStorage.getItem", "plm_locale").SetResult(locale);
        }

        private void AssertStorageInvocation(
            string identifier,
            string storageKey,
            string? value,
            int expectedCount)
        {
            IEnumerable<JSRuntimeInvocation> invocations = testContext.JSInterop.Invocations.Where(invocation =>
                string.Equals(invocation.Identifier, identifier) &&
                invocation.Arguments.Count > 0 &&
                string.Equals(invocation.Arguments[0]?.ToString(), storageKey));

            if (value is not null)
            {
                invocations = invocations.Where(invocation =>
                    invocation.Arguments.Count > 1 &&
                    string.Equals(invocation.Arguments[1]?.ToString(), value));
            }

            Assert.That(invocations.ToList(), Has.Count.EqualTo(expectedCount));
        }
    }
}
