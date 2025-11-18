using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using FinalDotnetCoreBuild.Helpers;

namespace FinalDotnetCoreBuild
{
    public partial class MainForm : Form
    {
        // فیلدها
        private readonly List<Letter> _letters = new List<Letter>();
        private readonly System.Windows.Forms.Timer _clockTimer = new System.Windows.Forms.Timer();
        private readonly System.Windows.Forms.Timer _notifyTimer = new System.Windows.Forms.Timer();
        private readonly PersianCalendar _pc = new PersianCalendar();

        // سازنده
        public MainForm()
        {
            InitializeComponent();
            Load += MainForm_Load;
            FormClosing += MainForm_FormClosing;
        }

        // رویدادهای فرم
        private void MainForm_Load(object? sender, EventArgs e)
        {
            _letters = ExcelHelper.Load();
            RefreshGrid();

            _clockTimer.Interval = 1000;
            _clockTimer.Tick += (s, ev) => UpdateClock();
            _clockTimer.Start();
            UpdateClock();

            _notifyTimer.Interval = 60 * 60 * 1000; // هر یک ساعت
            _notifyTimer.Tick += (s, ev) =>
            {
                bool changed = NotificationHelper.CheckAndNotify(_letters);
                if (changed) RefreshGrid();
            };
            _notifyTimer.Start();

            if (NotificationHelper.CheckAndNotify(_letters))
                RefreshGrid();
        }

        private void MainForm_FormClosing(object? sender, FormClosingEventArgs e)
        {
            ExcelHelper.Save(_letters);
        }

        // ساعت بالای فرم
        private void UpdateClock()
        {
            var now = DateTime.Now;
            var y = _pc.GetYear(now);
            var m = _pc.GetMonth(now).ToString("00");
            var d = _pc.GetDayOfMonth(now).ToString("00");
            var time = now.ToString("HH:mm:ss");
            lblDateTime.Text = $"{y}/{m}/{d} {time}";
        }

        // رویدادهای دکمه‌ها
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
                    FileAttachmentHelper.CopyAttachments(l.StableKey, l.Attachments);
                    l.Attachments = FileAttachmentHelper.GetSavedAttachments(l.StableKey);
                }
                RefreshGrid();
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dgvLetters.SelectedRows.Count == 0) return;
            var rowNumberText = dgvLetters.SelectedRows[0].Cells[0].Value?.ToString();
            if (!int.TryParse(rowNumberText, out var rowNumber)) return;

            var letter = _letters.FirstOrDefault(x => x.RowNumber == rowNumber);
            if (letter == null) return;

            var form = new LetterEditForm(letter, _letters.Select(x => x.Recipient).Distinct().ToList());
            if (form.ShowDialog() == DialogResult.OK)
            {
                var l = form.Letter;
                l.DueDate = l.SentDate.AddDays(l.ResponseDays);

                if (l.Attachments.Any())
                {
                    FileAttachmentHelper.CopyAttachments(l.StableKey, l.Attachments);
                    l.Attachments = FileAttachmentHelper.GetSavedAttachments(l.StableKey);
                }
                RefreshGrid();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvLetters.SelectedRows.Count == 0) return;
            var rowNumberText = dgvLetters.SelectedRows[0].Cells[0].Value?.ToString();
            if (!int.TryParse(rowNumberText, out var rowNumber)) return;

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
            if (!string.IsNullOrWhiteSpace(statusFilter) && Enum.TryParse<LetterStatus>(statusFilter, out var st))
                filtered = filtered.Where(x => x.Status == st);

            dgvLetters.Rows.Clear();
            foreach (var l in filtered)
            {
                var rowIdx = dgvLetters.Rows.Add(
                    l.RowNumber.ToString(),               // نمایش به صورت رشته
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

        // متدهای کمکی
        private void RefreshGrid()
        {
            dgvLetters.Rows.Clear();
            foreach (var l in _letters)
            {
                var rowIdx = dgvLetters.Rows.Add(
                    l.RowNumber.ToString(),               // نمایش به صورت رشته
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

        private string ToPersianDateString(DateTime dt)
        {
            return $"{_pc.GetYear(dt)}/{_pc.GetMonth(dt):00}/{_pc.GetDayOfMonth(dt):00}";
        }

        private void ApplyRowColor(DataGridViewRow row, Letter l)
        {
            if (l.Status == LetterStatus.پاسخ_داده_شده)
                row.DefaultCellStyle.BackColor = System.Drawing.Color.LightGreen;
            else if (l.Status == LetterStatus.پاسخ_داده_نشده)
                row.DefaultCellStyle.BackColor = System.Drawing.Color.LightCoral;
            else if (l.Status == LetterStatus.در_حال_پیگیری)
                row.DefaultCellStyle.BackColor = System.Drawing.Color.LightYellow;
        }
    }
}
