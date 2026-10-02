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

namespace PersonalLogManagerClient.IntegrationTests
{
    [TestFixture]
    public sealed class HomeWorkflowIntegrationTests
    {
        private BunitContext testContext = null!;
        private Mock<INuciApiClient> client = null!;
        private LocaleService localeService = null!;
        private List<string> calendarLogs = null!;
        private List<string> searchLogs = null!;

        [SetUp]
        public async Task SetUp()
        {
            testContext = new BunitContext();
            testContext.JSInterop.Mode = JSRuntimeMode.Loose;
            ConfigureStorage("NucileRullz!", "false", "false");
            calendarLogs = [];
            searchLogs = [];

            client = new Mock<INuciApiClient>();
            client.Setup(apiClient => apiClient.SendRequestAsync<GetLogsRequest, GetLogsResponse>(
                    HttpMethod.Get,
                    It.IsAny<GetLogsRequest>(),
                    It.IsAny<NuciApiRequestAuthorisationInfo>(),
                    "/PersonalLog"))
                .ReturnsAsync((HttpMethod method, GetLogsRequest request, NuciApiRequestAuthorisationInfo authorisation, string path) =>
                    new GetLogsResponse
                    {
                        Logs = request.Date is null ? searchLogs : calendarLogs
                    });
            client.Setup(apiClient => apiClient.SendRequestAsync<GetLogByIdRequest, GetLogByIdResponse>(
                    HttpMethod.Get,
                    It.IsAny<GetLogByIdRequest>(),
                    It.IsAny<NuciApiRequestAuthorisationInfo>(),
                    It.IsAny<string>()))
                .ReturnsAsync(new GetLogByIdResponse
                {
                    Id = "L42",
                    Date = "2020-05-21",
                    Data = new Dictionary<string, string> { ["text"] = "Visited Solara" }
                });
            client.Setup(apiClient => apiClient.SendRequestAsync<UpdateLogRequest, NuciApiSuccessResponse>(
                    HttpMethod.Put,
                    It.IsAny<UpdateLogRequest>(),
                    It.IsAny<NuciApiRequestAuthorisationInfo>(),
                    It.IsAny<string>()))
                .ReturnsAsync(new NuciApiSuccessResponse());
            client.Setup(apiClient => apiClient.SendRequestAsync<DeleteLogRequest, NuciApiSuccessResponse>(
                    HttpMethod.Delete,
                    It.IsAny<DeleteLogRequest>(),
                    It.IsAny<NuciApiRequestAuthorisationInfo>(),
                    It.IsAny<string>()))
                .ReturnsAsync(new NuciApiSuccessResponse());

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
            await localeService.SetLocaleAsync(LocaleService.English);
        }

        [TearDown]
        public void TearDown() => testContext.Dispose();

        [Test]
        public void GivenCalendarLogs_WhenOpeningHome_ThenEntriesAreParsedAndRenderedNewestFirst()
        {
            calendarLogs =
            [
                "L4 2020-05-21: 08:16 Visited Cornova",
                "L8 2020-05-21: 16:32 Visited Solara"
            ];

            IRenderedComponent<Home> component = testContext.Render<Home>();

            Assert.That(
                component.FindAll(".entry-item").Select(element => element.TextContent.Trim()),
                Is.EqualTo(new[] { "16:32 Visited Solara", "08:16 Visited Cornova" }));
            Assert.That(component.Find(".entries-count").TextContent, Is.EqualTo("2 log entries"));
            VerifyListRequest(DateTime.Today.ToString("yyyy-MM-dd"), 1000, "en", Times.Once());
        }

        [Test]
        public void GivenAnAscendingPreference_WhenOpeningHome_ThenEntriesRetainApiOrder()
        {
            ConfigureStorage("NucileRullz!", "true", "false");
            calendarLogs =
            [
                "L4 2020-05-21: 08:16 Visited Cornova",
                "L8 2020-05-21: 16:32 Visited Solara"
            ];

            IRenderedComponent<Home> component = testContext.Render<Home>();

            Assert.That(
                component.FindAll(".entry-item").Select(element => element.TextContent.Trim()),
                Is.EqualTo(new[] { "08:16 Visited Cornova", "16:32 Visited Solara" }));
            Assert.That(component.Find(".btn-sort i").ClassList, Does.Contain("fa-arrow-up-1-9"));
        }

