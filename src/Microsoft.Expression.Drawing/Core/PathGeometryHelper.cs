using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.Drawing.Core
{
    public static class PathGeometryHelper
    {
        public static PathGeometry ConvertToPathGeometry(string abbreviatedGeometry)
        {
            if (abbreviatedGeometry == null)
            {
                throw new ArgumentNullException("abbreviatedGeometry");
            }
            PathGeometry pathGeometry = new PathGeometry();
            pathGeometry.Figures = new PathFigureCollection();
            int num = 0;
            while (num < abbreviatedGeometry.Length && char.IsWhiteSpace(abbreviatedGeometry, num))
            {
                num++;
            }
            if (num < abbreviatedGeometry.Length && abbreviatedGeometry[num] == 'F')
            {
                num++;
                while (num < abbreviatedGeometry.Length && char.IsWhiteSpace(abbreviatedGeometry, num))
                {
                    num++;
                }
                if (num == abbreviatedGeometry.Length || (abbreviatedGeometry[num] != '0' && abbreviatedGeometry[num] != '1'))
                {
                    throw new FormatException();
                }
                pathGeometry.FillRule = ((abbreviatedGeometry[num] == '0') ? FillRule.EvenOdd : FillRule.Nonzero);
                num++;
            }
            new AbbreviatedGeometryParser(pathGeometry).Parse(abbreviatedGeometry, num);
            return pathGeometry;
        }

        public static PathGeometry AsPathGeometry(this Geometry original)
        {
            PathGeometry pathGeometry = original as PathGeometry;
            if (pathGeometry == null)
                pathGeometry = PathGeometry.CreateFromGeometry(original);

            return pathGeometry;
        }

        public static bool IsStroked(this PathSegment pathSegment) => pathSegment.IsStroked;
         
        public static bool IsSmoothJoin(this PathSegment pathSegment) => pathSegment.IsSmoothJoin;
 
        public static bool IsFrozen(this Geometry geometry) => geometry.IsFrozen;
         
        public static bool SyncPolylineGeometry(ref Geometry geometry, IList<Point> points, bool isClosed)
        {
            bool flag = false;
            PathGeometry pathGeometry = geometry as PathGeometry;
            PathFigure figure;
            if (pathGeometry == null || pathGeometry.Figures.Count != 1 || (figure = pathGeometry.Figures[0]) == null)
            {
                geometry = pathGeometry = new PathGeometry();
                pathGeometry.Figures.Add(figure = new PathFigure());
                flag = true;
            }
            return flag | PathFigureHelper.SyncPolylineFigure(figure, points, isClosed, true);
        }
 
        internal static Geometry FixPathGeometryBoundary(Geometry geometry) => geometry;
         
        private class AbbreviatedGeometryParser
        {
            public AbbreviatedGeometryParser(PathGeometry geometry) => this.geometry = geometry;

            public void Parse(string data, int startIndex)
            {
                buffer = data;
                length = data.Length;
                index = startIndex;
                bool flag = true;
                while (ReadToken())
                {
                    char c = token;
                    if (flag)
                    {
                        if (c != 'M' && c != 'm')
                        {
                            throw new FormatException();
                        }
                        flag = false;
                    }
                    char c2 = c;
                    if (c2 <= 'Z')
                    {
                        if (c2 <= 'M')
                        {
                            switch (c2)
                            {
                                case 'A':
                                    break;
                                case 'B':
                                    goto IL_364;
                                case 'C':
                                    goto IL_225;
                                default:
                                    if (c2 == 'H')
                                    {
                                        goto IL_193;
                                    }
                                    switch (c2)
                                    {
                                        case 'L':
                                            goto IL_165;
                                        case 'M':
                                            goto IL_11B;
                                        default:
                                            goto IL_364;
                                    }
                            }
                        }
                        else
                        {
                            switch (c2)
                            {
                                case 'Q':
                                    goto IL_2BD;
                                case 'R':
                                    goto IL_364;
                                case 'S':
                                    goto IL_276;
                                default:
                                    if (c2 == 'V')
                                    {
                                        goto IL_1DA;
                                    }
                                    if (c2 != 'Z')
                                    {
                                        goto IL_364;
                                    }
                                    goto IL_35B;
                            }
                        }
                    }
                    else if (c2 <= 'm')
                    {
                        switch (c2)
                        {
                            case 'a':
                                break;
                            case 'b':
                                goto IL_364;
                            case 'c':
                                goto IL_225;
                            default:
                                if (c2 == 'h')
                                {
                                    goto IL_193;
                                }
                                switch (c2)
                                {
                                    case 'l':
                                        goto IL_165;
                                    case 'm':
                                        goto IL_11B;
                                    default:
                                        goto IL_364;
                                }
                        }
                    }
                    else
                    {
                        switch (c2)
                        {
                            case 'q':
                                goto IL_2BD;
                            case 'r':
                                goto IL_364;
                            case 's':
                                goto IL_276;
                            default:
                                if (c2 == 'v')
                                {
                                    goto IL_1DA;
                                }
                                if (c2 != 'z')
                                {
                                    goto IL_364;
                                }
                                goto IL_35B;
                        }
                    }
                    do
                    {
                        Size size = ReadSize(false);
                        double rotationAngle = ReadDouble(true);
                        bool isLargeArc = ReadBool01(true);
                        SweepDirection sweepDirection = ReadBool01(true) ? SweepDirection.Clockwise : SweepDirection.Counterclockwise;
                        lastPoint = ReadPoint(c, true);
                        ArcTo(size, rotationAngle, isLargeArc, sweepDirection, lastPoint);
                    }
                    while (IsNumber(true));
                    EnsureFigure();
                    continue;
                IL_11B:
                    lastPoint = ReadPoint(c, false);
                    BeginFigure(lastPoint);
                    char command = 'M';
                    while (IsNumber(true))
                    {
                        lastPoint = ReadPoint(command, false);
                        LineTo(lastPoint);
                        command = 'L';
                    }
                    continue;
                IL_165:
                    EnsureFigure();
                    do
                    {
                        lastPoint = ReadPoint(c, false);
                        LineTo(lastPoint);
                    }
                    while (IsNumber(true));
                    continue;
                IL_193:
                    EnsureFigure();
                    do
                    {
                        double num = ReadDouble(false);
                        if (c == 'h')
                        {
                            num += lastPoint.X;
                        }
                        lastPoint.X = num;
                        LineTo(lastPoint);
                    }
                    while (IsNumber(true));
                    continue;
                IL_1DA:
                    EnsureFigure();
                    do
                    {
                        double num2 = ReadDouble(false);
                        if (c == 'v')
                        {
                            num2 += lastPoint.Y;
                        }
                        lastPoint.Y = num2;
                        LineTo(lastPoint);
                    }
                    while (IsNumber(true));
                    continue;
                IL_225:
                    EnsureFigure();
                    do
                    {
                        Point point = ReadPoint(c, false);
                        secondLastPoint = ReadPoint(c, true);
                        lastPoint = ReadPoint(c, true);
                        BezierTo(point, secondLastPoint, lastPoint);
                    }
                    while (IsNumber(true));
                    continue;
                IL_276:
                    EnsureFigure();
                    do
                    {
                        Point smoothBeizerFirstPoint = GetSmoothBeizerFirstPoint();
                        Point point2 = ReadPoint(c, false);
                        lastPoint = ReadPoint(c, true);
                        BezierTo(smoothBeizerFirstPoint, point2, lastPoint);
                    }
                    while (IsNumber(true));
                    continue;
                IL_2BD:
                    EnsureFigure();
                    do
                    {
                        Point point3 = ReadPoint(c, false);
                        lastPoint = ReadPoint(c, true);
                        QuadraticBezierTo(point3, lastPoint);
                    }
                    while (IsNumber(true));
                    continue;
                IL_35B:
                    FinishFigure(true);
                    continue;
                IL_364:
                    throw new NotSupportedException();
                }
                FinishFigure(false);
            }

            private bool ReadToken()
            {
                SkipWhitespace(false);
                if (index < length)
                {
                    token = buffer[index++];
                    return true;
                }
                return false;
            }

            private Point ReadPoint(char command, bool allowComma)
            {
                double num = ReadDouble(allowComma);
                double num2 = ReadDouble(true);
                if (command >= 'a')
                {
                    num += lastPoint.X;
                    num2 += lastPoint.Y;
                }
                return new Point(num, num2);
            }

            private Size ReadSize(bool allowComma)
            {
                double width = ReadDouble(allowComma);
                double height = ReadDouble(true);
                return new Size(width, height);
            }

            private bool ReadBool01(bool allowComma)
            {
                double num = ReadDouble(allowComma);
                if (num == 0.0)
                {
                    return false;
                }
                if (num == 1.0)
                {
                    return true;
                }
                throw new FormatException();
            }

            private double ReadDouble(bool allowComma)
            {
                if (!IsNumber(allowComma))
                {
                    throw new FormatException();
                }
                bool flag = true;
                int i = index;
                if (index < length && (buffer[index] == '-' || buffer[index] == '+'))
                {
                    index++;
                }
                if (index < length && buffer[index] == 'I')
                {
                    index = Math.Min(index + 8, length);
                    flag = false;
                }
                else if (index < length && buffer[index] == 'N')
                {
                    index = Math.Min(index + 3, length);
                    flag = false;
                }
                else
                {
                    SkipDigits(false);
                    if (index < length && buffer[index] == '.')
                    {
                        flag = false;
                        index++;
                        SkipDigits(false);
                    }
                    if (index < length && (buffer[index] == 'E' || buffer[index] == 'e'))
                    {
                        flag = false;
                        index++;
                        SkipDigits(true);
                    }
                }
                if (flag && index <= i + 8)
                {
                    int num = 1;
                    if (buffer[i] == '+')
                    {
                        i++;
                    }
                    else if (buffer[i] == '-')
                    {
                        i++;
                        num = -1;
                    }
                    int num2 = 0;
                    while (i < index)
                    {
                        num2 = num2 * 10 + (int)(buffer[i] - '0');
                        i++;
                    }
                    return (double)(num2 * num);
                }
                string value = buffer.Substring(i, index - i);
                double result;
                try
                {
                    result = Convert.ToDouble(value, CultureInfo.InvariantCulture);
                }
                catch (FormatException)
                {
                    throw new FormatException();
                }
                return result;
            }

            private void SkipDigits(bool signAllowed)
            {
                if (signAllowed && index < length && (buffer[index] == '-' || buffer[index] == '+'))
                {
                    index++;
                }
                while (index < length && buffer[index] >= '0' && buffer[index] <= '9')
                {
                    index++;
                }
            }

            private bool IsNumber(bool allowComma)
            {
                bool flag = SkipWhitespace(allowComma);
                if (index < length)
                {
                    token = buffer[index];
                    if (token == '.' || token == '-' || token == '+' || (token >= '0' && token <= '9') || token == 'I' || token == 'N')
                    {
                        return true;
                    }
                }
                if (flag)
                {
                    throw new FormatException();
                }
                return false;
            }

            private bool SkipWhitespace(bool allowComma)
            {
                bool result = false;
                while (index < length)
                {
                    char c = buffer[index];
                    char c2 = c;
                    switch (c2)
                    {
                        case '\t':
                        case '\n':
                        case '\r':
                            break;
                        case '\v':
                        case '\f':
                            goto IL_4F;
                        default:
                            if (c2 != ' ')
                            {
                                if (c2 != ',')
                                {
                                    goto IL_4F;
                                }
                                if (!allowComma)
                                {
                                    throw new FormatException();
                                }
                                result = true;
                                allowComma = false;
                            }
                            break;
                    }
                IL_65:
                    index++;
                    continue;
                IL_4F:
                    if (c > ' ' && c <= 'z')
                    {
                        return result;
                    }
                    if (!char.IsWhiteSpace(c))
                    {
                        return result;
                    }
                    goto IL_65;
                }
                return false;
            }

            private void BeginFigure(Point startPoint)
            {
                FinishFigure(false);
                EnsureFigure();
                figure.StartPoint = startPoint;
                figure.IsFilled = true;
            }

            private void EnsureFigure()
            {
                if (figure == null)
                {
                    figure = new PathFigure();
                    figure.Segments = new PathSegmentCollection();
                }
            }

            private void FinishFigure(bool figureExplicitlyClosed)
            {
                if (figure != null)
                {
                    if (figureExplicitlyClosed)
                    {
                        figure.IsClosed = true;
                    }
                    geometry.Figures.Add(figure);
                    figure = null;
                }
            }

            private void LineTo(Point point)
            {
                LineSegment lineSegment = new LineSegment();
                lineSegment.Point = point;
                figure.Segments.Add(lineSegment);
            }

            private void BezierTo(Point point1, Point point2, Point point3)
            {
                BezierSegment bezierSegment = new BezierSegment();
                bezierSegment.Point1 = point1;
                bezierSegment.Point2 = point2;
                bezierSegment.Point3 = point3;
                figure.Segments.Add(bezierSegment);
            }

            private void QuadraticBezierTo(Point point1, Point point2)
            {
                QuadraticBezierSegment quadraticBezierSegment = new QuadraticBezierSegment();
                quadraticBezierSegment.Point1 = point1;
                quadraticBezierSegment.Point2 = point2;
                figure.Segments.Add(quadraticBezierSegment);
            } 

            private void ArcTo(Size size, double rotationAngle, bool isLargeArc, SweepDirection sweepDirection, Point point)
            {
                ArcSegment arcSegment = new ArcSegment();
                arcSegment.Size = size;
                arcSegment.RotationAngle = rotationAngle;
                arcSegment.IsLargeArc = isLargeArc;
                arcSegment.SweepDirection = sweepDirection;
                arcSegment.Point = point;
                figure.Segments.Add(arcSegment);
            }

            private Point GetSmoothBeizerFirstPoint()
            {
                Point result = lastPoint;
                if (figure.Segments.Count > 0)
                {
                    BezierSegment bezierSegment = figure.Segments[figure.Segments.Count - 1] as BezierSegment;
                    if (bezierSegment != null)
                    {
                        Point point = bezierSegment.Point2;
                        result.X += lastPoint.X - point.X;
                        result.Y += lastPoint.Y - point.Y;
                    }
                }
                return result;
            }

            private PathGeometry geometry;
            private PathFigure figure;
            private Point lastPoint;
            private Point secondLastPoint;
            private string buffer;
            private int index;
            private int length;
            private char token;
        }
    }
}
