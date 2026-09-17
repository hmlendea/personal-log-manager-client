using System;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.JSInterop.Infrastructure;
using Moq;
using NUnit.Framework;
using PersonalLogManagerClient.Services;

namespace PersonalLogManagerClient.UnitTests
{
    [TestFixture]
    public sealed class LocaleServiceTests
    {
        private Mock<IJSRuntime> js = null!;
        private LocaleService service = null!;

        [SetUp]
        public void SetUp()
        {
            js = new Mock<IJSRuntime>();
            service = new LocaleService(js.Object);
        }

        [Test]
        public void GivenARecentlyConstructedService_WhenReadingCurrent_ThenRomanianIsReturned()
            => Assert.That(service.Current, Is.EqualTo(LocaleService.Romanian));

        [Test]
        public void GivenARecentlyConstructedService_WhenReadingStrings_ThenRomanianStringsAreReturned()
            => Assert.That(service.Strings, Is.SameAs(LocalisationStrings.Romanian));

        [Test]
        public async Task GivenEnglishStored_WhenInitialising_ThenEnglishIsSelected()
        {
            js.Setup(runtime => runtime.InvokeAsync<string>(
                    "localStorage.getItem",
                    It.Is<object[]>(arguments => arguments.Length == 1 && (string)arguments[0] == "plm_locale")))
                .Returns(new ValueTask<string>(LocaleService.English));

            await service.InitialiseAsync();

            Assert.That(service.Current, Is.EqualTo(LocaleService.English));
            Assert.That(service.Strings, Is.SameAs(LocalisationStrings.English));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("en")]
        [TestCase("fr-FR")]
        public async Task GivenAnUnsupportedStoredLocale_WhenInitialising_ThenRomanianIsSelected(string storedLocale)
        {
            js.Setup(runtime => runtime.InvokeAsync<string>(
                    "localStorage.getItem",
                    It.IsAny<object[]>()))
                .Returns(new ValueTask<string>(storedLocale));

            await service.InitialiseAsync();

            Assert.That(service.Current, Is.EqualTo(LocaleService.Romanian));
        }

        [Test]
        public async Task GivenAnEnglishLocale_WhenSettingLocale_ThenItIsPersistedAndChangeIsRaised()
        {
            int changeCount = 0;
            service.OnChange += () => changeCount++;
            js.Setup(runtime => runtime.InvokeAsync<IJSVoidResult>(
                    "localStorage.setItem",
                    It.Is<object[]>(arguments => arguments.Length == 2 &&
                        (string)arguments[0] == "plm_locale" &&
                        (string)arguments[1] == LocaleService.English)))
                .Returns(new ValueTask<IJSVoidResult>(Task.FromResult<IJSVoidResult>((IJSVoidResult)null!)));

            await service.SetLocaleAsync(LocaleService.English);

            Assert.That(service.Current, Is.EqualTo(LocaleService.English));
            Assert.That(changeCount, Is.EqualTo(1));
            js.Verify(runtime => runtime.InvokeAsync<IJSVoidResult>("localStorage.setItem", It.IsAny<object[]>()), Times.Once);
        }

        [Test]
        public async Task GivenARomanianLocale_WhenSettingLocale_ThenItIsPersistedAndChangeIsRaised()
        {
            int changeCount = 0;
            service.OnChange += () => changeCount++;
            js.Setup(runtime => runtime.InvokeAsync<IJSVoidResult>("localStorage.setItem", It.IsAny<object[]>()))
                .Returns(new ValueTask<IJSVoidResult>(Task.FromResult<IJSVoidResult>((IJSVoidResult)null!)));

            await service.SetLocaleAsync(LocaleService.Romanian);

            Assert.That(service.Current, Is.EqualTo(LocaleService.Romanian));
            Assert.That(changeCount, Is.EqualTo(1));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("en")]
        [TestCase("fr-FR")]
        public async Task GivenAnUnsupportedLocale_WhenSettingLocale_ThenStateStorageAndEventsRemainUnchanged(string locale)
        {
            int changeCount = 0;
            service.OnChange += () => changeCount++;

            await service.SetLocaleAsync(locale);

            Assert.That(service.Current, Is.EqualTo(LocaleService.Romanian));
            Assert.That(changeCount, Is.Zero);
            js.Verify(runtime => runtime.InvokeAsync<IJSVoidResult>(It.IsAny<string>(), It.IsAny<object[]>()), Times.Never);
        }
    }
}
