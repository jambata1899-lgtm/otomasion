using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ClosedXML.Excel;
using PaygirLettersApp.Models;

namespace PaygirLettersApp
{
    public static class ExcelService
    {
        public static void SaveToExcel(IEnumerable<Letter> letters, string path)
        {
            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Letters");
            ws.Cell(1,1).Value = "Id";
            ws.Cell(1,2).Value = "Subject";
            ws.Cell(1,3).Value = "Recipient";
            ws.Cell(1,4).Value = "Number";
            ws.Cell(1,5).Value = "SentDate";
            ws.Cell(1,6).Value = "ResponseDays";
            ws.Cell(1,7).Value = "DueDate";
            ws.Cell(1,8).Value = "Status";
            ws.Cell(1,9).Value = "Notes";
            int r = 2;
            foreach(var l in letters)
            {
                ws.Cell(r,1).Value = l.Id;
                ws.Cell(r,2).Value = l.Subject;
                ws.Cell(r,3).Value = l.Recipient;
                ws.Cell(r,4).Value = l.Number;
                ws.Cell(r,5).Value = l.SentDate;
                ws.Cell(r,6).Value = l.ResponseDays;
                ws.Cell(r,7).Value = l.DueDate;
                ws.Cell(r,8).Value = l.Status;
                ws.Cell(r,9).Value = l.Notes;
                r++;
            }
            wb.SaveAs(path);
        }

        public static IEnumerable<Letter> LoadFromExcel(string path)
        {
            var list = new List<Letter>();
            using var wb = new XLWorkbook(path);
            var ws = wb.Worksheet(1);
            var rows = ws.RangeUsed().RowsUsed().Skip(1);
            foreach(var row in rows)
            {
                var l = new Letter
                {
                    Id = row.Cell(1).GetValue<int>(),
                    Subject = row.Cell(2).GetString(),
                    Recipient = row.Cell(3).GetString(),
                    Number = row.Cell(4).GetString(),
                    SentDate = row.Cell(5).GetDateTime(),
                    ResponseDays = row.Cell(6).GetValue<int>(),
                    DueDate = row.Cell(7).GetDateTime(),
                    Status = row.Cell(8).GetString(),
                    Notes = row.Cell(9).GetString()
                };
                list.Add(l);
            }
            return list;
        }
    }
}
