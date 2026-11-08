using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using WPFDevelopers.Controls;
using WPFDevelopers.Samples.ExampleViews;
using WPFDevelopers.Samples.Helpers;

namespace WPFDevelopers.Samples.ViewModels
{
    public class DialogExampleVM : ViewModelBase
    {
        private string _name;
        private string _email;
        private bool _isEnable;
        private string _resultText;

        public DialogExampleVM()
        {
            OpenDialogCommand = new RelayCommand(parameter =>
            {
                if (!(parameter is DependencyObject view))
                    throw new ArgumentException("The command parameter must be the dialog's owner view.", nameof(parameter));

                Name = string.Empty;
                Email = string.Empty;
                IsEnable = false;
                var diaUser = new DialogUserControl();
                diaUser.DataContext = this;
                var dialog = new Dialog
                {
                    Title = "编辑",
                    Content = diaUser,
                    Owner = Window.GetWindow(view),
                    DialogWidth = 440
                };
                dialog.SetBinding(Dialog.IsConfirmEnabledProperty, new Binding("CanConfirm")
                {
                    Source = this
                });
                dialog.ClosedCommand = DialogClosedCommand;
                dialog.ShowAsync();
            });
            DialogClosedCommand = new RelayCommand(parameter =>
            {
                var result = (DialogResult)parameter == DialogResult.Confirmed;
                ResultText = result ? "Confirmed: " + $"name:{Name} - email:{Email}" : "Canceled";
                if (result)
                    Toast.Push("修改成功", ToastImage.Success);
            });
        }

        public string Name
        {
            get => _name;
            set
            {
                if (SetProperty(ref _name, value, nameof(Name)))
                    NotifyPropertyChange(nameof(CanConfirm));
            }
        }
        public string Email
        {
            get => _email;
            set
            {
                if (SetProperty(ref _email, value, nameof(Email)))
                    NotifyPropertyChange(nameof(CanConfirm));
            }
        }
        public bool IsEnable
        {
            get => _isEnable;
            set
            {
                if (SetProperty(ref _isEnable, value, nameof(IsEnable)))
                    NotifyPropertyChange(nameof(CanConfirm));
            }
        }

        public bool CanConfirm => !string.IsNullOrWhiteSpace(Name) && !string.IsNullOrWhiteSpace(Email);

        public string ResultText
        {
            get => _resultText;
            private set => SetProperty(ref _resultText, value, nameof(ResultText));
        }

        public ICommand OpenDialogCommand { get; private set; }
        public ICommand DialogClosedCommand { get; private set; }
    }
}
