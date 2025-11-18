using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using FinalDotnetCoreBuild.Helpers;

namespace FinalDotnetCoreBuild
{
    public partial class MainForm : Form
    {
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

        private void MainForm_Load(object? sender, EventArgs e)
        {
            _letters.Clear();
            _letters.AddRange(ExcelHelper.Load());
            RefreshGrid();

            _clockTimer.Interval = 1000;
            _clockTimer.Tick += (s, ev) => UpdateClock();
            _clockTimer.Start();
            UpdateClock();

            _notifyTimer.Interval = 60 * 60 * 1000;
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
            lblDateTime.Text = $"{_pc.GetYear(now)}/{_pc.GetMonth(now):00}/{_pc.GetDayOfMonth(now):00} {now:HH:mm:ss}";
        }

        // ----------------------------
        // دکمه‌ها
        // ----------------------------

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
                // اصلاح CS1503: تبدیل StableKey رشته به ID عدد صحیح
                if (int.TryParse(l.StableKey, out int stableId))
                {
                    FileAttachmentHelper.CopyAttachments(stableId, l.Attachments);
                    l.Attachments = FileAttachmentHelper.GetSavedAttachments(stableId)?.ToList() ?? new List<string>();
                }
                else
                {
                    MessageBox.Show("خطا: کلید نامه (StableKey) برای ذخیره‌سازی فایل‌ها معتبر نیست و باید عدد صحیح باشد.", "خطای ذخیره‌سازی", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            RefreshGrid();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dgvLetters.SelectedRows.Count == 0) return;
            var row = dgvLetters.SelectedRows[0];
            if (row?.Tag is not Letter letter) return;

            var recipients = _letters.Select(x => x.Recipient).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().ToList();
            var form = new LetterEditForm(letter, recipients);
            if (form.ShowDialog() != DialogResult.OK) return;

            var l = form.Letter;
            if (l == null) return;

            l.DueDate = l.SentDate.AddDays(l.ResponseDays);

            if (l.Attachments != null && l.Attachments.Any())
            {
                // اصلاح CS1503: تبدیل StableKey رشته به ID عدد صحیح
                if (int.TryParse(l.StableKey, out int stableId))
                {
                    FileAttachmentHelper.CopyAttachments(stableId, l.Attachments);
                    l.Attachments = FileAttachmentHelper.GetSavedAttachments(stableId)?.ToList() ?? new List<string>();
                }
                else
                {
                    MessageBox.Show("خطا: کلید نامه (StableKey) برای ذخیره‌سازی فایل‌ها معتبر نیست و باید عدد صحیح باشد.", "خطای ذخیره‌سازی", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            RefreshGrid();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvLetters.SelectedRows.Count == 0) return;
            var row = dgvLetters.SelectedRows[0];
            if (row?.Tag is not Letter letter) return;

            if (MessageBox.Show("آیا مطمئن هستید؟", "حذف", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            _letters.Remove(letter);
            RefreshGrid();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            var kw = (txtSearch?.Text ?? "").Trim();
            var recipient = (txtSearchRecipient?.Text ?? "").Trim();
            var statusFilter = cmbFilterStatus?.Text ?? "";

            var filtered = _letters.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(kw))
                filtered = filtered.Where(x => (x.Subject ?? "").Contains(kw, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(recipient))
                filtered = filtered.Where(x => (x.Recipient ?? "").Contains(recipient, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(statusFilter) && Enum.TryParse<LetterStatus>(statusFilter, out var st))
                filtered = filtered.Where(x => x.Status == st);

            dgvLetters.Rows.Clear();
            foreach (var l in filtered)
            {
                int rowIdx = dgvLetters.Rows.Add();
                var row = dgvLetters.Rows[rowIdx];
                
                // اطمینان از تخصیص نوع داده صحیح در گرید
                row.Cells[0].Value = (rowIdx + 1).ToString();
                row.Cells[1].Value = l?.Subject ?? "";
                row.Cells[2].Value = l?.Recipient ?? "";
                row.Cells[3].Value = l?.LetterNumber ?? "";
                row.Cells[4].Value = ToPersianDateString(l?.SentDate ?? DateTime.MinValue);
                row.Cells[5].Value = l?.ResponseDays ?? 0;
                row.Cells[6].Value = ToPersianDateString(l?.DueDate == default ? DateTime.MinValue : l.DueDate);
                row.Cells[7].Value = l?.Status.ToString() ?? "";
                row.Cells[8].Value = l?.Notes ?? "";
                row.Cells[9].Value = string.Join(";", l?.Attachments ?? new List<string>());
                
                row.Tag = l;
                if (l != null) ApplyRowColor(row, l);
            }
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

        // ----------------------------
        // متدهای کمکی
        // ----------------------------

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
                int rowIdx = dgvLetters.Rows.Add();
                var row = dgvLetters.Rows[rowIdx];

                // اطمینان از تخصیص نوع داده صحیح در گرید
                row.Cells[0].Value = (rowIdx + 1).ToString();
                row.Cells[1].Value = l?.Subject ?? "";
                row.Cells[2].Value = l?.Recipient ?? "";
                row.Cells[3].Value = l?.LetterNumber ?? "";
                row.Cells[4].Value = ToPersianDateString(l?.SentDate ?? DateTime.MinValue);
                row.Cells[5].Value = l?.ResponseDays ?? 0;
                row.Cells[6].Value = ToPersianDateString(l?.DueDate == default ? DateTime.MinValue : l.DueDate);
                row.Cells[7].Value = l?.Status.ToString() ?? "";
                row.Cells[8].Value = l?.Notes ?? "";
                row.Cells[9].Value = string.Join(";", l?.Attachments ?? new List<string>());

                row.Tag = l;
                if (l != null) ApplyRowColor(row, l);
            }
        }

        private string ToPersianDateString(DateTime dt)
        {
            if (dt == DateTime.MinValue) return "";
            return $"{_pc.GetYear(dt)}/{_pc.GetMonth(dt):00}/{_pc.GetDayOfMonth(dt):00}";
        }

        private void ApplyRowColor(DataGridViewRow row, Letter l)
        {
            if (row == null || l == null) return;

            if (l.Status == LetterStatus.پاسخ_داده_شده)
                row.DefaultCellStyle.BackColor = Color.LightGreen;
            else if (l.Status == LetterStatus.پاسخ_داده_نشده)
                row.DefaultCellStyle.BackColor = Color.LightCoral;
            else if (l.Status == LetterStatus.در_حال_پیگیری)
                row.DefaultCellStyle.BackColor = Color.LightYellow;
            else
                row.DefaultCellStyle.BackColor = Color.White;
        }

    } // ⬅️ براکت بسته شدن کلاس MainForm
} // ⬅️ براکت بسته شدن Namespace FinalDotnetCoreBuild
            var l = form.Letter;
            if (l == null) return;

            l.DueDate = l.SentDate.AddDays(l.ResponseDays);

            if (l.Attachments != null && l.Attachments.Any())
            {
                // ✅ اصلاح خطاهای CS1503 (تبدیل StableKey رشته به ID عدد صحیح)
                if (int.TryParse(l.StableKey, out int stableId))
                {
                    FileAttachmentHelper.CopyAttachments(stableId, l.Attachments);
                    l.Attachments = FileAttachmentHelper.GetSavedAttachments(stableId)?.ToList() ?? new List<string>();
                }
                else
                {
                    MessageBox.Show("خطا: کلید نامه (StableKey) برای ذخیره‌سازی فایل‌ها معتبر نیست و باید عدد صحیح باشد.", "خطای ذخیره‌سازی", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            RefreshGrid();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvLetters.SelectedRows.Count == 0) return;
            var row = dgvLetters.SelectedRows[0];
            if (row?.Tag is not Letter letter) return;

            if (MessageBox.Show("آیا مطمئن هستید؟", "حذف", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            _letters.Remove(letter);
            RefreshGrid();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            var kw = (txtSearch?.Text ?? "").Trim();
            var recipient = (txtSearchRecipient?.Text ?? "").Trim();
            var statusFilter = cmbFilterStatus?.Text ?? "";

            var filtered = _letters.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(kw))
                filtered = filtered.Where(x => (x.Subject ?? "").Contains(kw, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(recipient))
                filtered = filtered.Where(x => (x.Recipient ?? "").Contains(recipient, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(statusFilter) && Enum.TryParse<LetterStatus>(statusFilter, out var st))
                filtered = filtered.Where(x => x.Status == st);

            dgvLetters.Rows.Clear();
            foreach (var l in filtered)
            {
                int rowIdx = dgvLetters.Rows.Add();
                var row = dgvLetters.Rows[rowIdx];
                
                // ✅ اطمینان از تخصیص نوع داده صحیح (برای جلوگیری از CS1503 در گرید)
                row.Cells[0].Value = (rowIdx + 1).ToString();
                row.Cells[1].Value = l?.Subject ?? "";
                row.Cells[2].Value = l?.Recipient ?? "";
                row.Cells[3].Value = l?.LetterNumber ?? "";
                row.Cells[4].Value = ToPersianDateString(l?.SentDate ?? DateTime.MinValue);
                row.Cells[5].Value = l?.ResponseDays ?? 0;
                row.Cells[6].Value = ToPersianDateString(l?.DueDate == default ? DateTime.MinValue : l.DueDate);
                row.Cells[7].Value = l?.Status.ToString() ?? "";
                row.Cells[8].Value = l?.Notes ?? "";
                row.Cells[9].Value = string.Join(";", l?.Attachments ?? new List<string>());
                
                row.Tag = l;
                if (l != null) ApplyRowColor(row, l);
            }
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

        // ----------------------------
        // متدهای کمکی
        // ----------------------------

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
                int rowIdx = dgvLetters.Rows.Add();
                var row = dgvLetters.Rows[rowIdx];

                // ✅ اطمینان از تخصیص نوع داده صحیح (برای جلوگیری از CS1503 در گرید)
                row.Cells[0].Value = (rowIdx + 1).ToString();
                row.Cells[1].Value = l?.Subject ?? "";
                row.Cells[2].Value = l?.Recipient ?? "";
                row.Cells[3].Value = l?.LetterNumber ?? "";
                row.Cells[4].Value = ToPersianDateString(l?.SentDate ?? DateTime.MinValue);
                row.Cells[5].Value = l?.ResponseDays ?? 0;
                row.Cells[6].Value = ToPersianDateString(l?.DueDate == default ? DateTime.MinValue : l.DueDate);
                row.Cells[7].Value = l?.Status.ToString() ?? "";
                row.Cells[8].Value = l?.Notes ?? "";
                row.Cells[9].Value = string.Join(";", l?.Attachments ?? new List<string>());

                row.Tag = l;
                if (l != null) ApplyRowColor(row, l);
            }
        }

        private string ToPersianDateString(DateTime dt)
        {
            if (dt == DateTime.MinValue) return "";
            return $"{_pc.GetYear(dt)}/{_pc.GetMonth(dt):00}/{_pc.GetDayOfMonth(dt):00}";
        }

        private void ApplyRowColor(DataGridViewRow row, Letter l)
        {
            // ✅ رفع اخطار CS8602
            if (row == null || l == null) return;

            if (l.Status == LetterStatus.پاسخ_داده_شده)
                row.DefaultCellStyle.BackColor = Color.LightGreen;
            else if (l.Status == LetterStatus.پاسخ_داده_نشده)
                row.DefaultCellStyle.BackColor = Color.LightCoral;
            else if (l.Status == LetterStatus.در_حال_پیگیری)
                row.DefaultCellStyle.BackColor = Color.LightYellow;
            else
                row.DefaultCellStyle.BackColor = Color.White;
        }
    }
}
                // رفع اخطار CS8602 (Dereference of a possibly null reference) با استفاده از Null-Conditional
                l.Attachments = FileAttachmentHelper.GetSavedAttachments(l.StableKey)?.ToList() ?? new List<string>();
            }

            RefreshGrid();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvLetters.SelectedRows.Count == 0) return;
            var row = dgvLetters.SelectedRows[0];
            if (row?.Tag is not Letter letter) return;

            if (MessageBox.Show("آیا مطمئن هستید؟", "حذف", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            _letters.Remove(letter);
            RefreshGrid();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            var kw = (txtSearch?.Text ?? "").Trim();
            var recipient = (txtSearchRecipient?.Text ?? "").Trim();
            var statusFilter = cmbFilterStatus?.Text ?? "";

            var filtered = _letters.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(kw))
                filtered = filtered.Where(x => (x.Subject ?? "").Contains(kw, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(recipient))
                filtered = filtered.Where(x => (x.Recipient ?? "").Contains(recipient, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(statusFilter) && Enum.TryParse<LetterStatus>(statusFilter, out var st))
                filtered = filtered.Where(x => x.Status == st);

            dgvLetters.Rows.Clear();
            foreach (var l in filtered)
            {
                int rowIdx = dgvLetters.Rows.Add();
                var row = dgvLetters.Rows[rowIdx];
                
                // --- رفع مشکل CS1503: تخصیص مقادیر عددی ---
                // اطمینان از اینکه مقادیر به صورت صحیح (رشته یا عدد) به سلول‌ها اختصاص یابد
                row.Cells[0].Value = (rowIdx + 1).ToString();
                row.Cells[1].Value = l?.Subject ?? "";
                row.Cells[2].Value = l?.Recipient ?? "";
                row.Cells[3].Value = l?.LetterNumber ?? "";
                row.Cells[4].Value = ToPersianDateString(l?.SentDate ?? DateTime.MinValue);
                row.Cells[5].Value = l?.ResponseDays ?? 0; // باید عدد باشد
                row.Cells[6].Value = ToPersianDateString(l?.DueDate == default ? DateTime.MinValue : l.DueDate);
                row.Cells[7].Value = l?.Status.ToString() ?? "";
                row.Cells[8].Value = l?.Notes ?? "";
                row.Cells[9].Value = string.Join(";", l?.Attachments ?? new List<string>());
                // ---------------------------------------------
                
                row.Tag = l;
                if (l != null) ApplyRowColor(row, l);
            }
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

        // ----------------------------
        // متدهای کمکی
        // ----------------------------

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
                int rowIdx = dgvLetters.Rows.Add();
                var row = dgvLetters.Rows[rowIdx];

                // --- رفع مشکل CS1503: تخصیص مقادیر عددی ---
                row.Cells[0].Value = (rowIdx + 1).ToString();
                row.Cells[1].Value = l?.Subject ?? "";
                row.Cells[2].Value = l?.Recipient ?? "";
                row.Cells[3].Value = l?.LetterNumber ?? "";
                row.Cells[4].Value = ToPersianDateString(l?.SentDate ?? DateTime.MinValue);
                row.Cells[5].Value = l?.ResponseDays ?? 0; // باید عدد باشد
                row.Cells[6].Value = ToPersianDateString(l?.DueDate == default ? DateTime.MinValue : l.DueDate);
                row.Cells[7].Value = l?.Status.ToString() ?? "";
                row.Cells[8].Value = l?.Notes ?? "";
                row.Cells[9].Value = string.Join(";", l?.Attachments ?? new List<string>());
                // ---------------------------------------------

                row.Tag = l;
                if (l != null) ApplyRowColor(row, l);
            }
        }

        private string ToPersianDateString(DateTime dt)
        {
            if (dt == DateTime.MinValue) return "";
            return $"{_pc.GetYear(dt)}/{_pc.GetMonth(dt):00}/{_pc.GetDayOfMonth(dt):00}";
        }

        private void ApplyRowColor(DataGridViewRow row, Letter l)
        {
            // رفع اخطار CS8602 با بررسی صریح 'null'
            if (row == null || l == null) return;

            if (l.Status == LetterStatus.پاسخ_داده_شده)
                row.DefaultCellStyle.BackColor = Color.LightGreen;
            else if (l.Status == LetterStatus.پاسخ_داده_نشده)
                row.DefaultCellStyle.BackColor = Color.LightCoral;
            else if (l.Status == LetterStatus.در_حال_پیگیری)
                row.DefaultCellStyle.BackColor = Color.LightYellow;
            else
                row.DefaultCellStyle.BackColor = Color.White;
        }
    }
}
