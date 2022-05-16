using ChemistryProg.MakeSphere;
using ChemistryProg.Figures;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace ChemistryProg._3D
{
    public class Polyhedra3D : ModelVisual3D
    {
        Atom3D baseAtom;

        Model3DGroup polyhedraModel { get; }
        public List<Triangles> sides { get; } = new List<Triangles>();

        private static int Sign(Vector3D normal, double d, Point3D target)
        {
            double val = Vector3D.DotProduct(normal, (Vector3D)target) + d;
            return val < 0 ? -1 : val - 0 < 1e-5 ? 0 : 1;
        }

        public void ShowSelectedAtoms()
        {
            foreach (Atom3D atomVisual3D in baseAtom.Nearest)
            {
                atomVisual3D.IsHidden = false;
                atomVisual3D.InPolyhedra = true;
            }
        }

        public void HideSelectedAtoms()
        {
            foreach (Atom3D atomVisual3D in baseAtom.Nearest)
            {
                atomVisual3D.IsHidden = atomVisual3D.ShouldBeHidden;
                atomVisual3D.InPolyhedra = false;
            }
        }

        public Polyhedra3D(Atom3D baseAtom)
        {
            this.baseAtom = baseAtom;
            MeshGeometry3D mesh = new MeshGeometry3D();
            var selected = baseAtom.Nearest;
            if (selected.Count < 3)
                throw new NotSupportedException("Should be more than three atoms!");

            List<Point3D> nearest = selected.ConvertAll(a => a.Center);
            HashSet<Edges> edges = new HashSet<Edges>();
            MeshGeometry3D edgesMesh = new MeshGeometry3D();
            for (int i = 0; i < selected.Count; i++)
            {
                for (int j = i + 1; j < selected.Count; j++)
                {
                    for (int k = j + 1; k < selected.Count; k++)
                    {
                        List<Point3D> other = new List<Point3D>(nearest);
                        Point3D p0 = nearest[i];
                        Point3D p1 = nearest[j];
                        Point3D p2 = nearest[k];
                        other.Remove(p0);
                        other.Remove(p1);
                        other.Remove(p2);
                        Vector3D a = p1 - p0;
                        Vector3D b = p2 - p0;
                        Vector3D normal = Vector3D.CrossProduct(a, b);
                        normal.Normalize();
                        double d = -normal.X * p0.X - normal.Y * p0.Y - normal.Z * p0.Z;
                        bool hasValue = false;
                        bool flag = true;
                        int sign = 0;
                        foreach (var item in other)
                        {
                            int curSign = Sign(normal, d, item);
                            if (hasValue)
                            {
                                if (curSign != sign)
                                {
                                    flag = false;
                                    break;
                                }
                            }
                            else
                            {
                                sign = curSign;
                                hasValue = true;
                            }
                        }
                        if (flag)
                        {
                            if (Sign(normal, d, baseAtom.Center) <= 0)
                            {

                                edgesMesh.AddPolygon(edges, 0.02, p0, p1, p2);
                                mesh.AddPolygon(p0, p1, p2);
                            }
                            if (Sign(normal, d, baseAtom.Center) >= 0)
                            {
                                edgesMesh.AddPolygon(edges, 0.02, p0, p2, p1);
                                mesh.AddPolygon(p0, p2, p1);
                            }
                            sides.Add(new Triangles(p0, p1, p2));
                        }

                    }
                }

            }
            Color transparent = baseAtom.Atom.Color;
            transparent.A = 127;
            Model3DGroup group = new Model3DGroup();
            group.Children.Add(new GeometryModel3D(mesh, new DiffuseMaterial(new SolidColorBrush(transparent))));
            group.Children.Add(new GeometryModel3D(edgesMesh, new DiffuseMaterial(new SolidColorBrush(Colors.Black))));
            polyhedraModel = group;
            this.Content = group;
        }

    }
}
