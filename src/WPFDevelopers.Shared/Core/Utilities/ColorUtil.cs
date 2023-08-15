using System;
using System.Windows.Media;
using WPFDevelopers.Controls;

namespace WPFDevelopers.Utilities
{
    public static class ColorUtil
    {
        public static Color ConvertHSLToColor(Color color, double h = double.NaN, double sl = double.NaN,
            double l = double.NaN)
        {
            var hsl = RgbToHSL(color);
            if (!double.IsNaN(h))
                hsl.H = h;
            if (!double.IsNaN(sl))
                hsl.S = sl;
            if (!double.IsNaN(l))
                hsl.L = l;
            var rgb = HSLToRgb(hsl);
            return rgb;
        }

        private static Color HSLToRgb(HSL hslColor)
        {
            var rgbColor = new Color();
            if (hslColor.S == 0)
            {
                rgbColor.R = (byte) (hslColor.L * 255);
                rgbColor.G = (byte) (hslColor.L * 255);
                rgbColor.B = (byte) (hslColor.L * 255);
                rgbColor.A = (byte) (hslColor.A * 255);
                return rgbColor;
            }

            double t1;
            if (hslColor.L < 0.5)
                t1 = hslColor.L * (1.0 + hslColor.S);
            else
                t1 = hslColor.L + hslColor.S - hslColor.L * hslColor.S;

            var t2 = 2.0 * hslColor.L - t1;

            var h = hslColor.H / 360;

            var tR = h + 1.0 / 3.0;
            var r = SetColor(t1, t2, tR);

            var tG = h;
            var g = SetColor(t1, t2, tG);

            var tB = h - 1.0 / 3.0;
            var b = SetColor(t1, t2, tB);

            rgbColor.R = (byte) (r * 255);
            rgbColor.G = (byte) (g * 255);
            rgbColor.B = (byte) (b * 255);
            rgbColor.A = (byte) (hslColor.A * 255);

            return rgbColor;
        }

        private static double SetColor(double t1, double t2, double t3)
        {
            if (t3 < 0) t3 += 1.0;
            if (t3 > 1) t3 -= 1.0;

            double color;
            if (6.0 * t3 < 1)
                color = t2 + (t1 - t2) * 6.0 * t3;
            else if (2.0 * t3 < 1)
                color = t1;
            else if (3.0 * t3 < 2)
                color = t2 + (t1 - t2) * (2.0 / 3.0 - t3) * 6.0;
            else
                color = t2;
            return color;
        }

        public static HSL RgbToHSL(Color rgbColor)
        {
            var hslColor = new HSL();
            var r = (double) rgbColor.R / 255;
            var g = (double) rgbColor.G / 255;
            var b = (double) rgbColor.B / 255;
            var a = (double) rgbColor.A / 255;

            var min = Math.Min(r, Math.Min(g, b));
            var max = Math.Max(r, Math.Max(g, b));
            var delta = max - min;


            if (max == min)
            {
                hslColor.H = 0;
                hslColor.S = 0;
                hslColor.L = max;
                return hslColor;
            }

            hslColor.L = (min + max) / 2;

            if (hslColor.L < 0.5)
                hslColor.S = delta / (max + min);
            else
                hslColor.S = delta / (2.0 - max - min);

            if (r == max) hslColor.H = (g - b) / delta;
            if (g == max) hslColor.H = 2.0 + (b - r) / delta;
            if (b == max) hslColor.H = 4.0 + (r - g) / delta;
            hslColor.H *= 60;
            if (hslColor.H < 0) hslColor.H += 360;

            hslColor.A = a;

            return hslColor;
        }
       
        public static void HsbFromColor(Color C, ref double H, ref double S, ref double B)
        {
            var r = C.R / 255d;
            var g = C.G / 255d;
            var b = C.B / 255d;

            var max = Math.Max(Math.Max(r, g), b);
            var min = Math.Min(Math.Min(r, g), b);
            var delta = max - min;

            var hue = 0d;
            var saturation = DoubleUtil.GreaterThan(max, 0) ? delta / max : 0.0;
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
            r.A = (byte) Math.Round(a * 255d);

            return r;
        }

        public static Color ColorFromHsb(double H, double S, double B)
        {
            double red = 0.0, green = 0.0, blue = 0.0;

            if (DoubleUtil.IsZero(S))
            {
                red = green = blue = B;
            }
            else
            {
                var h = DoubleUtil.IsOne(H) ? 0d : H * 6.0;
                var i = (int) Math.Floor(h);

                var f = h - i;
                var r = B * (1.0 - S);
                var s = B * (1.0 - S * f);
                var t = B * (1.0 - S * (1.0 - f));

                switch (i)
                {
                    case 0:
                        red = B;
                        green = t;
                        blue = r;
                        break;
                    case 1:
                        red = s;
                        green = B;
                        blue = r;
                        break;
                    case 2:
                        red = r;
                        green = B;
                        blue = t;
                        break;
                    case 3:
                        red = r;
                        green = s;
                        blue = B;
                        break;
                    case 4:
                        red = t;
                        green = r;
                        blue = B;
                        break;
                    case 5:
                        red = B;
                        green = r;
                        blue = s;
                        break;
                }
            }

            return Color.FromRgb((byte) Math.Round(red * 255.0), (byte) Math.Round(green * 255.0),
                (byte) Math.Round(blue * 255.0));
        }
    }

}