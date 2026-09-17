using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Moq;
using NuciAPI.Client;
using NuciAPI.Responses;
using NUnit.Framework;
using PersonalLogManagerClient.Layout;
using PersonalLogManagerClient.Models;
using PersonalLogManagerClient.Pages;
using PersonalLogManagerClient.Services;

namespace PersonalLogManagerClient.UnitTests
{
    [TestFixture]
    public sealed class HomeTests
    {
        private BunitContext testContext = null!;
        private Mock<INuciApiClient> client = null!;
        private LocaleService localeService = null!;
        private PageTitleService pageTitleService = null!;
        private IEnumerable<string> calendarLogs = [];
        private IEnumerable<string> searchLogs = [];

        [SetUp]
        public async Task SetUp()
        {
            testContext = new BunitContext();
            testContext.JSInterop.Mode = JSRuntimeMode.Loose;
            ConfigureStorage("api-key", "false", "false");

            client = new Mock<INuciApiClient>();
            client.Setup(api => api.SendRequestAsync<GetLogsRequest, GetLogsResponse>(
                    HttpMethod.Get,
                    It.IsAny<GetLogsRequest>(),
                    It.IsAny<NuciApiRequestAuthorisationInfo>(),
                    "/PersonalLog"))
                .ReturnsAsync((HttpMethod method, GetLogsRequest request, NuciApiRequestAuthorisationInfo authorisation, string path) =>
                    new GetLogsResponse
                    {
                        Logs = request.Date is null ? searchLogs.ToList() : calendarLogs.ToList()
                    });
            client.Setup(api => api.SendRequestAsync<GetLogByIdRequest, GetLogByIdResponse>(
                    HttpMethod.Get,
                    It.IsAny<GetLogByIdRequest>(),
                    It.IsAny<NuciApiRequestAuthorisationInfo>(),
                    It.IsAny<string>()))
                .ReturnsAsync(new GetLogByIdResponse { Id = "L4" });

            testContext.Services.AddSingleton<ApiKeyService>(serviceProvider =>
                new ApiKeyService(serviceProvider.GetRequiredService<IJSRuntime>()));
            testContext.Services.AddSingleton<LocaleService>(serviceProvider =>
                new LocaleService(serviceProvider.GetRequiredService<IJSRuntime>()));
            testContext.Services.AddSingleton<PageTitleService>();
            testContext.Services.AddSingleton<ApiKeyRateLimitService>();
            testContext.Services.AddSingleton<PersonalLogService>(serviceProvider =>
                new PersonalLogService(
                    client.Object,
                    serviceProvider.GetRequiredService<ApiKeyService>(),
                    serviceProvider.GetRequiredService<LocaleService>(),
                    serviceProvider.GetRequiredService<ApiKeyRateLimitService>()));

            localeService = testContext.Services.GetRequiredService<LocaleService>();
            pageTitleService = testContext.Services.GetRequiredService<PageTitleService>();

            await localeService.SetLocaleAsync(LocaleService.English);
        }

        [TearDown]
        public void TearDown() => testContext.Dispose();

        [Test]
        public void GivenTheHomePage_WhenRendering_ThenCalendarIsTheDefaultView()
        {
            calendarLogs = ["L4 2026-09-17: 08:00 Visited Solara"];

            IRenderedComponent<Home> component = testContext.Render<Home>();

            Assert.That(component.Find(".btn-view-mode.is-active").TextContent, Is.EqualTo("Calendar"));
            Assert.That(component.FindAll(".calendar-controls"), Has.Count.EqualTo(1));
            Assert.That(component.FindAll(".search-controls"), Is.Empty);
            Assert.That(component.FindAll(".entry-date"), Is.Empty);
            Assert.That(component.Find(".entry-item").TextContent.Trim(), Is.EqualTo("08:00 Visited Solara"));
            Assert.That(pageTitleService.Title, Is.EqualTo(LocalisationStrings.English.TitleToday));
        }

