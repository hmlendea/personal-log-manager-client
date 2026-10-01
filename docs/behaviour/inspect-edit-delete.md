# Inspect, Edit, And Delete Behaviour

**Purpose:** Describe detail retrieval and mutation from list selection.

**Scope:** [Home.razor](../../PersonalLogManager.Client/Pages/Home.razor), [EntryDetailPanel.razor](../../PersonalLogManager.Client/Layout/EntryDetailPanel.razor), and [PersonalLogService.cs](../../PersonalLogManager.Client/Services/PersonalLogService.cs).

**Primary source areas:** `SelectEntry`, `EntryDetailPanel.LoadEntryDetailsAsync`, `ExecuteEditAsync`, and `ExecuteDeleteAsync`.

**Related documents:** [Presentation](../components/presentation.md), [Detail mutation flow](../flows/detail-mutation.md), [Data model](../data-model.md), [Interfaces](../interfaces.md).

## Inspect

Selecting a `LogEntry` stores it in `Home.selectedEntry` and renders a detail panel. The panel requests `GET /PersonalLog/{Entry.Id}` on initialisation. While waiting, edit is disabled and a loading notice is shown. A typed detail response is formatted as indented JSON containing ID, date, time, time zone, template, data, created timestamp, and updated timestamp. An unexpected success response produces null and causes a fallback JSON object containing list ID, date, and text.

## Edit

Starting edit builds an indented JSON wrapper with `date`, `time`, `timeZone`, and `data`. Existing detail values are preferred; list date and text provide fallback values. Saving parses the complete text as JSON, reads string-valued wrapper fields when present, requires a `data` property, enumerates it into cloned `JsonElement` values, and calls `UpdateLogAsync` with the selected ID.

The client catches malformed JSON separately and reports `Invalid JSON format.`. A missing `data` property reports `JSON must contain a "data" object.` even if the property is not an object; enumeration errors are caught by the general exception handler. No client validation is applied to date format, time format, time zone, data keys, or data value types.

A successful update invokes `OnEdited`; `Home` closes the panel and reloads the current calendar or search result view. Save state disables edit controls while the request runs.

## Delete

The first delete click enters confirmation state. Confirmation calls `DeleteLogAsync` with the selected ID; cancellation returns to normal detail state. A successful delete invokes `OnDeleted`; `Home` closes the panel and reloads. Exceptions remain in the panel's delete error field. Delete state disables the delete control while active.

## Boundary and side effects

The panel does not mutate local list data directly and does not persist edits. The remote API is authoritative. A reload after mutation is the consistency mechanism and is performed only after the façade operation completes successfully.
