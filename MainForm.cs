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
        // داده‌ها و تایمرها
        private readonly List<Letter> _letters = new List<Letter>();
        private readonly System.Windows.Forms.Timer _clockTimer = new System.Windows.Forms.Timer();
        private readonly System.Windows.Forms.Timer _notifyTimer = new System.Windows.Forms.Timer();
        private readonly PersianCalendar _pc = new PersianCalendar();

        public MainForm()
        {
            InitializeComponent();
            Load += MainForm_Load;
            FormClosing += MainForm_FormClosing;
        }

        // رویدادهای فرم
        private void MainForm_Load(object? sender, EventArgs e)
        {
            _letters.Clear();
            _letters.AddRange(ExcelHelper.Load());
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

        private void UpdateClock()
        {
            var now = DateTime.Now;
            var y = _pc.GetYear(now);
            var m = _pc.GetMonth(now).ToString("00");
            var d = _pc.GetDayOfMonth(now).ToString("00");
            var time = now.ToString("HH:mm:ss");
            lblDateTime.Text = $"{y}/{m}/{d} {time}";
        }

        // دکمه‌ها
        private void btnAdd_Click(object sender, EventArgs e)
        {
            var recipients = _letters.Select(x => x.Recipient).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().ToList();
            var form = new LetterEditForm(null, recipients);
            if (form.ShowDialog() != DialogResult.OK) return;

            var l = form.Letter;
            if (l == null) return;

            l.DueDate = l.SentDate.AddDays(l.ResponseDays);
            _letters.Add(l);

            if (l.Attachments != null && l.Attachments.Any())
            {
                FileAttachmentHelper.CopyAttachments(l.StableKey, l.Attachments);
                l.Attachments = FileAttachmentHelper.GetSavedAttachments(l.StableKey);
            }

            RefreshGrid();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dgvLetters.SelectedRows == null || dgvLetters.SelectedRows.Count == 0) return;

            var row = dgvLetters.SelectedRows[0];
            var letter = row.Tag as Letter;
            if (letter == null) return;

            var recipients = _letters.Select(x => x.Recipient).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().ToList();
            var form = new LetterEditForm(letter, recipients);
            if (form.ShowDialog() != DialogResult.OK) return;

            var l = form.Letter;
            if (l == null) return;

            l.DueDate = l.SentDate.AddDays(l.ResponseDays);

            if (l.Attachments != null && l.Attachments.Any())
            {
                FileAttachmentHelper.CopyAttachments(l.StableKey, l.Attachments);
                l.Attachments = FileAttachmentHelper.GetSavedAttachments(l.StableKey);
            }

            RefreshGrid();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvLetters.SelectedRows == null || dgvLetters.SelectedRows.Count == 0) return;

            var row = dgvLetters.SelectedRows[0];
            var letter = row.Tag as Letter;
            if (letter == null) return;

            if (MessageBox.Show("آیا مطمئن هستید؟", "حذف", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            _letters.Remove(letter);
            RefreshGrid();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            var kw = (txtSearch?.Text ?? string.Empty).Trim();
            var recipient = (txtSearchRecipient?.Text ?? string.Empty).Trim();
            var statusFilter = cmbFilterStatus?.Text ?? string.Empty;

            var filtered = _letters.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(kw))
                filtered = filtered.Where(x => (x.Subject ?? string.Empty).Contains(kw, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(recipient))
                filtered = filtered.Where(x => (x.Recipient ?? string.Empty).Contains(recipient, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(statusFilter) && Enum.TryParse<LetterStatus>(statusFilter, out var st))
                filtered = filtered.Where(x => x.Status == st);

            PopulateGrid(filtered);
        }

        private void btnSaveExcel_Click(object sender, EventArgs e)
        {
            ExcelHelper.Save(_letters);
            MessageBox.Show("ذخیره انجام شد", "پیام", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnLoadExcel_Click(object sender, EventArgs e)
        {
            _letters.Clear();
            _letters.AddRange(ExcelHelper.Load());
            RefreshGrid();
            MessageBox.Show("بارگذاری انجام شد", "پیام", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // متدهای کمکی
        private void RefreshGrid()
        {
            PopulateGrid(_letters);
        }

        private void PopulateGrid(IEnumerable<Letter> source)
        {
            dgvLetters.Rows.Clear();

            if (source == null) return;

            foreach (var l in source)
            {
                int rowIdx = dgvLetters.Rows.Add(); // اضافه‌کردن ردیف بدون آرگومان
                var row = dgvLetters.Rows[rowIdx];

                // شماره ردیف نمایشی
                SetCellSafe(row, 0, (rowIdx + 1).ToString());

                // مقداردهی سلول‌ها مطابق ترتیب ستون‌ها
                SetCellSafe(row, 1, l?.Subject ?? "");
                SetCellSafe(row, 2, l?.Recipient ?? "");
                SetCellSafe(row, 3, l?.LetterNumber ?? "");
                SetCellSafe(row, 4, ToPersianDateString(l?.SentDate ?? DateTime.MinValue));
                SetCellSafe(row, 5, l?.ResponseDays ?? 0);
                SetCellSafe(row, 6, ToPersianDateString(l?.DueDate == default ? DateTime.MinValue : l.DueDate));
                SetCellSafe(row, 7, l?.Status.ToString() ?? "");
                SetCellSafe(row, 8, l?.Notes ?? "");
                SetCellSafe(row, 9, string.Join(";", l?.Attachments ?? new List<string>()));

                // نگه‌داشتن مرجع آیتم برای ویرایش/حذف
                row.Tag = l;

                // رنگ‌دهی
                if (l != null)
                    ApplyRowColor(row, l);
            }
        }

        private static void SetCellSafe(DataGridViewRow row, int index, object? value)
        {
            var grid = row?.DataGridView;
            if (grid == null) return;
            if (index < 0 || index >= grid.Columns.Count) return;

            row.Cells[index].Value = value ?? "";
        }

        private string ToPersianDateString(DateTime dt)
        {
            if (dt == DateTime.MinValue) return "";
            return $"{_pc.GetYear(dt)}/{_pc.GetMonth(dt):00}/{_pc.GetDayOfMonth(dt):00}";
        }

        private void ApplyRowColor(DataGridViewRow row, Letter l)
        {
            if (row == null) return;

            if (l.Status == LetterStatus.پاسخ_داده_شده)
                row.DefaultCellStyle.BackColor = System.Drawing.Color.LightGreen;
            else if (l.Status == LetterStatus.پاسخ_داده_نشده)
                row.DefaultCellStyle.BackColor = System.Drawing.Color.LightCoral;
            else if (l.Status == LetterStatus.در_حال_پیگیری)
                row.DefaultCellStyle.BackColor = System.Drawing.Color.LightYellow;
            else
                row.DefaultCellStyle.BackColor = System.Drawing.Color.White;
        }
    }
}
