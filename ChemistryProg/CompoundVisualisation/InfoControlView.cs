using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using ChemistryProg.Data;

namespace ChemistryProg.InformationMenu
{
    internal class InfoControlView : ViewModel
    {
        static InfoControlView instanse;

        InfoControlView()
        {

        }

        public static InfoControlView Instanse
        {
            get
            {
                return instanse ?? (instanse = new InfoControlView());
            }
        }

        private Compound variables = new Compound();

        public Compound Variables
        {
            get
            {
                return variables;
            }
            set
            {
                variables = value;
                OnPropertyChanged("Name");
                OnPropertyChanged("GroupName");
                OnPropertyChanged("A");
                OnPropertyChanged("B");
                OnPropertyChanged("C");
                OnPropertyChanged("Alpha");
                OnPropertyChanged("Beta");
                OnPropertyChanged("Gamma");
                OnPropertyChanged("Volume");
                OnPropertyChanged("CubicVolume");
                OnPropertyChanged("Density");
                AtomList.Clear();
                foreach (var atom in variables.Atoms)
                {
                    AtomList.Add(CoordToAtom.GetCoordinate(atom));
                }
            }
        }

        double density;

        public ObservableCollection<CoordToAtom> AtomList { get; set; } = new ObservableCollection<CoordToAtom>();

        public string Name
        {
            get => Variables.Name;
        }

        public string GroupName
        {
            get => Variables.GroupName;
        }

        public double A => Variables.Params.A;

        public double B => Variables.Params.B;

        public double C => Variables.Params.C;

        public double Alpha => Variables.Params.Alpha;

        public double Beta => Variables.Params.Beta;

        public double Gamma => Variables.Params.Gamma;

        public double Volume => Variables.Params.Volume;

        public double CubicVolume => Variables.Params.CubicVolume;

        public double Density => density;

    }
}
