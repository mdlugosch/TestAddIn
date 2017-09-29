using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinTIS30db_entwModel.Honorarkraefte;

namespace TIS3_WPF_TestMusterAddIn.Infrastructure
{
    public class Einsatzgebiet_Check
    {
        public bool IsChecked { get; set; }
        public wt2_konst_honorarkraft_einsatzgebiet Einsatzgebiet { get; set; }
    }
}
