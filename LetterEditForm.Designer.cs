using System;
using System.Windows.Forms;

namespace FinalDotnetCoreBuild
{
    partial class LetterEditForm
    {
        private System.ComponentModel.IContainer components = null;
        private TextBox txtRow, txtSubject, txtLetterNumber, txtSentDate, txtNotes;
        private ComboBox cmbRecipient, cmbStatus;
        private NumericUpDown nudResponseDays;
        private Button btnAttach, btnOk, btnRemoveAttach;
        private ListBox lstAttachments;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components!=null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            this.Text = "مدیریت نامه";
            this.Width = 600;
            this.Height = 480;
            this.StartPosition = FormStartPosition.CenterParent;
            this.RightToLeft = RightToLeft.Yes;
            this.RightToLeftLayout = true;

            var lblRow = new Label(){ Left=460, Top=20, Text="ردیف", AutoSize=true };
            txtRow = new TextBox(){ Left=260, Top=16, Width=180 };

            var lblSubject = new Label(){ Left=460, Top=60, Text="موضوع نامه", AutoSize=true };
            txtSubject = new TextBox(){ Left=260, Top=56, Width=180 };

            var lblRecipient = new Label(){ Left=460, Top=100, Text="گیرنده", AutoSize=true };
            cmbRecipient = new ComboBox(){ Left=260, Top=96, Width=180, DropDownStyle=ComboBoxStyle.DropDown };

            var lblLetterNumber = new Label(){ Left=460, Top=140, Text="شماره نامه", AutoSize=true };
            txtLetterNumber = new TextBox(){ Left=260, Top=136, Width=180 };

            var lblSentDate = new Label(){ Left=460, Top=180, Text="تاریخ ارسال (yyyy/MM/dd)", AutoSize=true };
            txtSentDate = new TextBox(){ Left=260, Top=176, Width=180, Text= DateTime.Now.ToString("yyyy/MM/dd") };

            var lblResponseDays = new Label(){ Left=460, Top=220, Text="فرصت پاسخ (روز)", AutoSize=true };
            nudResponseDays = new NumericUpDown(){ Left=260, Top=216, Width=80, Minimum=0, Maximum=365, Value=7 };

            var lblStatus = new Label(){ Left=460, Top=260, Text="وضعیت", AutoSize=true };
            cmbStatus = new ComboBox(){ Left=260, Top=256, Width=120, DropDownStyle=ComboBoxStyle.DropDownList};
            cmbStatus.Items.AddRange(new string[]{ "InProgress", "Answered", "NotAnswered"});

            var lblNotes = new Label(){ Left=460, Top=300, Text="توضیحات", AutoSize=true };
            txtNotes = new TextBox(){ Left=20, Top=296, Width=420, Height=80, Multiline=true };

            btnAttach = new Button(){ Left=260, Top=380, Width=120, Text="پیوست فایل" };
            btnRemoveAttach = new Button(){ Left=390, Top=380, Width=90, Text="حذف پیوست" };
            lstAttachments = new ListBox(){ Left=20, Top=380, Width=220, Height=60 };

            btnOk = new Button(){ Left=20, Top=460-40, Width=120, Text="تأیید" };

            this.Controls.AddRange(new Control[]{
                lblRow, txtRow,
                lblSubject, txtSubject,
                lblRecipient, cmbRecipient,
                lblLetterNumber, txtLetterNumber,
                lblSentDate, txtSentDate,
                lblResponseDays, nudResponseDays,
                lblStatus, cmbStatus,
                lblNotes, txtNotes,
                btnAttach, btnOk, lstAttachments, btnRemoveAttach
            });

            btnAttach.Click += btnAttach_Click;
            btnOk.Click += btnOk_Click;
            btnRemoveAttach.Click += btnRemoveAttach_Click;
        }
    }
}
