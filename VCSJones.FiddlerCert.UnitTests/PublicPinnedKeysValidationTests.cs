using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace VCSJones.FiddlerCert.UnitTests
{
    [TestFixture]
    public class PublicPinnedKeysValidationTests
    {
        [Test]
        public void ShouldHaveFlagForNoPins()
        {
            var keys = new PublicPinnedKeys(new List<PinnedKey>(), null, null, null);
            var validation = keys.IsValidPinning(new List<PinnedKey>());
            Assert.That(validation & PinCheckResult.ZeroPins, Is.EqualTo(PinCheckResult.ZeroPins));
        }

        [Test]
        public void ShouldHaveFlagForNoMaxAge()
        {
            var keys = new PublicPinnedKeys(new List<PinnedKey>(), null, null, null);
            var validation = keys.IsValidPinning(new List<PinnedKey>());
            Assert.That(validation & PinCheckResult.MissingMaxAge, Is.EqualTo(PinCheckResult.MissingMaxAge));
        }

        [Test]
        public void ShouldHaveFlagForOutOfBoundsMaxAgeLowerBound()
        {
            var keys = new PublicPinnedKeys(new List<PinnedKey>(), TimeSpan.FromSeconds(-1), null, null);
            var validation = keys.IsValidPinning(new List<PinnedKey>());
            Assert.That(validation & PinCheckResult.OutOfRangeMaxAge, Is.EqualTo(PinCheckResult.OutOfRangeMaxAge));
        }

        [Test]
        public void ShouldHaveFlagForOutOfBoundsMaxAgeUpperBound()
        {
            var keys = new PublicPinnedKeys(new List<PinnedKey>(), TimeSpan.FromSeconds(uint.MaxValue + 1L), null, null);
            var validation = keys.IsValidPinning(new List<PinnedKey>());
            Assert.That(validation & PinCheckResult.OutOfRangeMaxAge, Is.EqualTo(PinCheckResult.OutOfRangeMaxAge));
        }
    }
}
