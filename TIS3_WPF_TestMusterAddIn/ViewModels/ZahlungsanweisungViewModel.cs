using BFZ_Common_Lib.MVVM;
using Microsoft.Practices.Prism.Regions;
using PostSharp.Patterns.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    class ZahlungsanweisungViewModel : TIS3ActiveViewModel, INavigationAware
    {
        # region Data Access Objecte für Honorarkraefte Tabellen
        HonorarkraefteDAO hDAO = HonorarkraefteDAO.DAOFactory();
        LookupRepository LookRepo = new LookupRepository();
        # endregion

        # region Testdaten
        public ObservableCollection<wt2_honorarkraft> HonorarListe { get; set; }
        # endregion

        # region Elemente der Zahlungsanweisung-Suchmaske

        public String Tbx_Zahlung_Name{ get; set; }
        public String Tbx_Zahlung_Vorname{ get; set; }
        public String Tbx_Zahlung_Firma{ get; set; }
        public String Tbx_Zahlung_Nummer{ get; set; }
        public String Tbx_Zahlung_Datum{ get; set; }
        public String Tbx_Zahlung_Auftrag{ get; set; }
        public bool Chkbx_Zahlung_Gedruckt { get; set; }

        public int SelectedItem_Zahlung_Teams { get; set; }
        public LookupCollectionBO Cbx_Zahlung_Teams { get; set; }
        public int SelectedItem_Zahlung_Abteilung { get; set; }
        public LookupCollectionBO Cbx_Zahlung_Abteilung { get; set; }
        public int SelectedItem_Zahlung_Bildungstraeger { get; set; }
        public ObservableCollection<Bildungstraeger> Cbx_Zahlung_Bildungstraeger { get; set; }
        public int SelectedItem_Zahlung_Thema { get; set; }
        public ObservableCollection<wt2_konst_honorarkraft_thema> Cbx_Zahlung_Thema { get; set; }
        # endregion

        # region MenuCommands der Zahlungsanweisung-Suchmaske
        public RelayCommand ResetCommand { get; set; }
        public RelayCommand SearchCommand { get; set; }
        # endregion

        # region Initialisierung ZahlungsanweisungViewModel
        public override void Init()
        {
            # region Testdaten initializieren
            HonorarListe = hDAO.LoadTestdata();
            # endregion

            ResetCommand = new RelayCommand(_execute => { Reset(); }, _canExecute => { return true; });
            SearchCommand = new RelayCommand(_execute => { Search(); }, _canExecute => { return true; }); 
            InitComboBoxes();
        }
        # endregion

        # region allgemeine Service-Methoden
        public void InitComboBoxes()
        {
            Cbx_Zahlung_Teams = new LookupCollectionBO();
            Cbx_Zahlung_Abteilung = new LookupCollectionBO();
            Cbx_Zahlung_Bildungstraeger = new ObservableCollection<Bildungstraeger>();
            Cbx_Zahlung_Thema = new ObservableCollection<wt2_konst_honorarkraft_thema>();

            Cbx_Zahlung_Teams = LookRepo.GetTeams(true, "", true);
            Cbx_Zahlung_Abteilung = LookRepo.GetAbteilungen(true);
            Cbx_Zahlung_Bildungstraeger = LookRepo.GetBildungstraeger(true);
            Cbx_Zahlung_Thema = hDAO.HoleThema(false);

            // Comboboxen zum Programmstart auf ersten Eintrag stellen
            ResetComboBoxes();
        }

        public void ResetComboBoxes()
        {
            # region Comboboxen auf den ersten Eintrag stellen
            SelectedItem_Zahlung_Teams = 0;
            SelectedItem_Zahlung_Abteilung = 0;
            SelectedItem_Zahlung_Bildungstraeger = 0;
            SelectedItem_Zahlung_Thema = 0;
            # endregion
        }
        # endregion

        # region zu Command gehörige Methoden
        public void Reset()
        {
            # region Comboboxen wieder auf ersten Eintrag stellen
            ResetComboBoxes();
            # endregion
            # region Restliche Maskenelemente zurücksetzen
            Tbx_Zahlung_Name = "";
            Tbx_Zahlung_Vorname = "";
            Tbx_Zahlung_Firma = "";
            Tbx_Zahlung_Nummer = "";
            Tbx_Zahlung_Datum = "";
            Tbx_Zahlung_Auftrag = "";
            Chkbx_Zahlung_Gedruckt = false;
            # endregion
        }

        public void Search()
        {
            MessageBox.Show("It works!");
        }
        # endregion

        # region Prism-Navigation-Einstellungen
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
        # endregion
    }
}
