using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
// حذف using Microsoft.Toolkit.Uwp.Notifications; چون کلاس‌ها پیدا نمی‌شوند

namespace FinalDotnetCoreBuild.Helpers
{
    using FinalDotnetCoreBuild;
    public static class NotificationHelper
    {
        public static bool IsWindows10OrGreater()
        {
            // این تابع بررسی می کند که سیستم عامل ویندوز 10 یا بالاتر باشد
            return Environment.OSVersion.Version.Major >= 10;
        }

        public static void Notify(string title, string message)
        {
            // از آنجایی که خطاهای کامپایل مربوط به اعلان های UWP بود، 
            // برای تضمین بیلد موفق، به طور مستقیم به MessageBox باز می گردیم.
            // اگر بعداً نیاز به اعلان های مدرن بود، باید یک پکیج جدید نصب و استفاده شود.
            MessageBox.Show(message, title);
        }

        public static void CheckAndNotify(IEnumerable<Letter> letters)
        {
            var now = DateTime.Now;
            foreach(var l in letters)
            {
                if (l.Status == LetterStatus.Answered) continue;
                var daysLeft = (l.DueDate.Date - now.Date).TotalDays;
                if (daysLeft <= 3 && daysLeft >= 0)
                {
                    Notify("نامه در آستانه سررسید", $"{l.Subject} — سررسید در {l.DueDate.ToShortDateString()} ({(int)daysLeft} روز)");
                }
                else if (daysLeft < 0)
                {
                    Notify("نامه از سررسید گذشته", $"{l.Subject} — سررسید {l.DueDate.ToShortDateString()}");
                }
            }
        }
    }
}
