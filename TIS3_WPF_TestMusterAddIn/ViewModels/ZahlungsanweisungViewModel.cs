using PostSharp.Patterns.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TIS3_Base;
using TIS3_LookupBL;
using TIS3_WPF_TestMusterAddIn.Infrastructure;
using WinTIS30db_entwModel.Honorarkraefte;
using WinTIS30db_entwModel.Lookup;

namespace TIS3_WPF_TestMusterAddIn.ViewModels
{
    [NotifyPropertyChanged]
    class ZahlungsanweisungViewModel : TIS3ActiveViewModel
    {
        // Data Access Object für Honorarkraefte Tabellen
        HonorarkraefteDAO hDAO = HonorarkraefteDAO.DAOFactory();

        public LookupCollectionBO Cbx_Zahlung_Teams { get; set; }
        public LookupCollectionBO Cbx_Zahlung_Abteilung { get; set; }
        public ObservableCollection<Bildungstraeger> Cbx_Zahlung_Bildungstraeger { get; set; }
        public ObservableCollection<wt2_konst_honorarkraft_thema> Cbx_Zahlung_Thema { get; set; }
        public override void Init()
        {
            InitComboBoxes();
        }

        public void InitComboBoxes()
        {
            Cbx_Zahlung_Teams = new LookupCollectionBO();
            Cbx_Zahlung_Abteilung = new LookupCollectionBO();
            Cbx_Zahlung_Bildungstraeger = new ObservableCollection<Bildungstraeger>();
            Cbx_Zahlung_Thema = new ObservableCollection<wt2_konst_honorarkraft_thema>();

            LookupRepository LookRepo = new LookupRepository();

            Cbx_Zahlung_Teams = LookRepo.GetTeams(true, "", true);
            Cbx_Zahlung_Abteilung = LookRepo.GetAbteilungen(true);
            Cbx_Zahlung_Bildungstraeger = LookRepo.GetBildungstraeger(true);
            Cbx_Zahlung_Thema = hDAO.HoleThema(false);
        }
    }
}
