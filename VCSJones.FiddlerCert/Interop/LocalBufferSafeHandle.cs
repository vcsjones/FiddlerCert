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

        public void CopyTo(byte[] buffer, int offset = 0, int? length = null)
        {
            if (IsClosed || IsInvalid)
            {
                throw new InvalidOperationException("Handle is closed or invalid.");
            }
            var len = length ?? buffer.Length;
            Marshal.Copy(handle, buffer, offset, len);
        }

        protected override bool ReleaseHandle()
        {
            return IntPtr.Zero == LocalFree(handle);
        }
    }
}