        [Test]
        public void GivenCalendarEntries_WhenTogglingSort_ThenThePreferencePersistsAndTheListReloads()
        {
            calendarLogs =
            [
                "L4 2020-05-21: 08:16 Visited Cornova",
                "L8 2020-05-21: 16:32 Visited Solara"
            ];
            IRenderedComponent<Home> component = testContext.Render<Home>();

            component.Find(".btn-sort").Click();

            Assert.That(
                component.FindAll(".entry-item").Select(element => element.TextContent.Trim()),
                Is.EqualTo(new[] { "08:16 Visited Cornova", "16:32 Visited Solara" }));
            AssertStorageWrite("sortAscending", "true");
            VerifyCalendarRequests(Times.Exactly(2));
        }

        [Test]
        public void GivenCollapsedEntries_WhenTogglingWidthTwice_ThenBothPreferencesPersist()
        {
            IRenderedComponent<Home> component = testContext.Render<Home>();

            component.Find(".btn-fullwidth").Click();
            Assert.That(component.Find(".entries-container").ClassList, Does.Contain("is-full-width"));
            component.Find(".btn-fullwidth").Click();

            Assert.That(component.Find(".entries-container").ClassList, Does.Not.Contain("is-full-width"));
            AssertStorageWrite("fullWidth", "true");
            AssertStorageWrite("fullWidth", "false");
        }

        [Test]
        public void GivenAnEmptyApiKey_WhenOpeningHome_ThenNoUpstreamRequestIsMade()
        {
            ConfigureStorage("", "false", "false");

            IRenderedComponent<Home> component = testContext.Render<Home>();

            Assert.That(component.Find(".entries-notice").TextContent, Is.EqualTo(LocalisationStrings.English.NoApiKeyNotice));
            Assert.That(client.Invocations, Is.Empty);
        }

        [Test]
        public void GivenAWhitespaceApiKey_WhenOpeningHome_ThenTheStoredValueIsSentUpstream()
        {
            ConfigureStorage("   ", "false", "false");

            testContext.Render<Home>();

            client.Verify(apiClient => apiClient.SendRequestAsync<GetLogsRequest, GetLogsResponse>(
                HttpMethod.Get,
                It.IsAny<GetLogsRequest>(),
                It.Is<NuciApiRequestAuthorisationInfo>(authorisation => string.Equals(authorisation.BearerToken, "   ")),
                "/PersonalLog"), Times.Once());
        }

        [TestCase("AUTHENTICATION_FAILURE")]
        [TestCase("UNAUTHORISED")]
        public void GivenAnAuthenticationError_WhenOpeningHome_ThenTheLocalisedFailureIsRendered(string errorCode)
        {
            ConfigureListResponse(new NuciApiErrorResponse { Code = errorCode });

            IRenderedComponent<Home> component = testContext.Render<Home>();

            Assert.That(component.Find(".entries-error").TextContent, Is.EqualTo(LocalisationStrings.English.InvalidApiKey));
            Assert.That(component.FindAll(".entry-item"), Is.Empty);
        }

        [Test]
        public void GivenAnUnexpectedApiResponse_WhenOpeningHome_ThenAnEmptyCalendarNoticeIsRendered()
        {
            ConfigureListResponse(new NuciApiSuccessResponse());

            IRenderedComponent<Home> component = testContext.Render<Home>();

            Assert.That(component.Find(".entries-notice").TextContent, Does.StartWith("No entries for"));
            Assert.That(component.FindAll(".entries-error"), Is.Empty);
        }

        [Test]
        public void GivenMalformedAndValidLogs_WhenOpeningHome_ThenEveryResponseItemRemainsVisible()
        {
            calendarLogs =
            [
                "not a structured log",
                "L42 2020-05-21: Visited Solara"
            ];

            IRenderedComponent<Home> component = testContext.Render<Home>();

            Assert.That(
                component.FindAll(".entry-item").Select(element => element.TextContent.Trim()),
                Is.EqualTo(new[] { "Visited Solara", "not a structured log" }));
        }

        [Test]
        public void GivenSearchResults_WhenSearchingCaseInsensitively_ThenOnlyMatchesAreRenderedWithDates()
        {
            searchLogs =
            [
                "L4 2020-05-21: Visited Cornova",
                "L8 2012-09-05: VISITED SOLARA",
                "L16 2020-05-21: Stayed home"
            ];
            IRenderedComponent<Home> component = RenderSearchView();

            SubmitSearch(component, "visited");

            Assert.That(component.FindAll(".entry-item"), Has.Count.EqualTo(2));
            Assert.That(
                component.FindAll(".entry-date").Select(element => element.TextContent),
                Is.EqualTo(new[] { "2012-09-05", "2020-05-21" }));
            VerifyListRequest(null, 100000, "en", Times.Once());
        }

