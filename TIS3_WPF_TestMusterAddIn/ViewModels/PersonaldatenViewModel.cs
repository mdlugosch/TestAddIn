using BFZ_Common_Lib.MVVM;
using Microsoft.Practices.Prism.Regions;
using PostSharp.Patterns.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TIS3_Base;
using TIS3_LookupBL;
using TIS3_WPF_TestMusterAddIn.Infrastructure;
using WinTIS30db_entwModel.Honorarkraefte;
using WinTIS30db_entwModel.Lookup;

namespace TIS3_WPF_TestMusterAddIn.ViewModels
{
    [NotifyPropertyChanged]
    public class PersonaldatenViewModel : TIS3ActiveViewModel, INavigationAware
    {
        // Data Access Object für Honorarkraefte Tabellen
        HonorarkraefteDAO hDAO = HonorarkraefteDAO.DAOFactory();

        public String Tbx_Personal_Name{ get; set; }
        public String Tbx_Personal_Vorname{ get; set; }
        public String Tbx_Personal_Firma{ get; set; }
        public bool Chkbx_Status_Selbstaendig{ get; set; }
        public bool Chkbx_Status_Verfolgen{ get; set; }
        public bool Chkbx_Status_Pruefen{ get; set; }
        public bool Chkbx_Status_bedenklich { get; set; }

        public RelayCommand ResetCommand { get; set; }
        public RelayCommand SearchCommand { get; set; }
        public LookupCollectionBO Cbx_Personal_Teams { get; set; }
        public LookupCollectionBO Cbx_Personal_Abteilung { get; set; }
        public ObservableCollection<wt2_konst_honorarkraft_einsatzgebiet> Cbx_Personal_Einsatzgebiet { get; set; }
        public ObservableCollection<Bildungstraeger> Cbx_Personal_Bildungstraeger { get; set; }
        public ObservableCollection<wt2_konst_honorarkraft_thema> Cbx_Personal_Thema { get; set; }
        public override void Init()
        {
            ResetCommand = new RelayCommand(_execute => { Reset(); }, _canExecute => { return true; });
            SearchCommand = new RelayCommand(_execute => { Search(); }, _canExecute => { return true; }); 
            InitComboBoxes();
        }

        public void InitComboBoxes()
        {
            Cbx_Personal_Teams = new LookupCollectionBO();
            Cbx_Personal_Abteilung = new LookupCollectionBO();
            Cbx_Personal_Bildungstraeger = new ObservableCollection<Bildungstraeger>();
            Cbx_Personal_Einsatzgebiet = new ObservableCollection<wt2_konst_honorarkraft_einsatzgebiet>();
            Cbx_Personal_Thema = new ObservableCollection<wt2_konst_honorarkraft_thema>();

            LookupRepository LookRepo = new LookupRepository();

            Cbx_Personal_Teams = LookRepo.GetTeams(true, "", true);
            Cbx_Personal_Abteilung = LookRepo.GetAbteilungen(true);
            Cbx_Personal_Bildungstraeger = LookRepo.GetBildungstraeger(true);
            Cbx_Personal_Einsatzgebiet = hDAO.HoleEinsatzgebiete(true);
            Cbx_Personal_Thema = hDAO.HoleThema(false);
        }
  
        public void Reset()
        {
            MessageBox.Show("It works");
        }

        public void Search()
        {
            MessageBox.Show(Tbx_Personal_Name + " / " 
                + Tbx_Personal_Vorname + " / " 
                + Tbx_Personal_Firma + " / "
                + Environment.NewLine + Chkbx_Status_Selbstaendig + " / " 
                + Chkbx_Status_Verfolgen + " / " 
                + Chkbx_Status_Pruefen + " / " 
                + Chkbx_Status_bedenklich);
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
