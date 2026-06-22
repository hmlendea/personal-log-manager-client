using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace PersonalLogManagerClient.Services
{
    public sealed class ApiKeyService(IJSRuntime js)
    {
        private const string StorageKey = "plm_api_key";

        public async Task<string> GetApiKeyAsync()
            => await js.InvokeAsync<string>("localStorage.getItem", StorageKey);

        public async Task SetApiKeyAsync(string key)
            => await js.InvokeVoidAsync("localStorage.setItem", StorageKey, key);

        public async Task ClearApiKeyAsync()
            => await js.InvokeVoidAsync("localStorage.removeItem", StorageKey);
    }
}
