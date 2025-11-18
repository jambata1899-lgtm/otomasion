using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace FinalDotnetCoreBuild.Helpers
{
    using FinalDotnetCoreBuild;

    public static class NotificationHelper
    {
        private static PersianCalendar _pc = new PersianCalendar();

        public static void Notify(string title, string message)
        {
            MessageBox.Show(message, title);
        }

        public static void CheckAndNotify(IEnumerable<Letter> letters)
        {
            var now = DateTime.Now;

            foreach (var l in letters)
            {
                if (l.Status == LetterStatus.پاسخ_داده_شده) continue;

                var daysLeft = (l.DueDate.Date - now.Date).TotalDays;

                if (daysLeft <= 3 && daysLeft >= 0)
                {
                    Notify("نامه در آستانه سررسید",
                        $"{l.Subject} — سررسید در {ToPersianDateString(l.DueDate)} ({(int)daysLeft} روز)");
                }
                else if (daysLeft < 0)
                {
                    l.Status = LetterStatus.پاسخ_داده_نشده;
                    Notify("نامه از سررسید گذشته",
                        $"{l.Subject} — سررسید {ToPersianDateString(l.DueDate)}");
                }
            }
        }

        private static string ToPersianDateString(DateTime dt)
        {
            var y = _pc.GetYear(dt);
            var m = _pc.GetMonth(dt).ToString("00");
            var d = _pc.GetDayOfMonth(dt).ToString("00");
            return $"{y}/{m}/{d}";
        }
    }
}
