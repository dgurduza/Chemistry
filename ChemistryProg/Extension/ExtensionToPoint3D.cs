using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media.Media3D;

namespace ChemistryProg.MakeSphere
{

    public static class ExtensionToPoint3D
    {
        public static Point3D GetOffsetPoint(this Point3D point, Point3D offset)
        {
            return new Point3D(point.X + offset.X, point.Y + offset.Y, point.Z + offset.Z);
        }

        // Return a rounded Point3D so close points match.
        public static Point3D Round(this Point3D point, int decimals = 3)
        {
            double x = Math.Round(point.X, decimals);
            double y = Math.Round(point.Y, decimals);
            double z = Math.Round(point.Z, decimals);
            return new Point3D(x, y, z);
        }

        // Move this point along the vector to the center
        // so it has the given distance from the center.
        public static Point3D SetDistanceFrom(this Point3D point, Point3D center, double distance)
        {
            Vector3D v = point - center;
            return center + v / v.Length * distance;
        }

        public static string ToStringF3(this Point3D point)
        {
            return $"{point.X:F3}; {point.Y:F3}; {point.Z:F3}";
        }
    }

}
