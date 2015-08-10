using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace VCSJones.FiddlerCert
{
    public class DistinguishedNameParser
    {
        [method: DllImport("ntdsapi.dll", EntryPoint = "DsGetRdnW", ExactSpelling = true, CallingConvention = CallingConvention.Winapi, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Error)]
        private static extern int DsGetRdnW(
            [param: In, Out, MarshalAs(UnmanagedType.SysInt)] ref IntPtr ppDN,
            [param: In, Out, MarshalAs(UnmanagedType.U4)] ref uint pcDN,
            [param: Out, MarshalAs(UnmanagedType.SysInt)] out IntPtr ppKey,
            [param: Out, MarshalAs(UnmanagedType.U4)] out uint pcKey,
            [param: Out, MarshalAs(UnmanagedType.SysInt)] out IntPtr ppVal,
            [param: Out, MarshalAs(UnmanagedType.U4)] out uint pcVal
        );

        public static Dictionary<string, List<string>> Parse(string distingishedName)
        {
            var result = new Dictionary<string, List<string>>(StringComparer.CurrentCultureIgnoreCase);
            var distinguishedNamePtr = IntPtr.Zero;
            try
            {
                distinguishedNamePtr = Marshal.StringToCoTaskMemUni(distingishedName);
                //We need to copy the IntPtr.
                //The copy is necessary because DsGetRdnW modifies the pointer to advance it. We need to keep
                //The original so we can free it later, otherwise we'll leak memory.
                var distinguishedNamePtrCopy = distinguishedNamePtr;
                uint pcDN = (uint)distingishedName.Length, pcKey, pcVal;
                IntPtr ppKey, ppVal;
                while (DsGetRdnW(ref distinguishedNamePtrCopy, ref pcDN, out ppKey, out pcKey, out ppVal, out pcVal) == 0)
                {
                    if (pcDN == 0)
                    {
                        break;
                    }
                    if (pcKey == 0 || pcVal == 0)
                    {
                        continue;
                    }
                    var key = Marshal.PtrToStringUni(ppKey, (int)pcKey);
                    var value = Marshal.PtrToStringUni(ppVal, (int)pcVal);
                    if (result.ContainsKey(key))
                    {
                        result[key].Add(value);
                    }
                    else
                    {
                        result.Add(key, new List<string> { value });
                    }
                    if (pcDN == 0)
                    {
                        break;
                    }
                }
                return result;

            }
            finally
            {
                Marshal.FreeCoTaskMem(distinguishedNamePtr);
            }
        }
    }
}
