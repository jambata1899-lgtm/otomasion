FinalDotnetCoreBuild - Windows Forms app (Persian RTL)

Build:
- Requires .NET 8 SDK and Visual Studio 2022/2023
- Packages: ClosedXML, Microsoft.Toolkit.Uwp.Notifications
- Run `dotnet restore` then open FinalDotnetCoreBuild.sln in Visual Studio or run `.uild.ps1`

Notes:
- Auto-loads Documents\PaygirLettersData.xlsx on startup (if exists)
- Auto-saves on exit
- Attachments stored in Documents\PaygirLetters_Attachments\[LetterID]\
- Toast notifications on Windows 10/11; MessageBox fallback on older Windows
