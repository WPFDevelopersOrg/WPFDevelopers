using Microsoft.Expression.Drawing.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.Media
{
    public sealed class SketchGeometryEffect : GeometryEffect
    {
        protected override GeometryEffect DeepCopy()
        {
            return new SketchGeometryEffect();
        }

        public override bool Equals(GeometryEffect geometryEffect)
        {
            return geometryEffect is SketchGeometryEffect;
        }

        protected override bool UpdateCachedGeometry(Geometry input)
        {
            bool flag = false;
            PathGeometry pathGeometry = input.AsPathGeometry();
            if (pathGeometry != null)
            {
                flag |= UpdateSketchGeometry(pathGeometry);
            }
            else
            {
                cachedGeometry = input;
            }
            return flag;
        }

        private bool UpdateSketchGeometry(PathGeometry inputPath)
        {
            bool flag = false;
            PathGeometry pathGeometry;
            flag |= GeometryHelper.EnsureGeometryType<PathGeometry>(out pathGeometry, ref cachedGeometry, () => new PathGeometry());
            flag |= pathGeometry.Figures.EnsureListCount(inputPath.Figures.Count, () => new PathFigure());
            RandomEngine random = new RandomEngine(randomSeed);
            for (int i = 0; i < inputPath.Figures.Count; i++)
            {
                PathFigure pathFigure = inputPath.Figures[i];
                bool isClosed = pathFigure.IsClosed;
                bool isFilled = pathFigure.IsFilled;
                if (pathFigure.Segments.Count == 0)
                {
                    flag |= pathGeometry.Figures[i].SetIfDifferent(PathFigure.StartPointProperty, pathFigure.StartPoint);
                    flag |= pathGeometry.Figures[i].Segments.EnsureListCount(0, null);
                }
                else
                {
                    List<Point> list = new List<Point>(pathFigure.Segments.Count * 3);
                    foreach (SimpleSegment simpleSegment in GetEffectiveSegments(pathFigure))
                    {
                        List<Point> list2 = new List<Point>();
                        list2.Add(simpleSegment.Points[0]);
                        SimpleSegment simpleSegment2 = simpleSegment;
                        double tolerance = 0.0;
                        IList<double> resultParameters = null;
                        simpleSegment2.Flatten(list2, tolerance, resultParameters);
                        PolylineData polyline = new PolylineData(list2);
                        if (list2.Count > 1 && polyline.TotalLength > 4.0)
                        {
                            double a = polyline.TotalLength / 8.0;
                            int sampleCount = (int)Math.Max(2.0, Math.Ceiling(a));
                            double interval = polyline.TotalLength / (double)sampleCount;
                            double scale = interval / 8.0;
                            List<Point> samplePoints = new List<Point>(sampleCount);
                            List<Vector> sampleNormals = new List<Vector>(sampleCount);
                            int sampleIndex = 0;
                            PolylineHelper.PathMarch(polyline, 0.0, 0.0, delegate (MarchLocation location)
                            {
                                if (location.Reason == MarchStopReason.CompletePolyline)
                                {
                                    return double.NaN;
                                }
                                if (location.Reason != MarchStopReason.CompleteStep)
                                {
                                    return location.Remain;
                                }
                                if (sampleIndex++ == sampleCount)
                                {
                                    return double.NaN;
                                }
                                samplePoints.Add(location.GetPoint(polyline.Points));
                                sampleNormals.Add(location.GetNormal(polyline, 0.0));
                                return interval;
                            });
                            DisturbPoints(random, scale, samplePoints, sampleNormals);
                            list.AddRange(samplePoints);
                        }
                        else
                        {
                            list.AddRange(list2);
                            list.RemoveLast<Point>();
                        }
                    }
                    if (!isClosed)
                    {
                        list.Add(pathFigure.Segments.Last<PathSegment>().GetLastPoint());
                    }
                    flag |= PathFigureHelper.SyncPolylineFigure(pathGeometry.Figures[i], list, isClosed, isFilled);
                }
            }
            if (flag)
            {
                cachedGeometry = PathGeometryHelper.FixPathGeometryBoundary(cachedGeometry);
            }
            return flag;
        }

        private static void DisturbPoints(RandomEngine random, double scale, IList<Point> points, IList<Vector> normals)
        {
            int count = points.Count;
            for (int i = 1; i < count; i++)
            {
                double num = random.NextGaussian(0.0, 1.0 * scale);
                double num2 = random.NextUniform(-0.5, 0.5) * scale;
                points[i] = new Point(points[i].X + normals[i].X * num2 - normals[i].Y * num, points[i].Y + normals[i].X * num + normals[i].Y * num2);
            }
        }

        private IEnumerable<SimpleSegment> GetEffectiveSegments(PathFigure pathFigure)
        {
            Point lastPoint = pathFigure.StartPoint;
            foreach (PathSegmentData data in pathFigure.AllSegments())
            {
                foreach (SimpleSegment segment in data.PathSegment.GetSimpleSegments(data.StartPoint))
                {
                    yield return segment;
                    lastPoint = segment.Points.Last<Point>();
                }
            }
            if (pathFigure.IsClosed)
            {
                yield return SimpleSegment.Create(lastPoint, pathFigure.StartPoint);
            }
            yield break;
        }

        private const double expectedLengthMean = 8.0;

        private const double normalDisturbVariance = 0.5;

        private const double tangentDisturbVariance = 1.0;

        private const double bsplineWeight = 0.05;

        private readonly long randomSeed = DateTime.Now.Ticks;
    }
}
