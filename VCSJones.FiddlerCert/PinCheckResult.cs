using System;

namespace VCSJones.FiddlerCert
{
    [Flags]
    public enum PinCheckResult
    {
        Pass = 0x00,
        ZeroPins = 0x01,
        NoBackupPin = 0x02,
        MultiplePinsInSameChain = 0x04,
        MissingMaxAge = 0x08,
        OutOfRangeMaxAge = 0x10,
        NoPinsMatchChain = 0x20
    }
}
