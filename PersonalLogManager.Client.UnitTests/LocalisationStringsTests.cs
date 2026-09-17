using System;
using NUnit.Framework;
using PersonalLogManagerClient.Services;

namespace PersonalLogManagerClient.UnitTests
{
    [TestFixture]
    public sealed class LocalisationStringsTests
    {
        [Test]
        public void GivenEnglishStrings_WhenFormattingNoEntries_ThenTheDateIsIncluded()
            => Assert.That(LocalisationStrings.English.NoEntries("2026-09-17"), Is.EqualTo("No entries for 2026-09-17."));

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(42)]
        [TestCase(-1)]
        public void GivenEnglishStrings_WhenFormattingLogEntries_ThenTheCountIsIncluded(int count)
            => Assert.That(LocalisationStrings.English.LogEntries(count), Is.EqualTo($"{count} log entries"));

        [Test]
        public void GivenEnglishStrings_WhenFormattingLockout_ThenTheTimeUsesTheExpectedFormat()
        {
            DateTime lockedUntil = new(2026, 9, 17, 5, 6, 7);

            Assert.That(
                LocalisationStrings.English.LockedOut(lockedUntil),
                Is.EqualTo("Too many failed attempts. Requests blocked until 05:06."));
        }

        [Test]
        public void GivenRomanianStrings_WhenFormattingNoEntries_ThenTheDateIsIncluded()
            => Assert.That(LocalisationStrings.Romanian.NoEntries("2026-09-17"), Is.EqualTo("Nu există intrări pentru 2026-09-17."));

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(42)]
        [TestCase(-1)]
        public void GivenRomanianStrings_WhenFormattingLogEntries_ThenTheCountIsIncluded(int count)
            => Assert.That(LocalisationStrings.Romanian.LogEntries(count), Is.EqualTo($"{count} intrări în jurnal"));

        [Test]
        public void GivenRomanianStrings_WhenFormattingLockout_ThenTheTimeUsesTheExpectedFormat()
        {
            DateTime lockedUntil = new(2026, 9, 17, 5, 6, 7);

            Assert.That(
                LocalisationStrings.Romanian.LockedOut(lockedUntil),
                Is.EqualTo("Prea multe încercări eșuate. Cererile sunt blocate până la 05:06."));
        }

        [Test]
        public void GivenTheEnglishAndRomanianDefinitions_WhenComparingTheirIdentity_ThenTheyAreDistinctInstances()
            => Assert.That(LocalisationStrings.English, Is.Not.SameAs(LocalisationStrings.Romanian));

        [Test]
        public void GivenTheEnglishDefinition_WhenReadingItsLabels_ThenAllLabelsArePopulated()
        {
            LocalisationStrings strings = LocalisationStrings.English;

            Assert.That(strings.TitleToday, Is.Not.Empty);
            Assert.That(strings.TitleYesterday, Is.Not.Empty);
            Assert.That(strings.TitleEntries, Is.Not.Empty);
            Assert.That(strings.PreviousDay, Is.Not.Empty);
            Assert.That(strings.NextDay, Is.Not.Empty);
            Assert.That(strings.Loading, Is.Not.Empty);
            Assert.That(strings.Refresh, Is.Not.Empty);
            Assert.That(strings.SortAscending, Is.Not.Empty);
            Assert.That(strings.SortDescending, Is.Not.Empty);
            Assert.That(strings.NoApiKeyNotice, Is.Not.Empty);
            Assert.That(strings.InvalidApiKey, Is.Not.Empty);
            Assert.That(strings.EntryDetails, Is.Not.Empty);
            Assert.That(strings.EditEntry, Is.Not.Empty);
            Assert.That(strings.Close, Is.Not.Empty);
            Assert.That(strings.Edit, Is.Not.Empty);
            Assert.That(strings.Delete, Is.Not.Empty);
            Assert.That(strings.ConfirmDelete, Is.Not.Empty);
            Assert.That(strings.ClearKey, Is.Not.Empty);
            Assert.That(strings.ApiKeyPlaceholder, Is.Not.Empty);
            Assert.That(strings.Save, Is.Not.Empty);
            Assert.That(strings.NotFoundTitle, Is.Not.Empty);
            Assert.That(strings.NotFoundMessage, Is.Not.Empty);
            Assert.That(strings.FooterApiSource, Is.Not.Empty);
            Assert.That(strings.FooterClientSource, Is.Not.Empty);
            Assert.That(strings.NoEntries, Is.Not.Null);
            Assert.That(strings.LogEntries, Is.Not.Null);
            Assert.That(strings.LockedOut, Is.Not.Null);
        }
    }
}
