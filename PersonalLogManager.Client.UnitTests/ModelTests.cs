using System.Collections.Generic;
using System.Text.Json;
using NUnit.Framework;
using PersonalLogManagerClient.Models;

namespace PersonalLogManagerClient.UnitTests
{
    [TestFixture]
    public sealed class ModelTests
    {
        [Test]
        public void GivenALogEntry_WhenAssigningAllProperties_ThenTheyCanBeReadUnchanged()
        {
            LogEntry entry = new()
            {
                Id = "L42",
                Date = "2026-09-17",
                Text = "Entry text",
                RawText = "L42 2026-09-17: Entry text"
            };

            Assert.That(entry.Id, Is.EqualTo("L42"));
            Assert.That(entry.Date, Is.EqualTo("2026-09-17"));
            Assert.That(entry.Text, Is.EqualTo("Entry text"));
            Assert.That(entry.RawText, Is.EqualTo("L42 2026-09-17: Entry text"));
        }

        [Test]
        public void GivenAGetLogsRequest_WhenAssigningProperties_ThenTheyCanBeReadUnchanged()
        {
            GetLogsRequest request = new()
            {
                Date = "2026-09-17",
                Count = 25,
                Localisation = "ro"
            };

            Assert.That(request.Date, Is.EqualTo("2026-09-17"));
            Assert.That(request.Count, Is.EqualTo(25));
            Assert.That(request.Localisation, Is.EqualTo("ro"));
        }

        [Test]
        public void GivenAGetLogsResponse_WhenAssigningProperties_ThenTheyCanBeReadUnchanged()
        {
            GetLogsResponse response = new()
            {
                Logs = ["first", "second"],
                Count = 2
            };

            Assert.That(response.Logs, Has.Count.EqualTo(2));
            Assert.That(response.Logs, Is.EqualTo(new[] { "first", "second" }));
            Assert.That(response.Count, Is.EqualTo(2));
        }

        [Test]
        public void GivenAGetLogsResponse_WhenConstructingIt_ThenLogsIsAnEmptyCollection()
            => Assert.That(new GetLogsResponse().Logs, Is.Empty);

        [Test]
        public void GivenAGetLogByIdRequest_WhenConstructingIt_ThenItIsCreatedSuccessfully()
            => Assert.That(new GetLogByIdRequest(), Is.Not.Null);

        [Test]
        public void GivenAGetLogByIdResponse_WhenAssigningAllProperties_ThenTheyCanBeReadUnchanged()
        {
            GetLogByIdResponse response = new()
            {
                Id = "L42",
                Date = "2026-09-17",
                Time = "08:09:10",
                TimeZone = "Europe/London",
                Template = "template",
                Data = new Dictionary<string, string> { ["key"] = "value" },
                CreatedDateTime = "2026-09-17T08:09:10Z",
                UpdatedDateTime = "2026-09-17T08:10:10Z"
            };

            Assert.That(response.Id, Is.EqualTo("L42"));
            Assert.That(response.Date, Is.EqualTo("2026-09-17"));
            Assert.That(response.Time, Is.EqualTo("08:09:10"));
            Assert.That(response.TimeZone, Is.EqualTo("Europe/London"));
            Assert.That(response.Template, Is.EqualTo("template"));
            Assert.That(response.Data["key"], Is.EqualTo("value"));
            Assert.That(response.CreatedDateTime, Is.EqualTo("2026-09-17T08:09:10Z"));
            Assert.That(response.UpdatedDateTime, Is.EqualTo("2026-09-17T08:10:10Z"));
        }

        [Test]
        public void GivenADeleteLogRequest_WhenSerialisingIt_ThenTheIdUsesTheExpectedJsonName()
        {
            DeleteLogRequest request = new() { Id = "L42" };

            string json = JsonSerializer.Serialize(request);

            Assert.That(json, Does.Contain("\"id\":\"L42\""));
        }

        [Test]
        public void GivenAnUpdateLogRequest_WhenSerialisingIt_ThenAllPropertiesUseTheExpectedJsonNames()
        {
            UpdateLogRequest request = new()
            {
                Date = "2026-09-17",
                Time = "08:09:10",
                TimeZone = "Europe/London",
                Data = new Dictionary<string, JsonElement>
                {
                    ["key"] = JsonDocument.Parse("\"value\"").RootElement
                }
            };

            string json = JsonSerializer.Serialize(request);

            Assert.That(json, Does.Contain("\"date\":\"2026-09-17\""));
            Assert.That(json, Does.Contain("\"time\":\"08:09:10\""));
            Assert.That(json, Does.Contain("\"timeZone\":\"Europe/London\""));
            Assert.That(json, Does.Contain("\"data\":{\"key\":\"value\"}"));
        }
    }
}
