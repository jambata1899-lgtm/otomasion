using System;
using System.Collections.Generic;
using System.IO;
using ClosedXML.Excel;

namespace FinalDotnetCoreBuild.Helpers
{
    using static System.Environment;
    using FinalDotnetCoreBuild;
    public static class ExcelHelper
    {
        public static string GetDataPath()
        {
            return Path.Combine(GetFolderPath(SpecialFolder.MyDocuments), "PaygirLettersData.xlsx");
        }

        public static void Save(List<Letter> letters)
        {
            var path = GetDataPath();
            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Letters");
            ws.Cell(1,1).Value = "Id";
            ws.Cell(1,2).Value = "RowNumber";
            ws.Cell(1,3).Value = "Subject";
            ws.Cell(1,4).Value = "Recipient";
            ws.Cell(1,5).Value = "LetterNumber";
            ws.Cell(1,6).Value = "SentDate";
            ws.Cell(1,7).Value = "ResponseDays";
            ws.Cell(1,8).Value = "DueDate";
            ws.Cell(1,9).Value = "Status";
            ws.Cell(1,10).Value = "Notes";
            ws.Cell(1,11).Value = "Attachments";

            int r = 2;
            foreach(var l in letters)
            {
                ws.Cell(r,1).Value = l.Id;
                ws.Cell(r,2).Value = l.RowNumber;
                ws.Cell(r,3).Value = l.Subject;
                ws.Cell(r,4).Value = l.Recipient;
                ws.Cell(r,5).Value = l.LetterNumber;
                ws.Cell(r,6).Value = l.SentDate.ToString("o");
                ws.Cell(r,7).Value = l.ResponseDays;
                ws.Cell(r,8).Value = l.DueDate.ToString("o");
                ws.Cell(r,9).Value = l.Status.ToString();
                ws.Cell(r,10).Value = l.Notes;
                ws.Cell(r,11).Value = string.Join(";", l.Attachments ?? new List<string>());
                r++;
            }
            wb.SaveAs(path);
        }

        public static List<Letter> Load()
        {
            var path = GetDataPath();
            var outList = new List<Letter>();
            if (!File.Exists(path)) return outList;
            using var wb = new XLWorkbook(path);
            var ws = wb.Worksheet("Letters");
            if (ws == null) return outList;
            var row = 2;
            while(true)
            {
                var idCell = ws.Cell(row,1);
                if (idCell.IsEmpty()) break;
                try
                {
                    var l = new Letter();
                    l.Id = (int)ws.Cell(row,1).GetValue<int>();
                    l.RowNumber = ws.Cell(row,2).GetString();
                    l.Subject = ws.Cell(row,3).GetString();
                    l.Recipient = ws.Cell(row,4).GetString();
                    l.LetterNumber = ws.Cell(row,5).GetString();
                    l.SentDate = DateTime.Parse(ws.Cell(row,6).GetString());
                    l.ResponseDays = ws.Cell(row,7).GetValue<int>();
                    l.DueDate = DateTime.Parse(ws.Cell(row,8).GetString());
                    l.Status = Enum.TryParse<LetterStatus>(ws.Cell(row,9).GetString(), out var st) ? st : LetterStatus.InProgress;
                    l.Notes = ws.Cell(row,10).GetString();
                    var attach = ws.Cell(row,11).GetString();
                    l.Attachments = string.IsNullOrWhiteSpace(attach) ? new List<string>() : new List<string>(attach.Split(';'));
                    outList.Add(l);
                }
                catch { }
                row++;
            }
            return outList;
        }
    }
}
