using System;
using System.Net.Mail;
using System.Windows.Controls;
using WPFDevelopers.Controls;

namespace WPFDevelopers.Samples.ExampleViews
{
    public partial class DialogUserControl : UserControl
    {
        public DialogUserControl()
        {
            InitializeComponent();
        }

        public void OnDialogClosing(object sender, DialogClosingEventArgs e)
        {
            if (e.Reason != DialogCloseReason.Confirm || IsValidEmail(EmailTextBox.Text))
                return;

            e.Cancel = true;
            Toast.Push(
                SampleLanguageManager.Instance.TranslateText("邮箱格式错误"),
                ToastImage.Error,
                true);
        }

        private static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                var address = new MailAddress(email);
                return string.Equals(address.Address, email, StringComparison.OrdinalIgnoreCase);
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}
