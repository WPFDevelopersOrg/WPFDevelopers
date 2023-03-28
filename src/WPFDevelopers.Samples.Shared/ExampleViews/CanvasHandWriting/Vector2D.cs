using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WPFDevelopers.Samples.ExampleViews.CanvasHandWriting
{
    public class Vector2D
    {
        public double X { get; set; } = 0;
        public double Y { get; set; } = 0;
       
        /// <summary>
        /// 向量的模
        /// </summary>
        public double Mold
        {
            get
            {
                //自身各分量平方运算.
                double X = this.X * this.X;
                double Y = this.Y * this.Y;
                return Math.Sqrt(X + Y);//开根号,最终返回向量的长度/模/大小.
            }
        }
        /// <summary>
        /// 单位向量
        /// </summary>
        public Vector2D UnitVector
        {
            get
            {
                double sumSquares = (X * X) + (Y * Y);
                return new Vector2D(X / Math.Sqrt(sumSquares), Y / Math.Sqrt(sumSquares));
            }
        }

        public Vector2D()
        {

        }
        public Vector2D(double x, double y)
        {
            X = x;
            Y = y;
        }
        public Vector2D(System.Windows.Point point)
        {
            X = point.X;
            Y = point.Y;
        }

        public void Offset(double angle, double distance, AngleType angleType = AngleType.Radian)
        {
            var vector2D = Vector2D.CalculateVectorOffset(this, angle, distance, angleType);
            X = vector2D.X;
            Y = vector2D.Y;
        }
        public void Rotate(double angle, Vector2D vectorCenter = null, AngleType angleType = AngleType.Radian)
        {
            vectorCenter = vectorCenter == null ? this : vectorCenter;
            var vector2D = Vector2D.CalculateVectorRotation(this, vectorCenter, angle, angleType);
            X = vector2D.X;
            Y = vector2D.Y;
        }

        #region 静态方法
        /// <summary>
        /// 计算两个向量之间的距离
        /// </summary>
        public static double CalculateVectorDistance(Vector2D vector2DA, Vector2D vector2DB)
        {
            Vector2D vector2D = vector2DA - vector2DB;
            return vector2D.Mold;
        }

        /// <summary>
        /// 计算两点夹角，右侧X轴线为0度，向下为正，向上为负
        /// </summary>
        public static double IncludedAngleXAxis(Vector2D vector2DA, Vector2D vector2DB, AngleType angleType = AngleType.Radian)
        {
            double radian = Math.Atan2(vector2DB.Y - vector2DA.Y, vector2DB.X - vector2DA.X); //弧度：1.1071487177940904
            return angleType == AngleType.Radian ? radian : ComputingHelper.RadianToAngle(radian);
        }

        /// <summary>
        /// 计算两点夹角，下侧Y轴线为0度，向右为正，向左为负
        /// </summary>
        public static double IncludedAngleYAxis(Vector2D vector2DA, Vector2D vector2DB, AngleType angleType = AngleType.Radian)
        {
            double radian = Math.Atan2(vector2DB.X - vector2DA.X, vector2DB.Y - vector2DA.Y); //弧度：0.46364760900080609
            return angleType == AngleType.Radian ? radian : ComputingHelper.RadianToAngle(radian);
        }

        /// <summary>
        /// 偏移向量到指定角度，指定距离
        /// </summary>
        public static Vector2D CalculateVectorOffset(Vector2D vector2D, double angle, double distance, AngleType angleType = AngleType.Radian)
        {
            Vector2D pointVector2D = new Vector2D();
            if (angleType == AngleType.Angle)
            {
                angle = angle / (180 / Math.PI);//角度转弧度
            }
            double width = Math.Cos(Math.Abs(angle)) * distance;
            double height = Math.Sin(Math.Abs(angle)) * distance;
            if(angle <= Math.PI && angle >= 0)
            //if (angle is <= Math.PI and >= 0)
            {
                pointVector2D.X = vector2D.X - width;
                pointVector2D.Y = vector2D.Y - height;
            }
            if (angle >= (-Math.PI) && angle <= 0)
            //if (angle is >= (-Math.PI) and <= 0)
            {
                pointVector2D.X = vector2D.X - width;
                pointVector2D.Y = vector2D.Y + height;
            }
            return pointVector2D;
        }

        /// <summary>
        /// 围绕一个中心点，旋转一个向量,相对旋转
        /// </summary>
        public static Vector2D CalculateVectorRotation(Vector2D vector2D, Vector2D vectorCenter, double radian, AngleType angleType = AngleType.Radian)
        {
            radian = angleType == AngleType.Radian ? radian : ComputingHelper.RadianToAngle(radian);
            double x1 = (vector2D.X - vectorCenter.X) * Math.Sin(radian) + (vector2D.Y - vectorCenter.Y) * Math.Cos(radian) + vectorCenter.X;
            double y1 = -(vector2D.X - vectorCenter.X) * Math.Cos(radian) + (vector2D.Y - vectorCenter.Y) * Math.Sin(radian) + vectorCenter.Y;
            return new Vector2D(x1, y1);
        }
        public static Vector2D CalculateVectorCenter(Vector2D vector2DA, Vector2D vector2DB)
        {
            return new Vector2D((vector2DA.X + vector2DB.X) / 2, (vector2DA.Y + vector2DB.Y) / 2);
        }

        /// <summary>
        /// 判断坐标点是否在多边形区域内，射线法
        /// </summary>
        public static bool IsPointPolygonalArea(Vector2D vector2D, List<Vector2D> aolygonaArrayList)
        {
            var N = aolygonaArrayList.Count;
            var boundOrVertex = true; //如果点位于多边形的顶点或边上，也算做点在多边形内，直接返回true
            var crossNumber = 0; //x的交叉点计数
            var precision = 2e-10; //浮点类型计算时候与0比较时候的容差
            Vector2D p1, p2; //neighbour bound vertices
            var p = vector2D; //测试点
            p1 = aolygonaArrayList[0]; //left vertex        
            for (var i = 1; i <= N; ++i)
            {
                //check all rays            
                if (p.X.Equals(p1.X) && p.Y.Equals(p1.Y))
                {
                    return boundOrVertex; //p is an vertex
                }

                p2 = aolygonaArrayList[i % N]; //right vertex            
                if (p.X < Math.Min(p1.X, p2.X) || p.X > Math.Max(p1.X, p2.X))
                {
                    //ray is outside of our interests                
                    p1 = p2;
                    continue; //next ray left point
                }

                if (p.X > Math.Min(p1.X, p2.X) && p.X < Math.Max(p1.X, p2.X))
                {
                    //ray is crossing over by the algorithm (common part of)
                    if (p.Y <= Math.Max(p1.Y, p2.Y))
                    {
                        //x is before of ray                    
                        if (p1.X == p2.X && p.Y >= Math.Min(p1.Y, p2.Y))
                        {
                            //overlies on a horizontal ray
                            return boundOrVertex;
                        }

                        if (p1.Y == p2.Y)
                        {
                            //ray is vertical                        
                            if (p1.Y == p.Y)
                            {
                                //overlies on a vertical ray
                                return boundOrVertex;
                            }
                            else
                            {
                                //before ray
                                ++crossNumber;
                            }
                        }
                        else
                        {
                            //cross point on the left side                        
                            var xinters =
                                (p.X - p1.X) * (p2.Y - p1.Y) / (p2.X - p1.X) +
                                p1.Y; //cross point of Y                        
                            if (Math.Abs(p.Y - xinters) < precision)
                            {
                                //overlies on a ray
                                return boundOrVertex;
                            }

                            if (p.Y < xinters)
                            {
                                //before ray
                                ++crossNumber;
                            }
                        }
                    }
                }
                else
                {
                    //special case when ray is crossing through the vertex                
                    if (p.X == p2.X && p.Y <= p2.Y)
                    {
                        //p crossing over p2                    
                        var p3 = aolygonaArrayList[(i + 1) % N]; //next vertex                    
                        if (p.X >= Math.Min(p1.X, p3.X) && p.X <= Math.Max(p1.X, p3.X))
                        {
                            //p.X lies between p1.X & p3.X
                            ++crossNumber;
                        }
                        else
                        {
                            crossNumber += 2;
                        }
                    }
                }
                p1 = p2; //next ray left point
            }
            if (crossNumber % 2 == 0)
            {
                //偶数在多边形外
                return false;
            }
            else
            {
                //奇数在多边形内
                return true;
            }
        }

        /// <summary>
        /// 判断一个点是否在一条边内
        /// </summary>
      
        public static bool IsPointEdge(Vector2D point, Vector2D startPoint, Vector2D endPoint)
        {
            return (point.X - startPoint.X) * (endPoint.Y - startPoint.Y) == (endPoint.X - startPoint.X) * (point.Y - startPoint.Y)
                && Math.Min(startPoint.X, endPoint.X) <= point.X && point.X <= Math.Max(startPoint.X, endPoint.X)
                && Math.Min(startPoint.Y, endPoint.Y) <= point.Y && point.Y <= Math.Max(startPoint.Y, endPoint.Y);
        }
        #endregion 静态方法

        #region 运算符重载
        /// <summary>
        /// 重载运算符，和运算,可以用来计算两向量距离
        /// </summary>
        public static Vector2D operator +(Vector2D vector2DA, Vector2D vector2DB)
        {
            Vector2D vector2D = new Vector2D();
            vector2D.X = vector2DA.X + vector2DB.X;
            vector2D.Y = vector2DA.Y + vector2DB.Y;
            return vector2D;
        }

        /// <summary>
        /// 重载运算符，差运算,可以用来计算两向量距离
        /// </summary>
        public static Vector2D operator -(Vector2D vector2DA, Vector2D vector2DB)
        {
            Vector2D vector2D = new Vector2D();
            vector2D.X = vector2DA.X - vector2DB.X;
            vector2D.Y = vector2DA.Y - vector2DB.Y;
            return vector2D;
        }

        /// <summary>
        /// 重载运算符，差运算,可以用来计算两向量距离
        /// </summary>
        public static Vector2D operator -(Vector2D vector2D, double _float)
        {
            return new Vector2D(vector2D.X - _float, vector2D.Y - _float);
        }

        /// <summary>
        /// 重载运算符，点积运算,可以用来计算两向量夹角
        /// </summary>
        public static double operator *(Vector2D vector2DA, Vector2D vector2DB)
        {
            return (vector2DA.X * vector2DB.X) + (vector2DA.Y * vector2DB.Y);
        }
        public static double operator *(Vector2D vector2D, double _float)
        {
            return (vector2D.X * _float) + (vector2D.Y * _float);
        }

        /// <summary>
        /// 重载运算符，点积运算,可以用来计算两向量夹角
        /// </summary>
        public static double operator /(Vector2D vector2D, double para)
        {
            return (vector2D.X / para) + (vector2D.Y / para);
        }

        /// <summary>
        /// 重载运算符
        /// </summary>
        
        public static bool operator >=(Vector2D vector2D, double para)
        {
            if (vector2D.Mold >= para)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool operator <=(Vector2D vector2D, double para)
        {
            if (vector2D.Mold <= para)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool operator >(Vector2D vector2D, double para)
        {
            if (vector2D.Mold > para)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool operator <(Vector2D vector2D, double para)
        {
            if (vector2D.Mold < para)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion 运算符重载

        #region 隐式转换
        /// <summary>
        /// 重载隐式转换，可以直接使用Point
        /// </summary>
        /// <param name="v"></param>
        public static implicit operator Vector2D(System.Windows.Point v)//隐式转换
        {
            return new Vector2D(v.X, v.Y);
        }
        /// <summary>
        /// 重载隐式转换，可以直接使用Point
        /// </summary>
        /// <param name="v"></param>
        public static implicit operator System.Windows.Point(Vector2D v)//隐式转换
        {
            return new System.Windows.Point(v.X, v.Y);
        }
        /// <summary>
        /// 重载隐式转换，可以直接使用double
        /// </summary>
        /// <param name="v"></param>
        public static implicit operator Vector2D(double v)//隐式转换
        {
            return new Vector2D(v, v);
        }
        #endregion 隐式转换

        #region ToString
        public override string ToString()
        {
            return X.ToString() + "," + Y.ToString();
        }
        public string ToString(string symbol)
        {
            return X.ToString() + symbol + Y.ToString();
        }
        public string ToString(string sender, string symbol)
        {
            return X.ToString(sender) + symbol + Y.ToString(sender);
        }
        #endregion
    }
    public enum AngleType 
    {
        Angle,
        Radian
    }
}
