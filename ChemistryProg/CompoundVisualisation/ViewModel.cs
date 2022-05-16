using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace ChemistryProg.InformationMenu
{
    public class ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
