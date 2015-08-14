using System;
using NUnit.Framework;

namespace VCSJones.FiddlerCert.UnitTests
{
    [TestFixture]
    public class PublicKeyPinsParserTests
    {
        [Test]
        public void ShouldParseSha256HpkpDirectives()
        {
            var input = "pin-sha256=\"jV54RY1EPxNKwrQKIa5QMGDNPSbj3VwLPtXaHiEE8y8=\";" +
                        "pin-sha256=\"7qVfhXJFRlcy/9VpKFxHBuFzvQZSqajgfRwvsdx1oG8=\";" +
                        "pin-sha256=\"/sMEqQowto9yX5BozHLPdnciJkhDiL5+Ug0uil3DkUM=\";" +
                        "max-age=5184000;";
            var result = PublicKeyPinsParser.Parse(input);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.IncludeSubDomains, Is.Null);
            Assert.That(result.MaxAge, Is.EqualTo(TimeSpan.FromDays(60)));
            Assert.That(result.ReportUri, Is.Null);
            Assert.That(result.PinnedKeys.Count, Is.EqualTo(3));
        }

        [Test]
        public void ComparePinnedKeysByAlgorithm()
        {
            var pinnedKeySha1 = new PinnedKey(PinAlgorithm.SHA1, new byte[0]);
            var pinnedKeySha256 = new PinnedKey(PinAlgorithm.SHA256, new byte[0]);
            Assert.That(pinnedKeySha1, Is.Not.EqualTo(pinnedKeySha256));
        }

        [Test]
        public void ComparePinnedKeysByFingerprints()
        {
            var pinnedKey1 = new PinnedKey(PinAlgorithm.SHA1, new byte[] { 1 });
            var pinnedKey2 = new PinnedKey(PinAlgorithm.SHA1, new byte[] { 2 });
            Assert.That(pinnedKey1, Is.Not.EqualTo(pinnedKey2));
        }

        [Test]
        public void ComparePinnedKeysByFingerprintsAllSame()
        {
            var pinnedKey1 = new PinnedKey(PinAlgorithm.SHA1, new byte[] { 1 });
            var pinnedKey2 = new PinnedKey(PinAlgorithm.SHA1, new byte[] { 1 });
            Assert.That(pinnedKey1, Is.EqualTo(pinnedKey2));
        }
    }
}
