using ChemistryProg._3D;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media.Media3D;
using MakeSphere;

namespace ChemistryProg.MakeSphere
{
    public struct Sphere
    {
        public Point3D Center { get; private set; }
        public double Radius { get; private set; }

        public Sphere(Point3D center, double radius)
        {
            this.Center = center;
            this.Radius = radius;
        }

        public Sphere(Sphere3D sphere) : this()
        {
            Center = sphere.center;
            Radius = sphere.radius;
        }

        public static implicit operator Sphere(MakeSphere.Sphere sphere)
        {
            return new Sphere(sphere.Center, sphere.Radius);
        }
    }


    public static class SphereAdapter
    {
        public static Sphere GetSphere(List<Sphere> spheres)
        {
            List<Point3D> centeres = spheres.ConvertAll(x => x.Center);
            List<double> radiuses = spheres.ConvertAll(x => x.Radius);

            return MakeSphere.MakeSphere.Solution(centeres, radiuses);
        }

    }
}
