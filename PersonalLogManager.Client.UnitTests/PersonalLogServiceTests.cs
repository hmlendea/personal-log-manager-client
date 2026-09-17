using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Moq;
using NuciAPI.Client;
using NuciAPI.Responses;
using NUnit.Framework;
using PersonalLogManagerClient.Models;
using PersonalLogManagerClient.Services;

namespace PersonalLogManagerClient.UnitTests
{
    [TestFixture]
    public sealed class PersonalLogServiceTests
    {
        private Mock<INuciApiClient> client = null!;
        private Mock<IJSRuntime> js = null!;
        private ApiKeyService apiKeyService = null!;
        private LocaleService localeService = null!;
        private ApiKeyRateLimitService rateLimitService = null!;
        private PersonalLogService service = null!;

        [SetUp]
        public void SetUp()
        {
            client = new Mock<INuciApiClient>();
            js = new Mock<IJSRuntime>();
            js.Setup(runtime => runtime.InvokeAsync<string>("localStorage.getItem", It.IsAny<object[]>()))
                .Returns(new ValueTask<string>("api-key"));
            apiKeyService = new ApiKeyService(js.Object);
            localeService = new LocaleService(js.Object);
            rateLimitService = new ApiKeyRateLimitService();
            service = new PersonalLogService(client.Object, apiKeyService, localeService, rateLimitService);
        }

        [Test]
        public async Task GivenAnAscendingResponse_WhenGettingLogsForADate_ThenEntriesAreParsedAndRetainedInOrder()
        {
            client.Setup(api => api.SendRequestAsync<GetLogsRequest, GetLogsResponse>(
                    HttpMethod.Get,
                    It.Is<GetLogsRequest>(request => request.Date == "2026-09-17" && request.Count == 25 && request.Localisation == "ro"),
                    It.Is<NuciApiRequestAuthorisationInfo>(auth => auth.BearerToken == "api-key"),
                    "/PersonalLog"))
                .ReturnsAsync(new GetLogsResponse
                {
                    Logs = ["L1 2026-09-17: First", "L2 2026-09-17: Second"]
                });

            List<LogEntry> result = await service.GetLogsForDateAsync("2026-09-17", 25, true);

            Assert.That(result.Select(entry => entry.Id), Is.EqualTo(new[] { "L1", "L2" }));
            Assert.That(result[0].Date, Is.EqualTo("2026-09-17"));
            Assert.That(result[0].Text, Is.EqualTo("First"));
            Assert.That(result[0].RawText, Is.EqualTo("L1 2026-09-17: First"));
        }

        [Test]
        public async Task GivenADescendingResponse_WhenGettingLogsForADate_ThenEntriesAreReturnedInReverseOrder()
        {
            client.Setup(api => api.SendRequestAsync<GetLogsRequest, GetLogsResponse>(
                    It.IsAny<HttpMethod>(),
                    It.IsAny<GetLogsRequest>(),
                    It.IsAny<NuciApiRequestAuthorisationInfo>(),
                    It.IsAny<string>()))
                .ReturnsAsync(new GetLogsResponse
                {
                    Logs = ["L1 2026-09-17: First", "L2 2026-09-17: Second"]
                });

            List<LogEntry> result = await service.GetLogsForDateAsync("2026-09-17");

            Assert.That(result.Select(entry => entry.Id), Is.EqualTo(new[] { "L2", "L1" }));
        }

        [TestCase("unstructured")]
        [TestCase("")]
        [TestCase("  ")]
        public async Task GivenAnUnparseableLog_WhenGettingLogsForADate_ThenTheRawTextIsRetained(string rawText)
        {
            client.Setup(api => api.SendRequestAsync<GetLogsRequest, GetLogsResponse>(
                    It.IsAny<HttpMethod>(), It.IsAny<GetLogsRequest>(), It.IsAny<NuciApiRequestAuthorisationInfo>(), It.IsAny<string>()))
                .ReturnsAsync(new GetLogsResponse { Logs = [rawText] });

            List<LogEntry> result = await service.GetLogsForDateAsync("2026-09-17", ascending: true);

            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result[0].Id, Is.Empty);
            Assert.That(result[0].Date, Is.Empty);
            Assert.That(result[0].Text, Is.EqualTo(rawText));
            Assert.That(result[0].RawText, Is.EqualTo(rawText));
        }

