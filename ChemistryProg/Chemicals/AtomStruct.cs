using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace ChemistryProg.Data
{
    public class AtomStruct
    {
        public string Name { get; set; }
        public Point3D Center { get; set; }

        public double R { get; set; }
        public int OxidationState { get; set; }
        public Color Color { get; set; } = Colors.Blue;

        public AtomStruct(string name, Point3D center, double r, int oxidationState, Color color)
        {
            this.Name = name;
            this.Center = center;
            R = r;
            this.OxidationState = oxidationState;
            this.Color = color;
        }

        public AtomStruct()
        {
        }

        public override string ToString()
        {
            return $"{Name} \n {Center}";
        }
    }
}
