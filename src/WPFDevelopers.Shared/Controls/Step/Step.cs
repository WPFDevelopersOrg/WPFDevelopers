using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace WPFDevelopers.Controls
{
    [TemplatePart(Name = ProgressBarTemplateName, Type = typeof(ProgressBar))]
    public class Step : ItemsControl
    {
        private const string ProgressBarTemplateName = "PART_ProgressBar";
        private ProgressBar _progressBar;
        public int StepIndex
        {
            get => (int)GetValue(StepIndexProperty);
            set => SetValue(StepIndexProperty, value);
        }

        public static readonly DependencyProperty StepIndexProperty = DependencyProperty.Register(
           "StepIndex", typeof(int), typeof(Step), new PropertyMetadata(0, OnStepIndexChanged));

        private static void OnStepIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var step = (Step)d;
            var stepIndex = (int)e.NewValue;
            step.UpdateStepItemState(stepIndex);
        }
        void UpdateStepItemState(int stepIndex)
        {
            var count = Items.Count;
            if (count <= 0) return;
            if (stepIndex >= count)
            {
                StepIndex--;
                return;
            }
            if (stepIndex < 0)
            {
                StepIndex++;
                return;
            }
            for (var i = 0; i < stepIndex; i++)
            {
                if (ItemContainerGenerator.ContainerFromIndex(i) is StepItem stepItem)
                    stepItem.Status = Status.Complete;
            }

            if (ItemContainerGenerator.ContainerFromIndex(stepIndex) is StepItem itemInProgress)
                itemInProgress.Status = Status.InProgress;
            for (var i = stepIndex + 1; i < Items.Count; i++)
            {
                if (ItemContainerGenerator.ContainerFromIndex(i) is StepItem stepItem)
                    stepItem.Status = Status.Waiting;
            }
        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _progressBar = GetTemplateChild(ProgressBarTemplateName) as ProgressBar;
        }
        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            var count = Items.Count;
            if (_progressBar == null || count <= 0) return;
            _progressBar.Maximum = count - 1;
            _progressBar.Value = StepIndex;
            _progressBar.Width = (ActualWidth / count) * (count - 1);


        }
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is StepItem;
        }
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new StepItem();
        }
        public Step()
        {
            ItemContainerGenerator.StatusChanged += ItemContainerGenerator_StatusChanged;
        }

        public void Next()
        {
            StepIndex++;
        }
        public void Previous()
        {
            StepIndex--;
        }
        private void ItemContainerGenerator_StatusChanged(object sender, EventArgs e)
        {
            if (ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
            {
                var count = Items.Count;
                if (count <= 0) return;
                UpdateStepItemState(StepIndex);
            }
        }
    }
}
