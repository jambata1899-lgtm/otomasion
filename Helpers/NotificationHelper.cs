using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace FinalDotnetCoreBuild.Helpers
{
    using FinalDotnetCoreBuild;
    public static class NotificationHelper
    {
        public static bool IsWindows10OrGreater()
        {
            return Environment.OSVersion.Version.Major >= 10;
        }

        public static void Notify(string title, string message)
        {
            if (IsWindows10OrGreater())
            {
                try
                {
                    new Microsoft.Toolkit.Uwp.Notifications.ToastContentBuilder()
                        .AddText(title)
                        .AddText(message)
                        .Show();
                }
                catch
                {
                    MessageBox.Show(message, title);
                }
            }
            else
            {
                MessageBox.Show(message, title);
            }
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
