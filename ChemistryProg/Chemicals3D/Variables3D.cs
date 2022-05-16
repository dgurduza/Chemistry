using System;
using System.Collections.Generic;
using System.Text;
using ChemistryProg.Data;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using ChemistryProg.MakeSphere;
using ChemistryProg.InformationMenu;

namespace ChemistryProg._3D
{
    class CompoundVisual3D : ModelVisual3D
    {
        internal List<TracingAtom3D> repAtoms = new List<TracingAtom3D>();
        static internal List<Atom3D> allAtoms = new List<Atom3D>();
        readonly Params Params;
        Point3D center;


        Model3DGroup modelGroup = new Model3DGroup();

        public CompoundVisual3D(Compound variables)
        {
            int xCount = SettingsViewModel.GetInstanse().XCount;
            int yCount = SettingsViewModel.GetInstanse().YCount;
            int zCount = SettingsViewModel.GetInstanse().ZCount;
            allAtoms.Clear();
            Groups group = variables.Group;
            Params = variables.Params;
            center = Params.ToCartesian(new Point3D(xCount / 2.0, yCount / 2.0, zCount / 2.0));
            foreach (var atom in variables.Atoms)
            {
                TracingAtom3D repAtom = new TracingAtom3D(atom, group.Tracings, Params);
                allAtoms.AddRange(repAtom.atoms);
                repAtoms.Add(repAtom);
                Children.Add(repAtom);
                AddVisual3DChild(repAtom);
            }

            this.Content = modelGroup;
            modelGroup.Children.Add(ExtensionToMesh.XAxisArrow(Params, xCount + 0.1).SetMaterial(Brushes.Red, false));
            modelGroup.Children.Add(ExtensionToMesh.YAxisArrow(Params, yCount + 0.1).SetMaterial(Brushes.Green, false));
            modelGroup.Children.Add(ExtensionToMesh.ZAxisArrow(Params, zCount + 0.1).SetMaterial(Brushes.Blue, false));
            modelGroup.Children.Add(ExtensionToMesh.Border(Params, xCount, yCount, zCount).SetMaterial(Brushes.Black, false));

        }



        internal Atom3D FindAtomByGeometryModel(GeometryModel3D model)
        {
            foreach (var repAtom in repAtoms)
            {
                Atom3D atom = repAtom.FindAtomByGeometryModel(model);
                if (atom != null)
                    return atom;
            }
            return null;
        }
    }
}
