using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TIS3_WPF_TestMusterAddIn.Infrastructure
{
    public class DummyData
    {
        private string name;
        private string vorname;
        private string firma;

        public string Name { get { return name; } set { name = value; } }
        public string Vorname { get { return vorname; } set { vorname = value; } }
        public string Firma { get { return firma; } set { firma = value; } }

        public DummyData(string name, string vorname, string firma)
        {
            this.name = name;
            this.vorname = vorname;
            this.firma = firma;
        }
    }
}
