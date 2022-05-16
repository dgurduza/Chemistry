using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media.Media3D;

namespace ChemistryProg.Figures
{
    public class Edges : IEquatable<Edges>, IComparable<Edges>
    {
        public Point3D Point1, Point2;
        public Edges(Point3D point1, Point3D point2)
        {
            bool p1smaller =
                (point1.X < point2.X) ||
                ((point1.X == point2.X) && (point1.Y < point2.Y)) ||
                ((point1.X == point2.X) && (point1.Y == point2.Y) && (point1.Z < point2.Z));
            if (p1smaller)
            {
                Point1 = point1;
                Point2 = point2;
            }
            else
            {
                Point1 = point2;
                Point2 = point1;
            }
        }

        public bool Equals(Edges other)
        {
            if (ReferenceEquals(other, null)) return false;
            if ((Point1 == other.Point1) && (Point2 == other.Point2)) return true;
            return false;
        }
        public static bool operator ==(Edges edge1, Edges edge2)
        {
            if (ReferenceEquals(edge1, edge2)) return true;
            if ((edge1 == null)) return false;
            return edge1.Equals(edge2);
        }
        public static bool operator !=(Edges edge1, Edges edge2)
        {
            return !(edge1 == edge2);
        }
        public override bool Equals(object obj)
        {
            if (!(obj is Edges)) return false;
            return Equals(obj as Edges);
        }

        public override int GetHashCode()
        {
            return Point1.GetHashCode() ^ Point2.GetHashCode();
        }

        public override string ToString()
        {
            return Point1.ToString() + " --> " + Point2.ToString();
        }

        public int CompareTo(Edges other)
        {
            if (Point1.X < Point2.X) return -1;
            if (Point1.X > Point2.X) return 1;
            if (Point1.Y < Point2.Y) return -1;
            if (Point1.Y > Point2.Y) return 1;
            if (Point1.Z < Point2.Z) return -1;
            if (Point1.Z > Point2.Z) return 1;
            return 0;
        }
    }
}
