using BFZ_Common_Lib.MVVM;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;
using PostSharp.Patterns.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
    public class BewertungsbogenViewModel : TIS3ActiveViewModel, INavigationAware
    {
        // Laden des aktuellen RegionManagers
        public IRegionManager regionManager = ServiceLocator.Current.GetInstance<IRegionManager>();

        # region Data Access Objecte für Honorarkraefte Tabellen
        LookupRepository LookRepo = new LookupRepository();
        HonorarkraefteDAO hDAO;
        # endregion

        string personId;

        # region Auswahlliste
        public ObservableCollection<wt2_honorarkraft_bewertung> HonorarListe { get; set; }
        # endregion

        # region Elemente der Personaldaten-Suchmaske
        public bool IsBusy { get; set; }
        public String Tbx_Bewertung_Name{ get; set; }
        public String Tbx_Bewertung_Vorname{ get; set; }
        public String Tbx_Bewertung_Firma{ get; set; }
        public String Tbx_Bewertung_TNVon { get; set; }
        public String Tbx_Bewertung_TNBis { get; set; }
        public String Tbx_Bewertung_TLVon { get; set; }
        public String Tbx_Bewertung_TLBis { get; set; }
        public String Dp_Bewertung_TerminVon{ get; set; }
        public String Dp_Bewertung_TerminBis{ get; set; }
        # endregion

        # region Comboboxen und zugehörige Combobox-Selection-Properties
        // Die Combobox-Selection-Properties ermöglichen das festhalten/steuern der Combobox-Selection
        public int SelectedItem_Bewertung_Abteilung{ get; set; }
        public LookupCollectionBO Cbx_Bewertung_Abteilung { get; set; }
        public int SelectedItem_Bewertung_Bildungstraeger{ get; set; }
        public ObservableCollection<Bildungstraeger> Cbx_Bewertung_Bildungstraeger { get; set; }
        #endregion

        # region MenuCommands der Bewertungsbogen-Suchmaske
        public RelayCommand OpenEditViewCommand { get; set; }
        public RelayCommand ResetCommand { get; set; }
        public RelayCommand SearchCommand { get; set; }
        # endregion

        # region Initialisierung BewertungsbogenViewModel
        public BewertungsbogenViewModel() : base() 
        {
           // Aktuelle Guid übergeben um ViewModel und Dao mit einem Context zu verbinden.
           hDAO = new HonorarkraefteDAO(this.ViewModelGuid);
           InitComboBoxes();
        }
        public override void Init()
        {
            this.IsBusy = false;
            this.OpenEditViewCommand = new RelayCommand(_execute => this.OpenEditView(_execute), _canExecute => true);
            ResetCommand = new RelayCommand(_execute => { Reset(); }, _canExecute => { return true; });
            SearchCommand = new RelayCommand(_execute => { Search(); }, _canExecute => { return true; });       
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
            # endregion

            // Comboboxen zum Programmstart auf ersten Eintrag stellen
            ResetComboBoxes();
        }

        public void ResetComboBoxes()
        {
            # region Comboboxen auf den ersten Eintrag stellen    
            SelectedItem_Bewertung_Abteilung = 0;
            SelectedItem_Bewertung_Bildungstraeger = 0;
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
            Tbx_Bewertung_Name = "";
            Tbx_Bewertung_Vorname = "";
            Tbx_Bewertung_Firma = "";
            Tbx_Bewertung_TNVon = "";
            Tbx_Bewertung_TNBis = "";
            Tbx_Bewertung_TLVon = "";
            Tbx_Bewertung_TLBis = "";
            Dp_Bewertung_TerminVon = "";
            Dp_Bewertung_TerminBis = "";
            # endregion
        }

        public void Search()
        {
            // Wir erwarten eine längere Aktion. Also ein Busy signalisieren:
            this.IsBusy = true;

            // Die UI kann nur aktualisiert werden (für den Busy Indicator), wenn
            // die weitere Bearbeitung in einem weiteren Thread läuft. Also einen
            // Backgroundworker anlegen und ihn die Arbeit tun lassen:
            BackgroundWorker worker = new BackgroundWorker();

            worker.DoWork += delegate(object sender, DoWorkEventArgs e)
            {
                HonorarListe = hDAO.BewertungbogenSuche(this, this.ViewModelGuid);
            };

            // wird ausgeführt, wenn der Worker fertig ist:
            worker.RunWorkerCompleted += delegate(object sender, RunWorkerCompletedEventArgs e)
            {
                this.IsBusy = false;
            };
            // den Worker nun starten:
            worker.RunWorkerAsync();

        }

        private void OpenEditView(object dataObj)
        {
            var navigationParameters = new NavigationParameters();
            navigationParameters.Add("ID", ((wt2_honorarkraft_bewertung)dataObj).wt2_honorarkraft.hk_ident);
            regionManager.RequestNavigate(CompositionPoints.Regions.MainWorkspace, new Uri("/EditView" + navigationParameters.ToString(), UriKind.Relative));
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
