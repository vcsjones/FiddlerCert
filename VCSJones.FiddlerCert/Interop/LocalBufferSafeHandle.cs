using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace VCSJones.FiddlerCert.Interop
{
    internal sealed class LocalBufferSafeHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        [return: MarshalAs(UnmanagedType.SysInt)]
        [method: DllImport("kernel32.dll", EntryPoint = "LocalFree", CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern IntPtr LocalFree
            (
            [param: In, MarshalAs(UnmanagedType.SysInt)] IntPtr hMem
            );
        public LocalBufferSafeHandle(bool ownsHandle) : base(ownsHandle)
        {
        }

        public LocalBufferSafeHandle() : this(true)
        {
        }

        public static LocalBufferSafeHandle Zero
        {
            get
            {
                var instance = new LocalBufferSafeHandle(true);
                instance.SetHandle(IntPtr.Zero);
                return instance;
            }
        }

        protected override bool ReleaseHandle()
        {
            return IntPtr.Zero == LocalFree(handle);
        }
    }
}