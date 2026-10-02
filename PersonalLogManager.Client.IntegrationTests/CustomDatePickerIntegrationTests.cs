using System;
using System.Collections.Generic;
using System.Linq;

using Bunit;
using NUnit.Framework;
using PersonalLogManagerClient.Layout;

namespace PersonalLogManagerClient.IntegrationTests
{
    [TestFixture]
    public sealed class CustomDatePickerIntegrationTests
    {
        private BunitContext testContext = null!;

        [SetUp]
        public void SetUp() => testContext = new BunitContext();

        [TearDown]
        public void TearDown() => testContext.Dispose();

        [Test]
        public void GivenAnIsoDate_WhenRendering_ThenItsDayMonthAndYearPopulateTheInputs()
        {
            IRenderedComponent<CustomDatePicker> component = RenderPicker("2020-05-21", "2020-05-21");

            Assert.That(component.Find(".date-part-day").GetAttribute("value"), Is.EqualTo("21"));
            Assert.That(component.Find(".date-part-month").GetAttribute("value"), Is.EqualTo("5"));
            Assert.That(component.Find(".date-part-year").GetAttribute("value"), Is.EqualTo("2020"));
        }

        [TestCase("", "0", "0", "0")]
        [TestCase("not-a-date", "0", "0", "0")]
        public void GivenAnInvalidDate_WhenRendering_ThenAllDatePartsRemainZero(
            string value,
            string expectedDay,
            string expectedMonth,
            string expectedYear)
        {
            IRenderedComponent<CustomDatePicker> component = RenderPicker(value, "");

            Assert.That(component.Find(".date-part-day").GetAttribute("value"), Is.EqualTo(expectedDay));
            Assert.That(component.Find(".date-part-month").GetAttribute("value"), Is.EqualTo(expectedMonth));
            Assert.That(component.Find(".date-part-year").GetAttribute("value"), Is.EqualTo(expectedYear));
        }

        [Test]
        public void GivenADisabledPicker_WhenClickingItsWrapper_ThenInputsStayDisabledAndCalendarDoesNotOpen()
        {
            IRenderedComponent<CustomDatePicker> component = RenderPicker("2020-05-21", "2020-05-21", true);

            component.Find(".date-picker-custom").Click();

            Assert.That(component.FindAll("input").All(input => input.HasAttribute("disabled")));
            Assert.That(component.Find(".date-picker-custom").ClassList, Does.Contain("is-disabled"));
            Assert.That(component.FindAll(".date-calendar"), Is.Empty);
        }

        [Test]
        public void GivenAnEnabledPicker_WhenOpeningTheCalendar_ThenWeekdayLabelsAndMonthDaysRender()
        {
            IRenderedComponent<CustomDatePicker> component = RenderPicker("2020-05-21", "2020-05-31");

            component.Find(".date-picker-custom").Click();

            Assert.That(
                component.FindAll(".calendar-day-label").Select(label => label.TextContent),
                Is.EqualTo(new[] { "L", "M", "M", "J", "V", "S", "D" }));
            Assert.That(component.FindAll(".calendar-day"), Has.Count.EqualTo(31));
            Assert.That(component.Find(".calendar-day.is-selected").TextContent, Is.EqualTo("21"));
        }

        [Test]
        public void GivenAnOpenCalendar_WhenClickingTheOverlay_ThenTheCalendarCloses()
        {
            IRenderedComponent<CustomDatePicker> component = RenderPicker("2020-05-21", "2020-05-31");
            component.Find(".date-picker-custom").Click();

            component.Find(".date-calendar-overlay").Click();

            Assert.That(component.FindAll(".date-calendar"), Is.Empty);
        }

        [Test]
        public void GivenAnOpenCalendar_WhenClickingTheWrapperAgain_ThenTheCalendarCloses()
        {
            IRenderedComponent<CustomDatePicker> component = RenderPicker("2020-05-21", "2020-05-31");
            component.Find(".date-picker-custom").Click();

            component.Find(".date-picker-custom").Click();

            Assert.That(component.FindAll(".date-calendar"), Is.Empty);
        }

        [Test]
        public void GivenAnOpenCalendar_WhenSelectingADay_ThenIsoValueIsEmittedAndCalendarCloses()
        {
            List<string> emittedValues = [];
            IRenderedComponent<CustomDatePicker> component = RenderPicker(
                "2020-05-21",
                "2020-05-31",
                false,
                emittedValues.Add);
            component.Find(".date-picker-custom").Click();

            component.FindAll(".calendar-day").Single(day => string.Equals(day.TextContent, "8")).Click();

            Assert.That(emittedValues, Is.EqualTo(new[] { "2020-05-08" }));
            Assert.That(component.FindAll(".date-calendar"), Is.Empty);
        }

