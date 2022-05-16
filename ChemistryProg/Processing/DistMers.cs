using ChemistryProg._3D;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;

namespace ChemistryProg.Сalculations
{
    public class DistanceMesurer : INotifyPropertyChanged
    {
        string[] buttonNames = { "Вычисление расстояния", "Выберите первый атом", "Выберите второй атом" };
        string buttonName = "Вычисление расстояния";
        public string ButtonName
        {
            get => buttonName;
            set
            {
                buttonName = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ButtonName"));
            }
        }
        public bool isStarted = false;
        public int step = 0;
        Atom3D first, second;

        public DistanceMesurer(Action<Distance> action)
        {
            DistanceCalculated += action;
        }


        public event Action<Distance> DistanceCalculated;
        public event PropertyChangedEventHandler PropertyChanged;

        public void AtomClicked(Atom3D atom)
        {
            switch (step)
            {
                case 0:
                    return;
                case 1:
                    first = atom;
                    step = 2;
                    break;
                case 2:
                    second = atom;
                    DistanceCalculated(new Distance(first, second));
                    step = 1;
                    break;
            }
            ButtonName = buttonNames[step];
        }

        public void Dist_Button_Click(object sender, RoutedEventArgs e)
        {
            isStarted = !isStarted;
            if (!isStarted)
            {
                step = 0;
                first = second = null;
            }
            else
                step = 1;
            ButtonName = buttonNames[step];

        }
    }
}
