; NSIS script skeleton - requires NSIS to build
OutFile "PaygirLettersApp-Setup.exe"
InstallDir "$PROGRAMFILES\PaygirLettersApp"
Page directory
Page instfiles
Section ""
  SetOutPath "$INSTDIR"
  File /r "bin\Release\*"
  CreateShortCut "$DESKTOP\PaygirLettersApp.lnk" "$INSTDIR\PaygirLettersApp.exe"
SectionEnd
