using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TIS3_WPF_TestMusterAddIn.Infrastructure
{
    public static class DummyList
    {
        public static ObservableCollection<DummyData> dummyListe = new ObservableCollection<DummyData>();

        public static ObservableCollection<DummyData> HoleDummyListe(int anzahl)
        {
                for (int i = 1; i < anzahl; i++)
                {
                    dummyListe.Add(new DummyData("Name" + i, "Vorname" + i, "Firma" + i));
                }
                return dummyListe; 
        }
    }
}
