using PaygirLettersApp.Data;
using PaygirLettersApp.Helpers;
using PaygirLettersApp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace PaygirLettersApp
{
    public partial class MainForm : Form
    {
        private BindingList<Letter> _letters = new BindingList<Letter>();
        private int _nextId = 1;

        public MainForm()
        {
            InitializeComponent();

            // DataGridView setup
            dgvLetters.AutoGenerateColumns = false;
            dgvLetters.DataSource = _letters;

            // create columns if not created in designer
            if (dgvLetters.Columns.Count == 0)
            {
                dgvLetters.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Id", HeaderText = "ردیف" });
                dgvLetters.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Subject", HeaderText = "موضوع" });
                dgvLetters.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Recipient", HeaderText = "گیرنده" });
                dgvLetters.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "LetterNumber", HeaderText = "شماره نامه" });
                dgvLetters.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "SentDate", HeaderText = "تاریخ ارسال" });
                dgvLetters.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "ResponseDays", HeaderText = "فرصت (روز)" });
                dgvLetters.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Status", HeaderText = "وضعیت" });
            }

            // load saved data if exists
            LoadData();
            ApplyRowColors();

            _letters.ListChanged += (s, e) => ApplyRowColors();
        }

        private void LoadData()
        {
            try
            {
                var path = LetterRepository.DefaultPath;
                var list = LetterRepository.LoadFromExcel(path);
                if (list.Count > 0)
                {
                    foreach (var l in list)
                    {
                        _letters.Add(l);
                    }
                    _nextId = _letters.Max(x => x.Id) + 1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("خطا در بارگذاری داده‌ها: " + ex.Message);
            }
        }

        private void SaveData()
        {
            try
            {
                var path = LetterRepository.DefaultPath;
                LetterRepository.SaveToExcel(path, _letters);
            }
            catch (Exception ex)
            {
                MessageBox.Show("خطا در ذخیره‌سازی: " + ex.Message);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            var form = new LetterForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                var letter = form.Result;
                letter.Id = _nextId++;
                _letters.Add(letter);
                SaveData();
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dgvLetters.CurrentRow == null) return;
            var letter = dgvLetters.CurrentRow.DataBoundItem as Letter;
            if (letter == null) return;

            var clone = new Letter
            {
                Id = letter.Id,
                Subject = letter.Subject,
                Recipient = letter.Recipient,
                LetterNumber = letter.LetterNumber,
                SentDate = letter.SentDate,
                ResponseDays = letter.ResponseDays,
                Status = letter.Status,
                Notes = letter.Notes,
                Attachments = new List<string>(letter.Attachments)
            };

            var form = new LetterForm(clone);
            if (form.ShowDialog() == DialogResult.OK)
            {
                var updated = form.Result;
                // apply updated fields
                letter.Subject = updated.Subject;
                letter.Recipient = updated.Recipient;
                letter.LetterNumber = updated.LetterNumber;
                letter.SentDate = updated.SentDate;
                letter.ResponseDays = updated.ResponseDays;
                letter.Status = updated.Status;
                letter.Notes = updated.Notes;
                letter.Attachments = updated.Attachments;
                dgvLetters.Refresh();
                SaveData();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvLetters.CurrentRow == null) return;
            var letter = dgvLetters.CurrentRow.DataBoundItem as Letter;
            if (letter == null) return;
            if (MessageBox.Show("آیا از حذف اطمینان دارید؟", "تأیید", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                _letters.Remove(letter);
                SaveData();
            }
        }

        private void ApplyRowColors()
        {
            foreach (DataGridViewRow row in dgvLetters.Rows)
            {
                if (!(row.DataBoundItem is Letter l)) continue;
                var daysLeft = (l.DueDate.Date - DateTime.Now.Date).TotalDays;
                // default
                row.DefaultCellStyle.BackColor = System.Drawing.Color.White;

                if (l.Status == LetterStatus.Answered)
                {
                    // سبز
                    row.DefaultCellStyle.BackColor = System.Drawing.Color.LightGreen;
                }
                else
                {
                    if (daysLeft < 0)
                    {
                        // منقضی -> قرمز
                        row.DefaultCellStyle.BackColor = System.Drawing.Color.LightCoral;
                    }
                    else if (daysLeft <= 3)
                    {
                        // نزدیک -> زرد
                        row.DefaultCellStyle.BackColor = System.Drawing.Color.LightYellow;
                    }
                }
            }
        }

        private void btnSaveExcel_Click(object sender, EventArgs e)
        {
            using (var sfd = new SaveFileDialog())
            {
                sfd.Filter = "Excel Files|*.xlsx";
                sfd.FileName = "PaygirLettersData.xlsx";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    LetterRepository.SaveToExcel(sfd.FileName, _letters);
                    MessageBox.Show("ذخیره انجام شد.");
                }
            }
        }

        private void btnLoadExcel_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Filter = "Excel Files|*.xlsx";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    var list = LetterRepository.LoadFromExcel(ofd.FileName);
                    _letters.Clear();
                    foreach (var l in list) _letters.Add(l);
                    _nextId = _letters.Any() ? _letters.Max(x => x.Id) + 1 : 1;
                    ApplyRowColors();
                }
            }
        }

        private void btnOpenAttachments_Click(object sender, EventArgs e)
        {
            if (dgvLetters.CurrentRow == null) return;
            var letter = dgvLetters.CurrentRow.DataBoundItem as Letter;
            if (letter == null) return;
            var folder = AttachmentManager.EnsureLetterFolder(letter.Id);
            if (Directory.Exists(folder))
            {
                System.Diagnostics.Process.Start("explorer.exe", folder);
            }
            else
            {
                MessageBox.Show("پوشه‌ی پیوست وجود ندارد.");
            }
        }
    }
}