        [Test]
        public void GivenNoApiKey_WhenRendering_ThenTheApiKeyNoticeIsDisplayedWithoutRequestingEntries()
        {
            ConfigureStorage("", "false", "false");

            IRenderedComponent<Home> component = testContext.Render<Home>();

            Assert.That(component.Find(".entries-notice").TextContent, Is.EqualTo(LocalisationStrings.English.NoApiKeyNotice));
            Assert.That(client.Invocations, Is.Empty);
        }

        [Test]
        public void GivenAStoredFullWidthPreference_WhenRenderingAndTogglingWidth_ThenBothWidthStatesAreDisplayed()
        {
            ConfigureStorage("api-key", "false", "true");
            IRenderedComponent<Home> component = testContext.Render<Home>();

            Assert.That(component.Find(".entries-container").ClassList, Does.Contain("is-full-width"));
            Assert.That(component.Find(".btn-fullwidth i").ClassList, Does.Contain("fa-compress"));

            component.Find(".btn-fullwidth").Click();

            Assert.That(component.Find(".entries-container").ClassList, Does.Not.Contain("is-full-width"));
            Assert.That(component.Find(".btn-fullwidth i").ClassList, Does.Contain("fa-expand"));
        }

        [Test]
        public void GivenAPendingCalendarRequest_WhenRendering_ThenLoadingControlsAndNoticeAreDisplayed()
        {
            TaskCompletionSource<NuciApiResponse> responseCompletion = new();
            client.Setup(api => api.SendRequestAsync<GetLogsRequest, GetLogsResponse>(
                    HttpMethod.Get,
                    It.Is<GetLogsRequest>(request => request.Date != null),
                    It.IsAny<NuciApiRequestAuthorisationInfo>(),
                    "/PersonalLog"))
                .Returns(responseCompletion.Task);

            IRenderedComponent<Home> component = testContext.Render<Home>();

            Assert.That(component.Find(".entries-notice").TextContent, Is.EqualTo(LocalisationStrings.English.Loading));
            Assert.That(component.Find(".btn-refresh").HasAttribute("disabled"));
            Assert.That(component.Find(".btn-refresh i").ClassList, Does.Contain("fa-spinner"));

            responseCompletion.SetResult(new GetLogsResponse());
            component.WaitForAssertion(() =>
                Assert.That(component.Find(".entries-notice").TextContent, Does.StartWith("No entries for")));
        }

        [Test]
        public void GivenTheCalendarView_WhenNavigatingToYesterdayAndBack_ThenTitlesAndRequestsAreUpdated()
        {
            IRenderedComponent<Home> component = testContext.Render<Home>();

            component.Find(".btn-nav[title='Previous day']").Click();

            Assert.That(pageTitleService.Title, Is.EqualTo(LocalisationStrings.English.TitleYesterday));
            component.Find(".btn-nav[title='Next day']").Click();
            Assert.That(pageTitleService.Title, Is.EqualTo(LocalisationStrings.English.TitleToday));
            VerifyCalendarRequest(DateTime.Today.AddDays(-1), Times.Once());
            VerifyCalendarRequest(DateTime.Today, Times.Exactly(2));
        }

        [Test]
        public async Task GivenTheCalendarView_WhenSelectingAnEarlierDate_ThenTheEntriesTitleIsDisplayed()
        {
            IRenderedComponent<Home> component = testContext.Render<Home>();
            IRenderedComponent<CustomDatePicker> datePicker = component.FindComponent<CustomDatePicker>();
            string selectedDate = DateTime.Today.AddDays(-8).ToString("yyyy-MM-dd");

            await component.InvokeAsync(() => datePicker.Instance.ValueChanged.InvokeAsync(selectedDate));

            Assert.That(pageTitleService.Title, Is.EqualTo(LocalisationStrings.English.TitleEntries));
            VerifyCalendarRequest(DateTime.Today.AddDays(-8), Times.Once());
        }

