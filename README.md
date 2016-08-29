# Fiddler Certificate Inspector
A Fiddler extension for examining certificates.

This is an extension for the excellent Fiddler tool, a web debugging tool developed by Eric Lawrence at Telerik.
This extension provides an Inspector for HTTPS traffic that allows you to view, export, and install certificates.

With this extension, you can quickly:

* View the certificate chain.
* View common properties of the certificate such as the Common Name (CN in subject) and the Subject Alternative Name.
* Save the certificate to disk.
* Import the certificate into a certificate store.
* View the SPKI Fingerprints for HPKP.

## Installing

Head on over to the [releases](https://github.com/vcsjones/FiddlerCert/releases/latest) section on GitHub and grab the latest
release. A pre-built, signed, installer is available for convenience.

## Building

### Installer

You'll need Visual Studio 2015 or later to open the solution, or the [Microsoft Build Tools 2015](https://www.microsoft.com/en-us/download/details.aspx?id=48159)
to run the build script. You'll also need NSIS installed to build the installer. You will also need the
[NSIS](http://nsis.sourceforge.net/) framework installed to the default location.

To build and create an installer, simply point MSBuild to the `build` directory of the repository. For example:

    msbuild C:\projects\FiddlerCert\build

This will compile the project and package it into an installer.

### Signing

If you wish to sign the installer, first you need to make sure you have a valid Authenticode signing certificate.

Then, use the `Sign` build target with MSBuild:

    msbuild /t:Sign C:\projects\FiddlerCert\build
    
This target will also compile the project. The sign process will automatically select a code signing certificate from
the certificate store. The signing process is done twice, once with SHA1 and another with SHA256. This may cause multiple
UI prompts if a UI prompt is required.


### Manual Installation

If you want to install the project yourself, take the `VCSJones.FiddlerCert.dll` assembly and place it in the `Inspectors` and
`Scripts` directory in `Documents\Fiddler2` (note this directory is used for both Fiddler2 and Fiddler4).

## Requirements

* Windows Vista / Windows Server 2008 or later is required.
* Fiddler4 is required.

## Affiliation

This is not affiliated with nor endorsed by Eric Lawrence or Telerik.
