# Presentation Components

**Purpose:** Describe the Blazor route, page state, shared layout, and user interaction components.

**Scope:** [Home.razor](../../PersonalLogManager.Client/Pages/Home.razor), [EntryDetailPanel.razor](../../PersonalLogManager.Client/Layout/EntryDetailPanel.razor), [MainLayout.razor](../../PersonalLogManager.Client/Layout/MainLayout.razor), [ApiKeyWidget.razor](../../PersonalLogManager.Client/Layout/ApiKeyWidget.razor), [LocaleSelector.razor](../../PersonalLogManager.Client/Layout/LocaleSelector.razor), [CustomDatePicker.razor](../../PersonalLogManager.Client/Layout/CustomDatePicker.razor), [AppFooter.razor](../../PersonalLogManager.Client/Layout/AppFooter.razor), [App.razor](../../PersonalLogManager.Client/App.razor), and [Routes.razor](../../PersonalLogManager.Client/Routes.razor).

**Primary source areas:** The linked Razor components and [PageTitleService.cs](../../PersonalLogManager.Client/Services/PageTitleService.cs).

**Related documents:** [Browse and search](../behaviour/browse-and-search.md), [Inspect, edit, and delete](../behaviour/inspect-edit-delete.md), [Calendar flow](../flows/calendar-search.md), [State and persistence](../state-and-persistence.md).

## Home page

`Home` is routed at `/` and implements `IDisposable` to unsubscribe its locale handler. It owns `today`, `yesterday`, `currentDate`, `entries`, `viewMode`, `loading`, `hasApiKey`, `ascending`, `fullWidth`, `error`, `searchTerm`, `appliedSearchTerm`, and `selectedEntry`.

Initialisation reads `sortAscending`, `fullWidth`, and the API key from browser storage, sets the page title, and loads entries only when a key exists. Calendar mode requests `currentDate`; search mode requests only after a trimmed term is submitted. Refresh, date navigation, ordering changes, locale changes, and successful mutations reload the active view.

The page catches exceptions from list/search operations and renders `Exception.Message`. It clears selected state before loads and disables relevant controls while loading. Search displays each item's parsed date separately; calendar mode displays parsed text only.

## Entry detail panel

`EntryDetailPanel` receives a `LogEntry` and callbacks for close, delete, and edit completion. It retrieves details by ID during initialisation. It displays formatted JSON, creates an edit wrapper containing `date`, `time`, `timeZone`, and `data`, parses the edited JSON, requires a `data` property, and forwards the data dictionary to `UpdateLogAsync`. Delete requires a separate confirmation state.

Load, edit, and delete exceptions are retained in separate panel error fields. Successful edit or delete invokes the parent callback; the parent closes the panel and reloads.

## Shared controls

- `MainLayout` renders the API key widget, title, locale selector, body, and footer, and subscribes to title changes.
- The page navigation overlays the application top bar at widths of 1100 pixels or greater. Narrower viewports retain a dedicated navigation row to prevent control and title overlap.
- `ApiKeyWidget` opens a modal authentication dialog, masks input, reports blank submissions, trims saved keys, supports keyboard submission and cancellation, and clears stored credentials during deauthentication.
- `LocaleSelector` initialises the locale service and only submits supported locale values.
- `CustomDatePicker` accepts `Value`, `Max`, and `Disabled`, clamps numeric parts, rejects invalid or future dates, and emits ISO `yyyy-MM-dd` values. Its selectable year range starts at 1995.
- `AppFooter` renders links to the API and client repositories and responds to locale changes.
- `App` computes the base href from `ServerSettings.PathBase` and uses non-prerendered interactive server routes.
- `Routes` maps discovered routes to `MainLayout` and supplies a not-found layout view.

## Boundary

Components orchestrate presentation and invoke services. They do not construct `HttpClient`, decide API response codes, or persist logs locally. The edit panel intentionally passes arbitrary JSON data through as `JsonElement` values; upstream validation remains external.
