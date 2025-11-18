using System;
using System.Windows.Forms;

namespace FinalDotnetCoreBuild
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;
        private DataGridView dgvLetters;
        private Button btnAdd, btnEdit, btnDelete, btnSearch, btnSaveExcel, btnLoadExcel;
        private TextBox txtSearch, txtSearchRecipient;
        private ComboBox cmbFilterStatus;
        private Label lblDateTime;
        private Label lblDesigner;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            this.Text = "سیستم پیگیر نامه‌ها";
            this.RightToLeft = RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.Width = 1000;
            this.Height = 650;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.MinimumSize = new System.Drawing.Size(900, 600);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.AutoScroll = true;

            // جدول نامه‌ها
            dgvLetters = new DataGridView();
            dgvLetters.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvLetters.Location = new System.Drawing.Point(12, 12);
            dgvLetters.Size = new System.Drawing.Size(960, 480);
            dgvLetters.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvLetters.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvLetters.MultiSelect = false;
            dgvLetters.AllowUserToAddRows = false;
            dgvLetters.RowHeadersVisible = false;

            dgvLetters.Columns.Add("RowNumber", "ردیف");
            dgvLetters.Columns.Add("Subject", "موضوع نامه");
            dgvLetters.Columns.Add("Recipient", "گیرنده");
            dgvLetters.Columns.Add("LetterNumber", "شماره نامه");
            dgvLetters.Columns.Add("SentDate", "تاریخ ارسال");
            dgvLetters.Columns.Add("ResponseDays", "فرصت (روز)");
            dgvLetters.Columns.Add("DueDate", "تاریخ سررسید");
            dgvLetters.Columns.Add("Status", "وضعیت");
            dgvLetters.Columns.Add("Notes", "توضیحات");
            dgvLetters.Columns.Add("Attachments", "پیوست‌ها");

            this.Controls.Add(dgvLetters);

            // دکمه‌ها
            btnAdd = new Button() { Text = "افزودن نامه جدید", Width = 140, Height = 34 };
            btnEdit = new Button() { Text = "ویرایش", Width = 100, Height = 34 };
            btnDelete = new Button() { Text = "حذف", Width = 100, Height = 34 };
            btnSearch = new Button() { Text = "جستجو و فیلتر", Width = 120, Height = 34 };
            btnSaveExcel = new Button() { Text = "ذخیره در اکسل", Width = 120, Height = 34 };
            btnLoadExcel = new Button() { Text = "بارگذاری از اکسل", Width = 140, Height = 34 };

            // فیلترها
            txtSearch = new TextBox() { Width = 170, Height = 34, PlaceholderText = "موضوع..." };
            txtSearchRecipient = new TextBox() { Width = 170, Height = 28, PlaceholderText = "گیرنده..." };
            cmbFilterStatus = new ComboBox() { Width = 140, Height = 28, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbFilterStatus.Items.AddRange(new string[] { "", "در حال پیگیری", "پاسخ داده شده", "پاسخ داده نشده" });

            // لیبل‌ها
            lblDateTime = new Label() { AutoSize = true };
            lblDesigner = new Label() { AutoSize = true, Text = "طراحی توسط پیمان بطحایی لو" };

            // پنل دکمه‌ها
            var panelButtons = new FlowLayoutPanel()
            {
                FlowDirection = FlowDirection.RightToLeft,
                Dock = DockStyle.Bottom,
                Padding = new Padding(10),
                AutoSize = true
            };
            panelButtons.Controls.AddRange(new Control[] {
                btnAdd, btnEdit, btnDelete, btnSearch, btnSaveExcel, btnLoadExcel
            });

            // پنل فیلترها
            var panelFilters = new FlowLayoutPanel()
            {
                FlowDirection = FlowDirection.RightToLeft,
                Dock = DockStyle.Bottom,
                Padding = new Padding(10),
                AutoSize = true
            };
            panelFilters.Controls.AddRange(new Control[] {
                txtSearch, txtSearchRecipient, cmbFilterStatus
            });

            // پنل لیبل‌ها
            var panelLabels = new FlowLayoutPanel()
            {
                FlowDirection = FlowDirection.LeftToRight,
                Dock = DockStyle.Bottom,
                Padding = new Padding(10),
                AutoSize = true
            };
            panelLabels.Controls.AddRange(new Control[] {
                lblDesigner, lblDateTime
            });

            // افزودن به فرم
            this.Controls.Add(panelButtons);
            this.Controls.Add(panelFilters);
            this.Controls.Add(panelLabels);

            // رویدادها
            btnAdd.Click += btnAdd_Click;
            btnEdit.Click += btnEdit_Click;
            btnDelete.Click += btnDelete_Click;
            btnSearch.Click += btnSearch_Click;
            btnSaveExcel.Click += btnSaveExcel_Click;
            btnLoadExcel.Click += btnLoadExcel_Click;
        }
    }
}    btnLoadExcel.Click += btnLoadExcel_Click;
}
