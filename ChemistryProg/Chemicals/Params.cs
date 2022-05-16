using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace ChemistryProg.Data
{
    public class Params
    {
        private double coordA;
        private double coordB;
        private double coordC;
        private double angleAlpha;
        private double angleBeta;
        private double angleGamma;
        public double AlphaInRadians => AngleToRadians(Alpha);

        public double BetaInRadians => AngleToRadians(Beta);

        public double GammaInRadians => AngleToRadians(Gamma);

        Vector3D x, y, z;

        MatrixTransform3D matrix;

        public double Volume => Math.Abs(Vector3D.DotProduct(Vector3D.CrossProduct(x, y), z));

        public double CubicVolume => Math.Pow(Volume, 1.0 / 3);
        public Params()
        {

        }
        public Params(double a, double b, double c, double alpha, double beta, double gamma)
        {
            A = a;
            B = b;
            C = c;
            Alpha = alpha;
            Beta = beta;
            Gamma = gamma;

        }
        /// <summary>
        /// Размер ячейки по оси A
        /// </summary>
        public double A
        {
            get
            {
                return coordA;
            }
            set
            {
                coordA = value;
                CalculateAngle();
            }
        }
        /// <summary>
        /// Размер ячейки по оси B
        /// </summary>
        public double B
        {
            get
            {
                return coordB;
            }
            set
            {
                coordB = value;
                CalculateAngle();
            }
        }
        /// <summary>
        /// Размер ячейки по оси C
        /// </summary>
        public double C
        {
            get
            {
                return coordC;
            }
            set
            {
                coordC = value;
                CalculateAngle();
            }
        }
        double AngleToRadians(double angle)
        {
            return angle / 180 * Math.PI;
        }

        /// <summary>
        /// Угол альфа между осями  Y и Z
        /// </summary>
        public double Alpha
        {
            get
            {
                return angleAlpha;
            }
            set
            {
                angleAlpha = value;
                CalculateAngle();
            }
        }
        /// <summary>
        /// Угол бета между осями  X и Z
        /// </summary>
        public double Beta
        {
            get
            {
                return angleBeta;
            }
            set
            {
                angleBeta = value;
                CalculateAngle();
            }
        }
        /// <summary>
        /// Угол гамма между осями  X и Y
        /// </summary>
        public double Gamma
        {
            get
            {
                return angleGamma;
            }
            set
            {
                angleGamma = value;
                CalculateAngle();
            }
        }
        public Point3D ToCartesian(double x, double y, double z)
        {
            return ToCartesian(new Point3D(x, y, z));
        }

        public Point3D ToCartesian(Point3D point)
        {
            return matrix.Transform(point);
        }

        public Point3D FromCartesian(Point3D absolute)
        {
            return matrix.Inverse.Transform(absolute);
        }


        void CalculateAngle()
        {
            x = new Vector3D(A, 0, 0);
            y = new Vector3D(B * Math.Cos(GammaInRadians), B * Math.Sin(GammaInRadians), 0);
            double a = C * Math.Cos(BetaInRadians);
            double b = (B * C * Math.Cos(AlphaInRadians) - a * y.X) / x.Y;
            double c = Math.Sqrt(C * C - Math.Pow(a, 2) - Math.Pow(b,2));
            z = new Vector3D(a, b, c);
            matrix = new MatrixTransform3D(new Matrix3D(x.X, x.Z, x.Y, 0, y.X, y.Z, y.Y, 0,
                z.X, z.Z, z.Y, 0, 0, 0, 0, 1));
        }
    }
}
