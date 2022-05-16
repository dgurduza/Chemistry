using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChemistryProg.Data
{
    public class Compound
    {
        public string Name { get; set; }
        public Groups Group { get; set; }
        public string GroupName { get; set; }
        public Params Params { get; set; } = new Params();
        public List<AtomStruct> Atoms { get; set; } = new List<AtomStruct>();

        public Compound()
        {
        }

        public Compound(string name, string groupName, Params cellParams, List<AtomStruct> atoms)
        {
            this.Name = name;
            this.GroupName = groupName;
            this.Params = cellParams;
            this.Atoms = atoms;
        }


        public static Compound LoadCompound(string path)
        {
            using (StreamReader file = new StreamReader(path))
            {
                JsonSerializer serializer = new JsonSerializer();
                return (Compound)serializer.Deserialize(file, typeof(Compound));
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
