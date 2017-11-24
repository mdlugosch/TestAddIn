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
using System.Windows;
using TIS3_Base;
using TIS3_LookupBL;
using TIS3_WPF_TestMusterAddIn.Infrastructure;
using WinTIS30db_entwModel.Honorarkraefte;
using WinTIS30db_entwModel.Lookup;

namespace TIS3_WPF_TestMusterAddIn.ViewModels
{
    [NotifyPropertyChanged]
    public class VertragsdatenViewModel : TIS3ActiveViewModel, INavigationAware
    {
        // Laden des aktuellen RegionManagers
        public IRegionManager regionManager = ServiceLocator.Current.GetInstance<IRegionManager>();

        # region Data Access Objecte für Honorarkraefte Tabellen
        LookupRepository LookRepo = new LookupRepository();
        HonorarkraefteDAO hDAO;
        # endregion

        # region Auswahlliste
        public ObservableCollection<wt2_honorarkraft_vertrag> HonorarListe { get; set; }
        # endregion

        # region Elemente der Vertragsdaten-Suchmaske
        public bool IsBusy { get; set; }
        public String Tbx_Vertrag_Name{ get; set; }
        public String Tbx_Vertrag_Vorname{ get; set; }
        public String Tbx_Vertrag_Firma{ get; set; }
        public String Tbx_Vertrag_Nummer{ get; set; }
        public String Dp_Vertrag_Datum{ get; set; }
        public String Tbx_Vertrag_Auftrag { get; set; }
        public String Tbx_Vertrag_Thema { get; set; }
        public bool Chkbx_Vertrag_Gedruckt{ get; set; }
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
        # endregion

        # region MenuCommands der Vertragsdaten-Suchmaske
        public RelayCommand OpenEditViewCommand { get; set; }
        public RelayCommand ResetCommand { get; set; }
        public RelayCommand SearchCommand { get; set; }
        # endregion

        # region Initialisierung VertragsdatenViewModel
        public VertragsdatenViewModel() : base() 
        {
            // Aktuelle Guid übergeben um ViewModel und Dao mit einem Context zu verbinden.
            hDAO = new HonorarkraefteDAO(this.ViewModelGuid);
            /*
             * InitComboBoxes muss im Konstruktor stehen damit die Guid nicht null ist.
             * Die Init-Methode wäre der falsche Ort für InitComboBoxes da diese zu früh
             * initialisiert werden würde zu einem Zeitpunkt an dem die Guid noch Null ist.
             */
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
            Cbx_Vertrag_Teams = new LookupCollectionBO();
            Cbx_Vertrag_Abteilung = new LookupCollectionBO();
            Cbx_Vertrag_Bildungstraeger = new ObservableCollection<Bildungstraeger>();

            Cbx_Vertrag_Teams = LookRepo.GetTeams(true, "", true,true);
            Cbx_Vertrag_Abteilung = LookRepo.GetAbteilungen(true);
            Cbx_Vertrag_Bildungstraeger = LookRepo.GetBildungstraeger(true);

            // Comboboxen zum Programmstart auf ersten Eintrag stellen
            ResetComboBoxes();
        }

        public void ResetComboBoxes()
        {
            # region Comboboxen auf den ersten Eintrag stellen
            SelectedItem_Vertrag_Teams = 0;
            SelectedItem_Vertrag_Abteilung = 0;
            SelectedItem_Vertrag_Bildungstraeger = 0;
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
            Dp_Vertrag_Datum = "";
            Tbx_Vertrag_Auftrag = "";
            Chkbx_Vertrag_Gedruckt = false;
            Chkbx_Vertrag_400 = false;
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
                HonorarListe = hDAO.VertragsdatenSuche(this, this.ViewModelGuid);
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
            navigationParameters.Add("ID", ((wt2_honorarkraft_vertrag)dataObj).wt2_honorarkraft.hk_ident);
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
