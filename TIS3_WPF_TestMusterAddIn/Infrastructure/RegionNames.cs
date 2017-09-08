using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TIS3_WPF_TestMusterAddIn.Infrastructure
{
    public class RegionNames
    {
        private string searchRegion = "SearchMaskRegion";
        private string resultRegion = "SearchResultRegion";

        public string SearchRegion { get { return searchRegion; } set { searchRegion = value; } }
        public string ResultRegion { get { return resultRegion; } set { searchRegion = value; } }

    }
}
