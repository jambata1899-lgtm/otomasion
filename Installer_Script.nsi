
; NSIS installer script for PaygirLettersApp
; Generates Setup.exe that installs to C:\Program Files\PaygirLettersApp\
; Creates Desktop shortcut and Start Menu folder "سیستم پیگیر نامه‌ها"
; Uses default Windows icon

!include "MUI2.nsh"

Name "سیستم پیگیر نامه‌های اتوماسیونی"
OutFile "PaygirLettersApp_Setup.exe"
InstallDir "$PROGRAMFILES\PaygirLettersApp"
ShowInstDetails show

!define SHORTCUT_NAME "سیستم پیگیر نامه‌های اتوماسیونی"
!define LAUNCH_EXE "PaygirLettersApp.exe"

Section "Install"
  SetOutPath "$INSTDIR"
  ; Copy all files from package to install dir
  File /r "PaygirLettersApp\*.*"

  ; Create shortcuts
  CreateDirectory "$SMPROGRAMS\سیستم پیگیر نامه‌ها"
  CreateShortcut "$SMPROGRAMS\سیستم پیگیر نامه‌ها\${SHORTCUT_NAME}.lnk" "$INSTDIR\${LAUNCH_EXE}"
  CreateShortCut "$DESKTOP\${SHORTCUT_NAME}.lnk" "$INSTDIR\${LAUNCH_EXE}"

  ; Write uninstall info
  WriteUninstaller "$INSTDIR\Uninstall.exe"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\PaygirLettersApp" "DisplayName" "سیستم پیگیر نامه‌های اتوماسیونی"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\PaygirLettersApp" "UninstallString" "$INSTDIR\Uninstall.exe"
SectionEnd

Section "Uninstall"
  Delete "$SMPROGRAMS\سیستم پیگیر نامه‌ها\${SHORTCUT_NAME}.lnk"
  Delete "$DESKTOP\${SHORTCUT_NAME}.lnk"
  RMDir /r "$INSTDIR"
  DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\PaygirLettersApp"
SectionEnd