        [Test]
        public async Task GivenInvalidCalendarSelections_WhenChangingTheDate_ThenEntriesAreNotReloaded()
        {
            IRenderedComponent<Home> component = testContext.Render<Home>();
            IRenderedComponent<CustomDatePicker> datePicker = component.FindComponent<CustomDatePicker>();

            await component.InvokeAsync(() => datePicker.Instance.ValueChanged.InvokeAsync(""));
            await component.InvokeAsync(() => datePicker.Instance.ValueChanged.InvokeAsync(DateTime.Today.ToString("yyyy-MM-dd")));
            await component.InvokeAsync(() => datePicker.Instance.ValueChanged.InvokeAsync(DateTime.Today.AddDays(1).ToString("yyyy-MM-dd")));

            VerifyCalendarRequests(Times.Once());
        }

        [Test]
        public void GivenTheCalendarView_WhenSelectingSearch_ThenSearchControlsAndTheEmptyPromptAreDisplayed()
        {
            IRenderedComponent<Home> component = testContext.Render<Home>();

            component.FindAll(".btn-view-mode")[1].Click();

            Assert.That(component.Find(".btn-view-mode.is-active").TextContent, Is.EqualTo("Search"));
            Assert.That(component.FindAll(".calendar-controls"), Is.Empty);
            Assert.That(component.FindAll(".search-controls"), Has.Count.EqualTo(1));
            Assert.That(component.Find(".entries-notice").TextContent, Is.EqualTo(LocalisationStrings.English.NoSearchTerm));
            Assert.That(component.Find(".btn-search").HasAttribute("disabled"));
            Assert.That(pageTitleService.Title, Is.EqualTo(LocalisationStrings.English.Search));
            VerifySearchRequests(Times.Never());
        }

        [Test]
        public void GivenTheSearchView_WhenSelectingSearchAgain_ThenTheViewIsNotReloaded()
        {
            IRenderedComponent<Home> component = RenderSearchView();

            component.FindAll(".btn-view-mode")[1].Click();

            Assert.That(component.Find(".entries-notice").TextContent, Is.EqualTo(LocalisationStrings.English.NoSearchTerm));
            VerifySearchRequests(Times.Never());
            VerifyCalendarRequests(Times.Once());
        }

        [Test]
        public void GivenMatchingJournalEntries_WhenSubmittingSearch_ThenTrimmedFinalTextMatchesDisplayTheirDates()
        {
            searchLogs =
            [
                "L4 2026-09-17: 08:00 Visited Bencheamobil",
                "L8 2026-08-16: 09:00 BENCheamobil service",
                "L16 2026-07-15: 10:00 Visited Cornova"
            ];
            IRenderedComponent<Home> component = RenderSearchView();

            component.Find(".search-input").Input("  bencheamobil  ");
            Assert.That(component.Find(".btn-search").HasAttribute("disabled"), Is.False);
            component.Find(".search-controls").Submit();

            IReadOnlyList<AngleSharp.Dom.IElement> entries = component.FindAll(".entry-item");
            Assert.That(entries, Has.Count.EqualTo(2));
            Assert.That(entries.Select(entry => entry.TextContent.Trim()), Is.EqualTo(new[]
            {
                "2026-08-1609:00 BENCheamobil service",
                "2026-09-1708:00 Visited Bencheamobil"
            }));
            Assert.That(component.FindAll(".entry-date").Select(element => element.TextContent),
                Is.EqualTo(new[] { "2026-08-16", "2026-09-17" }));
            Assert.That(component.Find(".entries-count").TextContent, Is.EqualTo("2 log entries"));
            VerifySearchRequests(Times.Once());
        }

        [Test]
        public void GivenNoMatchingJournalEntries_WhenSubmittingSearch_ThenTheTermIsIncludedInTheNotice()
        {
            searchLogs = ["L4 2026-09-17: 08:00 Visited Solara"];
            IRenderedComponent<Home> component = RenderSearchView();

            SubmitSearch(component, "Bencheamobil");

            Assert.That(
                component.Find(".entries-notice").TextContent,
                Is.EqualTo(LocalisationStrings.English.NoSearchResults("Bencheamobil")));
        }

