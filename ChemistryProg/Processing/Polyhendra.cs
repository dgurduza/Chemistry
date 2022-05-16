using ChemistryProg._3D;
using ChemistryProg.Figures;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media.Media3D;

namespace ChemistryProg.Сalculations
{
    public class Polyhedra
    {
        public Polyhedra(Atom3D atom, List<Triangles> edges)
        {
            Name = atom.Atom.Name;
            double vol = GetVolume(atom.Center, edges);
            double dens = 4.0 / 3 * Math.PI * Math.Pow(atom.Atom.R, 3) / vol;
            Volume = vol.ToString("G5");
            Density = dens.ToString("G5");
        }

        private double GetVolume(Point3D center, List<Triangles> edges)
        {
            double s = 0;
            foreach (var edge in edges)
            {
                Vector3D a = edge.Points[0] - center;
                Vector3D b = edge.Points[1] - center;
                Vector3D c = edge.Points[2] - center;
                s += Math.Abs(Vector3D.DotProduct(a, Vector3D.CrossProduct(b, c)) / 6);
            }
            return s;
        }

        public string Name { get; set; }
        public string Volume { get; set; }
        public string Density { get; set; }
    }
}
