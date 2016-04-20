using System;
using Xunit;

namespace VCSJones.FiddlerCert.UnitTests
{
    public class PublicKeyPinsParserTests
    {
        [Fact]
        public void ShouldParseSha256HpkpDirectives()
        {
            var input = "pin-sha256=\"jV54RY1EPxNKwrQKIa5QMGDNPSbj3VwLPtXaHiEE8y8=\";" +
                        "pin-sha256=\"7qVfhXJFRlcy/9VpKFxHBuFzvQZSqajgfRwvsdx1oG8=\";" +
                        "pin-sha256=\"/sMEqQowto9yX5BozHLPdnciJkhDiL5+Ug0uil3DkUM=\";" +
                        "max-age=5184000;";
            var result = PublicKeyPinsParser.Parse(input);
            Assert.NotNull(result);
            Assert.Null(result.IncludeSubDomains);
            Assert.Equal(TimeSpan.FromDays(60), result.MaxAge);
            Assert.Null(result.ReportUri);
            Assert.Equal(3, result.PinnedKeys.Count);
        }

        [Fact]
        public void ComparePinnedKeysByAlgorithm()
        {
            var pinnedKeySha1 = new PinnedKey(PinAlgorithm.SHA1, new byte[0]);
            var pinnedKeySha256 = new PinnedKey(PinAlgorithm.SHA256, new byte[0]);
            Assert.NotStrictEqual(pinnedKeySha256, pinnedKeySha1);
        }

        [Fact]
        public void ComparePinnedKeysByFingerprints()
        {
            var pinnedKey1 = new PinnedKey(PinAlgorithm.SHA1, new byte[] { 1 });
            var pinnedKey2 = new PinnedKey(PinAlgorithm.SHA1, new byte[] { 2 });
            Assert.NotStrictEqual(pinnedKey1, pinnedKey2);
        }

        [Fact]
        public void ComparePinnedKeysByFingerprintsAllSame()
        {
            var pinnedKey1 = new PinnedKey(PinAlgorithm.SHA1, new byte[] { 1 });
            var pinnedKey2 = new PinnedKey(PinAlgorithm.SHA1, new byte[] { 1 });
            Assert.StrictEqual(pinnedKey1, pinnedKey2);
        }
    }
}