        [TestCase("0", "2020-05-01")]
        [TestCase("1", "2020-05-01")]
        [TestCase("31", "2020-05-31")]
        [TestCase("32", "2020-05-31")]
        public void GivenADayBoundary_WhenChangingDay_ThenTheClampedValidDateIsEmitted(
            string enteredDay,
            string expectedDate)
        {
            List<string> emittedValues = [];
            IRenderedComponent<CustomDatePicker> component = RenderPicker(
                "2020-05-21",
                "",
                false,
                emittedValues.Add);

            component.Find(".date-part-day").Change(enteredDay);

            Assert.That(emittedValues, Is.EqualTo(new[] { expectedDate }));
        }

        [TestCase("0", "2020-01-21")]
        [TestCase("1", "2020-01-21")]
        [TestCase("12", "2020-12-21")]
        [TestCase("13", "2020-12-21")]
        public void GivenAMonthBoundary_WhenChangingMonth_ThenTheClampedValidDateIsEmitted(
            string enteredMonth,
            string expectedDate)
        {
            List<string> emittedValues = [];
            IRenderedComponent<CustomDatePicker> component = RenderPicker(
                "2020-05-21",
                "",
                false,
                emittedValues.Add);

            component.Find(".date-part-month").Change(enteredMonth);

            Assert.That(emittedValues, Is.EqualTo(new[] { expectedDate }));
        }

        [TestCase("1994", "1995-05-21")]
        [TestCase("1995", "1995-05-21")]
        [TestCase("2099", "2099-05-21")]
        [TestCase("2100", "2099-05-21")]
        public void GivenAYearBoundary_WhenChangingYear_ThenTheClampedValidDateIsEmitted(
            string enteredYear,
            string expectedDate)
        {
            List<string> emittedValues = [];
            IRenderedComponent<CustomDatePicker> component = RenderPicker(
                "2020-05-21",
                "",
                false,
                emittedValues.Add);

            component.Find(".date-part-year").Change(enteredYear);

            Assert.That(emittedValues, Is.EqualTo(new[] { expectedDate }));
        }

        [TestCase("day", "21")]
        [TestCase("month", "5")]
        [TestCase("year", "2020")]
        public void GivenAnUnchangedDatePart_WhenChangingIt_ThenNoDuplicateValueIsEmitted(
            string datePart,
            string enteredValue)
        {
            List<string> emittedValues = [];
            IRenderedComponent<CustomDatePicker> component = RenderPicker(
                "2020-05-21",
                "",
                false,
                emittedValues.Add);

            component.Find($".date-part-{datePart}").Change(enteredValue);

            Assert.That(emittedValues, Is.Empty);
        }

        [Test]
        public void GivenAnImpossibleDayForTheSelectedMonth_WhenChangingMonth_ThenNoValueIsEmitted()
        {
            List<string> emittedValues = [];
            IRenderedComponent<CustomDatePicker> component = RenderPicker(
                "2020-05-31",
                "",
                false,
                emittedValues.Add);

            component.Find(".date-part-month").Change("4");

            Assert.That(emittedValues, Is.Empty);
        }

        [Test]
        public void GivenAMaximumDate_WhenEditingBeyondIt_ThenNoValueIsEmitted()
        {
            List<string> emittedValues = [];
            IRenderedComponent<CustomDatePicker> component = RenderPicker(
                "2020-05-21",
                "2020-05-21",
                false,
                emittedValues.Add);

            component.Find(".date-part-day").Change("22");

            Assert.That(emittedValues, Is.Empty);
        }

        [Test]
        public void GivenAMaximumDate_WhenOpeningItsMonth_ThenLaterDaysAreDisabled()
        {
            IRenderedComponent<CustomDatePicker> component = RenderPicker("2020-05-21", "2020-05-21");

            component.Find(".date-picker-custom").Click();

            Assert.That(component.FindAll(".calendar-day").Single(day => string.Equals(day.TextContent, "21")).HasAttribute("disabled"), Is.False);
            Assert.That(component.FindAll(".calendar-day").Single(day => string.Equals(day.TextContent, "22")).HasAttribute("disabled"));
            Assert.That(component.FindAll(".calendar-day.is-future"), Has.Count.EqualTo(10));
        }

        [Test]
        public void GivenTheMinimumSupportedMonth_WhenOpeningTheCalendar_ThenPreviousNavigationIsDisabled()
        {
            IRenderedComponent<CustomDatePicker> component = RenderPicker("1995-01-08", "2020-05-21");

            component.Find(".date-picker-custom").Click();

            Assert.That(component.FindAll(".btn-cal-nav")[0].HasAttribute("disabled"));
        }

        [Test]
        public void GivenTheMaximumMonth_WhenOpeningTheCalendar_ThenNextNavigationIsDisabled()
        {
            IRenderedComponent<CustomDatePicker> component = RenderPicker("2020-05-08", "2020-05-21");

            component.Find(".date-picker-custom").Click();

            Assert.That(component.FindAll(".btn-cal-nav")[1].HasAttribute("disabled"));
        }

