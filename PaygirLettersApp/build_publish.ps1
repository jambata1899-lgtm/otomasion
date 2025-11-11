Param(
    [string]$Configuration = "Release"
)

Write-Host "Restoring NuGet packages..."
if (Test-Path "nuget.exe") {
    .\nuget.exe restore PaygirLettersApp.sln
} else {
    Write-Host "nuget.exe not found — Visual Studio will usually restore packages automatically."
}

Write-Host "Building solution..."
msbuild PaygirLettersApp.sln /p:Configuration=$Configuration

$exe = Join-Path -Path "$(Resolve-Path .)" -ChildPath "PaygirLettersApp\bin\$Configuration\PaygirLettersApp.exe"
if (Test-Path $exe) {
    Write-Host "Build succeeded. EXE at: $exe"
    $zip = "PaygirLettersApp_$Configuration.zip"
    if (Test-Path $zip) { Remove-Item $zip }
    Write-Host "Compressing output to $zip..."
    Compress-Archive -Path "PaygirLettersApp\bin\$Configuration\*" -DestinationPath $zip
    Write-Host "Done."
} else {
    Write-Host "Build failed or EXE not found. Check build output."
}
