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
    class VertragsdatenViewModel : TIS3ActiveViewModel, INavigationAware
    {
        # region Data Access Objecte für Honorarkraefte Tabellen
        LookupRepository LookRepo = new LookupRepository();
        HonorarkraefteDAO hDAO = HonorarkraefteDAO.DAOFactory();
        # endregion

        # region Testdaten
        public ObservableCollection<wt2_honorarkraft> HonorarListe { get; set; }
        # endregion

        # region Elemente der Vertragsdaten-Suchmaske
        public String Tbx_Vertrag_Name{ get; set; }
        public String Tbx_Vertrag_Vorname{ get; set; }
        public String Tbx_Vertrag_Firma{ get; set; }
        public String Tbx_Vertrag_Nummer{ get; set; }
        public String Tbx_Vertrag_Datum{ get; set; }
        public String Tbx_Vertrag_Auftrag { get; set; }
        public bool Chkbx_Vertrag_Gedruckt{ get; set; }
        public bool Chkbx_Vertrag_Ausgezahlt{ get; set; }
        public bool Chkbx_Vertrag_400 { get; set; }
        # endregion

        # region Comboboxen und zugehörige Combobox-Selection-Properties
        // Die Combobox-Selection-Properties ermöglichen das festhalten/steuern der Combobox-Selection
        public int SelectedItem_Vertrag_Teams { get; set; }
        public LookupCollectionBO Cbx_Vertrag_Teams { get; set; }
        public int SelectedItem_Vertrag_Abteilung { get; set; }
        public LookupCollectionBO Cbx_Vertrag_Abteilung { get; set; }
        public int SelectedItem_Vertrag_Bildungstraeger { get; set; }
        public ObservableCollection<Bildungstraeger> Cbx_Vertrag_Bildungstraeger { get; set; }
        public int SelectedItem_Vertrag_Thema { get; set; }
        public ObservableCollection<wt2_konst_honorarkraft_thema> Cbx_Vertrag_Thema { get; set; }
        # endregion

        # region MenuCommands der Vertragsdaten-Suchmaske
        public RelayCommand ResetCommand { get; set; }
        public RelayCommand SearchCommand { get; set; }
        # endregion

        # region Initialisierung VertragsdatenViewModel
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
            Cbx_Vertrag_Teams = new LookupCollectionBO();
            Cbx_Vertrag_Abteilung = new LookupCollectionBO();
            Cbx_Vertrag_Bildungstraeger = new ObservableCollection<Bildungstraeger>();
            Cbx_Vertrag_Thema = new ObservableCollection<wt2_konst_honorarkraft_thema>();

            Cbx_Vertrag_Teams = LookRepo.GetTeams(true, "", true);
            Cbx_Vertrag_Abteilung = LookRepo.GetAbteilungen(true);
            Cbx_Vertrag_Bildungstraeger = LookRepo.GetBildungstraeger(true);
            Cbx_Vertrag_Thema = hDAO.HoleThema(false);

            // Comboboxen zum Programmstart auf ersten Eintrag stellen
            ResetComboBoxes();
        }

        public void ResetComboBoxes()
        {
            # region Comboboxen auf den ersten Eintrag stellen
            SelectedItem_Vertrag_Teams = 0;
            SelectedItem_Vertrag_Abteilung = 0;
            SelectedItem_Vertrag_Bildungstraeger = 0;
            SelectedItem_Vertrag_Thema = 0;
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
            Tbx_Vertrag_Name = "";
            Tbx_Vertrag_Vorname = "";
            Tbx_Vertrag_Firma = "";
            Tbx_Vertrag_Nummer = "";
            Tbx_Vertrag_Datum = "";
            Tbx_Vertrag_Auftrag = "";
            Chkbx_Vertrag_Gedruckt = false;
            Chkbx_Vertrag_Ausgezahlt = false;
            Chkbx_Vertrag_400 = false;
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