        [TestCase("")]
        [TestCase(" ")]
        [TestCase("\t")]
        public void GivenBlankSearchText_WhenViewingSearch_ThenSubmissionRemainsDisabled(string searchText)
        {
            IRenderedComponent<Home> component = RenderSearchView();

            component.Find(".search-input").Input(searchText);

            Assert.That(component.Find(".btn-search").HasAttribute("disabled"));
            VerifySearchRequests(Times.Never());
        }

        [Test]
        public void GivenAnAppliedSearch_WhenRefreshingAfterEditingTheInput_ThenTheAppliedTermIsReused()
        {
            searchLogs =
            [
                "L4 2020-05-21: Visited Cornova",
                "L8 2012-09-05: Visited Solara"
            ];
            IRenderedComponent<Home> component = RenderSearchView();
            SubmitSearch(component, "Cornova");
            component.Find(".search-input").Input("Solara");

            component.Find(".btn-refresh").Click();

            Assert.That(component.Find(".entry-item").TextContent, Does.Contain("Cornova"));
            Assert.That(component.Markup, Does.Not.Contain("Visited Solara"));
            VerifySearchRequests(Times.Exactly(2));
        }

        [Test]
        public async Task GivenSearchResults_WhenChangingLocale_ThenTheRequestAndRenderedCountAreLocalised()
        {
            searchLogs = ["L42 2020-05-21: Visited Solara"];
            IRenderedComponent<Home> component = RenderSearchView();
            SubmitSearch(component, "Solara");

            await component.InvokeAsync(() => localeService.SetLocaleAsync(LocaleService.Romanian));

            component.WaitForAssertion(() =>
                Assert.That(component.Find(".entries-count").TextContent, Is.EqualTo("1 intrări în jurnal")));
            VerifyListRequest(null, 100000, "ro", Times.Once());
        }

        [Test]
        public void GivenFiveAuthenticationFailures_WhenRefreshingAgain_ThenLockoutPreventsAnotherRequest()
        {
            ConfigureListResponse(new NuciApiErrorResponse { Code = "UNAUTHORISED" });
            IRenderedComponent<Home> component = testContext.Render<Home>();

            for (int failureIndex = 1; failureIndex < 5; failureIndex += 1)
            {
                component.Find(".btn-refresh").Click();
            }

            Assert.That(component.Find(".entries-error").TextContent, Does.StartWith("Too many failed attempts."));
            Assert.That(GetListRequestCount(), Is.EqualTo(5));

            component.Find(".btn-refresh").Click();

            Assert.That(GetListRequestCount(), Is.EqualTo(5));
        }

        [Test]
        public void GivenASelectedEntry_WhenClosingTheDetailPanel_ThenTheSelectionAndPanelAreCleared()
        {
            calendarLogs = ["L42 2020-05-21: Visited Solara"];
            IRenderedComponent<Home> component = testContext.Render<Home>();
            component.Find(".entry-item").Click();

            component.Find(".btn-close-panel").Click();

            Assert.That(component.FindAll(".entry-detail-panel"), Is.Empty);
            Assert.That(component.Find(".entry-item").ClassList, Does.Not.Contain("is-selected"));
            client.Verify(apiClient => apiClient.SendRequestAsync<GetLogByIdRequest, GetLogByIdResponse>(
                HttpMethod.Get,
                It.IsAny<GetLogByIdRequest>(),
                It.IsAny<NuciApiRequestAuthorisationInfo>(),
                "/PersonalLog/L42"), Times.Once());
        }

        [Test]
        public void GivenASelectedEntry_WhenSavingAnEdit_ThenThePanelClosesAndCalendarReloads()
        {
            calendarLogs = ["L42 2020-05-21: Visited Solara"];
            IRenderedComponent<Home> component = testContext.Render<Home>();
            component.Find(".entry-item").Click();
            component.Find(".btn-edit-entry").Click();

            component.Find(".btn-save-edit").Click();

            Assert.That(component.FindAll(".entry-detail-panel"), Is.Empty);
            VerifyCalendarRequests(Times.Exactly(2));
            client.Verify(apiClient => apiClient.SendRequestAsync<UpdateLogRequest, NuciApiSuccessResponse>(
                HttpMethod.Put,
                It.IsAny<UpdateLogRequest>(),
                It.IsAny<NuciApiRequestAuthorisationInfo>(),
                "/PersonalLog/L42"), Times.Once());
        }