        [Test]
        public async Task GivenAResponseWithANullLog_WhenGettingLogsForADate_ThenAnEmptyEntryIsCreated()
        {
            client.Setup(api => api.SendRequestAsync<GetLogsRequest, GetLogsResponse>(
                    It.IsAny<HttpMethod>(), It.IsAny<GetLogsRequest>(), It.IsAny<NuciApiRequestAuthorisationInfo>(), It.IsAny<string>()))
                .ReturnsAsync(new GetLogsResponse { Logs = [null] });

            List<LogEntry> result = await service.GetLogsForDateAsync("2026-09-17", ascending: true);

            Assert.That(result[0].Id, Is.Empty);
            Assert.That(result[0].Date, Is.Empty);
            Assert.That(result[0].Text, Is.Empty);
            Assert.That(result[0].RawText, Is.Empty);
        }

        [Test]
        public async Task GivenAResponseWithNoLogs_WhenGettingLogsForADate_ThenAnEmptyListIsReturned()
        {
            client.Setup(api => api.SendRequestAsync<GetLogsRequest, GetLogsResponse>(
                    It.IsAny<HttpMethod>(), It.IsAny<GetLogsRequest>(), It.IsAny<NuciApiRequestAuthorisationInfo>(), It.IsAny<string>()))
                .ReturnsAsync(new GetLogsResponse { Logs = null });

            List<LogEntry> result = await service.GetLogsForDateAsync("2026-09-17");

            Assert.That(result, Is.Empty);
        }

        [Test]
        public async Task GivenAnUnexpectedLogsResponse_WhenGettingLogsForADate_ThenAnEmptyListIsReturned()
        {
            client.Setup(api => api.SendRequestAsync<GetLogsRequest, GetLogsResponse>(
                    It.IsAny<HttpMethod>(), It.IsAny<GetLogsRequest>(), It.IsAny<NuciApiRequestAuthorisationInfo>(), It.IsAny<string>()))
                .ReturnsAsync(new NuciApiSuccessResponse());

            List<LogEntry> result = await service.GetLogsForDateAsync("2026-09-17");

            Assert.That(result, Is.Empty);
        }

        [Test]
        public async Task GivenMatchingEntries_WhenSearchingLogs_ThenOnlyFinalTextMatchesAreReturnedCaseInsensitively()
        {
            client.Setup(api => api.SendRequestAsync<GetLogsRequest, GetLogsResponse>(
                    HttpMethod.Get,
                    It.Is<GetLogsRequest>(request => request.Date == null && request.Count == 100000),
                    It.IsAny<NuciApiRequestAuthorisationInfo>(),
                    "/PersonalLog"))
                .ReturnsAsync(new GetLogsResponse
                {
                    Logs =
                    [
                        "L4 2026-09-17: 08:00 Visited Solara",
                        "L8 2026-09-16: 09:00 SOLARA forecast",
                        "L16 2026-09-15: 10:00 Visited Cornova"
                    ]
                });

            List<LogEntry> result = await service.SearchLogsAsync("solara", true);

            Assert.That(result.Select(entry => entry.Id), Is.EqualTo(new[] { "L4", "L8" }));
        }

        [TestCase("")]
        [TestCase("  ")]
        [TestCase(null)]
        public async Task GivenAnEmptySearchTerm_WhenSearchingLogs_ThenNoEntriesAreReturned(string searchTerm)
        {
            client.Setup(api => api.SendRequestAsync<GetLogsRequest, GetLogsResponse>(
                    It.IsAny<HttpMethod>(),
                    It.IsAny<GetLogsRequest>(),
                    It.IsAny<NuciApiRequestAuthorisationInfo>(),
                    It.IsAny<string>()))
                .ReturnsAsync(new GetLogsResponse { Logs = ["L4 2026-09-17: 08:00 Visited Solara"] });

            List<LogEntry> result = await service.SearchLogsAsync(searchTerm);

            Assert.That(result, Is.Empty);
            Assert.That(client.Invocations, Is.Empty);
        }

        [Test]
        public async Task GivenMatchingEntries_WhenSearchingLogsWithTheDefaultOrder_ThenMatchesAreReturnedInDescendingOrder()
        {
            client.Setup(api => api.SendRequestAsync<GetLogsRequest, GetLogsResponse>(
                    It.IsAny<HttpMethod>(),
                    It.IsAny<GetLogsRequest>(),
                    It.IsAny<NuciApiRequestAuthorisationInfo>(),
                    It.IsAny<string>()))
                .ReturnsAsync(new GetLogsResponse
                {
                    Logs =
                    [
                        "L4 2026-09-16: 08:00 Bencheamobil service",
                        "L8 2026-09-17: 09:00 Visited Bencheamobil"
                    ]
                });

            List<LogEntry> result = await service.SearchLogsAsync("Bencheamobil");

            Assert.That(result.Select(entry => entry.Id), Is.EqualTo(new[] { "L8", "L4" }));
        }

