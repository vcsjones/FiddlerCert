# FiddlerCert
A Fiddler extension for examining certificates.

This is an extension for the excellent Fiddler tool, a web debugging tool developed by Eric Lawrence at Telerik.
This extension provides an Inspector for HTTPS traffic that allows you to view, export, and install certificates.

![Fiddler Cert](https://vcsjones.com/wp-content/uploads/fiddlercert.png)

With this extension, you can quickly:

* View the certificate chain.
* View common properties of the certificate such as the Common Name (CN in subject) and the Subject Alternative Name.
* Save the certificate to disk.
* Import the certificate into a certificate store.

##Building

You'll need Visual Studio 2015 to build it, and a unit test runner for NUnit if you want to run the unit tests. Otherwise,
it's as simple as opening the solution and building. The project is also set up so all you have to do is start debugging (F5)
and it will automatically run and attach to Fiddler.

##Installing

The Visual Studio project is set up to install the extension in the %USERPROFILE% location for Fiddler when built. See the
[documentation](http://docs.telerik.com/fiddler/extend-fiddler/extendwithdotnet#direct-fiddler-to-load-extension-assemblies) of
Fiddler for more information.

##Requirements

Windows Vista / Windows Server 2008 or later is required.

##Affiliation

This is not affiliated with nor endorsed by Eric Lawrence or Telerik.
