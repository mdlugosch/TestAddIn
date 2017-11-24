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
using TIS3_WPF_TestMusterAddIn.Infrastructure;
using WinTIS30db_entwModel.Honorarkraefte;

namespace TIS3_WPF_TestMusterAddIn.ViewModels
{
    [NotifyPropertyChanged]
    public class UeberpruefungsViewModel : TIS3ActiveViewModel, INavigationAware
    {
        // Laden des aktuellen RegionManagers
        public IRegionManager regionManager = ServiceLocator.Current.GetInstance<IRegionManager>();

        # region Data Access Objecte für Honorarkraefte Tabellen
        HonorarkraefteDAO hDAO;
        # endregion

        # region MenuCommands der Bewertungsbogen-Suchmaske
        public bool IsBusy { get; set; }
        public RelayCommand OpenEditViewCommand { get; set; }
        public RelayCommand ResetCommand { get; set; }
        public RelayCommand SearchCommand { get; set; }
        # endregion

        # region Auswahlliste
        public ObservableCollection<wt2_honorarkraft> HonorarListe { get; set; }
        # endregion

        # region Comboboxen und zugehörige Combobox-Selection-Properties
        public int SelectedItem_Pruefung_Jahr { get; set; }
        public ObservableCollection<string> Cbx_Pruefung_Jahr { get; set; }
        # endregion

        # region Initialisierung BewertungsbogenViewModel
        public UeberpruefungsViewModel() : base()
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

        public void InitComboBoxes()
        {
            Cbx_Pruefung_Jahr = HoleJahresliste(true);

            ResetComboBoxes();
        }

        # region Comboboxen auf den ersten Eintrag stellen
        public void Reset()
        { 
            ResetComboBoxes();
        }
        public void ResetComboBoxes()
        {       
            SelectedItem_Pruefung_Jahr = 0;      
        }
        # endregion

        public ObservableCollection<string> HoleJahresliste(Boolean leeresElement)
        {
            ObservableCollection<string> result = new ObservableCollection<string>();

            if (leeresElement)
            {
                result.Add("");
            }

            DateTime localDate = DateTime.Now;

            for (int i = 2001; i <= (localDate.Year) + 2; i++)
            {
                result.Add(i.ToString());
            }

            return result;
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
                HonorarListe = hDAO.UeberpruefungsSuche(this, this.ViewModelGuid);
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
