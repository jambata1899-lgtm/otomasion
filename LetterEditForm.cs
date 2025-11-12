using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using PaygirLettersApp.Helpers;
using System.Globalization;

namespace PaygirLettersApp
{
    public partial class LetterEditForm : Form
    {
        public Letter Letter { get; private set; }
        private List<string> _recipients;

        public LetterEditForm(Letter? existing, List<string> recipients)
        {
            _recipients = recipients;
            InitializeComponent();
            if (existing != null)
            {
                Letter = existing;
                txtRow.Text = existing.RowNumber;
                txtSubject.Text = existing.Subject;
                cmbRecipient.Text = existing.Recipient;
                txtLetterNumber.Text = existing.LetterNumber;
                txtSentDate.Text = ToPersianDateString(existing.SentDate);
                nudResponseDays.Value = existing.ResponseDays;
                cmbStatus.SelectedItem = existing.Status.ToString();
                txtNotes.Text = existing.Notes;
                // attachments
                lstAttachments.Items.Clear();
                foreach(var a in existing.Attachments) lstAttachments.Items.Add(a);
            }
            else
            {
                Letter = new Letter();
                cmbStatus.SelectedIndex = 0;
            }
            cmbRecipient.Items.AddRange(_recipients.ToArray());
        }

        private PersianCalendar _pc = new PersianCalendar();
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
            if (parts.Length!=3) return DateTime.Now;
            try
            {
                int y=int.Parse(parts[0]);
                int m=int.Parse(parts[1]);
                int d=int.Parse(parts[2]);
                return _pc.ToDateTime(y,m,d,0,0,0,0);
            }
            catch { return DateTime.Now; }
        }

        private void btnAttach_Click(object sender, EventArgs e)
        {
            using var dlg = new OpenFileDialog();
            dlg.Multiselect = true;
            if (dlg.ShowDialog()==DialogResult.OK)
            {
                foreach(var f in dlg.FileNames)
                {
                    lstAttachments.Items.Add(f);
                }
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            Letter.RowNumber = txtRow.Text.Trim();
            Letter.Subject = txtSubject.Text.Trim();
            Letter.Recipient = cmbRecipient.Text.Trim();
            Letter.LetterNumber = txtLetterNumber.Text.Trim();
            Letter.SentDate = ParsePersianDate(txtSentDate.Text.Trim());
            Letter.ResponseDays = (int)nudResponseDays.Value;
            Letter.DueDate = Letter.SentDate.AddDays(Letter.ResponseDays);
            if (Enum.TryParse<LetterStatus>(cmbStatus.Text, out var st)) Letter.Status = st;
            Letter.Notes = txtNotes.Text;
            Letter.Attachments = lstAttachments.Items.Cast<string>().ToList();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnRemoveAttach_Click(object sender, EventArgs e)
        {
            var sel = lstAttachments.SelectedIndex;
            if (sel>=0) lstAttachments.Items.RemoveAt(sel);
        }
    }
}
