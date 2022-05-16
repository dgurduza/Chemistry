using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ChemistryProg.Data
{
    public class Groups
    {
        public string Name { get; set; }

        public List<Tracing> Tracings = new List<Tracing>();


        public Groups()
        {


        }

        public Groups(string name, List<Tracing> tracings)
        {
            Name = name;
            this.Tracings = tracings;
        }

        public static Groups LoadGroup(string path)
        {
            using (StreamReader file = new StreamReader(path))
            {
                JsonSerializer serializer = new JsonSerializer();
                return (Groups)serializer.Deserialize(file, typeof(Groups));
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