        [Test]
        public void GivenTheApiRejectsASearch_WhenSubmittingSearch_ThenTheLocalisedErrorIsDisplayed()
        {
            client.Setup(api => api.SendRequestAsync<GetLogsRequest, GetLogsResponse>(
                    HttpMethod.Get,
                    It.Is<GetLogsRequest>(request => request.Date == null),
                    It.IsAny<NuciApiRequestAuthorisationInfo>(),
                    "/PersonalLog"))
                .ReturnsAsync(new NuciApiErrorResponse { Code = "UNAUTHORISED" });
            IRenderedComponent<Home> component = RenderSearchView();

            SubmitSearch(component, "Bencheamobil");

            Assert.That(component.Find(".entries-error").TextContent, Is.EqualTo(LocalisationStrings.English.InvalidApiKey));
            Assert.That(component.FindAll(".entry-item"), Is.Empty);
        }

        [Test]
        public void GivenSearchResults_WhenChangingTheOrder_ThenEntriesAreReloadedInAscendingOrder()
        {
            searchLogs =
            [
                "L4 2026-08-16: 08:00 Bencheamobil service",
                "L8 2026-09-17: 09:00 Visited Bencheamobil"
            ];
            IRenderedComponent<Home> component = RenderSearchView();
            SubmitSearch(component, "Bencheamobil");

            component.Find(".btn-sort").Click();

            Assert.That(
                component.FindAll(".entry-date").Select(element => element.TextContent),
                Is.EqualTo(new[] { "2026-08-16", "2026-09-17" }));
            VerifySearchRequests(Times.Exactly(2));
        }

        [Test]
        public void GivenAnAppliedSearch_WhenEditingTheInputAndRefreshing_ThenTheAppliedTermIsRetained()
        {
            searchLogs =
            [
                "L4 2026-09-17: 08:00 Visited Bencheamobil",
                "L8 2026-09-16: 09:00 Visited Cornova"
            ];
            IRenderedComponent<Home> component = RenderSearchView();
            SubmitSearch(component, "Bencheamobil");

            component.Find(".search-input").Input("Cornova");
            component.Find(".btn-refresh").Click();

            Assert.That(component.Find(".entry-item").TextContent, Does.Contain("Bencheamobil"));
            Assert.That(component.Markup, Does.Not.Contain("Visited Cornova"));
            VerifySearchRequests(Times.Exactly(2));
        }

        [Test]
        public void GivenTheSearchView_WhenReturningToCalendar_ThenCalendarEntriesAreReloadedWithoutDates()
        {
            calendarLogs = ["L4 2026-09-17: 08:00 Visited Solara"];
            IRenderedComponent<Home> component = RenderSearchView();

            component.FindAll(".btn-view-mode")[0].Click();

            Assert.That(component.Find(".btn-view-mode.is-active").TextContent, Is.EqualTo("Calendar"));
            Assert.That(component.FindAll(".entry-date"), Is.Empty);
            Assert.That(component.Find(".entry-item").TextContent.Trim(), Is.EqualTo("08:00 Visited Solara"));
            VerifyCalendarRequests(Times.Exactly(2));
        }

