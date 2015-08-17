using System;
using System.Collections.Generic;
using System.Linq;

namespace VCSJones.FiddlerCert
{
    public class PublicPinnedKeys
    {
        public PublicPinnedKeys(IList<PinnedKey> pinnedKeys, TimeSpan? maxAge, bool? includeSubDomains, Uri reportUri)
        {
            PinnedKeys = pinnedKeys;
            MaxAge = maxAge;
            IncludeSubDomains = includeSubDomains;
            ReportUri = reportUri;
        }

        public IList<PinnedKey> PinnedKeys { get; }
        public TimeSpan? MaxAge { get; }
        public bool? IncludeSubDomains { get; }
        public Uri ReportUri { get; }

        public PinCheckResult IsValidPinning(IList<PinnedKey> certificateSpki)
        {
            var result = PinCheckResult.Pass;
            if (PinnedKeys.Count == 0)
            {
                result |= PinCheckResult.ZeroPins;
            }
            if (MaxAge == null)
            {
                result |= PinCheckResult.MissingMaxAge;
            }
            else if (MaxAge.Value < TimeSpan.Zero)
            {
                result |= PinCheckResult.OutOfRangeMaxAge;
            }
            else if (MaxAge.Value > TimeSpan.FromSeconds(uint.MaxValue))
            {
                result |= PinCheckResult.OutOfRangeMaxAge;
            }
            if (!PinnedKeys.Intersect(certificateSpki).Any())
            {
                result |= PinCheckResult.NoPinsMatchChain;
            }
            if (!PinnedKeys.Except(certificateSpki).Any())
            {
                result |= PinCheckResult.NoBackupPin;
            }
            if (PinnedKeys.Intersect(certificateSpki).Count() > 1)
            {
                result |= PinCheckResult.MultiplePinsInSameChain;
            }
            return result;
        }
    }
}