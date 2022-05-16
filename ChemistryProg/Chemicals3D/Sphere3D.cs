using ChemistryProg.MakeSphere;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace ChemistryProg._3D
{
    public class Sphere3D : ModelVisual3D
    {
        Model3DGroup sphereModel { get; }

        public Point3D center { get; set; }

        public double radius { get; set; }

        public Sphere3D(Point3D center, double radius, Color color, byte trancparency = 127)
        {
            this.center = center;
            this.radius = radius;
            MeshGeometry3D sphere = new MeshGeometry3D();
            sphere.AddGeodesicSphere(center, radius, 15);//new Point3D(18.872, 6.885, 10.672)
            Color transparent = color.GetSolidColor(0.5);
            //transparent.A = trancparency;
            Material material = new EmissiveMaterial(new SolidColorBrush(transparent));
            MaterialGroup materialGroup = new MaterialGroup();
            materialGroup.Children.Add(material);
            materialGroup.Children.Add(new DiffuseMaterial(new SolidColorBrush(transparent)));
            this.Content = new GeometryModel3D(sphere, materialGroup);

        }
        public Sphere3D(Atom3D atom) : this(atom.Center, atom.Nearest.Average((p) => ((p.Center - atom.Center).Length)), atom.Atom.Color)
        {

        }



    }
}