        [Test]
        public async Task GivenTheSearchView_WhenChangingLocale_ThenTheSearchTitleAndResultsAreReloaded()
        {
            searchLogs = ["L4 2026-09-17: 08:00 Bencheamobil"];
            IRenderedComponent<Home> component = RenderSearchView();
            SubmitSearch(component, "Bencheamobil");

            await component.InvokeAsync(() => localeService.SetLocaleAsync(LocaleService.Romanian));

            component.WaitForAssertion(() =>
            {
                Assert.That(pageTitleService.Title, Is.EqualTo(LocalisationStrings.Romanian.Search));
                Assert.That(component.Find(".entries-count").TextContent, Is.EqualTo("1 intrări în jurnal"));
            });
            VerifySearchRequests(Times.Exactly(2));
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task GivenASelectedSearchResult_WhenAMutationCompletes_ThenSearchResultsAreReloaded(bool entryWasDeleted)
        {
            searchLogs = ["L4 2026-09-17: 08:00 Bencheamobil"];
            IRenderedComponent<Home> component = RenderSearchView();
            SubmitSearch(component, "Bencheamobil");
            component.Find(".entry-item").Click();
            IRenderedComponent<EntryDetailPanel> panel = component.FindComponent<EntryDetailPanel>();

            if (entryWasDeleted)
            {
                await component.InvokeAsync(() => panel.Instance.OnDeleted.InvokeAsync());
            }
            else
            {
                await component.InvokeAsync(() => panel.Instance.OnEdited.InvokeAsync());
            }

            Assert.That(component.FindAll(".entry-detail-panel"), Is.Empty);
            Assert.That(component.Find(".entry-item").TextContent, Does.Contain("Bencheamobil"));
            VerifySearchRequests(Times.Exactly(2));
        }

        [Test]
        public void GivenASelectedSearchResult_WhenClosingThePanel_ThenTheSelectionIsCleared()
        {
            searchLogs = ["L4 2026-09-17: 08:00 Bencheamobil"];
            IRenderedComponent<Home> component = RenderSearchView();
            SubmitSearch(component, "Bencheamobil");
            component.Find(".entry-item").Click();

            component.Find(".btn-close-panel").Click();

            Assert.That(component.FindAll(".entry-detail-panel"), Is.Empty);
            Assert.That(component.Find(".entry-item").ClassList, Does.Not.Contain("is-selected"));
        }

        private IRenderedComponent<Home> RenderSearchView()
        {
            IRenderedComponent<Home> component = testContext.Render<Home>();
            component.FindAll(".btn-view-mode")[1].Click();

            return component;
        }

        private static void SubmitSearch(IRenderedComponent<Home> component, string searchTerm)
        {
            component.Find(".search-input").Input(searchTerm);
            component.Find(".search-controls").Submit();
        }

        private void ConfigureStorage(string apiKey, string sortAscending, string fullWidth)
        {
            testContext.JSInterop.Setup<string>("localStorage.getItem", "sortAscending").SetResult(sortAscending);
            testContext.JSInterop.Setup<string>("localStorage.getItem", "fullWidth").SetResult(fullWidth);
            testContext.JSInterop.Setup<string>("localStorage.getItem", "plm_api_key").SetResult(apiKey);
        }

        private void VerifySearchRequests(Times times)
            => client.Verify(api => api.SendRequestAsync<GetLogsRequest, GetLogsResponse>(
                HttpMethod.Get,
                It.Is<GetLogsRequest>(request => request.Date == null && request.Count == 100000),
                It.IsAny<NuciApiRequestAuthorisationInfo>(),
                "/PersonalLog"), times);

        private void VerifyCalendarRequests(Times times)
            => client.Verify(api => api.SendRequestAsync<GetLogsRequest, GetLogsResponse>(
                HttpMethod.Get,
                It.Is<GetLogsRequest>(request => request.Date == DateTime.Today.ToString("yyyy-MM-dd") && request.Count == 1000),
                It.IsAny<NuciApiRequestAuthorisationInfo>(),
                "/PersonalLog"), times);

        private void VerifyCalendarRequest(DateTime date, Times times)
            => client.Verify(api => api.SendRequestAsync<GetLogsRequest, GetLogsResponse>(
                HttpMethod.Get,
                It.Is<GetLogsRequest>(request => request.Date == date.ToString("yyyy-MM-dd") && request.Count == 1000),
                It.IsAny<NuciApiRequestAuthorisationInfo>(),
                "/PersonalLog"), times);
    }
}