using System;
using System.Runtime.InteropServices;

namespace VCSJones.FiddlerCert
{
    internal static class Cryptui
    {
        [return: MarshalAs(UnmanagedType.Bool)]
        [method: DllImport("Cryptui.dll", ExactSpelling = true, EntryPoint = "CryptUIDlgViewCertificateW", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool CryptUIDlgViewCertificate(ref CRYPTUI_VIEWCERTIFICATE_STRUCT pCertViewInfo, ref bool pfPropertiesChanged);

        [return: MarshalAs(UnmanagedType.Bool)]
        [method: DllImport("Cryptui.dll", ExactSpelling = true, EntryPoint = "CryptUIWizImport", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool CryptUIWizImport(ImportCertificateFlags dwFlags, IntPtr hwndParent, string pwszWizardTitle, ref CRYPTUI_WIZ_IMPORT_SRC_INFO pImportSrc, IntPtr hDestCertStore);
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct CRYPTUI_WIZ_IMPORT_SRC_INFO
    {
        public uint dwSize;
        public ImportSourceSubjectChoice dwSubjectChoice;
        public IntPtr pCertContext;
        public uint dwFlags;
        public string pwszPassword;
    }

    internal enum ImportSourceSubjectChoice : uint
    {
        CRYPTUI_WIZ_IMPORT_SUBJECT_FILE = 1,
        CRYPTUI_WIZ_IMPORT_SUBJECT_CERT_CONTEXT = 2,
        CRYPTUI_WIZ_IMPORT_SUBJECT_CTL_CONTEXT = 3,
        CRYPTUI_WIZ_IMPORT_SUBJECT_CRL_CONTEXT = 4,
        CRYPTUI_WIZ_IMPORT_SUBJECT_CERT_STORE = 5,
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct CRYPTUI_VIEWCERTIFICATE_STRUCT
    {
        public uint dwSize;
        public IntPtr hwndParent;
        public ViewCertificateFlags dwFlags;
        public string szTitle;
        public IntPtr pCertContext;
        public IntPtr rgszPurposes;
        public int cPurposes;
        public IntPtr pCryptProviderData;
        public Boolean fpCryptProviderDataTrustedUsage;
        public int idxSigner;
        public int idxCert;
        public Boolean fCounterSigner;
        public int idxCounterSigner;
        public int cStores;
        public IntPtr rghStores;
        public int cPropSheetPages;
        public IntPtr rgPropSheetPages;
        public int nStartPage;
    }

    [Flags]
    internal enum ViewCertificateFlags : uint
    {
        CRYPTUI_HIDE_HIERARCHYPAGE = 0x00000001,
        CRYPTUI_HIDE_DETAILPAGE = 0x00000002,
        CRYPTUI_DISABLE_EDITPROPERTIES = 0x00000004,
        CRYPTUI_ENABLE_EDITPROPERTIES = 0x00000008,
        CRYPTUI_DISABLE_ADDTOSTORE = 0x00000010,
        CRYPTUI_ENABLE_ADDTOSTORE = 0x00000020,
        CRYPTUI_ACCEPT_DECLINE_STYLE = 0x00000040,
        CRYPTUI_IGNORE_UNTRUSTED_ROOT = 0x00000080,
        CRYPTUI_DONT_OPEN_STORES = 0x00000100,
        CRYPTUI_ONLY_OPEN_ROOT_STORE = 0x00000200,
        CRYPTUI_WARN_UNTRUSTED_ROOT = 0x00000400,
        CRYPTUI_ENABLE_REVOCATION_CHECKING = 0x00000800,
        CRYPTUI_WARN_REMOTE_TRUST = 0x00001000,
        CRYPTUI_DISABLE_EXPORT = 0x00002000,
        CRYPTUI_ENABLE_REVOCATION_CHECK_END_CERT = 0x00004000,
        CRYPTUI_ENABLE_REVOCATION_CHECK_CHAIN = 0x00008000,
        CRYPTUI_ENABLE_REVOCATION_CHECK_CHAIN_EXCLUDE_ROOT = CRYPTUI_ENABLE_REVOCATION_CHECKING,
        CRYPTUI_DISABLE_HTMLLINK = 0x00010000,
        CRYPTUI_DISABLE_ISSUERSTATEMENT = 0x00020000,
        CRYPTUI_CACHE_ONLY_URL_RETRIEVAL = 0x00040000
    }

    [Flags]
    internal enum ImportCertificateFlags : uint
    {
        CRYPTUI_WIZ_NO_UI = 0x0001,
        CRYPTUI_WIZ_IGNORE_NO_UI_FLAG_FOR_CSPS = 0x0002,
        CRYPTUI_WIZ_NO_UI_EXCEPT_CSP = 0x0003,
        CRYPTUI_WIZ_IMPORT_ALLOW_CERT = 0x00020000,
        CRYPTUI_WIZ_IMPORT_ALLOW_CRL = 0x00040000,
        CRYPTUI_WIZ_IMPORT_ALLOW_CTL = 0x00080000,
        CRYPTUI_WIZ_IMPORT_NO_CHANGE_DEST_STORE = 0x00010000,
        CRYPTUI_WIZ_IMPORT_TO_LOCALMACHINE = 0x00100000,
        CRYPTUI_WIZ_IMPORT_TO_CURRENTUSER = 0x00200000,
        CRYPTUI_WIZ_IMPORT_REMOTE_DEST_STORE = 0x00400000
    }
}
