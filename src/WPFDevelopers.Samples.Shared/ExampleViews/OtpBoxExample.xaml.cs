using System.Windows.Controls;
using System.Windows.Input;
using WPFDevelopers.Controls;
using WPFDevelopers.Samples.Helpers;

namespace WPFDevelopers.Sample.ExampleViews
{
    /// <summary>
    /// OtpBoxExample.xaml 的交互逻辑
    /// </summary>
    public partial class OtpBoxExample : UserControl
    {
        private enum OtpBoxType
        {
            OtpBox1,
            OtpBox2
        }

        private string _otpPassword = "0426";

        public OtpBoxExample()
        {
            InitializeComponent();
        }

        private void HandleOtpCheck(string pwd, OtpBoxType otpType)
        {
            if (pwd != _otpPassword)
            {
                SetOtpState(otpType, ControlState.Error);
                Toast.PushDesktop("OTP错误，请重试!", ToastImage.Error, true);
                return;
            }

            SetOtpState(otpType, ControlState.Success);
            Toast.Push("OTP正确!", ToastImage.Success, true);
        }

        private void SetOtpState(OtpBoxType otpType, ControlState state)
        {
            if (otpType == OtpBoxType.OtpBox1)
            {
                myOtpBox.State = state;
            }
            else if (otpType == OtpBoxType.OtpBox2)
            {
                myOtpBox2.State = state;
            }
        }

        private void OtpBoxCompleted(object sender, System.Windows.RoutedEventArgs e)
        {
            var otpBox = e.OriginalSource as OtpBox;
            HandleOtpCheck(otpBox?.Value ?? string.Empty, OtpBoxType.OtpBox1);
        }

        public ICommand CompletedCommand => new RelayCommand(param =>
        {
            var pwd = param.ToString();
            HandleOtpCheck(pwd, OtpBoxType.OtpBox2);
        });
    }
}
