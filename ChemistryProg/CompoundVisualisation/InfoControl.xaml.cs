using ChemistryProg.InformationMenu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Shapes;

namespace ChemistryProg.InformationMenu
{
    /// <summary>
    /// Логика взаимодействия для InfoControl.xaml
    /// </summary>
    public partial class InfoControl : UserControl
    {
        public InfoControl()
        {
            InitializeComponent();
            InfoControlView model = InfoControlView.Instanse;
            this.DataContext = model;
        }


        private void DataGrid_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            e.Handled = true;
        }
    }
}
