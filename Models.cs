using System;
using System.Collections.Generic;

namespace PaygirLettersApp
{
    public enum LetterStatus
    {
        InProgress,
        Answered,
        NotAnswered
    }

    public class Letter
    {
        public int Id { get; set; }
        public string RowNumber { get; set; } = "";
        public string Subject { get; set; } = "";
        public string Recipient { get; set; } = "";
        public string LetterNumber { get; set; } = "";
        // Stored as Gregorian DateTime but displayed as Persian date
        public DateTime SentDate { get; set; }
        public int ResponseDays { get; set; }
        public DateTime DueDate { get; set; }
        public LetterStatus Status { get; set; }
        public string Notes { get; set; } = "";
        public List<string> Attachments { get; set; } = new List<string>();
    }
}
