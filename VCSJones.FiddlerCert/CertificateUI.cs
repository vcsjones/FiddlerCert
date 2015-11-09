using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Forms;

namespace VCSJones.FiddlerCert
{
    public static class CertificateUI
    {
        public static void ShowImportCertificate(X509Certificate2 certificate, IWin32Window parent = null)
        {
            var cryptuiWizImportSrcInfo = new CRYPTUI_WIZ_IMPORT_SRC_INFO();
            cryptuiWizImportSrcInfo.dwSize = (uint)Marshal.SizeOf(typeof(CRYPTUI_WIZ_IMPORT_SRC_INFO));
            cryptuiWizImportSrcInfo.dwSubjectChoice = ImportSourceSubjectChoice.CRYPTUI_WIZ_IMPORT_SUBJECT_CERT_CONTEXT;
            cryptuiWizImportSrcInfo.pCertContext = certificate.Handle;
            cryptuiWizImportSrcInfo.pwszPassword = "";
            cryptuiWizImportSrcInfo.dwFlags = 0u;
            if (!Cryptui.CryptUIWizImport(ImportCertificateFlags.CRYPTUI_WIZ_IMPORT_ALLOW_CERT, parent?.Handle ?? IntPtr.Zero, "Fiddler: Import Certificate", ref cryptuiWizImportSrcInfo, IntPtr.Zero) && Marshal.GetLastWin32Error() != WinErr.ERROR_CANCELLED)
            {
                MessageBox.Show("An error occurred installing the certificate.");
            }
        }

        public static void ShowCertificate(X509Certificate2 certificate, IWin32Window parent = null)
        {
            var structConfiguration = new CRYPTUI_VIEWCERTIFICATE_STRUCT();
            structConfiguration.dwSize = (uint)Marshal.SizeOf(typeof(CRYPTUI_VIEWCERTIFICATE_STRUCT));
            structConfiguration.pCertContext = certificate.Handle;
            structConfiguration.szTitle = "Fiddler: Certificate Information";
            structConfiguration.dwFlags = ViewCertificateFlags.CRYPTUI_DISABLE_EDITPROPERTIES;
            structConfiguration.nStartPage = 0;
            structConfiguration.hwndParent = parent?.Handle ?? IntPtr.Zero;
            var propertiesChanged = false;
            if (!Cryptui.CryptUIDlgViewCertificate(ref structConfiguration, ref propertiesChanged) && Marshal.GetLastWin32Error() != WinErr.ERROR_CANCELLED)
            {
                MessageBox.Show("An error occurred viewing the certificate.");
            }
        }
    }
}