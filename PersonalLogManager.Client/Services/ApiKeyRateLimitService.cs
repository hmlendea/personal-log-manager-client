using System;
using System.Collections.Generic;

namespace PersonalLogManagerClient.Services
{
    public sealed class ApiKeyRateLimitService
    {
        private const int FailureThreshold = 5;
        private static readonly TimeSpan FailureWindow = TimeSpan.FromMinutes(10);
        private static readonly TimeSpan LockoutDuration = TimeSpan.FromMinutes(30);

        private readonly List<DateTime> failures = [];
        private DateTime? lockExpiryDate;

        public bool IsLocked => lockExpiryDate.HasValue && DateTime.UtcNow < lockExpiryDate;
        public DateTime? LockedUntil => IsLocked ? lockExpiryDate : null;

        public void RecordFailure()
        {
            if (IsLocked)
            {
                return;
            }

            DateTime now = DateTime.UtcNow;
            failures.Add(now);
            failures.RemoveAll(t => t < now - FailureWindow);

            if (failures.Count >= FailureThreshold)
            {
                lockExpiryDate = now + LockoutDuration;
                failures.Clear();
            }
        }
    }
}
