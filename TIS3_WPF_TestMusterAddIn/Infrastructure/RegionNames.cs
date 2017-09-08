using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TIS3_WPF_TestMusterAddIn.Infrastructure
{
    public class RegionNames
    {
        private static int idNr = 0;

        private string searchRegion; //+ (idNr++).ToString();
        private string resultRegion = "SearchResultRegion"; //+ (idNr++).ToString();

       
        public RegionNames()
        {
            searchRegion = "SearchMaskRegion" + idNr;
            //idNr++;
            //string test = (idNr++).ToString();

      
            //searchRegion = "SearchMaskRegion" + test;
            //resultRegion = String.Format("SearchResultRegion{0}",test);
        }

        public string SearchRegion { get { return searchRegion; } set { searchRegion = value; } }
        public string ResultRegion { get { return resultRegion; } set { resultRegion = value; } }

    }
}
