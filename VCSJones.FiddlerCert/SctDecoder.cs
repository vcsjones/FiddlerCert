using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using VCSJones.FiddlerCert.Interop;
using static VCSJones.FiddlerCert.KnownDecodeEncodeConstants;

namespace VCSJones.FiddlerCert
{
    public enum SctHashAlgorithm : byte
    {
        HASH_ALGO_NONE = 0,
        HASH_ALGO_MD5 = 1,
        HASH_ALGO_SHA1 = 2,
        HASH_ALGO_SHA224 = 3,
        HASH_ALGO_SHA256 = 4,
        HASH_ALGO_SHA384 = 5,
        HASH_ALGO_SHA512 = 6,
    }

    public enum SctSignatureAlgorithm : byte
    {
        SIG_ALGO_ANONYMOUS = 0,
        SIG_ALGO_RSA = 1,
        SIG_ALGO_DSA = 2,
        SIG_ALGO_ECDSA = 3
    }

    public enum SctVersion : byte
    {
        SCT_VERSION_1 = 0,
    }

    public class SctSignature
    {
        public byte[] Extensions { get; set; }
        public SctHashAlgorithm HashAlgorithm { get; set; }
        public byte[] LogId { get; set; }
        public SctSignatureAlgorithm SignatureAlgorithm { get; set; }
        public DateTime Timestamp { get; set; }
        public byte[] Signature { get; set; }
        public int Index { get; set; }
    }

    public static class SctDecoder
    {
        private const int LOG_ID_SIZE = 32;
        public static IList<SctSignature> DecodeData(byte[] rawData)
        {
            var signatures = new List<SctSignature>();
            var rData = new ArrayOffset<byte>(rawData, 0);
            if (!ReadChunkUInt16Header(ref rData, out var list))
            {
                return signatures;
            }
            int index = 0;
            while (ReadChunkUInt16Header(ref list, out var sct))
            {
                var originalSct = sct;
                if (
                    !ReadByteEnumeration(ref sct, out SctVersion version) ||
                    !ReadFixedData(ref sct, LOG_ID_SIZE, out byte[] logId) ||
                    !ReadTimestamp(ref sct, out DateTime timestamp) ||
                    !ReadVariableDataUInt16Header(ref sct, out byte[] extensions) ||
                    !ReadByteEnumeration(ref sct, out SctHashAlgorithm hashAlgorithm) ||
                    !ReadByteEnumeration(ref sct, out SctSignatureAlgorithm signatureAlgorithm) ||
                    !ReadVariableDataUInt16Header(ref sct, out byte[] signature)
                )
                {
                    continue;
                }
                signatures.Add(new SctSignature
                {
                    LogId = logId,
                    Timestamp = timestamp,
                    HashAlgorithm = hashAlgorithm,
                    SignatureAlgorithm = signatureAlgorithm,
                    Extensions = extensions,
                    Signature = signature,
                    Index = index++
                });
            }
            return signatures;
        }

        public static IList<SctSignature> DecodeData(AsnEncodedData data) => DecodeData(DecodeOctetString(data.RawData));

        private static bool ReadChunkUInt16Header(ref ArrayOffset<byte> input, out ArrayOffset<byte> output)
        {
            var inputCopy = input;
            const int headerSize = sizeof(ushort);
            if (inputCopy.Length < headerSize)
            {
                output = default(ArrayOffset<byte>);
                return false;
            }
            var chunkSize = (ushort)(inputCopy[0] << 8 | inputCopy[1]); //TLS records are big endian.
            if (chunkSize > inputCopy.Length-headerSize)
            {
                output = default(ArrayOffset<byte>);
                return false;
            }
            var amountToConsume = headerSize + chunkSize;
            output = inputCopy + headerSize;
            input += amountToConsume;
            return true;
        }

        private static bool ReadVariableDataUInt16Header(ref ArrayOffset<byte> input, out byte[] data)
        {
            const int headerSize = sizeof(ushort);
            if (input.Length < headerSize)
            {
                data = null;
                return false;
            }
            var chunkSize = (ushort)(input[0] << 8 | input[1]); //Big endian read
            input += headerSize;
            if (chunkSize == 0)
            {
                data = new byte[0];
                return true;
            }
            var result = new byte[chunkSize];
            input.CopyTo(result, 0, chunkSize);
            input += chunkSize;
            data = result;
            return true;
        }

        private static bool ReadByteEnumeration<TEnumType>(ref ArrayOffset<byte> input, out TEnumType value)
        {
            if (!typeof(TEnumType).IsEnum || typeof(TEnumType).GetEnumUnderlyingType() != typeof(byte))
            {
                throw new ArgumentException(nameof(TEnumType));
            }
            const int size = sizeof(byte);
            if (input.Length < size)
            {
                value = default(TEnumType);
                return false;
            }
            var val = input[0];
            value = (TEnumType)(object)val;
            input += size;
            return true;
        }

        private static bool ReadFixedData(ref ArrayOffset<byte> input, int length, out byte[] data)
        {
            if (input.Length < length)
            {
                data = null;
                return false;
            }
            data = new byte[length];
            input.CopyTo(data, 0, length);
            input += length;
            return true;
        }

        private static bool ReadTimestamp(ref ArrayOffset<byte> input, out DateTime timestamp)
        {
            const int size = sizeof(ulong);
            if (input.Length < size)
            {
                timestamp = default(DateTime);
                return false;
            }
            var epochTime = 0UL;
            for (int i = 0, shift = size - 1; i < size; i++, shift--)
            {
                epochTime |= ((ulong)input[i] << (shift * 8));
            }
            timestamp = Epoch.UNIX_EPOCH.AddMilliseconds(epochTime);
            input += size;
            return true;
        }

        private static byte[] DecodeOctetString(byte[] data)
        {
            const EncodingType type = EncodingType.X509_ASN_ENCODING;
            var handle = default(GCHandle);
            try
            {
                handle = GCHandle.Alloc(data, GCHandleType.Pinned);
                var size = 0u;
                const CryptDecodeFlags flags = CryptDecodeFlags.CRYPT_DECODE_ALLOC_FLAG;
                if (!Crypto32.CryptDecodeObjectEx(type, X509_OCTET_STRING, handle.AddrOfPinnedObject(), (uint)data.Length, flags, IntPtr.Zero, out LocalBufferSafeHandle buffer, ref size))
                {
                    return null;
                }
                using (buffer)
                {
                    unsafe
                    {
                        var structure = (CRYPT_OBJID_BLOB)Marshal.PtrToStructure(buffer.DangerousGetHandle(), typeof(CRYPT_OBJID_BLOB));
                        var ret = new byte[structure.cbData];
                        Marshal.Copy(structure.pbData, ret, 0, ret.Length);
                        return ret;
                    }
                }
            }
            finally
            {
                if (handle.IsAllocated)
                {
                    handle.Free();
                }
            }
        }
    }
}
