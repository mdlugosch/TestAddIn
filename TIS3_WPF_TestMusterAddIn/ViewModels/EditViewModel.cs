using Microsoft.Practices.Prism.Regions;
using PostSharp.Patterns.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TIS3_Base;

namespace TIS3_WPF_TestMusterAddIn.ViewModels
{
    [NotifyPropertyChanged]
    class EditViewModel : TIS3ActiveViewModel, INavigationAware
    {
        public override void Init()
        {
            Header.Title = "Änderungsbogen";                         // Header Attribut zum bestimmen des Tab-Namens
            Header.Group = "Honorarkräfteverwaltung";           // Gruppenname der Anwendungs-Tabs
        }
    }
}
