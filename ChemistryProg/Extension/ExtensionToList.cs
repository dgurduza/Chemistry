using System;
using System.Collections.Generic;
using System.Text;

namespace ChemistryProg.MakeSphere
{
    public static class ExtensionToList
    {
        public static List<T> Head<T>(this List<T> list, int Count)
        {
            list.RemoveRange(Count, list.Count - Count);
            return list;
        }
    }
}
