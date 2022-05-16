using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ChemistryProg.InformationMenu
{
    public class SettingsViewModel : INotifyPropertyChanged
    {
        private const string pathToSettings = "app.settings";
        private string pathToGroups;
        private string pathToCompounds;
        private double atomRadius;
        private double oxygenRadius;
        int xCount = 1;
        int yCount = 1;
        int zCount = 1;

        static SettingsViewModel instance;

        internal static SettingsViewModel GetInstanse(string path = pathToSettings)
        {
            if (instance == null)
                Read(path);
            return instance;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public int XCount
        {
            get => xCount;
            set
            {
                xCount = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("XCount"));
            }
        }

        public int YCount
        {
            get => yCount;
            set
            {
                yCount = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("YCount"));
            }
        }

        public int ZCount
        {
            get => zCount;
            set
            {
                zCount = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ZCount"));
            }
        }



        public string PathToGroups
        {
            get => pathToGroups; set
            {
                pathToGroups = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PathToGroups"));
            }
        }
        public string PathToCompounds
        {
            get => pathToCompounds; set
            {
                pathToCompounds = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PathToCompounds"));
            }
        }
        public double AtomRadius
        {
            get => atomRadius; set
            {
                atomRadius = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("AtomRadius"));
            }
        }
        public double OxygenRadius
        {
            get => oxygenRadius; set
            {
                oxygenRadius = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("OxygenRadius"));
            }
        }

        private SettingsViewModel()
        {
            PathToGroups = "Groups/";
            PathToCompounds = "Compounds/";
            AtomRadius = 0.8;
            OxygenRadius = 0.3;
        }

        private static void Read(string path = pathToSettings)
        {
            try
            {
                if (File.Exists(path))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    using (StreamReader sr = new StreamReader("app.settings"))
                    using (JsonReader jsonReader = new JsonTextReader(sr))
                    {
                        instance = (SettingsViewModel)(serializer.Deserialize(jsonReader, typeof(SettingsViewModel)));
                        return;
                    }

                }
            }
            catch
            {
                //  MessageBox.Show(ex.Message);
            }
            instance = new SettingsViewModel();
        }

        ~SettingsViewModel()
        {
            JsonSerializer serializer = new JsonSerializer();
            using (StreamWriter sw = new StreamWriter(pathToSettings))
            using (JsonWriter jsonWriter = new JsonTextWriter(sw))
            {
                serializer.Serialize(jsonWriter, this);
            }
        }


    }
}
