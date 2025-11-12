PaygirLettersApp - simple Windows Forms app (Persian RTL)

Build:
- Requires .NET 6 SDK and Visual Studio 2022
- Packages: ClosedXML, Microsoft.Toolkit.Uwp.Notifications
- Run `dotnet restore` then open in Visual Studio or run `.uild.ps1`

Behavior:
- Auto-loads Documents\PaygirLettersData.xlsx on startup (if exists)
- Auto-saves on exit to same path
- Attachments stored in Documents\PaygirLetters_Attachments\[LetterID]\
- Toast notifications on Windows 10/11; MessageBox fallback on older Windows

Notes:
- Persian date input expects yyyy/MM/dd (Persian calendar). Display uses PersianCalendar.
- Numbers in date/time are left-to-right (standard ASCII digits).
