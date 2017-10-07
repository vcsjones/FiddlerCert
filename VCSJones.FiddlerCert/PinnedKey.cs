using System;

namespace VCSJones.FiddlerCert
{
    public class PinnedKey : IEquatable<PinnedKey>
    {
        public PinnedKey(PinAlgorithm algorithm, byte[] fingerprint)
        {
            if (fingerprint == null)
            {
                throw new ArgumentNullException(nameof(fingerprint));
            }
            Algorithm = algorithm;
            Fingerprint = (byte[]) fingerprint.Clone();
        }

        public PinAlgorithm Algorithm { get; }
        public byte[] Fingerprint { get; }

        public string FingerprintBase64 => Convert.ToBase64String(Fingerprint);

        public bool Equals(PinnedKey other)
        {
            if (other == null) return false;
            if (ReferenceEquals(this, other)) return true;
            if (Algorithm != other.Algorithm) return false;
            if (Fingerprint.Length != other.Fingerprint.Length) return false;
            if (!Fingerprint.MemoryCompare(other.Fingerprint)) return false;
            return true;
        }
    }
}