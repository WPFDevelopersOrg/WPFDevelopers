using System;
using System.Collections.Generic;
using WPFDevelopers.Helpers;

namespace WPFDevelopers.Sample.Models
{
    public class UserModel : ViewModelBase
    {
        private bool _isChecked;

        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                _isChecked = value;
                NotifyPropertyChange("IsChecked");
            }
        }

        public DateTime Date { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public List<UserModel> Children { get; set; }
    }
}