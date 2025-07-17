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
            DataContext = this;
        }

        private void HandleGestureUnlock(string pwd, GestureUnlockType unlockType)
        {
            if (pwd.Length < 4)
            {
                SetGestureState(unlockType, GestureState.Error);
                Message.PushDesktop("手势错误，最少 4 个节点!", MessageBoxImage.Error, true);
                return;
            }

            if (pwd != _password)
            {
                SetGestureState(unlockType, GestureState.Error);
                Message.PushDesktop("手势错误，请重新解锁!", MessageBoxImage.Error, true);
                return;
            }

            SetGestureState(unlockType, GestureState.Success);
            Message.Push("手势正确!", MessageBoxImage.Information, true);
        }

        private void SetGestureState(GestureUnlockType unlockType, GestureState state)
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
            var pwd = e.OriginalSource.ToString();
            HandleGestureUnlock(pwd, GestureUnlockType.Unlock1);
        }

        public ICommand GestureCompletedCommand => new RelayCommand(param =>
        {
            var pwd = param.ToString();
            HandleGestureUnlock(pwd, GestureUnlockType.Unlock2);
        });
    }
}
