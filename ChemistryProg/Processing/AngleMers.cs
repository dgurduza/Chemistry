using ChemistryProg._3D;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;

namespace ChemistryProg.Сalculations
{
    public class AngleMesurer : INotifyPropertyChanged
    {
        string[] buttonNames = { "Вычисление угла", "Выберите первый атом", "Выберите второй атом", "Выберите третий атом" };
        string buttonName = "Вычисление угла";
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
        Atom3D first, second, third;

        public AngleMesurer(Action<Angle> action)
        {
            AngleCalculated += action;
        }


        public event Action<Angle> AngleCalculated;
        public event PropertyChangedEventHandler PropertyChanged;

        public void AtomClicked(Atom3D atom)
        {
            switch (step)
            {
                case 0:
                    return;
                case 1:
                    first = atom;
                    step += 1;
                    break;
                case 2:
                    second = atom;
                    step += 1;
                    break;
                case 3:
                    third = atom;
                    AngleCalculated(new Angle(first, second, third));
                    step = 1;
                    break;
            }
            ButtonName = buttonNames[step];
        }

        public void Angle_Button_Click(object sender, RoutedEventArgs e)
        {
            isStarted = !isStarted;
            if (!isStarted)
            {
                step = 0;
                first = second = third = null;
            }
            else
                step = 1;
            ButtonName = buttonNames[step];

        }
    }
}
