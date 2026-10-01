# Browser State And Localisation

**Purpose:** Document browser-backed state and localised presentation support.

**Scope:** [ApiKeyService.cs](../../PersonalLogManager.Client/Services/ApiKeyService.cs), [LocaleService.cs](../../PersonalLogManager.Client/Services/LocaleService.cs), [LocalisationStrings.cs](../../PersonalLogManager.Client/Services/LocalisationStrings.cs), [PageTitleService.cs](../../PersonalLogManager.Client/Services/PageTitleService.cs), [ApiKeyWidget.razor](../../PersonalLogManager.Client/Layout/ApiKeyWidget.razor), and [LocaleSelector.razor](../../PersonalLogManager.Client/Layout/LocaleSelector.razor).

**Primary source areas:** The linked services and components.

**Related documents:** [Presentation](presentation.md), [State and persistence](../state-and-persistence.md), [Security](../security.md), [Configuration](../configuration.md).

## API-key state

The widget reads `plm_api_key`, shows either a masked input and save button or a clear button, trims non-blank input before storage, and clears the input field after saving. `Home` independently reads the same key to decide whether list operations are permitted. Clearing the key changes widget state but does not reset rate-limit state.

## Locale state

`LocaleSelector` calls `LocaleService.InitialiseAsync` during initialisation. Romanian is the default before and after unsupported values; English is selected only for exact `en-GB`. Supported changes persist exact locale identifiers and raise `OnChange`. `Home`, `MainLayout`-related components, and `AppFooter` subscribe to refresh labels and titles.

The API receives only `en` or `ro`, derived from the current locale. Localisation strings include functional notices, controls, titles, entry counts, lockout messages, and footer labels.

## Title state

`PageTitleService` is a scoped evented value. `Home` sets Today, Yesterday, Entries, or Search titles; `MainLayout` reflects changes in its top bar. Equal values do not raise events.

## Lifecycle rule

Components subscribing to locale or title events must unsubscribe in `Dispose`. JS interop failures are not translated and can propagate to the owning component operation.
