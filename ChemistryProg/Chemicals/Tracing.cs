using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace ChemistryProg.Data
{
    public class TracingComponent
    {

        public TracingComponent()
        {
        }

        public TracingComponent(double x, double y, double z, double c = 0)
        {
            X = x;
            Y = y;
            Z = z;
            C = c;
        }

        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public double C { get; set; }

        public double Copy(double x, double y, double z) => x * X + y * Y + z * Z + C;
        public double Copy(Point3D point) => point.X * X + point.Y * Y + point.Z * Z + C;

    }


    public class Tracing
    {


        public int Number { get; set; }
        public TracingComponent X
        {
            get; set;
        }
        public TracingComponent Y { get; set; }
        public TracingComponent Z { get; set; }


        public Tracing()
        {

        }

        public Tracing(int number = 0)
        {
            Number = number;
            X = new TracingComponent();
            Y = new TracingComponent();
            Z = new TracingComponent();
        }


        public Tracing(int number, TracingComponent x, TracingComponent y, TracingComponent z)
        {
            Number = number;
            X = x;
            Y = y;
            Z = z;
        }

        public static Point3D operator *(Point3D point, Tracing replication)
        {
            double nX = replication.X.Copy(point);
            double nY = replication.Y.Copy(point);
            double nZ = replication.Z.Copy(point);
            return new Point3D(nX, nY, nZ);
        }
    }
}
