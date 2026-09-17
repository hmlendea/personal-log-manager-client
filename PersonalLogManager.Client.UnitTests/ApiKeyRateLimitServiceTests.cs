using System;
using NUnit.Framework;
using PersonalLogManagerClient.Services;

namespace PersonalLogManagerClient.UnitTests
{
    [TestFixture]
    public sealed class ApiKeyRateLimitServiceTests
    {
        private ApiKeyRateLimitService service = null!;

        [SetUp]
        public void SetUp()
        {
            service = new ApiKeyRateLimitService();
        }

        [Test]
        public void GivenARecentlyConstructedService_WhenReadingLockState_ThenItIsUnlocked()
        {
            Assert.That(service.IsLocked, Is.False);
            Assert.That(service.LockedUntil, Is.Null);
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        public void GivenFewerThanFiveFailures_WhenRecordingFailures_ThenTheServiceRemainsUnlocked(int failureCount)
        {
            for (int index = 0; index < failureCount; index++)
            {
                service.RecordFailure();
            }

            Assert.That(service.IsLocked, Is.False);
            Assert.That(service.LockedUntil, Is.Null);
        }

        [Test]
        public void GivenFourFailures_WhenRecordingTheFifthFailure_ThenTheServiceLocksForApproximatelyThirtyMinutes()
        {
            DateTime before = DateTime.UtcNow.AddMinutes(29);
            for (int index = 0; index < 5; index++)
            {
                service.RecordFailure();
            }
            DateTime after = DateTime.UtcNow.AddMinutes(31);

            Assert.That(service.IsLocked, Is.True);
            Assert.That(service.LockedUntil, Is.InRange(before, after));
        }

        [Test]
        public void GivenAStoredLock_WhenRecordingAnotherFailure_ThenTheLockExpiryDoesNotChange()
        {
            for (int index = 0; index < 5; index++)
            {
                service.RecordFailure();
            }
            DateTime? lockedUntil = service.LockedUntil;

            service.RecordFailure();

            Assert.That(service.LockedUntil, Is.EqualTo(lockedUntil));
        }
    }
}
