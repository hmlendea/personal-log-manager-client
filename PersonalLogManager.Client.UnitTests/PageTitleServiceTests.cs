using NUnit.Framework;
using PersonalLogManagerClient.Services;

namespace PersonalLogManagerClient.UnitTests
{
    [TestFixture]
    public sealed class PageTitleServiceTests
    {
        private PageTitleService service = null!;

        [SetUp]
        public void SetUp()
        {
            service = new PageTitleService();
        }

        [Test]
        public void GivenARecentlyConstructedService_WhenReadingTitle_ThenAnEmptyTitleIsReturned()
            => Assert.That(service.Title, Is.Empty);

        [Test]
        public void GivenAService_WhenSettingANewTitle_ThenTheTitleChangesAndOneEventIsRaised()
        {
            int changeCount = 0;
            service.OnChange += () => changeCount++;

            service.SetTitle("Entries");

            Assert.That(service.Title, Is.EqualTo("Entries"));
            Assert.That(changeCount, Is.EqualTo(1));
        }

        [Test]
        public void GivenAService_WhenSettingTheSameTitle_ThenTheTitleAndEventCountRemainUnchanged()
        {
            service.SetTitle("Entries");
            int changeCount = 0;
            service.OnChange += () => changeCount++;

            service.SetTitle("Entries");

            Assert.That(service.Title, Is.EqualTo("Entries"));
            Assert.That(changeCount, Is.Zero);
        }

        [TestCase("")]
        [TestCase(" ")]
        [TestCase("Entries")]
        public void GivenAService_WhenSettingATitle_ThenTheExactTitleIsRetained(string title)
        {
            service.SetTitle(title);

            Assert.That(service.Title, Is.EqualTo(title));
        }
    }
}
