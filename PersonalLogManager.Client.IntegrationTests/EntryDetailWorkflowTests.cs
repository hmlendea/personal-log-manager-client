using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Moq;
using NuciAPI.Client;
using NuciAPI.Requests;
using NuciAPI.Responses;
using NUnit.Framework;
using PersonalLogManagerClient.Layout;
using PersonalLogManagerClient.Models;
using PersonalLogManagerClient.Services;

namespace PersonalLogManagerClient.IntegrationTests
{
    [TestFixture]
    public sealed class EntryDetailWorkflowTests
    {
        private BunitContext testContext = null!;
        private Mock<INuciApiClient> client = null!;
        private GetLogByIdResponse entryDetails = null!;

        [SetUp]
        public async Task SetUp()
        {
            testContext = new BunitContext();
            testContext.JSInterop.Mode = JSRuntimeMode.Loose;
            testContext.JSInterop.Setup<string>("localStorage.getItem", "plm_api_key").SetResult("NucileRullz!");

            entryDetails = new GetLogByIdResponse
            {
                Id = "L42",
                Date = "2020-05-21",
                Time = "08:16:32",
                TimeZone = "Europe/Bucharest",
                Template = "journal",
                Data = new Dictionary<string, string> { ["text"] = "Visited Solara" },
                CreatedDateTime = "2020-05-21T08:16:32.0000000+03:00",
                UpdatedDateTime = "2020-05-21T08:16:32.0000000+03:00"
            };

            client = new Mock<INuciApiClient>();
            client.Setup(apiClient => apiClient.SendRequestAsync<GetLogByIdRequest, GetLogByIdResponse>(
                    HttpMethod.Get,
                    It.IsAny<GetLogByIdRequest>(),
                    It.IsAny<NuciApiRequestAuthorisationInfo>(),
                    "/PersonalLog/L42"))
                .ReturnsAsync(() => entryDetails);
            client.Setup(apiClient => apiClient.SendRequestAsync<UpdateLogRequest, NuciApiSuccessResponse>(
                    HttpMethod.Put,
                    It.IsAny<UpdateLogRequest>(),
                    It.IsAny<NuciApiRequestAuthorisationInfo>(),
                    "/PersonalLog/L42"))
                .ReturnsAsync(new NuciApiSuccessResponse());
            client.Setup(apiClient => apiClient.SendRequestAsync<DeleteLogRequest, NuciApiSuccessResponse>(
                    HttpMethod.Delete,
                    It.IsAny<DeleteLogRequest>(),
                    It.IsAny<NuciApiRequestAuthorisationInfo>(),
                    "/PersonalLog/L42"))
                .ReturnsAsync(new NuciApiSuccessResponse());

            testContext.Services.AddSingleton<ApiKeyService>(serviceProvider =>
                new ApiKeyService(serviceProvider.GetRequiredService<IJSRuntime>()));
            testContext.Services.AddSingleton<LocaleService>(serviceProvider =>
                new LocaleService(serviceProvider.GetRequiredService<IJSRuntime>()));
            testContext.Services.AddSingleton<ApiKeyRateLimitService>();
            testContext.Services.AddSingleton<PersonalLogService>(serviceProvider =>
                new PersonalLogService(
                    client.Object,
                    serviceProvider.GetRequiredService<ApiKeyService>(),
                    serviceProvider.GetRequiredService<LocaleService>(),
                    serviceProvider.GetRequiredService<ApiKeyRateLimitService>()));

            LocaleService localeService = testContext.Services.GetRequiredService<LocaleService>();
            await localeService.SetLocaleAsync(LocaleService.English);
        }

        [TearDown]
        public void TearDown() => testContext.Dispose();

        [Test]
        public void GivenAnEntry_WhenOpeningItsDetails_ThenTheCompleteApiResponseIsRendered()
        {
            IRenderedComponent<EntryDetailPanel> component = RenderPanel();

            string renderedJson = component.Find(".entry-detail-json").TextContent;

            Assert.That(renderedJson, Does.Contain("L42"));
            Assert.That(renderedJson, Does.Contain("2020-05-21"));
            Assert.That(renderedJson, Does.Contain("08:16:32"));
            Assert.That(renderedJson, Does.Contain("Europe/Bucharest"));
            Assert.That(renderedJson, Does.Contain("Visited Solara"));

            VerifyAuthorisation<GetLogByIdRequest, GetLogByIdResponse>(HttpMethod.Get, Times.Once());
        }

