using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using WPFDevelopers.Sample.Models;

namespace WPFDevelopers.Samples.ExampleViews.Basics
{
    /// <summary>
    /// Shared data helper for Basic control examples.
    /// Provides UserCollection, ContactMethods, and AllSelected logic.
    /// </summary>
    public static class BasicDataHelper
    {
        public static List<string> ContactMethods { get; } = new List<string> { "Tel", "Fax", "MB" };

        public static ObservableCollection<UserModel> CreateUserCollection()
        {
            var time = DateTime.Now;
            var collection = new ObservableCollection<UserModel>();
            for (var i = 0; i < 40; i++)
            {
                collection.Add(new UserModel
                {
                    Date = time,
                    Name = "WPFDevelopers",
                    Address = "One Microsoft Way, Redmond",
                    ContactMethod = "MB",
                    Children = new List<UserModel>
                    {
                        new UserModel { Name = "WPFDevelopers1.1", Children = new List<UserModel>
                        {
                            new UserModel { Name = "WPFDevelopers1.1.1" }
                        }},
                        new UserModel { Name = "WPFDevelopers1.2" },
                        new UserModel { Name = "WPFDevelopers1.3" },
                        new UserModel { Name = "WPFDevelopers1.4" },
                        new UserModel { Name = "WPFDevelopers1.5" },
                        new UserModel { Name = "WPFDevelopers1.6" }
                    }
                });
                time = time.AddDays(2);
            }
            return collection;
        }

        public static void ApplyAllSelected(ObservableCollection<UserModel> collection, bool isChecked)
        {
            collection?.ToList().ForEach(y => y.IsChecked = isChecked);
        }
    }
}
