using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TIS3_WPF_TestMusterAddIn.Infrastructure
{
    public class SortedList_Thema
    {
        public string Gruppe { get; set; }
        public ObservableCollection<Thema_Check> ThemeGroup { get; set; }

    }
}