        [Test]
        public void GivenDetailsCannotBeLoaded_WhenOpeningAnEntry_ThenTheErrorAndFallbackDataAreRendered()
        {
            client.Setup(apiClient => apiClient.SendRequestAsync<GetLogByIdRequest, GetLogByIdResponse>(
                    HttpMethod.Get,
                    It.IsAny<GetLogByIdRequest>(),
                    It.IsAny<NuciApiRequestAuthorisationInfo>(),
                    "/PersonalLog/L42"))
                .ReturnsAsync(new NuciApiErrorResponse { Code = "UNAUTHORISED" });

            IRenderedComponent<EntryDetailPanel> component = RenderPanel();

            Assert.That(component.Find(".entry-detail-error").TextContent, Is.EqualTo(LocalisationStrings.English.InvalidApiKey));
            Assert.That(component.Find(".entry-detail-json").TextContent, Does.Contain("Visited Solara"));
        }

        [Test]
        public void GivenValidEditedJson_WhenSaving_ThenTheUpdateIsSentAndTheEditedCallbackRuns()
        {
            int editedCallbackCount = 0;
            IRenderedComponent<EntryDetailPanel> component = RenderPanel(
                () => editedCallbackCount += 1,
                () => { });
            component.Find(".btn-edit-entry").Click();
            component.Find(".entry-edit-json").Change(
                "{\"date\":\"2012-09-05\",\"time\":\"16:32:48\",\"timeZone\":\"UTC\",\"data\":{\"text\":\"Visited Cornova\",\"score\":42}}");

            component.Find(".btn-save-edit").Click();

            Assert.That(editedCallbackCount, Is.EqualTo(1));
            client.Verify(apiClient => apiClient.SendRequestAsync<UpdateLogRequest, NuciApiSuccessResponse>(
                HttpMethod.Put,
                It.Is<UpdateLogRequest>(request =>
                    string.Equals(request.Date, "2012-09-05") &&
                    string.Equals(request.Time, "16:32:48") &&
                    string.Equals(request.TimeZone, "UTC") &&
                    string.Equals(request.Data["text"].GetString(), "Visited Cornova") &&
                    request.Data["score"].GetInt32() == 42),
                It.IsAny<NuciApiRequestAuthorisationInfo>(),
                "/PersonalLog/L42"), Times.Once());
            VerifyAuthorisation<UpdateLogRequest, NuciApiSuccessResponse>(HttpMethod.Put, Times.Once());
        }

        [TestCase("")]
        [TestCase("not-json")]
        [TestCase("{")]
        public void GivenMalformedEditedJson_WhenSaving_ThenNoUpdateIsSent(string editedJson)
        {
            IRenderedComponent<EntryDetailPanel> component = RenderPanel();
            component.Find(".btn-edit-entry").Click();
            component.Find(".entry-edit-json").Change(editedJson);

            component.Find(".btn-save-edit").Click();

            Assert.That(component.Find(".entry-detail-error").TextContent, Is.EqualTo("Invalid JSON format."));
            VerifyNoUpdateRequest();
        }

        [Test]
        public void GivenEditedJsonWithoutData_WhenSaving_ThenAValidationErrorIsRendered()
        {
            IRenderedComponent<EntryDetailPanel> component = RenderPanel();
            component.Find(".btn-edit-entry").Click();
            component.Find(".entry-edit-json").Change("{\"date\":\"2012-09-05\"}");

            component.Find(".btn-save-edit").Click();

            Assert.That(component.Find(".entry-detail-error").TextContent, Is.EqualTo("JSON must contain a \"data\" object."));
            VerifyNoUpdateRequest();
        }

        [Test]
        public void GivenAnUpdateAuthenticationFailure_WhenSaving_ThenTheErrorIsRenderedWithoutCallingBack()
        {
            int editedCallbackCount = 0;
            client.Setup(apiClient => apiClient.SendRequestAsync<UpdateLogRequest, NuciApiSuccessResponse>(
                    HttpMethod.Put,
                    It.IsAny<UpdateLogRequest>(),
                    It.IsAny<NuciApiRequestAuthorisationInfo>(),
                    "/PersonalLog/L42"))
                .ReturnsAsync(new NuciApiErrorResponse { Code = "AUTHENTICATION_FAILURE" });
            IRenderedComponent<EntryDetailPanel> component = RenderPanel(
                () => editedCallbackCount += 1,
                () => { });
            component.Find(".btn-edit-entry").Click();

            component.Find(".btn-save-edit").Click();

            Assert.That(component.Find(".entry-detail-error").TextContent, Is.EqualTo(LocalisationStrings.English.InvalidApiKey));
            Assert.That(editedCallbackCount, Is.Zero);
        }

        [Test]
        public void GivenAnEntry_WhenCancellingAnEdit_ThenTheReadOnlyDetailsReturnWithoutUpdating()
        {
            IRenderedComponent<EntryDetailPanel> component = RenderPanel();
            component.Find(".btn-edit-entry").Click();
            component.Find(".entry-edit-json").Change("not-json");

            component.Find(".btn-cancel-edit").Click();

            Assert.That(component.FindAll(".entry-edit-json"), Is.Empty);
            Assert.That(component.Find(".entry-detail-json").TextContent, Does.Contain("Visited Solara"));
            VerifyNoUpdateRequest();
        }

