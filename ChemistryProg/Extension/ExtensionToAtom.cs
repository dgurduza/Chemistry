using ChemistryProg.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace ChemistryProg.MakeSphere
{
    public static class ExtensionToAtom
    {
        const int numOfRows = 15;
        private const double SpecularPower = 24;

        public static GeometryModel3D MakeAtom(this MeshGeometry3D mesh,
           Point3D center, double radius, AtomStruct atom)
        {
            mesh.AddSphere(center, radius, numOfRows, numOfRows, true, null);
            Material material = new DiffuseMaterial(new SolidColorBrush(atom.Color));
            MaterialGroup materialGroup = new MaterialGroup();
            materialGroup.Children.Add(material);
            materialGroup.Children.Add(new SpecularMaterial(new SolidColorBrush(atom.Color), SpecularPower));
            return new GeometryModel3D(mesh, materialGroup);
        }

    }
}
