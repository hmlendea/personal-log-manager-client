using System;

namespace PersonalLogManagerClient.Services
{
    public sealed class PageTitleService
    {
        public event Action OnChange;

        public string Title { get; private set; } = string.Empty;

        public void SetTitle(string title)
        {
            if (Title == title)
            {
                return;
            }

            Title = title;
            OnChange?.Invoke();
        }
    }
}
