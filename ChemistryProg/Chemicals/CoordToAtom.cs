using ChemistryProg._3D;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChemistryProg.Data
{
    public class CoordToAtom
    {
        public CoordToAtom()
        {
        }


        public static CoordToAtom GetRelativeCoordinate(Atom3D atom)
        {
            string Name = atom.Atom.Name;
            string X = atom.CenterRelative.X.ToString("0.#####");
            string Y = atom.CenterRelative.Y.ToString("0.#####");
            string Z = atom.CenterRelative.Z.ToString("0.#####");
            return new CoordToAtom(Name, X, Y, Z);
        }

        public static CoordToAtom GetCoordinate(Atom3D atom)
        {
            string Name = atom.Atom.Name;
            string X = atom.Center.X.ToString("0.#####");
            string Y = atom.Center.Y.ToString("0.#####");
            string Z = atom.Center.Z.ToString("0.#####");
            return new CoordToAtom(Name, X, Y, Z);
        }

        public static CoordToAtom GetCoordinate(AtomStruct atom)
        {
            string Name = atom.Name;
            string X = atom.Center.X.ToString("0.#####");
            string Y = atom.Center.Y.ToString("0.#####");
            string Z = atom.Center.Z.ToString("0.#####");
            return new CoordToAtom(Name, X, Y, Z);
        }

        public CoordToAtom(string name, string x, string y, string z)
        {
            Name = name;
            X = x;
            Y = y;
            Z = z;
        }

        public string Name { get; set; }
        public string X { get; set; }
        public string Y { get; set; }
        public string Z { get; set; }



    }
}
