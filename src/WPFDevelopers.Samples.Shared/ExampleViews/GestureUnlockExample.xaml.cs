using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WPFDevelopers.Controls;
using WPFDevelopers.Samples.Helpers;

namespace WPFDevelopers.Sample.ExampleViews
{
    /// <summary>
    /// GestureUnlockExample.xaml 的交互逻辑
    /// </summary>
    public partial class GestureUnlockExample : UserControl
    {
        private string _password = "0426";
        private enum GestureUnlockType
        {
            Unlock1,
            Unlock2
        }
        public GestureUnlockExample()
        {
            InitializeComponent();
        }

        private void HandleGestureUnlock(string pwd, GestureUnlockType unlockType)
        {
            if (pwd.Length < 4)
            {
                SetControlState(unlockType, ControlState.Error);
                Toast.PushDesktop("手势错误，最少 4 个节点!", ToastImage.Error, true);
                return;
            }

            if (pwd != _password)
            {
                SetControlState(unlockType, ControlState.Error);
                Toast.PushDesktop("手势错误，请重新解锁!", ToastImage.Error, true);
                return;
            }

            SetControlState(unlockType, ControlState.Success);
            Toast.Push("手势正确!", ToastImage.Success, true);
        }

        private void SetControlState(GestureUnlockType unlockType, ControlState state)
        {
            if (unlockType == GestureUnlockType.Unlock1)
            {
                myGestureUnlock.State = state;
            }
            else if (unlockType == GestureUnlockType.Unlock2)
            {
                myGestureUnlock2.State = state;
            }
        }

        private void GestureCompleted(object sender, RoutedEventArgs e)
        {
            var unlock = e.OriginalSource as GestureUnlock;
            HandleGestureUnlock(unlock?.Password ?? string.Empty, GestureUnlockType.Unlock1);
        }

        public ICommand CompletedCommand => new RelayCommand(param =>
        {
            var pwd = param.ToString();
            HandleGestureUnlock(pwd, GestureUnlockType.Unlock2);
        });
    }
}
