using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using ChemistryProg.Data;
using ChemistryProg.MakeSphere;
using ChemistryProg.Figures;
using ChemistryProg.InformationMenu;

namespace ChemistryProg._3D
{
    public class Atom3D : ModelVisual3D, IEquatable<Atom3D>
    {

        private const double epsilon = 0.005;
        public AtomStruct Atom { get; set; }
        public Polyhedra3D polyhedra = null;
        public Sphere3D sphere = null;
        Lazy<GeometryModel3D> atomModel;
        bool hidden = true;
        bool hasPolyhedra;
        bool hasSphere;

        List<Atom3D> nearest;

        public List<Atom3D> Nearest
        {
            get
            {
                if (nearest == null)
                {
                    nearest = (from a in CompoundVisual3D.allAtoms
                               where a.Center != this.Center
                               orderby (a.Center - this.Center).Length
                               select a).ToList().Head(Math.Abs(Atom.OxidationState));
                }
                return nearest;
            }

        }




        public event Action<Atom3D, List<Triangles>> PolyhedraCreated;

        public bool InPolyhedra { get; set; } = false;
        public bool IsHidden
        {
            get => hidden;
            set
            {
                if (value == IsHidden)
                    return;
                if (!value)
                    this.Content = atomModel.Value;
                else
                    this.Content = null;


                hidden = value;
            }
        }

        public bool HasPolyhedra
        {
            get => hasPolyhedra;
            set
            {
                if (value == hasPolyhedra)
                    return;
                if (value)
                {
                    polyhedra = polyhedra ?? new Polyhedra3D(this);
                    if (Children.Contains(polyhedra))
                        return;
                    polyhedra.ShowSelectedAtoms();
                    AddVisual3DChild(polyhedra);
                    Children.Add(polyhedra);
                    PolyhedraCreated(this, polyhedra.sides);

                }
                else
                {
                    Children.Remove(polyhedra);
                    RemoveVisual3DChild(polyhedra);
                    polyhedra?.HideSelectedAtoms();
                }


                hasPolyhedra = value;
            }
        }

        public bool HasSphere
        {
            get => hasSphere;
            set
            {
                if (value == hasSphere)
                    return;
                if (value)
                {
                    sphere = sphere ?? new Sphere3D(this);
                    if (Children.Contains(sphere))
                        return;
                    Children.Add(sphere);
                    AddVisual3DChild(sphere);

                }
                else
                {
                    Children.Remove(sphere);
                    RemoveVisual3DChild(sphere);
                }
                hasSphere = value;
            }
        }

        public int ReplicationNumber { get; private set; }
        public Point3D Center { get; }
        public Point3D CenterRelative { get; }
        public bool ShouldBeHidden => CenterRelative.X < 0 - epsilon || CenterRelative.X > SettingsViewModel.GetInstanse().XCount + epsilon ||
            CenterRelative.Y < 0 - epsilon || CenterRelative.Y > SettingsViewModel.GetInstanse().YCount + epsilon ||
            CenterRelative.Z < 0 - epsilon || CenterRelative.Z > SettingsViewModel.GetInstanse().ZCount + epsilon;

        public Atom3D(Point3D point, double radius, AtomStruct atom, Params cellParams, int replicationNumber = -1)
        {

            CenterRelative = point;
            Point3D realCenter = cellParams.ToCartesian(point);
            atomModel = new Lazy<GeometryModel3D>(() =>
            {
                MeshGeometry3D mesh = new MeshGeometry3D();
                return mesh.MakeAtom(Center, radius, atom);
            });
            this.Atom = atom;
            Center = realCenter;
            this.ReplicationNumber = replicationNumber;
            Content = new Model3DGroup();
        }

        public bool Equals(Atom3D other)
        {
            return (Center - other.Center).Length < epsilon;
        }

        internal Atom3D GetAtomByGeometryModel(GeometryModel3D model)
        {
            if (atomModel.IsValueCreated && model == atomModel.Value)
                return this;
            return null;
        }

        public override string ToString()
        {
            return $"{Atom.Name} {ReplicationNumber} R = {Atom.R:F3} \n {CenterRelative.ToStringF3()} \n {Center.ToStringF3()} ";
        }

        internal void RedrawSphere()
        {
            if (!HasSphere)
                return;
            HasSphere = false;
            sphere = null;
            HasSphere = true;
        }
    }
}
