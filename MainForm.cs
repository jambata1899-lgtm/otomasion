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
            // بارگذاری
            _letters.Clear();
            _letters.AddRange(ExcelHelper.Load());
            RefreshGrid();

            // ساعت
            _clockTimer.Interval = 1000;
            _clockTimer.Tick += (s, ev) => UpdateClock();
            _clockTimer.Start();
            UpdateClock();

            // نوتیفیکیشن
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
            var form = new LetterEditForm(null, _letters.Select(x => x.Recipient).Distinct().ToList());
            if (form.ShowDialog() != DialogResult.OK) return;

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

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dgvLetters.SelectedRows.Count == 0) return;

            var row = dgvLetters.SelectedRows[0];
            var letter = row.Tag as Letter;
            if (letter == null) return;

            var form = new LetterEditForm(letter, _letters.Select(x => x.Recipient).Distinct().ToList());
            if (form.ShowDialog() != DialogResult.OK) return;

            var l = form.Letter;
            l.DueDate = l.SentDate.AddDays(l.ResponseDays);

            if (l.Attachments.Any())
            {
                FileAttachmentHelper.CopyAttachments(l.StableKey, l.Attachments);
                l.Attachments = FileAttachmentHelper.GetSavedAttachments(l.StableKey);
            }

            RefreshGrid();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvLetters.SelectedRows.Count == 0) return;

            var row = dgvLetters.SelectedRows[0];
            var letter = row.Tag as Letter;
            if (letter == null) return;

            if (MessageBox.Show("آیا مطمئن هستید؟", "حذف", MessageBoxButtons.YesNo) != DialogResult.Yes)
                return;

            _letters.Remove(letter);
            RefreshGrid();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            var kw = txtSearch.Text.Trim();
            var recipient = txtSearchRecipient.Text.Trim();
            var statusFilter = cmbFilterStatus.Text;

            var filtered = _letters.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(kw))
                filtered = filtered.Where(x => x.Subject.Contains(kw, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(recipient))
                filtered = filtered.Where(x => x.Recipient.Contains(recipient, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(statusFilter) &&
                Enum.TryParse<LetterStatus>(statusFilter, out var st))
                filtered = filtered.Where(x => x.Status == st);

            PopulateGrid(filtered);
        }

        private void btnSaveExcel_Click(object sender, EventArgs e)
        {
            ExcelHelper.Save(_letters);
            MessageBox.Show("ذخیره انجام شد", "پیام");
        }

        private void btnLoadExcel_Click(object sender, EventArgs e)
        {
            _letters.Clear();
            _letters.AddRange(ExcelHelper.Load());
            RefreshGrid();
            MessageBox.Show("بارگذاری انجام شد", "پیام");
        }

        // متدهای کمکی
        private void RefreshGrid()
        {
            PopulateGrid(_letters);
        }

        private void PopulateGrid(IEnumerable<Letter> source)
        {
            dgvLetters.Rows.Clear();

            foreach (var l in source)
            {
                // اضافه‌کردن ردیف بدون استفاده از اورلودهای مبهم
                int rowIdx = dgvLetters.Rows.Add();
                var row = dgvLetters.Rows[rowIdx];

                // نمایش شماره ردیف
                SetCellSafe(row, 0, (rowIdx + 1).ToString());

                // مقداردهی سلول‌ها مطابق ترتیب ستون‌ها
                SetCellSafe(row, 1, l.Subject);
                SetCellSafe(row, 2, l.Recipient);
                SetCellSafe(row, 3, l.LetterNumber);
                SetCellSafe(row, 4, ToPersianDateString(l.SentDate));
                SetCellSafe(row, 5, l.ResponseDays);
                SetCellSafe(row, 6, ToPersianDateString(l.DueDate));
                SetCellSafe(row, 7, l.Status.ToString());
                SetCellSafe(row, 8, l.Notes);
                SetCellSafe(row, 9, string.Join(";", l.Attachments ?? new List<string>()));

                // نگه‌داشتن مرجع آیتم برای ویرایش/حذف
                row.Tag = l;

                // رنگ‌دهی
                ApplyRowColor(row, l);
            }
        }

        private static void SetCellSafe(DataGridViewRow row, int index, object? value)
        {
            // اگر ستون وجود ندارد، از خطا جلوگیری می‌کند
            if (index < 0 || index >= row.DataGridView.Columns.Count) return;
            row.Cells[index].Value = value ?? "";
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