        [TestCase("L4")]
        [TestCase("2026-09-17")]
        public async Task GivenAMatchOutsideTheFinalText_WhenSearchingLogs_ThenNoEntriesAreReturned(string searchTerm)
        {
            client.Setup(api => api.SendRequestAsync<GetLogsRequest, GetLogsResponse>(
                    It.IsAny<HttpMethod>(),
                    It.IsAny<GetLogsRequest>(),
                    It.IsAny<NuciApiRequestAuthorisationInfo>(),
                    It.IsAny<string>()))
                .ReturnsAsync(new GetLogsResponse { Logs = ["L4 2026-09-17: 08:00 Visited Solara"] });

            List<LogEntry> result = await service.SearchLogsAsync(searchTerm);

            Assert.That(result, Is.Empty);
        }

        [TestCase("AUTHENTICATION_FAILURE")]
        [TestCase("UNAUTHORISED")]
        public void GivenAnAuthenticationError_WhenGettingLogsForADate_ThenAnInvalidKeyExceptionIsThrown(string errorCode)
        {
            client.Setup(api => api.SendRequestAsync<GetLogsRequest, GetLogsResponse>(
                    It.IsAny<HttpMethod>(), It.IsAny<GetLogsRequest>(), It.IsAny<NuciApiRequestAuthorisationInfo>(), It.IsAny<string>()))
                .ReturnsAsync(new NuciApiErrorResponse { Code = errorCode });

            InvalidOperationException exception = Assert.ThrowsAsync<InvalidOperationException>(
                () => service.GetLogsForDateAsync("2026-09-17"))!;

            Assert.That(exception.Message, Is.EqualTo(LocalisationStrings.Romanian.InvalidApiKey));
        }

        [Test]
        public async Task GivenAValidLogId_WhenGettingLogDetails_ThenTheResponseIsReturned()
        {
            GetLogByIdResponse expected = new() { Id = "L42" };
            client.Setup(api => api.SendRequestAsync<GetLogByIdRequest, GetLogByIdResponse>(
                    HttpMethod.Get,
                    It.Is<GetLogByIdRequest>(request => request != null),
                    It.Is<NuciApiRequestAuthorisationInfo>(auth => auth.BearerToken == "api-key"),
                    "/PersonalLog/L42"))
                .ReturnsAsync(expected);

            GetLogByIdResponse? result = await service.GetLogByIdAsync("L42");

            Assert.That(result, Is.SameAs(expected));
        }

