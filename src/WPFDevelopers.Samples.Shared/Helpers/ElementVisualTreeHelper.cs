using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using MessageBox = WPFDevelopers.Controls.MessageBox;

namespace WPFDevelopers.Samples.Helpers
{
    public class ElementVisualTreeHelper
    {
        /// <summary>
        ///     利用visualtreehelper寻找对象的子级对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static List<T> FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
        {
            try
            {
                var TList = new List<T>();
                for (var i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
                {
                    var child = VisualTreeHelper.GetChild(obj, i);
                    if (child != null && child is T)
                    {
                        TList.Add((T)child);
                        var childOfChildren = FindVisualChild<T>(child);
                        if (childOfChildren != null) TList.AddRange(childOfChildren);
                    }
                    else
                    {
                        var childOfChildren = FindVisualChild<T>(child);
                        if (childOfChildren != null) TList.AddRange(childOfChildren);
                    }
                }

                return TList;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }
    }
}