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
        private CultureInfo _persianCulture = new CultureInfo("fa-IR");
        private PersianCalendar _pc = new PersianCalendar();
        private int _nextId = 1;

        public MainForm()
        {
            InitializeComponent();
            Load += MainForm_Load;
            FormClosing += MainForm_FormClosing;
        }

        private void MainForm_Load(object? sender, EventArgs e)
        {
            _letters = ExcelHelper.Load();
            if (_letters.Any()) _nextId = _letters.Max(x => x.Id) + 1;
            RefreshGrid();

            _clockTimer.Interval = 1000;
            _clockTimer.Tick += (s, ev) => UpdateClock();
            _clockTimer.Start();
            UpdateClock();

            _notifyTimer.Interval = 60 * 60 * 1000;
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
            int rowNum = 1;
            foreach (var l in _letters)
            {
                var statusText = GetPersianStatus(l.Status);

                var rowIdx = dgvLetters.Rows.Add(
                    rowNum++,
                    l.Subject,
                    l.Recipient,
                    l.LetterNumber,
                    ToPersianDateString(l.SentDate),
                    l.ResponseDays,
                    ToPersianDateString(l.DueDate),
                    statusText,
                    l.Notes,
                    string.Join(";", l.Attachments ?? new List<string>())
                );

                var row = dgvLetters.Rows[rowIdx];
                ApplyRowColor(row, l);
            }
        }

        private string GetPersianStatus(LetterStatus status)
        {
            return status switch
            {
                LetterStatus.InProgress => "در حال پیگیری",
                LetterStatus.Answered => "پاسخ داده شده",
                LetterStatus.NotAnswered => "پاسخ داده نشده",
                _ => ""
            };
        }

        private void ApplyRowColor(DataGridViewRow row, Letter l)
        {
            var daysLeft = (l.DueDate.Date - DateTime.Now.Date).TotalDays;

            if (l.Status == LetterStatus.Answered)
            {
                row.DefaultCellStyle.BackColor = System.Drawing.Color.LightGreen;
            }
            else if (daysLeft < 0)
            {
                row.DefaultCellStyle.BackColor = System.Drawing.Color.LightCoral;
            }
            else if (daysLeft <= 3)
            {
                row.DefaultCellStyle.BackColor = System.Drawing.Color.Khaki;
            }
            else
            {
                row.DefaultCellStyle.BackColor = System.Drawing.Color.White;
            }
        }

        private string ToPersianDateString(DateTime dt)
        {
            var y = _pc.GetYear(dt);
            var m = _pc.GetMonth(dt).ToString("00");
            var d = _pc.GetDayOfMonth(dt).ToString("00");
            return $"{y}/{m}/{d}";
        }

        private DateTime ParsePersianDate(string persian)
        {
            var parts = persian.Split('/');
            if (parts.Length != 3) return DateTime.Now;
            try
            {
                int y = int.Parse(parts[0]);
                int m = int.Parse(parts[1]);
                int d = int.Parse(parts[2]);
                return _pc.ToDateTime(y, m, d, 0, 0, 0, 0);
            }
            catch { return DateTime.Now; }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            var form = new LetterEditForm(null, _letters.Select(x => x.Recipient).Distinct().ToList());
            if (form.ShowDialog() == DialogResult.OK)
            {
                var l = form.Letter;
                l.Id = _nextId++;
                l.RowNumber = _letters.Count + 1; // ردیف خودکار
                l.DueDate = l.SentDate.AddDays(l.ResponseDays);

                _letters.Add(l);

                if (l.Attachments.Any())
                {
                    FileAttachmentHelper.CopyAttachments(l.Id, l.Attachments);
                    l.Attachments = FileAttachmentHelper.GetSavedAttachments(l.Id);
                }

                RefreshGrid();
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dgvLetters.SelectedRows.Count == 0) return;
            var rowIndex = dgvLetters.SelectedRows[0].Index;
            var letter = _letters[rowIndex];
            if (letter == null) return;

            var form = new LetterEditForm(letter, _letters.Select(x => x.Recipient).Distinct().ToList());
            if (form.ShowDialog() == DialogResult.OK)
            {
                var l = form.Letter;
                l.DueDate = l.SentDate.AddDays(l.ResponseDays);

                if (l.Attachments.Any(a => !a.StartsWith(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "PaygirLetters_Attachments"))))
                {
                    FileAttachmentHelper.CopyAttachments(l.Id, l.Attachments);
                    l.Attachments = FileAttachmentHelper.GetSavedAttachments(l.Id);
                }

                RefreshGrid();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvLetters.SelectedRows.Count == 0) return;
            var index = dgvLetters.SelectedRows[0].Index;
            if (index < 0 || index >= _letters.Count) return;

            if (MessageBox.Show("آیا مطمئن هستید؟", "حذف", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                _letters.RemoveAt(index);
                RefreshGrid();
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            var kw = txtSearch.Text.Trim();
            var recipient = txtSearchRecipient.Text.Trim();
            var statusFilter = cmbFilterStatus.Text.Trim();

            var filtered = _letters.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(kw))
                filtered = filtered.Where(x => x.Subject.Contains(kw));

            if (!string.IsNullOrWhiteSpace(recipient))
                filtered = filtered.Where(x => x.Recipient.Contains(recipient));

            if (!string.IsNullOrWhiteSpace(statusFilter))
            {
                filtered = filtered.Where(x => GetPersianStatus(x.Status) == statusFilter);
            }

            dgvLetters.Rows.Clear();
            int rowNum = 1;
            foreach (var l in filtered)
            {
                var rowIdx = dgvLetters.Rows.Add(
                    rowNum++,
                    l.Subject,
                    l.Recipient,
                    l.LetterNumber,
                    ToPersianDateString(l.SentDate),
                    l.ResponseDays,
                    ToPersianDateString(l.DueDate),
                    GetPersianStatus(l.Status),
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
            if (_letters.Any()) _nextId = _letters.Max(x => x.Id) + 1;
            RefreshGrid();
            MessageBox.Show("بارگذاری انجام شد", "پیام");
        }
    }
}
