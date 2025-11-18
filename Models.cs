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
        public string StableKey { get; set; } = Guid.NewGuid().ToString("N");
        public string RowNumber { get; set; } = "";

        public string Subject { get; set; } = "";
        public string Recipient { get; set; } = "";
        public string LetterNumber { get; set; } = "";

        public DateTime SentDate { get; set; }
        public int ResponseDays { get; set; }
        public DateTime DueDate { get; set; }

        public LetterStatus Status { get; set; } = LetterStatus.در_حال_پیگیری;

        public string Notes { get; set; } = "";
        public List<string> Attachments { get; set; } = new List<string>();
    }
}