        [Test]
        public void GivenAnEntry_WhenConfirmingDeletion_ThenTheDeleteIsSentAndTheDeletedCallbackRuns()
        {
            int deletedCallbackCount = 0;
            IRenderedComponent<EntryDetailPanel> component = RenderPanel(
                () => { },
                () => deletedCallbackCount += 1);
            component.Find(".btn-delete-entry").Click();

            component.Find(".btn-confirm-delete").Click();

            Assert.That(deletedCallbackCount, Is.EqualTo(1));
            client.Verify(apiClient => apiClient.SendRequestAsync<DeleteLogRequest, NuciApiSuccessResponse>(
                HttpMethod.Delete,
                It.Is<DeleteLogRequest>(request => string.Equals(request.Id, "L42")),
                It.IsAny<NuciApiRequestAuthorisationInfo>(),
                "/PersonalLog/L42"), Times.Once());
            VerifyAuthorisation<DeleteLogRequest, NuciApiSuccessResponse>(HttpMethod.Delete, Times.Once());
        }

        [Test]
        public void GivenADeletionRequest_WhenCancelling_ThenNoDeleteIsSent()
        {
            IRenderedComponent<EntryDetailPanel> component = RenderPanel();
            component.Find(".btn-delete-entry").Click();

            component.Find(".btn-cancel-delete").Click();

            Assert.That(component.FindAll(".btn-confirm-delete"), Is.Empty);
            client.Verify(apiClient => apiClient.SendRequestAsync<DeleteLogRequest, NuciApiSuccessResponse>(
                It.IsAny<HttpMethod>(),
                It.IsAny<DeleteLogRequest>(),
                It.IsAny<NuciApiRequestAuthorisationInfo>(),
                It.IsAny<string>()), Times.Never());
        }

        [Test]
        public void GivenADeleteAuthenticationFailure_WhenConfirming_ThenTheErrorIsRenderedWithoutCallingBack()
        {
            int deletedCallbackCount = 0;
            client.Setup(apiClient => apiClient.SendRequestAsync<DeleteLogRequest, NuciApiSuccessResponse>(
                    HttpMethod.Delete,
                    It.IsAny<DeleteLogRequest>(),
                    It.IsAny<NuciApiRequestAuthorisationInfo>(),
                    "/PersonalLog/L42"))
                .ReturnsAsync(new NuciApiErrorResponse { Code = "UNAUTHORISED" });
            IRenderedComponent<EntryDetailPanel> component = RenderPanel(
                () => { },
                () => deletedCallbackCount += 1);
            component.Find(".btn-delete-entry").Click();

            component.Find(".btn-confirm-delete").Click();

            Assert.That(component.Find(".entry-detail-error").TextContent, Is.EqualTo(LocalisationStrings.English.InvalidApiKey));
            Assert.That(deletedCallbackCount, Is.Zero);
        }

        private IRenderedComponent<EntryDetailPanel> RenderPanel()
            => RenderPanel(() => { }, () => { });

        private IRenderedComponent<EntryDetailPanel> RenderPanel(
            Action editedCallback,
            Action deletedCallback)
            => testContext.Render<EntryDetailPanel>(parameters => parameters
                .Add(component => component.Entry, new LogEntry
                {
                    Id = "L42",
                    Date = "2020-05-21",
                    Text = "Visited Solara",
                    RawText = "L42 2020-05-21: Visited Solara"
                })
                .Add(component => component.OnEdited, editedCallback)
                .Add(component => component.OnDeleted, deletedCallback));

        private void VerifyNoUpdateRequest()
            => client.Verify(apiClient => apiClient.SendRequestAsync<UpdateLogRequest, NuciApiSuccessResponse>(
                It.IsAny<HttpMethod>(),
                It.IsAny<UpdateLogRequest>(),
                It.IsAny<NuciApiRequestAuthorisationInfo>(),
                It.IsAny<string>()), Times.Never());

        private void VerifyAuthorisation<TRequest, TResponse>(HttpMethod method, Times times)
            where TRequest : NuciApiRequest
            where TResponse : NuciApiResponse
            => client.Verify(apiClient => apiClient.SendRequestAsync<TRequest, TResponse>(
                method,
                It.IsAny<TRequest>(),
                It.Is<NuciApiRequestAuthorisationInfo>(authorisation =>
                    string.Equals(authorisation.BearerToken, "NucileRullz!") &&
                    authorisation.ClientId.StartsWith("PersonalLogManagerClient_")),
                It.IsAny<string>()), times);
    }
}
