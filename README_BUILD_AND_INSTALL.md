
راهنمای سریع برای ساخت و نصب (PowerShell روی ویندوز)
===============================================

پیش‌نیازها (روی سیستم ویندوزی که می‌خواهی بیلد کنی):
1. Visual Studio 2019/2022 یا حداقل MSBuild و .NET Framework 4.8 نصب باشد.
2. (اختیاری اما پیشنهاد شده) نصب NSIS برای تولید Setup.exe:
   https://nsis.sourceforge.io/Download

مراحل برای تولید EXE برنامه:
1. فایل ZIP را استخراج کن.
2. پوشه پروژه را با Visual Studio باز کن (فایل PaygirLettersApp.sln).
   - از منوی Tools > NuGet Package Manager > Package Manager Console:
     > Update-Package -reinstall
   - یا از پوشه پروژه: nuget restore PaygirLettersApp.sln

3. یا با PowerShell (در مسیر پوشه پروژه) بسته‌ها را ری‌استور کن و build کن:

   # اگر می‌خواهی Policy را فقط برای جلسه فعلی تغییر دهی:
   Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope Process

   # اجرا کردن اسکریپت بیلد (در صورت وجود build_publish.ps1)
   cd .\PaygirLettersApp\
   .\build_publish.ps1 -Configuration Release

   # در انتها فایل اجرایی در:
   .\PaygirLettersApp\bin\Release\PaygirLettersApp.exe

4. تولید Setup.exe با NSIS:
   - نصب NSIS
   - باز کردن "MakeNSISW" یا اجرای:
     makensis.exe Installer_Script.nsi

   خروجی: PaygirLettersApp_Setup.exe

نکات مهم:
- اگر خطایی دیدی که مربوط به ExecutionPolicy است، از دستور Set-ExecutionPolicy بالا استفاده کن یا اسکریپت را با راست‌کلیک > Run with PowerShell اجرا کن.
- اگر می‌خواهی من خودم اسکریپت دقیق‌تر یا یک فایل PowerShell برای اتوماتیک‌سازی کامل (ری‌استور، بیلد، اجرای makensis) بسازم، بگو تا اضافه کنم.
