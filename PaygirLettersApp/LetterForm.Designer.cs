namespace PaygirLettersApp
{
    partial class LetterForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.TextBox txtSubject;
        private System.Windows.Forms.TextBox txtRecipient;
        private System.Windows.Forms.TextBox txtNumber;
        private System.Windows.Forms.DateTimePicker dtpSent;
        private System.Windows.Forms.NumericUpDown nudDays;
        private System.Windows.Forms.ComboBox cmbStatus;
        private System.Windows.Forms.TextBox txtNotes;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Label lblDue;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.txtSubject = new System.Windows.Forms.TextBox();
            this.txtRecipient = new System.Windows.Forms.TextBox();
            this.txtNumber = new System.Windows.Forms.TextBox();
            this.dtpSent = new System.Windows.Forms.DateTimePicker();
            this.nudDays = new System.Windows.Forms.NumericUpDown();
            this.cmbStatus = new System.Windows.Forms.ComboBox();
            this.txtNotes = new System.Windows.Forms.TextBox();
            this.btnOk = new System.Windows.Forms.Button();
            this.lblDue = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.nudDays)).BeginInit();
            this.SuspendLayout();
            // 
            // txtSubject
            // 
            this.txtSubject.Location = new System.Drawing.Point(12, 12);
            this.txtSubject.Name = "txtSubject";
            this.txtSubject.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.txtSubject.Size = new System.Drawing.Size(360, 23);
            this.txtSubject.PlaceholderText = "موضوع نامه";
            // 
            // txtRecipient
            // 
            this.txtRecipient.Location = new System.Drawing.Point(12, 41);
            this.txtRecipient.Name = "txtRecipient";
            this.txtRecipient.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.txtRecipient.Size = new System.Drawing.Size(360, 23);
            this.txtRecipient.PlaceholderText = "گیرنده نامه";
            // 
            // txtNumber
            // 
            this.txtNumber.Location = new System.Drawing.Point(12, 70);
            this.txtNumber.Name = "txtNumber";
            this.txtNumber.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.txtNumber.Size = new System.Drawing.Size(360, 23);
            this.txtNumber.PlaceholderText = "شماره نامه";
            // 
            // dtpSent
            // 
            this.dtpSent.Location = new System.Drawing.Point(12, 99);
            this.dtpSent.Name = "dtpSent";
            this.dtpSent.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.dtpSent.Size = new System.Drawing.Size(200, 23);
            // 
            // nudDays
            // 
            this.nudDays.Location = new System.Drawing.Point(218, 99);
            this.nudDays.Minimum = new decimal(new int[] {1,0,0,0});
            this.nudDays.Maximum = new decimal(new int[] {3650,0,0,0});
            this.nudDays.Name = "nudDays";
            this.nudDays.Size = new System.Drawing.Size(60, 23);
            this.nudDays.Value = new decimal(new int[] {7,0,0,0});
            this.nudDays.ValueChanged += new System.EventHandler(this.nudDays_ValueChanged);
            // 
            // cmbStatus
            // 
            this.cmbStatus.Location = new System.Drawing.Point(12, 128);
            this.cmbStatus.Name = "cmbStatus";
            this.cmbStatus.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.cmbStatus.Size = new System.Drawing.Size(200, 23);
            // 
            // txtNotes
            // 
            this.txtNotes.Location = new System.Drawing.Point(12, 157);
            this.txtNotes.Multiline = true;
            this.txtNotes.Name = "txtNotes";
            this.txtNotes.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.txtNotes.Size = new System.Drawing.Size(360, 80);
            // 
            // lblDue
            // 
            this.lblDue.Location = new System.Drawing.Point(278, 99);
            this.lblDue.Name = "lblDue";
            this.lblDue.Size = new System.Drawing.Size(94, 23);
            this.lblDue.Text = "تاریخ سررسید:";
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(150, 243);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(80, 30);
            this.btnOk.Text = "تایید";
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // LetterForm
            // 
            this.ClientSize = new System.Drawing.Size(384, 285);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.lblDue);
            this.Controls.Add(this.txtNotes);
            this.Controls.Add(this.cmbStatus);
            this.Controls.Add(this.nudDays);
            this.Controls.Add(this.dtpSent);
            this.Controls.Add(this.txtNumber);
            this.Controls.Add(this.txtRecipient);
            this.Controls.Add(this.txtSubject);
            this.Name = "LetterForm";
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.Text = "فرم نامه";
            ((System.ComponentModel.ISupportInitialize)(this.nudDays)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
