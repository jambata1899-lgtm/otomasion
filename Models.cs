using System;
using System.Collections.Generic;

namespace FinalDotnetCoreBuild
{
    public enum LetterStatus
    {
        پاسخ_داده_شده,
        پاسخ_داده_نشده,
        در_حال_پیگیری
    }

    public class Letter
    {
        // ردیف خودکار (به جای Id)
        public string RowNumber { get; set; } = "";

        public string Subject { get; set; } = "";
        public string Recipient { get; set; } = "";
        public string LetterNumber { get; set; } = "";

        // تاریخ‌ها به صورت میلادی ذخیره می‌شوند
        public DateTime SentDate { get; set; }
        public int ResponseDays { get; set; }
        public DateTime DueDate { get; set; }

        // وضعیت فارسی
        public LetterStatus Status { get; set; }

        public string Notes { get; set; } = "";
        public List<string> Attachments { get; set; } = new List<string>();
    }
}
