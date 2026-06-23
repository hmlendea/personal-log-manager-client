using System;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace PersonalLogManagerClient.Services
{
    public sealed class LocaleService(IJSRuntime js)
    {
        private const string StorageKey = "plm_locale";
        public const string English = "en-GB";
        public const string Romanian = "ro-RO";

        public event Action OnChange;

        private string _current;

        public string Current => _current ?? Romanian;

        public LocalisationStrings Strings => Current == Romanian ? LocalisationStrings.Romanian : LocalisationStrings.English;

        public async Task InitialiseAsync()
        {
            string stored = await js.InvokeAsync<string>("localStorage.getItem", StorageKey);
            _current = stored == English ? English : Romanian;
        }

        public async Task SetLocaleAsync(string locale)
        {
            if (locale != English && locale != Romanian) return;
            _current = locale;
            await js.InvokeVoidAsync("localStorage.setItem", StorageKey, locale);
            OnChange?.Invoke();
        }
    }
}
