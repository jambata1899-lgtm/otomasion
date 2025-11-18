using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using FinalDotnetCoreBuild.Helpers;

namespace FinalDotnetCoreBuild
{
    public partial class MainForm : Form
    {
        private List<Letter> _letters = new List<Letter>();
        private Timer _clockTimer = new Timer();
        private Timer _notifyTimer = new Timer();
        private PersianCalendar _pc = new PersianCalendar();

        public MainForm()
        {
            InitializeComponent();
            Load += MainForm_Load;
            FormClosing += MainForm_FormClosing;
        }

        private void MainForm_Load(object? sender, EventArgs e)
        {
            _letters = ExcelHelper.Load();
            RefreshGrid();

            _clockTimer.Interval = 1000;
            _clockTimer.Tick += (s, ev) => UpdateClock();
            _clockTimer.Start();
            UpdateClock();

            _notifyTimer.Interval = 60 * 60 * 1000; // هر یک ساعت
            _notifyTimer.Tick += (s, ev) => NotificationHelper.CheckAndNotify(_letters);
            _notifyTimer.Start();

            NotificationHelper.CheckAndNotify(_letters);
        }

        private void MainForm_FormClosing(object? sender, FormClosingEventArgs e)
        {
            ExcelHelper.Save(_letters);
        }

        private void UpdateClock()
        {
            var now = DateTime.Now;
            var y = _pc.GetYear(now);
            var m = _pc.GetMonth(now).ToString("00");
            var d = _pc.GetDayOfMonth(now).ToString("00");
            var time = now.ToString("HH:mm:ss");
            lblDateTime.Text = $"{y}/{m}/{d} {time}";
        }

        private void RefreshGrid()
        {
            dgvLetters.Rows.Clear();
            int rowCounter = 1;
            foreach (var l in _letters)
            {
                l.RowNumber = rowCounter.ToString();
                var rowIdx = dgvLetters.Rows.Add(
                    l.RowNumber,
                    l.Subject,
                    l.Recipient,
                    l.LetterNumber,
                    ToPersianDateString(l.SentDate),
                    l.ResponseDays,
                    ToPersianDateString(l.DueDate),
                    l.Status.ToString(),
                    l.Notes,
                    string.Join(";", l.Attachments ?? new List<string>())
                );
                var row = dgvLetters.Rows[rowIdx];
                ApplyRowColor(row, l);
                rowCounter++;
            }
        }

        private void ApplyRowColor(DataGridViewRow row, Letter l)
        {
            if (l.Status == LetterStatus.پاسخ_داده_شده)
            {
                row.DefaultCellStyle.BackColor = System.Drawing.Color.LightGreen;
            }
            else if (l.Status == LetterStatus.پاسخ_داده_نشده)
            {
                row.DefaultCellStyle.BackColor = System.Drawing.Color.LightCoral;
            }
            else if (l.Status == LetterStatus.در_حال_پیگیری)
            {
                row.DefaultCellStyle.BackColor = System.Drawing.Color.LightYellow;
            }
        }

        private string ToPersianDateString(DateTime dt)
        {
            var y = _pc.GetYear(dt);
            var m = _pc.GetMonth(dt).ToString("00");
            var d = _pc.GetDayOfMonth(dt).ToString("00");
            return $"{y}/{m}/{d}";
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            var form = new LetterEditForm(null, _letters.Select(x => x.Recipient).Distinct().ToList());
            if (form.ShowDialog() == DialogResult.OK)
            {
                var l = form.Letter;
                l.DueDate = l.SentDate.AddDays(l.ResponseDays);
                _letters.Add(l);

                if (l.Attachments.Any())
                {
                    FileAttachmentHelper.CopyAttachments(l.RowNumber, l.Attachments);
                    l.Attachments = FileAttachmentHelper.GetSavedAttachments(l.RowNumber);
                }
                RefreshGrid();
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dgvLetters.SelectedRows.Count == 0) return;
            var rowNumber = dgvLetters.SelectedRows[0].Cells[0].Value.ToString();
            var letter = _letters.FirstOrDefault(x => x.RowNumber == rowNumber);
            if (letter == null) return;

            var form = new LetterEditForm(letter, _letters.Select(x => x.Recipient).Distinct().ToList());
            if (form.ShowDialog() == DialogResult.OK)
            {
                var l = form.Letter;
                l.DueDate = l.SentDate.AddDays(l.ResponseDays);
                RefreshGrid();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvLetters.SelectedRows.Count == 0) return;
            var rowNumber = dgvLetters.SelectedRows[0].Cells[0].Value.ToString();
            var letter = _letters.FirstOrDefault(x => x.RowNumber == rowNumber);
            if (letter == null) return;

            if (MessageBox.Show("آیا مطمئن هستید؟", "حذف", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                _letters.Remove(letter);
                RefreshGrid();
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            var kw = txtSearch.Text.Trim();
            var recipient = txtSearchRecipient.Text.Trim();
            var statusFilter = cmbFilterStatus.Text;
            var filtered = _letters.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(kw)) filtered = filtered.Where(x => x.Subject.Contains(kw));
            if (!string.IsNullOrWhiteSpace(recipient)) filtered = filtered.Where(x => x.Recipient.Contains(recipient));
            if (!string.IsNullOrWhiteSpace(statusFilter))
            {
                if (Enum.TryParse<LetterStatus>(statusFilter, out var st))
                    filtered = filtered.Where(x => x.Status == st);
            }

            dgvLetters.Rows.Clear();
            foreach (var l in filtered)
            {
                var rowIdx = dgvLetters.Rows.Add(
                    l.RowNumber,
                    l.Subject,
                    l.Recipient,
                    l.LetterNumber,
                    ToPersianDateString(l.SentDate),
                    l.ResponseDays,
                    ToPersianDateString(l.DueDate),
                    l.Status.ToString(),
                    l.Notes,
                    string.Join(";", l.Attachments ?? new List<string>())
                );
                var row = dgvLetters.Rows[rowIdx];
                ApplyRowColor(row, l);
            }
        }

        private void btnSaveExcel_Click(object sender, EventArgs e)
        {
            ExcelHelper.Save(_letters);
            MessageBox.Show("ذخیره انجام شد", "پیام");
        }

        private void btnLoadExcel_Click(object sender, EventArgs e)
        {
            _letters = ExcelHelper.Load();
            RefreshGrid();
            MessageBox.Show("بارگذاری انجام شد", "پیام");
        }
    }
}
