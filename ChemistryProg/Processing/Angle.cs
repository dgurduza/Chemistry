using ChemistryProg._3D;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media.Media3D;

namespace ChemistryProg.Сalculations
{
    public class Angle
    {
        public Angle(Atom3D first, Atom3D second, Atom3D third)
        {
            Name1 = $"{first.Atom.Name} {first.ReplicationNumber}";
            Name2 = $"{second.Atom.Name} {second.ReplicationNumber}";
            Name3 = $"{third.Atom.Name} {third.ReplicationNumber}";
            Vector3D a = first.Center - second.Center;
            Vector3D b = third.Center - second.Center;
            number = Vector3D.AngleBetween(a, b);
            AngleStr = number.ToString("0.#####");
        }

        double number;
        public string Name1 { get; set; }
        public string Name2 { get; set; }
        public string Name3 { get; set; }
        public string AngleStr { get; set; }
    }
}
