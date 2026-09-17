using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.JSInterop.Infrastructure;
using Moq;
using NUnit.Framework;
using PersonalLogManagerClient.Services;

namespace PersonalLogManagerClient.UnitTests
{
    [TestFixture]
    public sealed class ApiKeyServiceTests
    {
        private Mock<IJSRuntime> js = null!;
        private ApiKeyService service = null!;

        [SetUp]
        public void SetUp()
        {
            js = new Mock<IJSRuntime>();
            service = new ApiKeyService(js.Object);
        }

        [Test]
        public async Task GivenAStoredKey_WhenGettingTheApiKey_ThenTheStoredKeyIsReturned()
        {
            js.Setup(runtime => runtime.InvokeAsync<string>("localStorage.getItem", It.IsAny<object[]>()))
                .Returns(new ValueTask<string>("key-value"));

            string result = await service.GetApiKeyAsync();

            Assert.That(result, Is.EqualTo("key-value"));
            js.Verify(runtime => runtime.InvokeAsync<string>(
                "localStorage.getItem",
                It.Is<object[]>(arguments => arguments.Length == 1 && (string)arguments[0] == "plm_api_key")),
                Times.Once);
        }

        [Test]
        public async Task GivenNoStoredKey_WhenGettingTheApiKey_ThenNullIsReturned()
        {
            js.Setup(runtime => runtime.InvokeAsync<string>("localStorage.getItem", It.IsAny<object[]>()))
                .Returns(new ValueTask<string>((string)null!));

            string result = await service.GetApiKeyAsync();

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GivenAKey_WhenSettingTheApiKey_ThenItIsPersisted()
        {
            js.Setup(runtime => runtime.InvokeAsync<IJSVoidResult>("localStorage.setItem", It.IsAny<object[]>()))
                .Returns(new ValueTask<IJSVoidResult>(Task.FromResult<IJSVoidResult>((IJSVoidResult)null!)));

            await service.SetApiKeyAsync("key-value");

            js.Verify(runtime => runtime.InvokeAsync<IJSVoidResult>(
                "localStorage.setItem",
                It.Is<object[]>(arguments => arguments.Length == 2 &&
                    (string)arguments[0] == "plm_api_key" &&
                    (string)arguments[1] == "key-value")),
                Times.Once);
        }

        [Test]
        public async Task GivenAnEmptyKey_WhenSettingTheApiKey_ThenTheEmptyKeyIsPersisted()
        {
            js.Setup(runtime => runtime.InvokeAsync<IJSVoidResult>("localStorage.setItem", It.IsAny<object[]>()))
                .Returns(new ValueTask<IJSVoidResult>(Task.FromResult<IJSVoidResult>((IJSVoidResult)null!)));

            await service.SetApiKeyAsync("");

            js.Verify(runtime => runtime.InvokeAsync<IJSVoidResult>(
                "localStorage.setItem",
                It.Is<object[]>(arguments => arguments.Length == 2 && (string)arguments[1] == "")),
                Times.Once);
        }

        [Test]
        public async Task GivenAStoredKey_WhenClearingTheApiKey_ThenItIsRemoved()
        {
            js.Setup(runtime => runtime.InvokeAsync<IJSVoidResult>("localStorage.removeItem", It.IsAny<object[]>()))
                .Returns(new ValueTask<IJSVoidResult>(Task.FromResult<IJSVoidResult>((IJSVoidResult)null!)));

            await service.ClearApiKeyAsync();

            js.Verify(runtime => runtime.InvokeAsync<IJSVoidResult>(
                "localStorage.removeItem",
                It.Is<object[]>(arguments => arguments.Length == 1 && (string)arguments[0] == "plm_api_key")),
                Times.Once);
        }
    }
}
