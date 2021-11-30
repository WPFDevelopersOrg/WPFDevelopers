using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace WPFDevelopers.Samples.ViewModels
{
    public class DrawingModel : ViewModelBase
    {
        private int _Index = 0;
        public int Index
        {
            get => _Index;
            set => SetProperty(ref _Index, value);
        }

        private string _Number = default;
        public string Number
        {
            get => _Number;
            set => SetProperty(ref _Number, value); 
        }


    }

    public class DrawingExampleVM : ViewModelBase
    {
        public DrawingExampleVM()
        {
            for (int i = 0; i < 10; ++i)
            {
                DrawingModel drawingModel = new DrawingModel()
                {
                    Index = i,
                    Number = (i + 1).ToString(),
                };

                Drawings.Add(drawingModel);
            }
        }


        public ObservableCollection<DrawingModel> Drawings { get; } = new ObservableCollection<DrawingModel>();



    }
}