        [Test]
        public void GivenAnIntermediateMonth_WhenNavigatingBackwardAndForward_ThenBothMonthsRender()
        {
            IRenderedComponent<CustomDatePicker> component = RenderPicker("2020-04-08", "2020-05-21");
            component.Find(".date-picker-custom").Click();

            component.FindAll(".btn-cal-nav")[0].Click();
            Assert.That(component.Find(".calendar-month-label").TextContent, Does.Contain("March"));
            component.FindAll(".btn-cal-nav")[1].Click();

            Assert.That(component.Find(".calendar-month-label").TextContent, Does.Contain("April"));
        }

        [Test]
        public void GivenTheMonthView_WhenOpeningMonthPicker_ThenAllMonthsRenderAndFutureMonthsAreDisabled()
        {
            IRenderedComponent<CustomDatePicker> component = RenderPicker("2020-05-08", "2020-05-21");
            component.Find(".date-picker-custom").Click();

            component.FindAll(".btn-cal-header")[0].Click();

            Assert.That(component.FindAll(".calendar-month-btn"), Has.Count.EqualTo(12));
            Assert.That(component.FindAll(".calendar-month-btn.is-future"), Has.Count.EqualTo(7));
            Assert.That(component.Find(".calendar-month-btn.is-selected").TextContent.Trim(), Is.EqualTo("May"));
        }

        [Test]
        public void GivenTheMonthPicker_WhenSelectingAMonth_ThenThatMonthCalendarRenders()
        {
            IRenderedComponent<CustomDatePicker> component = RenderPicker("2020-05-08", "2020-05-21");
            component.Find(".date-picker-custom").Click();
            component.FindAll(".btn-cal-header")[0].Click();

            component.FindAll(".calendar-month-btn").Single(month => string.Equals(month.TextContent.Trim(), "Mar")).Click();

            Assert.That(component.Find(".calendar-month-label").TextContent, Does.Contain("March"));
            Assert.That(component.FindAll(".calendar-day"), Has.Count.EqualTo(31));
        }

        [Test]
        public void GivenTheMonthView_WhenOpeningYearPicker_ThenEverySupportedYearThroughMaximumRenders()
        {
            IRenderedComponent<CustomDatePicker> component = RenderPicker("2020-05-08", "2020-05-21");
            component.Find(".date-picker-custom").Click();

            component.FindAll(".btn-cal-header")[1].Click();

            Assert.That(component.FindAll(".calendar-year-btn"), Has.Count.EqualTo(26));
            Assert.That(component.FindAll(".calendar-year-btn").First().TextContent.Trim(), Is.EqualTo("2020"));
            Assert.That(component.FindAll(".calendar-year-btn").Last().TextContent.Trim(), Is.EqualTo("1995"));
        }

        [Test]
        public void GivenTheYearPicker_WhenSelectingAYear_ThenMonthSelectionForThatYearRenders()
        {
            IRenderedComponent<CustomDatePicker> component = RenderPicker("2020-05-08", "2020-05-21");
            component.Find(".date-picker-custom").Click();
            component.FindAll(".btn-cal-header")[1].Click();

            component.FindAll(".calendar-year-btn").Single(year => string.Equals(year.TextContent.Trim(), "2012")).Click();

            Assert.That(component.Find(".btn-cal-header").TextContent.Trim(), Is.EqualTo("2012"));
            Assert.That(component.FindAll(".calendar-month-btn"), Has.Count.EqualTo(12));
            Assert.That(component.FindAll(".calendar-month-btn.is-future"), Is.Empty);
        }

        [Test]
        public void GivenNewParameters_WhenRerendering_ThenDatePartsSynchroniseToTheNewValue()
        {
            IRenderedComponent<CustomDatePicker> component = RenderPicker("2020-05-21", "2020-05-21");

            component.Render(parameters => parameters
                .Add(picker => picker.Value, "2012-09-05")
                .Add(picker => picker.Max, "2020-05-21"));

            Assert.That(component.Find(".date-part-day").GetAttribute("value"), Is.EqualTo("5"));
            Assert.That(component.Find(".date-part-month").GetAttribute("value"), Is.EqualTo("9"));
            Assert.That(component.Find(".date-part-year").GetAttribute("value"), Is.EqualTo("2012"));
        }

        private IRenderedComponent<CustomDatePicker> RenderPicker(string value, string maximum)
            => RenderPicker(value, maximum, false, changedValue => { });

        private IRenderedComponent<CustomDatePicker> RenderPicker(string value, string maximum, bool isDisabled)
            => RenderPicker(value, maximum, isDisabled, changedValue => { });

        private IRenderedComponent<CustomDatePicker> RenderPicker(
            string value,
            string maximum,
            bool isDisabled,
            Action<string> valueChanged)
            => testContext.Render<CustomDatePicker>(parameters => parameters
                .Add(picker => picker.Value, value)
                .Add(picker => picker.Max, maximum)
                .Add(picker => picker.Disabled, isDisabled)
                .Add(picker => picker.ValueChanged, valueChanged));
    }
}
