using System;
using System.Windows;
using System.Windows.Controls;

namespace WPFDevelopers.Controls
{
    public abstract class LoadingBase : ContentControl
    {
        public bool IsLoading
        {
            get { return (bool)GetValue(IsLoadingProperty); }
            set { SetValue(IsLoadingProperty, value); }
        }

        public static readonly DependencyProperty IsLoadingProperty =
            DependencyProperty.Register("IsLoading", typeof(bool), typeof(LoadingBase), new FrameworkPropertyMetadata(
                true,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                OnIsLoadingChanged,
                CoerceIsLoading));

        private static void OnIsLoadingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = (LoadingBase)d;
            ctrl.OnIsLoadingChanged((bool)e.OldValue, (bool)e.NewValue);
        }
        private static object CoerceIsLoading(DependencyObject d, object baseValue)
        {
            return baseValue;
        }
        protected virtual void OnIsLoadingChanged(bool oldValue, bool newValue)
        {
        }
    }
}
