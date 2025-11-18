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
                row.DefaultCellStyle.BackColor = System.Drawing.Color.LightGreen;
            else if (l.Status == Letter
