using System;
using System.Windows.Forms;
using System.Windows.Media;

namespace WPFDevelopers.Utilities
{
    public static class ColorUtil
    {
        public static Color ConvertHSLToColor(Color color, double hValue = double.NaN, double sValue = double.NaN, double lValue = double.NaN)
        {
            double hue = hValue;
            if (double.IsNaN(hue))
                hue = ColorFromH(color) % 360;
            double saturation = sValue;
            if (double.IsNaN(saturation))
            {
                saturation = ColorFromS(color);
                saturation = saturation / 100 ;
            }
            double lightness = lValue;
            if (double.IsNaN(lightness))
            {
                lightness = ColorFromL(color);
                lightness = lightness / 100;
            }

            //double max = Math.Max(Math.Max(r, g), b);
            //double min = Math.Min(Math.Min(r, g), b);
            //double delta = max - min;

            //int h = 0;
            //if (delta != 0)
            //{
            //    if (max == r)
            //        h = (int)(60 * (((g - b) / delta) % 6));
            //    else if (max == g)
            //        h = (int)(60 * (((b - r) / delta) + 2));
            //    else if (max == b)
            //        h = (int)(60 * (((r - g) / delta) + 4));
            //}

            //int s = max == 0 ? 0 : (int)(delta / max * 100);
            //int l = (int)((max + min) / 2 / 255 * 100);

            double chroma = (1 - Math.Abs(2 * lightness - 1)) * saturation;
            double huePrime = hue / 60.0;
            double x = chroma * (1 - Math.Abs(huePrime % 2 - 1));
            double red = 0, green = 0, blue = 0;
            if (huePrime >= 0 && huePrime < 1)
            {
                red = chroma;
                green = x;
            }
            else if (huePrime >= 1 && huePrime < 2)
            {
                red = x;
                green = chroma;
            }
            else if (huePrime >= 2 && huePrime < 3)
            {
                green = chroma;
                blue = x;
            }
            else if (huePrime >= 3 && huePrime < 4)
            {
                green = x;
                blue = chroma;
            }
            else if (huePrime >= 4 && huePrime < 5)
            {
                red = x;
                blue = chroma;
            }
            else if (huePrime >= 5 && huePrime < 6)
            {
                red = chroma;
                blue = x;
            }
            double m = lightness - chroma / 2;
            byte r = (byte)((red + m) * 255);
            byte g = (byte)((green + m) * 255);
            byte b = (byte)((blue + m) * 255);
            return Color.FromRgb(r, g, b);
        }

        public static void HsbFromColor(Color C, ref double H, ref double S, ref double B)
        {
            double r = C.R / 255d;
            double g = C.G / 255d;
            double b = C.B / 255d;

            var max = Math.Max(Math.Max(r, g), b);
            var min = Math.Min(Math.Min(r, g), b);
            var delta = max - min;

            var hue = 0d;
            var saturation = DoubleUtil.GreaterThan(max, 0) ? (delta / max) : 0.0;
            var brightness = max;

            if (!DoubleUtil.IsZero(delta))
            {
                if (DoubleUtil.AreClose(r, max))
                    hue = (g - b) / delta;
                else if (DoubleUtil.AreClose(g, max))
                    hue = 2 + (b - r) / delta;
                else if (DoubleUtil.AreClose(b, max))
                    hue = 4 + (r - g) / delta;

                hue = hue * 60;
                if (DoubleUtil.LessThan(hue, 0d))
                    hue += 360;
            }

            H = hue / 360d;
            S = saturation;
            B = brightness;
        }

        public static Color ColorFromAhsb(double a, double h, double s, double b)
        {
            var r = ColorFromHsb(h, s, b);
            r.A = (byte)Math.Round(a * 255d);

            return r;
        }

        public static Color ColorFromHsb(double H, double S, double B)
        {
            double red = 0.0, green = 0.0, blue = 0.0;

            if (DoubleUtil.IsZero(S))
                red = green = blue = B;
            else
            {
                var h = DoubleUtil.IsOne(H) ? 0d : (H * 6.0);
                int i = (int)Math.Floor(h);

                var f = h - i;
                var r = B * (1.0 - S);
                var s = B * (1.0 - S * f);
                var t = B * (1.0 - S * (1.0 - f));

                switch (i)
                {
                    case 0: red = B; green = t; blue = r; break;
                    case 1: red = s; green = B; blue = r; break;
                    case 2: red = r; green = B; blue = t; break;
                    case 3: red = r; green = s; blue = B; break;
                    case 4: red = t; green = r; blue = B; break;
                    case 5: red = B; green = r; blue = s; break;
                }
            }

            return Color.FromRgb((byte)Math.Round(red * 255.0), (byte)Math.Round(green * 255.0), (byte)Math.Round(blue * 255.0));
        }

        public static int ColorFromH(Color color)
        {
            double r = color.R / 255.0;
            double g = color.G / 255.0;
            double b = color.B / 255.0;

            double max = Math.Max(r, Math.Max(g, b));
            double min = Math.Min(r, Math.Min(g, b));

            double hue = 0;

            if (max == min)
            {
                hue = 0;
            }
            else if (max == r)
            {
                hue = ((g - b) / (max - min)) % 6;
            }
            else if (max == g)
            {
                hue = ((b - r) / (max - min)) + 2;
            }
            else if (max == b)
            {
                hue = ((r - g) / (max - min)) + 4;
            }

            hue *= 60;

            if (hue < 0)
            {
                hue += 360;
            }
            return (int)hue;
        }

        public static int ColorFromS(Color color)
        {
            double r = color.R / 255.0;
            double g = color.G / 255.0;
            double b = color.B / 255.0;
            double max = Math.Max(r, Math.Max(g, b));
            double min = Math.Min(r, Math.Min(g, b));
            double saturation = max == 0 ? 0 : (max - min) / max;
            return (int)(saturation * 100);
        }

        public static int ColorFromL(Color color)
        {
            double r = color.R / 255.0;
            double g = color.G / 255.0;
            double b = color.B / 255.0;

            double max = Math.Max(r, Math.Max(g, b));
            double min = Math.Min(r, Math.Min(g, b));

            double lightness = (max + min) / 2;

            return (int)(lightness * 100);
        }

    }
}
