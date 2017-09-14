using BFZ_Common_Lib.MVVM;
using Microsoft.Practices.Prism.Regions;
using PostSharp.Patterns.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TIS3_Base;
using TIS3_LookupBL;
using TIS3_WPF_TestMusterAddIn.Infrastructure;
using WinTIS30db_entwModel.Honorarkraefte;
using WinTIS30db_entwModel.Lookup;

namespace TIS3_WPF_TestMusterAddIn.ViewModels
{
    [NotifyPropertyChanged]
    class BewertungsbogenViewModel : TIS3ActiveViewModel, INavigationAware
    {
        # region Data Access Objecte für Honorarkraefte Tabellen
        LookupRepository LookRepo = new LookupRepository();
        HonorarkraefteDAO hDAO = HonorarkraefteDAO.DAOFactory();
        # endregion

        # region Testdaten
        public ObservableCollection<wt2_honorarkraft> HonorarListe { get; set; }
        # endregion

        # region Elemente der Personaldaten-Suchmaske
        public String Tbx_Bewertung_Name{ get; set; }
        public String Tbx_Bewertung_Vorname{ get; set; }
        public String Tbx_Bewertung_Firma{ get; set; }
        public String Tbx_Bewertung_TNVon{ get; set; }
        public String Tbx_Bewertung_TNBis{ get; set; }
        public String Tbx_Bewertung_TLVon{ get; set; }
        public String Tbx_Bewertung_TLBis{ get; set; }
        public String Tbx_Bewertung_TerminVon{ get; set; }
        public String Tbx_Bewertung_TerminBis{ get; set; }
        public bool Chkbx_Bewertung_Erforderlich { get; set; }
        # endregion

        # region Comboboxen und zugehörige Combobox-Selection-Properties
        // Die Combobox-Selection-Properties ermöglichen das festhalten/steuern der Combobox-Selection
        public int SelectedItem_Bewertung_Abteilung{ get; set; }
        public LookupCollectionBO Cbx_Bewertung_Abteilung { get; set; }
        public int SelectedItem_Bewertung_Bildungstraeger{ get; set; }
        public ObservableCollection<Bildungstraeger> Cbx_Bewertung_Bildungstraeger { get; set; }
        public int SelectedItem_Bewertung_Jahr { get; set; }
        public ObservableCollection<string> Cbx_Bewertung_Jahr { get; set; }
        #endregion

        # region MenuCommands der Bewertungsbogen-Suchmaske
        public RelayCommand ResetCommand { get; set; }
        public RelayCommand SearchCommand { get; set; }
        # endregion

        # region Initialisierung BewertungsbogenViewModel
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
            # region Combobox-Properties deklarieren
            Cbx_Bewertung_Abteilung = new LookupCollectionBO();
            Cbx_Bewertung_Bildungstraeger = new ObservableCollection<Bildungstraeger>();
            # endregion

            # region Daten in Comboboxen laden
            Cbx_Bewertung_Abteilung = LookRepo.GetAbteilungen(true);
            Cbx_Bewertung_Bildungstraeger = LookRepo.GetBildungstraeger(true);
            Cbx_Bewertung_Jahr = HoleJahresliste(true);
            # endregion

            // Comboboxen zum Programmstart auf ersten Eintrag stellen
            ResetComboBoxes();
        }

        public void ResetComboBoxes()
        {
            # region Comboboxen auf den ersten Eintrag stellen    
            SelectedItem_Bewertung_Abteilung = 0;
            SelectedItem_Bewertung_Bildungstraeger = 0;
            SelectedItem_Bewertung_Jahr = 0;
            # endregion
        }

        public ObservableCollection<string> HoleJahresliste(Boolean leeresElement)
        {
            ObservableCollection<string> result = new ObservableCollection<string>();

            if (leeresElement)
            {
                result.Add("");
            }

            DateTime localDate = DateTime.Now;

            for (int i = 2014; i <= (localDate.Year) + 2; i++)
            {
                result.Add(i.ToString());
            }

            return result;
        }
        # endregion

        # region zu Command gehörige Methoden
        public void Reset()
        {
            # region Comboboxen wieder auf ersten Eintrag stellen
            ResetComboBoxes();
            # endregion
            # region Restliche Maskenelemente zurücksetzen
            Tbx_Bewertung_Name = "";
            Tbx_Bewertung_Vorname = "";
            Tbx_Bewertung_Firma = "";
            Tbx_Bewertung_TNVon = "";
            Tbx_Bewertung_TNBis = "";
            Tbx_Bewertung_TLVon = "";
            Tbx_Bewertung_TLBis = "";
            Tbx_Bewertung_TerminVon = "";
            Tbx_Bewertung_TerminBis = "";
            Chkbx_Bewertung_Erforderlich = false;
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
