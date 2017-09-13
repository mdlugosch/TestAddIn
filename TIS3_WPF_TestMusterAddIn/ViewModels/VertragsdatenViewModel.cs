using Microsoft.Practices.Prism.Regions;
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
    class VertragsdatenViewModel : TIS3ActiveViewModel, INavigationAware
    {
        // Data Access Object für Honorarkraefte Tabellen
        HonorarkraefteDAO hDAO = HonorarkraefteDAO.DAOFactory();

        public LookupCollectionBO Cbx_Teams { get; set; }
        public LookupCollectionBO Cbx_Abteilung { get; set; }
        public ObservableCollection<Bildungstraeger> Cbx_Bildungstraeger { get; set; }
        public ObservableCollection<wt2_konst_honorarkraft_thema> Cbx_Vertag_Thema { get; set; }
        public override void Init()
        {
            InitComboBoxes();
        }

        public void InitComboBoxes()
        {
            Cbx_Teams = new LookupCollectionBO();
            Cbx_Abteilung = new LookupCollectionBO();
            Cbx_Bildungstraeger = new ObservableCollection<Bildungstraeger>();
            Cbx_Vertag_Thema = new ObservableCollection<wt2_konst_honorarkraft_thema>();

            LookupRepository LookRepo = new LookupRepository();

            Cbx_Teams = LookRepo.GetTeams(true, "", true);
            Cbx_Abteilung = LookRepo.GetAbteilungen(true);
            Cbx_Bildungstraeger = LookRepo.GetBildungstraeger(true);
            Cbx_Vertag_Thema = hDAO.HoleThema(false);
        }

        /*
         * IsNavigationTarget gibt zurück ob eine View den Request behandeln kann.
         * Sollen die Werte in den Properties des ViewModels bestehen bleiben bzw.
         * der Inhalt der View(Benutzereingaben) beim zurückkehren zu dieser View
         * wieder angezeigt werden muss diese Methode true zurückgeben.
         */
        bool INavigationAware.IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }
    }
}