        [Test]
        public async Task GivenAnUnexpectedDetailsResponse_WhenGettingLogDetails_ThenNullIsReturned()
        {
            client.Setup(api => api.SendRequestAsync<GetLogByIdRequest, GetLogByIdResponse>(
                    It.IsAny<HttpMethod>(), It.IsAny<GetLogByIdRequest>(), It.IsAny<NuciApiRequestAuthorisationInfo>(), It.IsAny<string>()))
                .ReturnsAsync(new NuciApiSuccessResponse());

            GetLogByIdResponse? result = await service.GetLogByIdAsync("L42");

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GivenAValidLogId_WhenDeletingTheLog_ThenADeleteRequestIsSent()
        {
            client.Setup(api => api.SendRequestAsync<DeleteLogRequest, NuciApiSuccessResponse>(
                    HttpMethod.Delete,
                    It.Is<DeleteLogRequest>(request => request.Id == "L42"),
                    It.IsAny<NuciApiRequestAuthorisationInfo>(),
                    "/PersonalLog/L42"))
                .ReturnsAsync(new NuciApiSuccessResponse());

            await service.DeleteLogAsync("L42");

            client.Verify(api => api.SendRequestAsync<DeleteLogRequest, NuciApiSuccessResponse>(
                HttpMethod.Delete,
                It.Is<DeleteLogRequest>(request => request.Id == "L42"),
                It.IsAny<NuciApiRequestAuthorisationInfo>(),
                "/PersonalLog/L42"), Times.Once);
        }

        [Test]
        public void GivenAnAuthenticationError_WhenGettingLogDetails_ThenAnInvalidKeyExceptionIsThrown()
        {
            client.Setup(api => api.SendRequestAsync<GetLogByIdRequest, GetLogByIdResponse>(
                    It.IsAny<HttpMethod>(), It.IsAny<GetLogByIdRequest>(), It.IsAny<NuciApiRequestAuthorisationInfo>(), It.IsAny<string>()))
                .ReturnsAsync(new NuciApiErrorResponse { Code = "UNAUTHORISED" });

            InvalidOperationException exception = Assert.ThrowsAsync<InvalidOperationException>(
                () => service.GetLogByIdAsync("L42"))!;

            Assert.That(exception.Message, Is.EqualTo(LocalisationStrings.Romanian.InvalidApiKey));
        }

        [Test]
        public void GivenAnAuthenticationError_WhenDeletingTheLog_ThenAnInvalidKeyExceptionIsThrown()
        {
            client.Setup(api => api.SendRequestAsync<DeleteLogRequest, NuciApiSuccessResponse>(
                    It.IsAny<HttpMethod>(), It.IsAny<DeleteLogRequest>(), It.IsAny<NuciApiRequestAuthorisationInfo>(), It.IsAny<string>()))
                .ReturnsAsync(new NuciApiErrorResponse { Code = "AUTHENTICATION_FAILURE" });

            InvalidOperationException exception = Assert.ThrowsAsync<InvalidOperationException>(
                () => service.DeleteLogAsync("L42"))!;

            Assert.That(exception.Message, Is.EqualTo(LocalisationStrings.Romanian.InvalidApiKey));
        }

        [Test]
        public async Task GivenAnUpdatePayload_WhenUpdatingTheLog_ThenAUpdateRequestIsSent()
        {
            Dictionary<string, System.Text.Json.JsonElement> data = new();
            client.Setup(api => api.SendRequestAsync<UpdateLogRequest, NuciApiSuccessResponse>(
                    HttpMethod.Put,
                    It.Is<UpdateLogRequest>(request => request.Date == "2026-09-17" &&
                        request.Time == "08:09" && request.TimeZone == "UTC" && request.Data == data),
                    It.IsAny<NuciApiRequestAuthorisationInfo>(),
                    "/PersonalLog/L42"))
                .ReturnsAsync(new NuciApiSuccessResponse());

            await service.UpdateLogAsync("L42", "2026-09-17", "08:09", "UTC", data);

            client.Verify(api => api.SendRequestAsync<UpdateLogRequest, NuciApiSuccessResponse>(
                HttpMethod.Put,
                It.IsAny<UpdateLogRequest>(),
                It.IsAny<NuciApiRequestAuthorisationInfo>(),
                "/PersonalLog/L42"), Times.Once);
        }

        [Test]
        public void GivenAnAuthenticationError_WhenUpdatingTheLog_ThenAnInvalidKeyExceptionIsThrown()
        {
            client.Setup(api => api.SendRequestAsync<UpdateLogRequest, NuciApiSuccessResponse>(
                    It.IsAny<HttpMethod>(), It.IsAny<UpdateLogRequest>(), It.IsAny<NuciApiRequestAuthorisationInfo>(), It.IsAny<string>()))
                .ReturnsAsync(new NuciApiErrorResponse { Code = "UNAUTHORISED" });

            InvalidOperationException exception = Assert.ThrowsAsync<InvalidOperationException>(
                () => service.UpdateLogAsync("L42", "2026-09-17", "08:09", "UTC", new()))!;

            Assert.That(exception.Message, Is.EqualTo(LocalisationStrings.Romanian.InvalidApiKey));
        }

        [Test]
        public async Task GivenFiveAuthenticationFailures_WhenCallingTheServiceAgain_ThenRequestsAreBlocked()
        {
            client.Setup(api => api.SendRequestAsync<GetLogsRequest, GetLogsResponse>(
                    It.IsAny<HttpMethod>(), It.IsAny<GetLogsRequest>(), It.IsAny<NuciApiRequestAuthorisationInfo>(), It.IsAny<string>()))
                .ReturnsAsync(new NuciApiErrorResponse { Code = "UNAUTHORISED" });

            for (int index = 0; index < 5; index++)
            {
                Assert.ThrowsAsync<InvalidOperationException>(() => service.GetLogsForDateAsync("2026-09-17"));
            }

            int callCount = client.Invocations.Count;
            InvalidOperationException exception = Assert.ThrowsAsync<InvalidOperationException>(
                () => service.GetLogsForDateAsync("2026-09-17"))!;

            Assert.That(exception.Message, Does.StartWith("Prea multe încercări eșuate."));
            Assert.That(client.Invocations, Has.Count.EqualTo(callCount));
        }

        [Test]
        public void GivenALockedRateLimitService_WhenGettingLogDetails_ThenTheClientIsNotCalled()
        {
            for (int index = 0; index < 5; index++)
            {
                rateLimitService.RecordFailure();
            }

            Assert.ThrowsAsync<InvalidOperationException>(() => service.GetLogByIdAsync("L42"));

            Assert.That(client.Invocations, Is.Empty);
        }
    }
}
