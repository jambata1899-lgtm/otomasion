# PowerShell build script
dotnet publish -c Release -r win-x64 --self-contained false -o .\bin\Release
# Output exe will be in bin\Release\PaygirLettersApp.exe
