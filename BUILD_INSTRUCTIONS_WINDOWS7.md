# BUILD INSTRUCTIONS (Windows 7 - 11)

این پروژه یک اسکِلفولد ساده Windows Forms برای .NET Framework 4.8 است.

پیش‌نیازها:
- Visual Studio 2019/2022 با .NET desktop development workloads
- (اختیاری) NuGet (برای بازیابی بسته‌ها) یا از Visual Studio استفاده کنید

مراحل ساخت:
1. پوشه‌ی پروژه را با Visual Studio باز کنید (`PaygirLettersApp.sln`).
2. اگر بسته‌ها را نپیدا کردید، از منوی Tools > NuGet Package Manager > Restore استفاده کنید.
3. Build > Build Solution را اجرا کنید (یا `msbuild PaygirLettersApp.sln /p:Configuration=Release` در PowerShell).
4. فایل اجرایی در `PaygirLettersApp\bin\Release\PaygirLettersApp.exe` قرار خواهد گرفت.

نکته‌ها:
- پروژه به ClosedXML و DocumentFormat.OpenXml وابسته است؛ NuGet آن‌ها را نصب خواهد کرد.
- اگر روی ویندوز 7 اجرا می‌کنید، نصب .NET Framework 4.8 ضروری است.

فایل `build_publish.ps1` برای خودکارسازی در همین پوشه قرار دارد.