        [Test]
        public void GivenASelectedEntry_WhenConfirmingDeletion_ThenThePanelClosesAndCalendarReloads()
        {
            calendarLogs = ["L42 2020-05-21: Visited Solara"];
            IRenderedComponent<Home> component = testContext.Render<Home>();
            component.Find(".entry-item").Click();
            component.Find(".btn-delete-entry").Click();

            component.Find(".btn-confirm-delete").Click();

            Assert.That(component.FindAll(".entry-detail-panel"), Is.Empty);
            VerifyCalendarRequests(Times.Exactly(2));
            client.Verify(apiClient => apiClient.SendRequestAsync<DeleteLogRequest, NuciApiSuccessResponse>(
                HttpMethod.Delete,
                It.IsAny<DeleteLogRequest>(),
                It.IsAny<NuciApiRequestAuthorisationInfo>(),
                "/PersonalLog/L42"), Times.Once());
        }

        [Test]
        public void GivenSearchMode_WhenReturningToCalendar_ThenSearchStateAndSelectionAreDiscarded()
        {
            searchLogs = ["L42 2020-05-21: Visited Solara"];
            calendarLogs = ["L8 2012-09-05: Visited Cornova"];
            IRenderedComponent<Home> component = RenderSearchView();
            SubmitSearch(component, "Solara");
            component.Find(".entry-item").Click();

            component.FindAll(".btn-view-mode")[0].Click();

            Assert.That(component.FindAll(".entry-detail-panel"), Is.Empty);
            Assert.That(component.Find(".entry-item").TextContent, Does.Contain("Cornova"));
            Assert.That(component.FindAll(".entry-date"), Is.Empty);
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
            testContext.JSInterop.Setup<string>("localStorage.getItem", "plm_api_key").SetResult(apiKey);
            testContext.JSInterop.Setup<string>("localStorage.getItem", "sortAscending").SetResult(sortAscending);
            testContext.JSInterop.Setup<string>("localStorage.getItem", "fullWidth").SetResult(fullWidth);
        }

        private void ConfigureListResponse(NuciApiResponse response)
            => client.Setup(apiClient => apiClient.SendRequestAsync<GetLogsRequest, GetLogsResponse>(
                    HttpMethod.Get,
                    It.IsAny<GetLogsRequest>(),
                    It.IsAny<NuciApiRequestAuthorisationInfo>(),
                    "/PersonalLog"))
                .ReturnsAsync(response);

        private void AssertStorageWrite(string storageKey, string value)
            => Assert.That(
                testContext.JSInterop.Invocations.Any(invocation =>
                    string.Equals(invocation.Identifier, "localStorage.setItem") &&
                    invocation.Arguments.Count == 2 &&
                    string.Equals(invocation.Arguments[0]?.ToString(), storageKey) &&
                    string.Equals(invocation.Arguments[1]?.ToString(), value)));

        private int GetListRequestCount()
            => client.Invocations.Count(invocation =>
                string.Equals(invocation.Method.Name, nameof(INuciApiClient.SendRequestAsync)) &&
                invocation.Method.GetGenericArguments().Contains(typeof(GetLogsRequest)));

        private void VerifyCalendarRequests(Times times)
            => VerifyListRequest(DateTime.Today.ToString("yyyy-MM-dd"), 1000, "en", times);

        private void VerifySearchRequests(Times times)
            => client.Verify(apiClient => apiClient.SendRequestAsync<GetLogsRequest, GetLogsResponse>(
                HttpMethod.Get,
                It.Is<GetLogsRequest>(request => request.Date == null && request.Count == 100000),
                It.IsAny<NuciApiRequestAuthorisationInfo>(),
                "/PersonalLog"), times);

        private void VerifyListRequest(string? date, int count, string localisation, Times times)
            => client.Verify(apiClient => apiClient.SendRequestAsync<GetLogsRequest, GetLogsResponse>(
                HttpMethod.Get,
                It.Is<GetLogsRequest>(request =>
                    string.Equals(request.Date, date) &&
                    request.Count == count &&
                    string.Equals(request.Localisation, localisation)),
                It.Is<NuciApiRequestAuthorisationInfo>(authorisation =>
                    string.Equals(authorisation.BearerToken, "NucileRullz!") &&
                    authorisation.ClientId.StartsWith("PersonalLogManagerClient_")),
                "/PersonalLog"), times);
    }
}
