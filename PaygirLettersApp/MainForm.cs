using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using PaygirLettersApp.Models;

namespace PaygirLettersApp
{
    public partial class MainForm : Form
    {
        private List<Letter> _letters = new List<Letter>();
        private int _nextId = 1;
        private readonly string defaultPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "PaygirLettersData.xlsx");

        public MainForm()
        {
            InitializeComponent();
            InitGrid();
            LoadFromDefaultIfExists();
            UpdateStatusBar();
        }

        private void InitGrid()
        {
            dataGridViewLetters.AutoGenerateColumns = false;
            dataGridViewLetters.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "ردیف", DataPropertyName = "Id" });
            dataGridViewLetters.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "موضوع", DataPropertyName = "Subject" });
            dataGridViewLetters.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "گیرنده", DataPropertyName = "Recipient" });
            dataGridViewLetters.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "شماره", DataPropertyName = "Number" });
            dataGridViewLetters.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "تاریخ ارسال", DataPropertyName = "SentDate" });
            dataGridViewLetters.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "تاریخ سررسید", DataPropertyName = "DueDate" });
            dataGridViewLetters.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "وضعیت", DataPropertyName = "Status" });
            RefreshGrid();
        }

        private void RefreshGrid()
        {
            dataGridViewLetters.DataSource = null;
            dataGridViewLetters.DataSource = _letters.OrderBy(l=>l.Id).ToList();
            ApplyRowColors();
        }

        private void ApplyRowColors()
        {
            foreach (DataGridViewRow r in dataGridViewLetters.Rows)
            {
                if (r.DataBoundItem is Letter l)
                {
                    if (l.Status == "پاسخ داده شده") r.DefaultCellStyle.BackColor = System.Drawing.Color.LightGreen;
                    else if (l.Status != "پاسخ داده شده" && (l.DueDate - DateTime.Now).TotalDays <= 0) r.DefaultCellStyle.BackColor = System.Drawing.Color.LightCoral;
                    else if (l.Status != "پاسخ داده شده" && (l.DueDate - DateTime.Now).TotalDays <= 3) r.DefaultCellStyle.BackColor = System.Drawing.Color.LightYellow;
                }
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            var frm = new LetterForm();
            if (frm.ShowDialog() == DialogResult.OK)
            {
                var l = frm.Letter;
                l.Id = _nextId++;
                _letters.Add(l);
                RefreshGrid();
                UpdateStatusBar();
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dataGridViewLetters.CurrentRow?.DataBoundItem is Letter l)
            {
                var frm = new LetterForm(l);
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    RefreshGrid();
                    UpdateStatusBar();
                }
            }
            else MessageBox.Show("ابتدا یک سطر را انتخاب کنید.");
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridViewLetters.CurrentRow?.DataBoundItem is Letter l)
            {
                if (MessageBox.Show("آیا مطمئن هستید؟", "حذف", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    _letters.Remove(l);
                    RefreshGrid();
                    UpdateStatusBar();
                }
            }
            else MessageBox.Show("ابتدا یک سطر را انتخاب کنید.");
        }

        private void btnSaveExcel_Click(object sender, EventArgs e)
        {
            var sfd = new SaveFileDialog();
            sfd.Filter = "Excel files|*.xlsx";
            sfd.FileName = Path.GetFileName(defaultPath);
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    ExcelService.SaveToExcel(_letters, sfd.FileName);
                    MessageBox.Show("ذخیره با موفقیت انجام شد.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("خطا در ذخیره: " + ex.Message);
                }
            }
        }

        private void btnLoadExcel_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog();
            ofd.Filter = "Excel files|*.xlsx";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var loaded = ExcelService.LoadFromExcel(ofd.FileName);
                    _letters = loaded.ToList();
                    _nextId = _letters.Any() ? _letters.Max(x=>x.Id)+1 : 1;
                    RefreshGrid();
                    UpdateStatusBar();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("خطا در بارگذاری: " + ex.Message);
                }
            }
        }

        private void LoadFromDefaultIfExists()
        {
            try
            {
                if (System.IO.File.Exists(defaultPath))
                {
                    _letters = ExcelService.LoadFromExcel(defaultPath).ToList();
                    _nextId = _letters.Any() ? _letters.Max(x=>x.Id)+1 : 1;
                }
            }
            catch { }
        }

        private void UpdateStatusBar()
        {
            toolStripStatusLabel1.Text = $"تعداد نامه‌ها: {_letters.Count}";
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            var q = txtSearch.Text?.Trim();
            if (string.IsNullOrEmpty(q)) { RefreshGrid(); return; }
            var found = _letters.Where(l => (l.Subject!=null && l.Subject.Contains(q)) || (l.Recipient!=null && l.Recipient.Contains(q))).ToList();
            dataGridViewLetters.DataSource = null;
            dataGridViewLetters.DataSource = found;
            ApplyRowColors();
        }

        private void btnOpenAttachments_Click(object sender, EventArgs e)
        {
            if (dataGridViewLetters.CurrentRow?.DataBoundItem is Letter l)
            {
                var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "PaygirLetters_Attachments", l.Id.ToString());
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                System.Diagnostics.Process.Start("explorer.exe", path);
            }
            else MessageBox.Show("ابتدا یک سطر را انتخاب کنید.");
        }
    }
}
