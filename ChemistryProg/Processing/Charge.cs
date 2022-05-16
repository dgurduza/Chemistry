using ChemistryProg._3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChemistryProg.Сalculations
{
    public class FactChargeResult
    {
        public FactChargeResult(Atom3D target, IEnumerable<Atom3D> atoms)
        {
            Name = target.Atom.Name;
            var sorted = from atom in atoms
                         where atom != target
                         orderby (atom.Center - target.Center).Length
                         select atom;
            double fact_charge = 0;
            int i = 0;
            foreach (Atom3D atom in sorted)
            {
                double dist = (target.Center - atom.Center).Length;
                fact_charge += Math.Pow(Math.E, (target.Atom.R + atom.Atom.R - dist) / 0.37);

                if (i >= target.Atom.OxidationState)
                    break;
            }
            Fact_Charge = fact_charge.ToString("G5");
        }

        public string Name { get; set; }
        public string Fact_Charge { get; set; }
    }
}
