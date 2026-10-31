using System;
using System.Collections.ObjectModel;

namespace WPFDevelopers.Samples.ViewModels
{
    public class WaterfallPanelExampleVM : ViewModelBase
    {
        public ObservableCollection<WaterfallPanelItem> Items { get; private set; }
        public ObservableCollection<WaterfallDemoItem> LargeItems { get; private set; }

        public WaterfallPanelExampleVM()
        {
            Items = new ObservableCollection<WaterfallPanelItem>
            {
                new WaterfallPanelItem("https://images.unsplash.com/photo-1510001618818-4b4e3d86bf0f?w=523&auto=format"),
                new WaterfallPanelItem("https://images.unsplash.com/photo-1507513319174-e556268bb244?w=523&auto=format"),
                new WaterfallPanelItem("https://images.unsplash.com/photo-1474181487882-5abf3f0ba6c2?w=523&auto=format"),
                new WaterfallPanelItem("https://images.unsplash.com/photo-1492778297155-7be4c83960c7?w=523&auto=format"),
                new WaterfallPanelItem("https://images.unsplash.com/photo-1508062878650-88b52897f298?w=523&auto=format"),
                new WaterfallPanelItem("https://images.unsplash.com/photo-1506158278516-d720e72406fc?w=523&auto=format"),
                new WaterfallPanelItem("https://images.unsplash.com/photo-1552203274-e3c7bd771d26?w=523&auto=format"),
                new WaterfallPanelItem("https://images.unsplash.com/photo-1528163186890-de9b86b54b51?w=523&auto=format"),
                new WaterfallPanelItem("https://images.unsplash.com/photo-1727423304224-6d2fd99b864c?w=523&auto=format"),
                new WaterfallPanelItem("https://images.unsplash.com/photo-1675090391405-432434e23595?w=523&auto=format"),
                new WaterfallPanelItem("https://images.unsplash.com/photo-1554196967-97a8602084d9?w=523&auto=format"),
                new WaterfallPanelItem("https://images.unsplash.com/photo-1491961865842-98f7befd1a60?w=523&auto=format"),
                new WaterfallPanelItem("https://images.unsplash.com/photo-1721728613411-d56d2ddda959?w=523&auto=format"),
                new WaterfallPanelItem("https://images.unsplash.com/photo-1731901245099-20ac7f85dbaa?w=523&auto=format"),
                new WaterfallPanelItem("https://images.unsplash.com/photo-1617694455303-59af55af7e58?w=523&auto=format"),
                new WaterfallPanelItem("https://images.unsplash.com/photo-1709198165282-1dab551df890?w=523&auto=format")
            };

            LargeItems = new ObservableCollection<WaterfallDemoItem>();
            for (var index = 0; index < 5000; index++)
            {
                LargeItems.Add(new WaterfallDemoItem
                {
                    Title = "Item " + (index + 1),
                    Height = 100 + (index * 37 % 220)
                });
            }
        }
    }

    public class WaterfallDemoItem
    {
        public string Title { get; set; }
        public double Height { get; set; }
    }

    public class WaterfallPanelItem
    {
        public Uri ImageUri { get; private set; }

        public WaterfallPanelItem(string imageUrl)
        {
            ImageUri = new Uri(imageUrl, UriKind.Absolute);
        }
    }
}
