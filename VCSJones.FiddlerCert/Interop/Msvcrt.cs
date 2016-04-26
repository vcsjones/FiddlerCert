    using System;
using System.Runtime.InteropServices;

namespace VCSJones.FiddlerCert.Interop
{
    internal static class Msvcrt
    {
        [method: DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "memcmp")]
        public static extern int memcmp(byte[] buf1, byte[] buf2, UIntPtr count);
    }
}
