using System;
using System.Collections.Generic;

namespace FinalDotnetCoreBuild
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
        public DateTime SentDate { get; set; }
        public int ResponseDays { get; set; }
        public DateTime DueDate { get; set; }
        public LetterStatus Status { get; set; }
        public string Notes { get; set; } = "";
        public List<string> Attachments { get; set; } = new List<string>();
    }
}
