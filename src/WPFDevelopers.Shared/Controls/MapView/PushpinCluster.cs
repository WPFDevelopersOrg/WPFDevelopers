using System;
using System.Collections.Generic;
using System.Globalization;

namespace WPFDevelopers.Controls
{
    public sealed class PushpinCluster
    {
        public PushpinCluster(double x, double y, Pushpin representative)
        {
            X = x;
            Y = y;
            Count = 1;
            Representative = representative;
            Items = new List<Pushpin>();
            if (representative != null)
            {
                Items.Add(representative);
            }
        }

        public double X { get; set; }

        public double Y { get; set; }

        public int Count { get; set; }

        public Pushpin Representative { get; private set; }

        public List<Pushpin> Items { get; private set; }

        public string CountText
        {
            get
            {
                return Count > 99 ? "99+" : Count.ToString(CultureInfo.InvariantCulture);
            }
        }

        public string DisplayText
        {
            get { return CountText; }
        }
    }
}
