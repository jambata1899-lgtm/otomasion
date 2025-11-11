using System;
using System.Windows.Forms;
using PaygirLettersApp.Models;

namespace PaygirLettersApp
{
    public partial class LetterForm : Form
    {
        public Letter Letter { get; private set; }

        public LetterForm() : this(new Letter { SentDate = DateTime.Now, ResponseDays = 7, Status = "در دست پیگیری" }) { }

        public LetterForm(Letter existing)
        {
            InitializeComponent();
            Letter = existing;
            Bind();
        }

        private void Bind()
        {
            txtSubject.Text = Letter.Subject;
            txtRecipient.Text = Letter.Recipient;
            txtNumber.Text = Letter.Number;
            dtpSent.Value = Letter.SentDate;
            nudDays.Value = Letter.ResponseDays;
            cmbStatus.Items.AddRange(new string[] { "در دست پیگیری", "پاسخ داده شده", "پاسخ داده نشده" });
            cmbStatus.SelectedItem = Letter.Status;
            txtNotes.Text = Letter.Notes;
            UpdateDueDate();
        }

        private void UpdateDueDate()
        {
            var due = dtpSent.Value.AddDays((double)nudDays.Value);
            lblDue.Text = "تاریخ سررسید: " + due.ToShortDateString();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            Letter.Subject = txtSubject.Text;
            Letter.Recipient = txtRecipient.Text;
            Letter.Number = txtNumber.Text;
            Letter.SentDate = dtpSent.Value;
            Letter.ResponseDays = (int)nudDays.Value;
            Letter.DueDate = dtpSent.Value.AddDays((double)nudDays.Value);
            Letter.Status = cmbStatus.SelectedItem?.ToString() ?? "در دست پیگیری";
            Letter.Notes = txtNotes.Text;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void nudDays_ValueChanged(object sender, EventArgs e) => UpdateDueDate();
        private void dtpSent_ValueChanged(object sender, EventArgs e) => UpdateDueDate();
    }
}
