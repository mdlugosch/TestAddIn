using BFZ_Common_Lib.MVVM;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;
using PostSharp.Patterns.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TIS3_Base;
using TIS3_Base.Services;
using TIS3_LookupBL;
using TIS3_WPF_TestMusterAddIn.Infrastructure;
using WinTIS30db_entwModel.Honorarkraefte;
using WinTIS30db_entwModel.Lookup;

namespace TIS3_WPF_TestMusterAddIn.ViewModels
{
    [NotifyPropertyChanged]
    public class PersonaldatenViewModel : TIS3ActiveViewModel, INavigationAware
    {
        // Laden des aktuellen RegionManagers
        public IRegionManager regionManager = ServiceLocator.Current.GetInstance<IRegionManager>();           

        # region Data Access Objecte für Honorarkraefte Tabellen
        LookupRepository LookRepo = new LookupRepository();
        HonorarkraefteDAO hDAO = HonorarkraefteDAO.DAOFactory();
        # endregion

        # region Auswahlliste
        public ObservableCollection<wt2_honorarkraft> HonorarListe { get; set; }
        public wt2_honorarkraft SelectedHonorar { get; set; } 
        # endregion

        # region Elemente der Personaldaten-Suchmaske
        public bool IsBusy { get; set; }
        public String Tbx_Personal_Name{ get; set; }
        public String Tbx_Personal_Vorname{ get; set; }
        public String Tbx_Personal_Firma{ get; set; }
        public bool Chkbx_Status_Selbstaendig{ get; set; }
        public bool Chkbx_Status_Verfolgen{ get; set; }
        public bool Chkbx_Status_Pruefen{ get; set; }
        public bool Chkbx_Status_bedenklich { get; set; }
        # endregion

        # region Comboboxen und zugehörige Combobox-Selection-Properties
        // Die Combobox-Selection-Properties ermöglichen das festhalten/steuern der Combobox-Selection
        public int SelectedItem_Personal_Teams { get; set; }
        public LookupCollectionBO Cbx_Personal_Teams { get; set; }
        public int SelectedItem_Personal_Abteilung { get; set; }
        public LookupCollectionBO Cbx_Personal_Abteilung { get; set; }
        public int SelectedItem_Personal_Einsatzgebiet { get; set; }
        public ObservableCollection<wt2_konst_honorarkraft_einsatzgebiet> Cbx_Personal_Einsatzgebiet { get; set; }
        public int SelectedItem_Personal_Bildungstraeger { get; set; }
        public ObservableCollection<Bildungstraeger> Cbx_Personal_Bildungstraeger { get; set; }
        public int SelectedItem_Personal_Thema { get; set; }
        public ObservableCollection<wt2_konst_honorarkraft_thema> Cbx_Personal_Thema { get; set; }
        # endregion

        # region Commands der Personaldaten-Suchmaske
        public RelayCommand OpenEditViewCommand { get; set; }
        public RelayCommand ResetCommand { get; set; }
        public RelayCommand SearchCommand { get; set; }
        # endregion

        # region Initialisierung PersonaldatenViewModel
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
            # region Combobox-Properties deklarieren
            Cbx_Personal_Teams = new LookupCollectionBO();
            Cbx_Personal_Abteilung = new LookupCollectionBO();
            Cbx_Personal_Bildungstraeger = new ObservableCollection<Bildungstraeger>();
            Cbx_Personal_Einsatzgebiet = new ObservableCollection<wt2_konst_honorarkraft_einsatzgebiet>();
            Cbx_Personal_Thema = new ObservableCollection<wt2_konst_honorarkraft_thema>();
            # endregion

            # region Daten in Comboboxen laden
            Cbx_Personal_Teams = LookRepo.GetTeams(true, "", true,true);
            Cbx_Personal_Abteilung = LookRepo.GetAbteilungen(true);
            Cbx_Personal_Bildungstraeger = LookRepo.GetBildungstraeger(true);
            Cbx_Personal_Einsatzgebiet = hDAO.HoleEinsatzgebiete(true);
            Cbx_Personal_Thema = hDAO.HoleThema(false);
            # endregion

            // Comboboxen zum Programmstart auf ersten Eintrag stellen
            ResetComboBoxes();
        }    

        public void ResetComboBoxes()
        {
            # region Comboboxen auf den ersten Eintrag stellen
            SelectedItem_Personal_Teams = 0;
            SelectedItem_Personal_Abteilung = 0;
            SelectedItem_Personal_Einsatzgebiet = 0;
            SelectedItem_Personal_Bildungstraeger = 0;
            SelectedItem_Personal_Thema = 1;
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
            Tbx_Personal_Name = "";
            Tbx_Personal_Vorname = "";
            Tbx_Personal_Firma = "";
            Chkbx_Status_Selbstaendig = false;
            Chkbx_Status_Verfolgen = false;
            Chkbx_Status_Pruefen = false;
            Chkbx_Status_bedenklich = false;
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
                HonorarListe = hDAO.PersonendatenSuche(this);
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
            navigationParameters.Add("ID", ((wt2_honorarkraft)dataObj).hk_ident);
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
