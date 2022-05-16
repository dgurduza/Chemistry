using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using ChemistryProg.Data;
using ChemistryProg.InformationMenu;
using ChemistryProg.MakeSphere;

namespace ChemistryProg._3D
{
    class TracingAtom3D : ModelVisual3D
    {

        internal AtomStruct atom;
        internal List<Atom3D> atoms = new List<Atom3D>();
        List<Atom3D> hiddenAtoms = new List<Atom3D>();

        public TracingAtom3D(AtomStruct atom, List<Tracing> replications, Params parameters)
        {
            int xCount = SettingsViewModel.GetInstanse().XCount;
            int yCount = SettingsViewModel.GetInstanse().YCount;
            int zCount = SettingsViewModel.GetInstanse().ZCount;


            this.atom = atom;
            foreach (var item in replications)
            {
                Point3D replicatedCenter = atom.Center * item;

                for (int x = -1; x <= xCount; x++)
                {
                    for (int y = -1; y <= yCount; y++)
                    {
                        for (int z = -1; z <= zCount; z++)
                        {
                            Point3D ofssetPoint = replicatedCenter.GetOffsetPoint(new Point3D(x, y, z));
                            double radius = atom.Name[0] == 'O' && (atom.Name.Length == 1 || !char.IsLetter(atom.Name[1])) ? SettingsViewModel.GetInstanse().OxygenRadius : SettingsViewModel.GetInstanse().AtomRadius;
                            Atom3D newAtom = new Atom3D(ofssetPoint, radius, atom, parameters, item.Number);
                            if (!atoms.Contains(newAtom))
                            {
                                atoms.Add(newAtom);
                                if (!newAtom.ShouldBeHidden)
                                    newAtom.IsHidden = false;
                                else
                                    hiddenAtoms.Add(newAtom);
                                Children.Add(newAtom);
                                AddVisual3DChild(newAtom);
                            }
                        }
                    }
                }

            }
        }



        internal Atom3D FindAtomByGeometryModel(GeometryModel3D model)
        {
            foreach (var visAtom in atoms)
            {
                Atom3D atom = visAtom.GetAtomByGeometryModel(model);
                if (atom != null)
                    return atom;

            }
            return null;
        }
    }
}
