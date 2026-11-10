using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using WPFDevelopers.Controls;
using WPFDevelopers.Samples.ViewModels;

namespace WPFDevelopers.Samples.ExampleViews
{
    public partial class DialogExample : UserControl
    {
        public DialogExample()
        {
            InitializeComponent();
        }

        private void OpenUserControl_Click(object sender, RoutedEventArgs e)
        {
            var model = new DialogExampleVM();
            var diaUser = new DialogUserControl();
            diaUser.DataContext = model;
            var dialog = new Dialog
            {
                Title = "编辑",
                Content = diaUser,
                Owner = Window.GetWindow(this),
                DialogWidth = 400
            };
            dialog.SetBinding(Dialog.IsConfirmEnabledProperty, new Binding("CanConfirm")
            {
                Source = model
            });
            dialog.Closing += diaUser.OnDialogClosing;
            dialog.ShowAsync().ContinueWith(task =>
            {
                var result = task.Result == DialogResult.Confirmed;
                ResultText.Text = result ? "Confirmed: " + $"name:{model.Name} - email:{model.Email}" : "Canceled";
                if(result)
                    Toast.Push("修改成功",ToastImage.Success);
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }
    }
}
