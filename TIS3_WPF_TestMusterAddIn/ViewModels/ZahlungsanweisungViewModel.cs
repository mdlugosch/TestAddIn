using PostSharp.Patterns.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TIS3_Base;
using TIS3_LookupBL;
using WinTIS30db_entwModel.Lookup;

namespace TIS3_WPF_TestMusterAddIn.ViewModels
{
    [NotifyPropertyChanged]
    class ZahlungsanweisungViewModel : TIS3ActiveViewModel
    {
        public LookupCollectionBO Cbx_Teams { get; set; }
        public LookupCollectionBO Cbx_Abteilung { get; set; }
        public ObservableCollection<Bildungstraeger> Cbx_Bildungstraeger { get; set; }
        public override void Init()
        {
            InitComboBoxes();
        }

        public void InitComboBoxes()
        {
            Cbx_Teams = new LookupCollectionBO();
            Cbx_Abteilung = new LookupCollectionBO();
            Cbx_Bildungstraeger = new ObservableCollection<Bildungstraeger>();

            LookupRepository LookRepo = new LookupRepository();

            Cbx_Teams = LookRepo.GetTeams(true, "", true);
            Cbx_Abteilung = LookRepo.GetAbteilungen(true);
            Cbx_Bildungstraeger = LookRepo.GetBildungstraeger(true);
        }
    }
}
