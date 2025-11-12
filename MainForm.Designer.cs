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
            if (disposing && (components!=null)) components.Dispose();
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

            dgvLetters = new DataGridView();
            dgvLetters.Anchor = AnchorStyles.Top|AnchorStyles.Bottom|AnchorStyles.Left|AnchorStyles.Right;
            dgvLetters.Location = new System.Drawing.Point(12,12);
            dgvLetters.Size = new System.Drawing.Size(960,480);
            dgvLetters.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvLetters.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvLetters.MultiSelect = false;
            dgvLetters.AllowUserToAddRows = false;
            dgvLetters.RowHeadersVisible = false;
            dgvLetters.Columns.Add("Id","Id");
            dgvLetters.Columns.Add("RowNumber","ردیف");
            dgvLetters.Columns.Add("Subject","موضوع نامه");
            dgvLetters.Columns.Add("Recipient","گیرنده");
            dgvLetters.Columns.Add("LetterNumber","شماره نامه");
            dgvLetters.Columns.Add("SentDate","تاریخ ارسال");
            dgvLetters.Columns.Add("ResponseDays","فرصت (روز)");
            dgvLetters.Columns.Add("DueDate","تاریخ سررسید");
            dgvLetters.Columns.Add("Status","وضعیت");
            dgvLetters.Columns.Add("Notes","توضیحات");
            dgvLetters.Columns.Add("Attachments","پیوست‌ها");

            this.Controls.Add(dgvLetters);

            btnAdd = new Button(){ Text="افزودن نامه جدید", Left=12, Top=510, Width=140, Height=34};
            btnEdit = new Button(){ Text="ویرایش", Left=160, Top=510, Width=100, Height=34};
            btnDelete = new Button(){ Text="حذف", Left=270, Top=510, Width=100, Height=34};
            btnSearch = new Button(){ Text="جستجو و فیلتر", Left=380, Top=510, Width=120, Height=34};
            btnSaveExcel = new Button(){ Text="ذخیره در اکسل", Left=510, Top=510, Width=120, Height=34};
            btnLoadExcel = new Button(){ Text="بارگذاری از اکسل", Left=640, Top=510, Width=140, Height=34};

            txtSearch = new TextBox(){ Left=800, Top=510, Width=170, Height=34, PlaceholderText="موضوع..."};
            txtSearchRecipient = new TextBox(){ Left=800, Top=550, Width=170, Height=28, PlaceholderText="گیرنده..."};
            cmbFilterStatus = new ComboBox(){ Left=640, Top=550, Width=140, Height=28, DropDownStyle=ComboBoxStyle.DropDownList};
            cmbFilterStatus.Items.AddRange(new string[]{ "", "InProgress", "Answered", "NotAnswered" });

            lblDateTime = new Label(){ Left=760, Top=590, AutoSize=true };
            lblDesigner = new Label(){ Left=12, Top=590, AutoSize=true, Text="طراحی توسط پیمان بطحایی لو" };

            this.Controls.Add(btnAdd);
            this.Controls.Add(btnEdit);
            this.Controls.Add(btnDelete);
            this.Controls.Add(btnSearch);
            this.Controls.Add(btnSaveExcel);
            this.Controls.Add(btnLoadExcel);
            this.Controls.Add(txtSearch);
            this.Controls.Add(txtSearchRecipient);
            this.Controls.Add(cmbFilterStatus);
            this.Controls.Add(lblDateTime);
            this.Controls.Add(lblDesigner);

            btnAdd.Click += btnAdd_Click;
            btnEdit.Click += btnEdit_Click;
            btnDelete.Click += btnDelete_Click;
            btnSearch.Click += btnSearch_Click;
            btnSaveExcel.Click += btnSaveExcel_Click;
            btnLoadExcel.Click += btnLoadExcel_Click;
        }
    }
}
