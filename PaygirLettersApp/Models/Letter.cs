using System;
using System.Collections.Generic;

namespace PaygirLettersApp.Models
{
    public class Letter
    {
        public int Id { get; set; }
        public string Subject { get; set; }
        public string Recipient { get; set; }
        public string Number { get; set; }
        public DateTime SentDate { get; set; }
        public int ResponseDays { get; set; }
        public DateTime DueDate { get; set; }
        public string Status { get; set; } // "در دست پیگیری", "پاسخ داده شده", "پاسخ داده نشده"
        public string Notes { get; set; }
        public List<string> Attachments { get; set; } = new List<string>();
    }
}
