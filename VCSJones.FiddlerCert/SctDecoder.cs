using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using VCSJones.FiddlerCert.Interop;
using static VCSJones.FiddlerCert.KnownDecodeEncodeConstants;

namespace VCSJones.FiddlerCert
{
    public enum SctVersion : byte
    {
        SCT_VERSION_1 = 0,
    }

    public class SctDecoder
    {
        private const int LOG_ID_SIZE = 32;
        private static readonly DateTime UNIX_EPOCH = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static void DecodeData(AsnEncodedData data)
        {
            var decodedOctets = DecodeOctetString(data.RawData);
            byte[] peices = decodedOctets;
            var sctList = ReadChunkUInt16Header(ref peices);
            byte[] listItem;
            while ((listItem = ReadChunkUInt16Header(ref sctList)) != null)
            {
                var version = ReadSctVersion(ref listItem);
                if (version != SctVersion.SCT_VERSION_1) continue;
                var logId = ReadChunk(ref listItem, LOG_ID_SIZE);
                if (logId == null)  continue;
                var timestamp = ReadTimestamp(ref listItem);
                if (timestamp == null) continue;
            }
        }

        public static IList<byte[]> ReadPieces(byte[] data)
        {
            var items = new List<byte[]>();

            return items;
        }

        private static byte[] ReadChunkUInt16Header(ref byte[] input)
        {
            const int size = sizeof(ushort);
            if (input.Length < size)
            {
                return null;
            }
            var chunkSize = (ushort)(input[0] << 8 | input[1]); //TLS records are big endian.
            if (chunkSize == 0)
            {
                return new byte[0];
            }
            var amountToConsume = size + chunkSize;
            var chunk = new byte[chunkSize];
            Buffer.BlockCopy(input, size, chunk, 0, chunkSize);
            var newInputSize = input.Length - amountToConsume;
            if (newInputSize > 0)
            {
                var resizeInput = new byte[newInputSize];
                Buffer.BlockCopy(input, amountToConsume, resizeInput, 0, newInputSize);
                input = resizeInput;
            }
            else
            {
                input = new byte[0];
            }
            return chunk;
        }

        private static SctVersion? ReadSctVersion(ref byte[] input)
        {
            const int size = sizeof(SctVersion);
            if (input.Length < size)
            {
                return null;
            }
            var version = (SctVersion)input[0];
            var newInput = new byte[input.Length - size];
            Buffer.BlockCopy(input, size, newInput, 0, newInput.Length);
            input = newInput;
            return version;
        }

        private static byte[] ReadChunk(ref byte[] input, int size)
        {
            if (input.Length < size)
            {
                return null;
            }
            var chunk = new byte[size];
            Buffer.BlockCopy(input, 0, chunk, 0, size);
            var newInputSize = input.Length - size;
            if (newInputSize > 0)
            {
                var resizeInput = new byte[newInputSize];
                Buffer.BlockCopy(input, size, resizeInput, 0, newInputSize);
                input = resizeInput;
            }
            else
            {
                input = new byte[0];
            }
            return chunk;
        }

        private static DateTime? ReadTimestamp(ref byte[] input)
        {
            const int size = sizeof(ulong);
            if (input.Length < size)
            {
                return null;
            }
            var epochTime = 0UL;
            for (int i = 0, shift = size-1; i < size; i++, shift --)
            {
                epochTime |= ((ulong)input[i] << (shift * 8));
            }
            var timestamp = UNIX_EPOCH.AddMilliseconds(epochTime);
            var newInput = new byte[input.Length - size];
            Buffer.BlockCopy(input, size, newInput, 0, newInput.Length);
            input = newInput;
            return timestamp;
        }

        private static byte[] DecodeOctetString(byte[] data)
        {
            const EncodingType type = EncodingType.X509_ASN_ENCODING;
            var handle = default(GCHandle);
            try
            {
                handle = GCHandle.Alloc(data, GCHandleType.Pinned);
                LocalBufferSafeHandle buffer;
                var size = 0u;
                const CryptDecodeFlags flags = CryptDecodeFlags.CRYPT_DECODE_ALLOC_FLAG;
                if (!Crypto32.CryptDecodeObjectEx(type, X509_OCTET_STRING, handle.AddrOfPinnedObject(), (uint)data.Length, flags, IntPtr.Zero, out buffer, ref size))
                {
                    return null;
                }
                using (buffer)
                {
                    unsafe
                    {
                        var structure = (CRYPT_OBJID_BLOB)Marshal.PtrToStructure(buffer.DangerousGetHandle(), typeof(CRYPT_OBJID_BLOB));
                        var ret = new byte[structure.cbData];
                        Marshal.Copy(new IntPtr(structure.pbData), ret, 0, ret.Length);
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
