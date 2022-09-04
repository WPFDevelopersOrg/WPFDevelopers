using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace WPFDevelopers.Samples.ExampleViews.CanvasHandWriting
{
    public class LineB
    {
        private List<Vector2D> _vector2DList = new List<Vector2D>();
        public List<Vector2D> Vector2DList
        {
            get { return _vector2DList; }
            set
            {
                _vector2DList = value;
            }
        }

        private List<BezierCurve> _bezierCurveList = new List<BezierCurve>();
        public List<BezierCurve> BezierCurveList
        {
            get { return _bezierCurveList; }
            private set { _bezierCurveList = value; }
        }


        private double _tension = 0.618;
        public double Tension
        {
            get { return _tension; }
            set
            {
                _tension = value;
                if (_tension > 10)
                    _tension = 10;
                if (_tension < 0)
                    _tension = 0;
            }
        }

        private bool _isClosedCurve = true;
        public bool IsClosedCurve
        {
            get { return _isClosedCurve; }
            set { _isClosedCurve = value; }
        }

        private string _pathData = string.Empty;
        public string PathData
        {
            get
            {
                if (_pathData == string.Empty)
                {
                    _pathData = Vector2DToBezierCurve();
                }
                return _pathData;
            }
        }

        private string Vector2DToBezierCurve() 
        {
            if (Vector2DList.Count < 3)
                return string.Empty;

            BezierCurveList.Clear();
            for (int i = 0; i < Vector2DList.Count; i++)
            {
                int pointTwoIndex = i + 1 < Vector2DList.Count ? i + 1 : 0;
                int pointThreeIndex = i + 2 < Vector2DList.Count ? i + 2 : i + 2 - Vector2DList.Count;
                Vector2D vector2D1 = Vector2DList[i];
                Vector2D vector2D2 = Vector2DList[pointTwoIndex];
                Vector2D vector2D3 = Vector2DList[pointThreeIndex];

                Vector2D startVector2D = Vector2D.CalculateVectorCenter(vector2D1, vector2D2);
                double startAngle = Vector2D.IncludedAngleXAxis(vector2D1, vector2D2);
                double startDistance = Vector2D.CalculateVectorDistance(startVector2D, vector2D2) * (1 - Tension);
                Vector2D startControlPoint = Vector2D.CalculateVectorOffset(vector2D2, startAngle, startDistance);

                Vector2D endVector2D = Vector2D.CalculateVectorCenter(vector2D2, vector2D3);
                double endAngle = Vector2D.IncludedAngleXAxis(endVector2D, vector2D2);
                double endDistance = Vector2D.CalculateVectorDistance(endVector2D, vector2D2) * (1 - Tension);
                Vector2D endControlPoint = Vector2D.CalculateVectorOffset(endVector2D, endAngle, endDistance);

                BezierCurve bezierCurve = new BezierCurve();
                bezierCurve.StartVector2D = startVector2D;
                bezierCurve.StartControlPoint = startControlPoint;
                bezierCurve.EndVector2D = endVector2D;
                bezierCurve.EndControlPoint = endControlPoint;
                BezierCurveList.Add(bezierCurve);
            }
            if (!IsClosedCurve)
            {
                BezierCurveList[0].StartVector2D = Vector2DList[0];
                BezierCurveList.RemoveAt(BezierCurveList.Count - 1);
                BezierCurveList[BezierCurveList.Count - 1].EndVector2D = Vector2DList[Vector2DList.Count - 1];
                BezierCurveList[BezierCurveList.Count - 1].EndControlPoint = BezierCurveList[BezierCurveList.Count - 1].EndVector2D;
            }
            string path = $"M {BezierCurveList[0].StartVector2D.ToString()} ";
            foreach (var item in BezierCurveList)
            {
                path += $"C {item.StartControlPoint.ToString(" ")},{item.EndControlPoint.ToString(" ")},{item.EndVector2D.ToString(" ")} ";
            }
            return path;
        }

        public LineB()
        {
        }
        public LineB(List<Vector2D> verVector2DList, bool isClosedCurve = true)
        {
            this.Vector2DList = verVector2DList;
            this.IsClosedCurve = isClosedCurve;
        }

        /// <summary>
        /// 重载隐式转换，可以直接使用Point
        /// </summary>
        /// <param name="v"></param>
        public static implicit operator Geometry(LineB lineB)//隐式转换
        {
            return Geometry.Parse(lineB.PathData);
        }
    }
    public class BezierCurve
    {
        private Vector2D _startVector2D = new Vector2D(0, 0);
        public Vector2D StartVector2D
        {
            get { return _startVector2D; }
            set { _startVector2D = value; }
        }

        private Vector2D _startControlPoint = new Vector2D(0, 100);
        public Vector2D StartControlPoint
        {
            get { return _startControlPoint; }
            set { _startControlPoint = value; }
        }

        private Vector2D _endControlPoint = new Vector2D(100, 0);
        public Vector2D EndControlPoint
        {
            get { return _endControlPoint; }
            set { _endControlPoint = value; }
        }

        private Vector2D _endVector2D = new Vector2D(100, 100);
        public Vector2D EndVector2D
        {
            get { return _endVector2D; }
            set { _endVector2D = value; }
        }


    }
}
