using System;
using System.Collections.Generic;
using Xunit;

namespace VCSJones.FiddlerCert.UnitTests
{
    public class PublicPinnedKeysValidationTests
    {
        [Fact]
        public void ShouldHaveFlagForNoPins()
        {
            var keys = new PublicPinnedKeys(new List<PinnedKey>(), null, null, null);
            var validation = keys.IsValidPinning(new List<PinnedKey>());
            Assert.Equal(PinCheckResult.ZeroPins, validation & PinCheckResult.ZeroPins);
        }

        [Fact]
        public void ShouldHaveFlagForNoMaxAge()
        {
            var keys = new PublicPinnedKeys(new List<PinnedKey>(), null, null, null);
            var validation = keys.IsValidPinning(new List<PinnedKey>());
            Assert.Equal(PinCheckResult.MissingMaxAge, validation & PinCheckResult.MissingMaxAge);
        }

        [Fact]
        public void ShouldHaveFlagForOutOfBoundsMaxAgeLowerBound()
        {
            var keys = new PublicPinnedKeys(new List<PinnedKey>(), TimeSpan.FromSeconds(-1), null, null);
            var validation = keys.IsValidPinning(new List<PinnedKey>());
            Assert.Equal(PinCheckResult.OutOfRangeMaxAge, validation & PinCheckResult.OutOfRangeMaxAge);
        }

        [Fact]
        public void ShouldHaveFlagForOutOfBoundsMaxAgeUpperBound()
        {
            var keys = new PublicPinnedKeys(new List<PinnedKey>(), TimeSpan.FromSeconds(uint.MaxValue + 1L), null, null);
            var validation = keys.IsValidPinning(new List<PinnedKey>());
            Assert.Equal(PinCheckResult.OutOfRangeMaxAge, validation & PinCheckResult.OutOfRangeMaxAge);
        }
    }
}
