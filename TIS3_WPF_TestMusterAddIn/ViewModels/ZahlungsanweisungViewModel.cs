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
    public class ZahlungsanweisungViewModel : TIS3ActiveViewModel, INavigationAware
    {
        // Laden des aktuellen RegionManagers
        public IRegionManager regionManager = ServiceLocator.Current.GetInstance<IRegionManager>();

        # region Data Access Objecte für Honorarkraefte Tabellen
        HonorarkraefteDAO hDAO = HonorarkraefteDAO.DAOFactory();
        LookupRepository LookRepo = new LookupRepository();
        # endregion

        # region Auswahlliste
        public ObservableCollection<wt2_honorarkraft_zahlungsanweisung> HonorarListe { get; set; }
        # endregion

        # region Elemente der Zahlungsanweisung-Suchmaske
        public bool IsBusy { get; set; }
        public String Tbx_Zahlung_Name{ get; set; }
        public String Tbx_Zahlung_Vorname{ get; set; }
        public String Tbx_Zahlung_Firma{ get; set; }
        public String Tbx_Zahlung_Nummer{ get; set; }
        public String Dp_Zahlung_Datum{ get; set; }
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

        # region Commands der Zahlungsanweisung-Suchmaske
        public RelayCommand OpenEditViewCommand { get; set; }
        public RelayCommand ResetCommand { get; set; }
        public RelayCommand SearchCommand { get; set; }
        # endregion

        # region Initialisierung ZahlungsanweisungViewModel
        public override void Init()
        {
            this.IsBusy = false;
            this.OpenEditViewCommand = new RelayCommand(_execute => this.OpenEditView(_execute), _canExecute => true);
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

            Cbx_Zahlung_Teams = LookRepo.GetTeams(true, "", true,true);
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
            SelectedItem_Zahlung_Thema = 1;
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
            Dp_Zahlung_Datum = "";
            Tbx_Zahlung_Auftrag = "";
            Chkbx_Zahlung_Gedruckt = false;
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
                HonorarListe = hDAO.ZahlungsanweisungSuche(this);
            };

            // wird ausgeführt, wenn der Worker fertig ist:
            worker.RunWorkerCompleted += delegate(object sender, RunWorkerCompletedEventArgs e)
            {
                this.IsBusy = false;
            };
            // den Worker nun starten:
            worker.RunWorkerAsync();
        }
        # endregion

        private void OpenEditView(object dataObj)
        {
            var navigationParameters = new NavigationParameters();
            navigationParameters.Add("ID", ((wt2_honorarkraft_zahlungsanweisung)dataObj).wt2_honorarkraft.hk_ident);
            regionManager.RequestNavigate(CompositionPoints.Regions.MainWorkspace, new Uri("/EditView" + navigationParameters.ToString(), UriKind.Relative));
        }

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
