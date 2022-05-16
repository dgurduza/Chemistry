using ChemistryProg._3D;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChemistryProg.Сalculations
{
    public class Distance
    {

        public Distance(Atom3D first, Atom3D second)
        {
            Name1 = $"{first.Atom.Name} {first.ReplicationNumber}";
            Name2 = $"{second.Atom.Name} {second.ReplicationNumber}";
            number = (first.Center - second.Center).Length;
            DistanceStr = number.ToString("0.#####");

        }

        double number;
        public string Name1 { get; set; }
        public string Name2 { get; set; }
        public string DistanceStr { get; set; }

    }
}
