Name "FiddlerCertInspector"

!define /date YEAR "%Y"

SetCompressionLevel 9
RequestExecutionLevel "user"

BrandingText "v${VERSION}"
VIProductVersion "${VERSION}"
VIAddVersionKey "ProductVersion" "${VERSION}"
VIAddVersionKey "FileVersion" "${VERSION}"
VIAddVersionKey "ProductName" "${ProductName}"
VIAddVersionKey "Comments" "${ProductUrl}"
VIAddVersionKey "LegalCopyright" "©${YEAR} ${ProductCopy}"
VIAddVersionKey "CompanyName" "${CompanyName}"
VIAddVersionKey "FileDescription" "${ProductDescription}"

InstallDir "$DOCUMENTS\Fiddler2"

Section "Main"
SetOverwrite on
SetOutPath "$INSTDIR\Inspectors"
File "..\out\VCSJones.FiddlerCert.dll"
File "..\out\VCSJones.FiddlerCert.pdb"
SetOutPath "$INSTDIR\Scripts"
File "..\out\VCSJones.FiddlerCert.dll"
File "..\out\VCSJones.FiddlerCert.pdb"

MessageBox MB_OK "YAY! Go inspect some certificates!"

SectionEnd