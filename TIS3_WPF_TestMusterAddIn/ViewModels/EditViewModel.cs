using BFZ_Common_Lib.MVVM;
using BFZ_WPF_Lib.BusinessObjects;
using Microsoft.Practices.Prism.PubSubEvents;
using Microsoft.Practices.Prism.Regions;
using PostSharp.Patterns.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Technewlogic.WpfDialogManagement;
using TIS3_Base;
using TIS3_Base.Prism;
using TIS3_Base.Services;
using TIS3_LookupBL;
using TIS3_WPF_TestMusterAddIn.Events;
using TIS3_WPF_TestMusterAddIn.Infrastructure;
using TIS3_WPF_TestMusterAddIn.Views;
using WinTIS30db_entwModel.Honorarkraefte;
using WinTIS30db_entwModel.Lookup;

namespace TIS3_WPF_TestMusterAddIn.ViewModels
{
    public enum OptionList
    {
        J, N, X
    }

    public enum SVBefreiungOptionList
    {
        beantragt,
        liegtvor,
        leer
    }

    [NotifyPropertyChanged]
    class EditViewModel : TIS3ActiveViewModel, INavigationAware
    {

        # region Data Access Objecte für Honorarkraefte Tabellen
        LookupRepository LookRepo = new LookupRepository();
        HonorarkraefteDAO hDAO;
        # endregion

        // Personal-Id zur Dublikatvermeidung 
        string personId;

        // Zugriff auf EventaggregatorListe
        readonly IEventAggregator _aggregator = HonorarAddInAggregatorList.AggregatorFactory;

        public wt2_honorarkraft EditViewDatensatz { get; set; }
        public ObservableCollection<wt2_honorarkraft_vertrag> EditViewVerträge { get; set; }
        public ObservableCollection<wt2_honorarkraft_zahlungsanweisung> EditViewZahlungsanweisungen { get; set; }
        public ObservableCollection<wt2_honorarkraft_bewertung> EditViewBewertungen { get; set; }

        # region EditViewCommands-Bereich: Menu
            public RelayCommand SaveCommand { get; set; }
        # endregion

        # region EditViewCommands-Bereich: Personal
            public RelayCommand AddThemeCommand { get; set; }
        # endregion

        # region EditViewCommands-Bereich: Verträge
            public RelayCommand DeleteVertragCommand { get; set; }
            public RelayCommand DeleteVertragPositionCommand { get; set; }
            public RelayCommand NewVertragCommand { get; set; }
            public RelayCommand NewVertragsPositionCommand { get; set; }
            public RelayCommand EditVertragsPositionCommand { get; set; }
        # endregion

        # region EditViewCommands-Bereich: Zahlungsanweisungen
            public RelayCommand NewZahlungsanweisungCommand { get; set; }
            public RelayCommand NewZahlungsanweisungsPositionCommand { get; set; }
            public RelayCommand EditZahlungsanweisungsPositionCommand { get; set; }
            public RelayCommand DeleteZahlungsanweisungCommand { get; set; }
            public RelayCommand DeleteZahlungsanweisungsPositionCommand { get; set; }
        # endregion

        # region EditViewCommands-Bereich: Zahlungsanweisungen
            public RelayCommand NewBewertungCommand { get; set; }
            public RelayCommand DeleteBewertungCommand { get; set; }
        # endregion

        # region EditView Properties

        # region 1/6 Regelung Radiobuttonfeld
            public OptionList regelungOption;
        public OptionList RegelungOption 
        { 
            get 
            {
                if (Honorarkraft_Status != null)
                    switch (Honorarkraft_Status.hks_1_6_Regelung)
                    {
                        case "J":
                            RegelungOption = OptionList.J;
                            break;
                        case "N":
                            RegelungOption = OptionList.N;
                            break;
                        case "":
                            RegelungOption = OptionList.X;
                            break;
                    }
                    else RegelungOption = OptionList.X;
                return regelungOption; 
            } 
            set 
            { 
                regelungOption = value;
                if (Honorarkraft_Status != null)
                switch (regelungOption) 
                {
                    case OptionList.J:
                        Honorarkraft_Status.hks_1_6_Regelung = "J";
                        break;
                    case OptionList.N:
                        Honorarkraft_Status.hks_1_6_Regelung = "N";
                        break;
                    case OptionList.X:
                        Honorarkraft_Status.hks_1_6_Regelung = "";
                        break;
                } 
            } 
        }
        # endregion

        # region ArbN Radiobuttonfeld
        public OptionList arbNOption;
        public OptionList ArbNOption
        {
            get
            {
                if (Honorarkraft_Status != null)
                    switch (Honorarkraft_Status.hks_arbn_beschaeftige)
                    {
                        case "J":
                            ArbNOption = OptionList.J;
                            break;
                        case "N":
                            ArbNOption = OptionList.N;
                            break;
                        case "":
                            ArbNOption = OptionList.X;
                            break;
                    } else ArbNOption = OptionList.X;
                return arbNOption;
            }
            set
            {
                arbNOption = value;
                if (Honorarkraft_Status != null)
                switch (arbNOption)
                {
                    case OptionList.J:
                        Honorarkraft_Status.hks_arbn_beschaeftige = "J";
                        break;
                    case OptionList.N:
                        Honorarkraft_Status.hks_arbn_beschaeftige = "N";
                        break;
                    case OptionList.X:
                        Honorarkraft_Status.hks_arbn_beschaeftige = "";
                        break;
                }
            }
        }
        # endregion

        # region Umst Radiobuttonfeld
        public OptionList umstOption;
        public OptionList UmstOption
        {
            get
            {
                if (Honorarkraft_Status != null)
                    switch (Honorarkraft_Status.hks_umsatzsteuerpflichtiger_unternehmer)
                    {
                        case "J":
                            UmstOption = OptionList.J;
                            break;
                        case "N":
                            UmstOption = OptionList.N;
                            break;
                        case "":
                            UmstOption = OptionList.X;
                            break;
                    } else UmstOption = OptionList.X;
                return umstOption;
            }
            set
            {
                umstOption = value;
                if (Honorarkraft_Status != null)
                switch (umstOption)
                {
                    case OptionList.J:
                        Honorarkraft_Status.hks_umsatzsteuerpflichtiger_unternehmer = "J";
                        break;
                    case OptionList.N:
                        Honorarkraft_Status.hks_umsatzsteuerpflichtiger_unternehmer = "N";
                        break;
                    case OptionList.X:
                        Honorarkraft_Status.hks_umsatzsteuerpflichtiger_unternehmer = "";
                        break;
                }
            }
        }
        # endregion

        # region SVPflicht Radiobuttonfeld
        public OptionList sVPflichtOption;
        public OptionList SVPflichtOption
        {
            get
            {
                if (Honorarkraft_Status != null)
                    switch (Honorarkraft_Status.hks_sv_pflichtbeitraege)
                    {
                        case "J":
                            SVPflichtOption = OptionList.J;
                            break;
                        case "N":
                            SVPflichtOption = OptionList.N;
                            break;
                        case "":
                            SVPflichtOption = OptionList.X;
                            break;
                    } else SVPflichtOption = OptionList.X;
                return sVPflichtOption;
            }
            set
            {
                sVPflichtOption = value;
                if (Honorarkraft_Status != null)
                switch (sVPflichtOption)
                {
                    case OptionList.J:
                        Honorarkraft_Status.hks_sv_pflichtbeitraege = "J";
                        break;
                    case OptionList.N:
                        Honorarkraft_Status.hks_sv_pflichtbeitraege = "N";
                        break;
                    case OptionList.X:
                        Honorarkraft_Status.hks_sv_pflichtbeitraege = "";
                        break;
                }
            }
        }
        # endregion

        # region Einsatzvolument Radiobuttonfeld
        public OptionList einsatzvolumenOption;
        public OptionList EinsatzvolumenOption
        {
            get
            {
                if (Honorarkraft_Status != null)
                    switch (Honorarkraft_Status.hks_einsatzvolumen_80_stunden)
                    {
                        case "J":
                            EinsatzvolumenOption = OptionList.J;
                            break;
                        case "N":
                            EinsatzvolumenOption = OptionList.N;
                            break;
                        case "":
                            EinsatzvolumenOption = OptionList.X;
                            break;
                    } else einsatzvolumenOption = OptionList.X;
                return einsatzvolumenOption;
            }
            set
            {
                einsatzvolumenOption = value;
                if (Honorarkraft_Status != null)
                switch (einsatzvolumenOption)
                {
                    case OptionList.J:
                        Honorarkraft_Status.hks_einsatzvolumen_80_stunden = "J";
                        break;
                    case OptionList.N:
                        Honorarkraft_Status.hks_einsatzvolumen_80_stunden = "N";
                        break;
                    case OptionList.X:
                        Honorarkraft_Status.hks_einsatzvolumen_80_stunden = "";
                        break;
                }
            }
        }
        # endregion

        public string Lbl_Personal_Bank
        {
            get;
            set;
        }

        public string Lbl_SaveDateTime 
        { 
            get; 
            set; 
        }

        public string Tbx_Personal_Blz 
        {
            get
            {
                if (Depends.Guard)
                { Depends.On(EditViewDatensatz); }
                return EditViewDatensatz.hk_blz; 
            }
            set 
            {
                EditViewDatensatz.hk_blz = value;
                this.Lbl_Personal_Bank = Check_BLZ(value);             
            }
        }

        # region SVBefreiung Radiobuttonfeld
        /*
         * SVBefreiung unterscheidet sich von den anderen Radiobuttonfeldern da hier
         * jeder der beiden Radiobuttons auf ein eigenes Attribut in der Tabelle zugreift.
         * Ist also "beantragt" wahr ist "liegt vor" falsch und andersherum. Wird das Enum
         * umgestellt ist es nötig zwei Attribute gleichzeitig zu ändern um diese Logik
         * abzubilden.
         */
        public SVBefreiungOptionList sVBefreiungOption;
        public SVBefreiungOptionList SVBefreiungOption
        { 
            get 
            {
                if (Honorarkraft_Status != null) 
                { 
                if (Honorarkraft_Status.hks_sv_befreiung_beantragt=="J") 
                { 
                    sVBefreiungOption = SVBefreiungOptionList.beantragt;
                }
                else if (Honorarkraft_Status.hks_sv_befreiung_liegt_vor=="J") 
                {
                    sVBefreiungOption = SVBefreiungOptionList.liegtvor;
                }
                else sVBefreiungOption = SVBefreiungOptionList.leer;
                } else sVBefreiungOption = SVBefreiungOptionList.leer;
                return sVBefreiungOption;
            }
            set 
            {
                sVBefreiungOption = value;
                if (Honorarkraft_Status != null)
                if (sVBefreiungOption == SVBefreiungOptionList.beantragt) 
                {
                    Honorarkraft_Status.hks_sv_befreiung_beantragt = "J";
                    Honorarkraft_Status.hks_sv_befreiung_liegt_vor = "N";
                }
                else if (sVBefreiungOption == SVBefreiungOptionList.liegtvor) 
                {
                    Honorarkraft_Status.hks_sv_befreiung_beantragt = "N";
                    Honorarkraft_Status.hks_sv_befreiung_liegt_vor = "J";
                } 
            }
        }

        /*
         * Die Zuweisung von Chkbx_Info_SVBefreiung wäre auch direkt in XAML mit einem Converter möglich,
         * Hier nehme ich den Umweg über eine Property um beim deaktivieren der Checkbox gleichzeitig die
         * verknüpften Radiobuttonwerte für "beantragt" und "liegt vor" leer zu schreiben. (set)
         */
        public bool Chkbx_Info_SVBefreiung
        {
            get
            {
                if (Honorarkraft_Status != null)
                {
                    if (Honorarkraft_Status.hks_sv_befreiung == "J")
                        return true;
                }
                return false;
            }
            set
            {
                if (Honorarkraft_Status != null)
                if (value) { Honorarkraft_Status.hks_sv_befreiung = "J"; }
                else
                {
                    sVBefreiungOption = SVBefreiungOptionList.leer;
                    Honorarkraft_Status.hks_sv_befreiung_beantragt = "";
                    Honorarkraft_Status.hks_sv_befreiung_liegt_vor = "";
                    Honorarkraft_Status.hks_sv_befreiung = "N";
                }
            }
        }
        # endregion

        #region Listen für Anrede und Titel die direkt in dieser Klasse generiert werden.
            public List<string> anredeListe = new List<string>() { "","Frau","Herr","Firma"};
            public List<string> titelListe = new List<string>() { "","Dr.","Prof."};
            public List<string> Cbx_Personal_Anrede { get { return anredeListe; } }
            public int SelectedItem_Personal_Anrede { get; set; }
            public List<string> Cbx_Personal_Titel { get { return titelListe; } }
            public string Current_Personal_Titel { get; set; }
            public int SelectedItem_Personal_Titel { get; set; }
        #endregion

        #endregion

        # region Properties für aktuell selektierte Haupttabellenitems
        public SyncObservableCollection<wt2_honorarkraft_vertrag_position> Vertragspositionen { get; set; }
        public SyncObservableCollection<wt2_honorarkraft_zahlungsanweisung_position> Zahlungspositionen { get; set; }
        public wt2_konst_honorarkraft_bewertung_vorgabe Bewertungsbogen_Berwertungsvorgaben { get; set; }
        wt2_honorarkraft_vertrag selectedItem_Vertrag_Grid;
        wt2_honorarkraft_vertrag_position selectedItem_VertragPosition_Grid;
        wt2_honorarkraft_zahlungsanweisung_position selectedItem_ZahlungsanweisungsPosition_Grid;

        // Beim setzen wird die Positionsliste automatisch mitgeneriert
        public wt2_honorarkraft_vertrag SelectedItem_Vertrag_Grid 
        { 
            get 
            { return selectedItem_Vertrag_Grid; } 
            set 
            { 
              selectedItem_Vertrag_Grid = value;
              if(selectedItem_Vertrag_Grid!=null)
              Vertragspositionen = new SyncObservableCollection<wt2_honorarkraft_vertrag_position>(selectedItem_Vertrag_Grid.wt2_honorarkraft_vertrag_position);          
            } 
        }

        // Referenzproperty für die selektierte Vertragsposition
        public wt2_honorarkraft_vertrag_position SelectedItem_VertragPosition_Grid 
        { 
            get 
            { return selectedItem_VertragPosition_Grid; } 
            set 
            { selectedItem_VertragPosition_Grid = value; } 
        }

        public wt2_honorarkraft_zahlungsanweisung_position SelectedItem_ZahlungsanweisungsPosition_Grid
        {
            get
            { return selectedItem_ZahlungsanweisungsPosition_Grid; }
            set
            { selectedItem_ZahlungsanweisungsPosition_Grid = value; }
        }

        wt2_honorarkraft_zahlungsanweisung selectedItem_Zahlung_Grid;
        public wt2_honorarkraft_zahlungsanweisung SelectedItem_Zahlung_Grid
        {
            get { return selectedItem_Zahlung_Grid; }
            set
            {
                selectedItem_Zahlung_Grid = value;
                if (selectedItem_Zahlung_Grid != null)
                Zahlungspositionen = new SyncObservableCollection<wt2_honorarkraft_zahlungsanweisung_position>(selectedItem_Zahlung_Grid.wt2_honorarkraft_zahlungsanweisung_position);
            }
        }

        wt2_honorarkraft_bewertung selectedItem_Bewertung_Grid;
        public wt2_honorarkraft_bewertung SelectedItem_Bewertung_Grid { 
            get { return selectedItem_Bewertung_Grid; } 
            set 
            { 
                selectedItem_Bewertung_Grid = value;
            } 
        }

        public wt2_honorarkraft_status Honorarkraft_Status { get; set; }

        # endregion

        public IEnumerable<object> Hk_Vertraege { get; set; }
        public ObservableCollection<SortedList_Thema> Tv_Themen { get; set; }
        public CheckedLookupCollection<wt2_konst_honorarkraft_einsatzgebiet> Lbx_Einsatzgebiete { get; set; }
        public ObservableCollection<Bildungstraeger> Cbx_Personal_Bildungstraeger { get; set; }
        public CheckedLookupCollectionBO Lbx_Personal_Abteilung { get; set; }
        public CheckedLookupCollectionBO Lbx_Personal_Teams { get; set; }
        public ObservableCollection<Bildungstraeger> Cbx_Vertrag_Bildungstraeger { get; set; }
        public LookupCollectionBO Cbx_Vertrag_Abteilung { get; set; }
        public LookupCollectionBO Cbx_Zahlung_Abteilung { get; set; }
        public ObservableCollection<Bildungstraeger> Cbx_Zahlung_Bildungstraeger { get; set; }
        public LookupCollectionBO Cbx_Bewertung_Abteilung { get; set; }
        public ObservableCollection<Bildungstraeger> Cbx_Bewertung_Bildungstraeger { get; set; }

        public bool Chkbx_Vertrag_400 { get; set; }

        # region Editview Init
        public EditViewModel() : base() 
        {
            // Aktuelle Guid übergeben um ViewModel und Dao mit einem Context zu verbinden.
            hDAO = new HonorarkraefteDAO(this.ViewModelGuid);

            /*
             * InitComboBoxes muss im Konstruktor stehen damit die Guid nicht null ist.
             * Die Init-Methode wäre der falsche Ort für InitComboBoxes da diese zu früh
             * initialisiert werden würde zu einem Zeitpunkt an dem die Guid noch Null ist.
             */
            InitComboBoxes();

            /*
             * Die Vorgabewerte des Bewertungsbogens werden über eine Tabelle festgelegt.
             * Diese Daten werden hier für das Formularblatt geladen.
             */
            Bewertungsbogen_Berwertungsvorgaben = hDAO.HoleBerwertungsvorgaben(this.ViewModelGuid);

            // Event subscribtion
            _aggregator.GetEvent<ThemePostedEvent>().Subscribe(GetDataMessage);
        }

        private void GetDataMessage(Message<wt2_konst_honorarkraft_thema> obj)
        {
            TV_Themen_aktualisieren();
        }

        public override void Init()
        {
            Header.Title = "Änderungsbogen";                         // Header Attribut zum bestimmen des Tab-Namens
            Header.Group = "Honorarkräfteverwaltung";                // Gruppenname der Anwendungs-Tabs

            Lbl_SaveDateTime = "---";

            # region Commands-Initialisieren
                AddThemeCommand = new RelayCommand(_execute => this.AddTheme(), _canExecute => true);
                SaveCommand = new RelayCommand(_execute => this.SaveChanges(), _canExecute => true);
                NewVertragCommand = new RelayCommand(_execute => this.NewVertrag(), _canExecute => true);
                NewVertragsPositionCommand = new RelayCommand(_execute => this.NewVertragsPosition(), _canExecute => { if (SelectedItem_Vertrag_Grid != null) return true; else return false; });
                DeleteVertragCommand = new RelayCommand(_execute => this.DeleteVertrag(), _canExecute => true);
                DeleteVertragPositionCommand = new RelayCommand(_execute => this.DeleteVertragPosition(), _canExecute => { if (SelectedItem_VertragPosition_Grid != null) return true; else return false; });
                DeleteZahlungsanweisungCommand = new RelayCommand(_execute => this.DeleteZahlungsanweisung(), _canExecute => { if (SelectedItem_Zahlung_Grid != null) return true; else return false; });
                DeleteZahlungsanweisungsPositionCommand = new RelayCommand(_execute => this.DeleteZahlungsanweisungsPosition(), _canExecute => { if (SelectedItem_ZahlungsanweisungsPosition_Grid != null) return true; else return false; });
                NewZahlungsanweisungsPositionCommand = new RelayCommand(_execute => this.NewZahlungsanweisungsPosition(), _canExecute => { if (SelectedItem_Zahlung_Grid != null) return true; else return false; });
                EditVertragsPositionCommand = new RelayCommand(_execute => this.EditVertragsPosition(), _canExecute => { if (SelectedItem_VertragPosition_Grid != null) return true; else return false; });
                EditZahlungsanweisungsPositionCommand = new RelayCommand(_execute => this.EditZahlungsanweisungsPosition(), _canExecute => { if (SelectedItem_ZahlungsanweisungsPosition_Grid != null) return true; else return false; });
                NewZahlungsanweisungCommand = new RelayCommand(_execute => this.NewZahlungsanweisung(), _canExecute => true);
                NewBewertungCommand = new RelayCommand(_execute => this.NewBewertung(), _canExecute => true);
                DeleteBewertungCommand = new RelayCommand(_execute => this.DeleteBewertung(), _canExecute => { if (SelectedItem_Bewertung_Grid!= null) return true; else return false; });
            # endregion

            // Hauptdatensatz für die EditView initialisieren
            EditViewDatensatz = new wt2_honorarkraft(); 
        }
        # endregion

        public void InitComboBoxes()
        {
            # region Combobox-Properties deklarieren
                Tv_Themen = new ObservableCollection<SortedList_Thema>();
                Lbx_Einsatzgebiete = new CheckedLookupCollection<wt2_konst_honorarkraft_einsatzgebiet>();
                Cbx_Personal_Bildungstraeger = new ObservableCollection<Bildungstraeger>();
                Lbx_Personal_Abteilung = new CheckedLookupCollectionBO();
                Lbx_Personal_Teams = new CheckedLookupCollectionBO();
                Cbx_Vertrag_Bildungstraeger = new ObservableCollection<Bildungstraeger>();
                Cbx_Vertrag_Abteilung = new LookupCollectionBO();
                Cbx_Zahlung_Abteilung = new LookupCollectionBO();
                Cbx_Zahlung_Bildungstraeger = new ObservableCollection<Bildungstraeger>();
                Cbx_Bewertung_Abteilung = new LookupCollectionBO();
                Cbx_Bewertung_Bildungstraeger = new ObservableCollection<Bildungstraeger>();      
            # endregion

            # region Daten in Comboboxen laden
                Tv_Themen = hDAO.HoleThemaListe(this.ViewModelGuid);
            
                Cbx_Personal_Bildungstraeger = LookRepo.GetBildungstraeger(true);          
                Cbx_Vertrag_Bildungstraeger = LookRepo.GetBildungstraeger(true);
                Cbx_Vertrag_Abteilung = LookRepo.GetAbteilungen(true);
                Cbx_Zahlung_Abteilung = LookRepo.GetAbteilungen(true);
                Cbx_Zahlung_Bildungstraeger = LookRepo.GetBildungstraeger(true);
                Cbx_Bewertung_Abteilung = LookRepo.GetAbteilungen(true);
                Cbx_Bewertung_Bildungstraeger = LookRepo.GetBildungstraeger(true);
            # endregion

            // Comboboxen zum Programmstart auf ersten Eintrag stellen
            ResetComboBoxes();
        }

        public void ResetComboBoxes()
        {
            # region Comboboxen auf den ersten Eintrag stellen
            # endregion
        }

        private void AddTheme()
        {
            // Das Eventpublishing muss nach dem Aufruf des Dialogfensters stattfinden damit die neue View das Event empfangen kann.
            ViewService.Current.ShowInlineDialog(this.DialogManager, "AddThemeView", "Thema hinzufügen", DialogMode.OkCancel, null, okAction => TV_Themen_aktualisieren());
      
        }

        public bool TV_Themen_aktualisieren()
        {
            Tv_Themen = hDAO.HoleThemaListe(this.ViewModelGuid);
            return true;
        }

        private void SaveChanges()
        {
            hDAO.SaveChanges(this.ViewModelGuid);
            Lbl_SaveDateTime = DateTime.Now.ToString("HH:mm:ss");
        }

        private void NewVertrag()
        {
            wt2_honorarkraft_vertrag newLine = new wt2_honorarkraft_vertrag();
            //newLine.hkv_ident = EditViewVerträge.Last().hkv_ident + 1;
            newLine.hkv_zeile = EditViewVerträge.Max(a => a.hkv_zeile) + 1;
            newLine.hkv_datum = DateTime.Now;
            newLine.hkv_aenderungsdatum = DateTime.Now;
            /*
             * hkv_vermittlungsagentur: Eine Defaultwert ist hier wichtig damit die 
             * Property hkvp_provision aus wt2_honorarkraft_vertrag_position nicht 
             * mit einem Fehler abbricht.
             */ 
            newLine.hkv_vermittlungsagentur = "N"; 
            newLine.wt2_honorarkraft_vertrag_position = new List<wt2_honorarkraft_vertrag_position>();       
            EditViewVerträge.Add(newLine);
        }

        private void NewZahlungsanweisung()
        {
            wt2_honorarkraft_zahlungsanweisung newLine = new wt2_honorarkraft_zahlungsanweisung();

            if (EditViewZahlungsanweisungen.Count > 0) 
            { 
                newLine.hkz_zeile = EditViewZahlungsanweisungen.Max(a => a.hkz_zeile) + 1;
            }
            else 
            {
                newLine.hkz_zeile = 1;
            }
            newLine.hkz_datum = DateTime.Now;
            newLine.hkz_aenderungsdatum = DateTime.Now;
            newLine.wt2_honorarkraft_zahlungsanweisung_position = new List<wt2_honorarkraft_zahlungsanweisung_position>();

            EditViewZahlungsanweisungen.Add(newLine);
        }

        private void NewVertragsPosition()
        {
            NavigationParameters navigationParameters = new NavigationParameters();
            navigationParameters.Add("guid", this.ViewModelGuid);

            
            // Neue Vertragsposition generieren
            wt2_honorarkraft_vertrag_position newLine = new wt2_honorarkraft_vertrag_position();
            newLine.hkvp_hkv_ident = SelectedItem_Vertrag_Grid.hkv_ident;

            if (Vertragspositionen.Count > 0)
                newLine.hkvp_lfdnr = Vertragspositionen.Max(a => a.hkvp_lfdnr) + 1;
            else newLine.hkvp_lfdnr = 1;
            newLine.wt2_honorarkraft_zahlungsanweisung_position = new List<wt2_honorarkraft_zahlungsanweisung_position>();
            newLine.wt2_honorarkraft_vertrag = SelectedItem_Vertrag_Grid;
            Vertragspositionen.Add(newLine);

            // Neue Vertragposition wird als Messageobjekt übergeben.
            Message<wt2_honorarkraft_vertrag_position> messageObj = new Message<wt2_honorarkraft_vertrag_position>(newLine, this.ViewModelGuid);

            // Das Eventpublishing muss nach dem Aufruf des Dialogfensters stattfinden damit die neue View das Event empfangen kann.
            ViewService.Current.ShowInlineDialog(this.DialogManager, "AddVertragspositionView", "Vertragsposition hinzufügen", DialogMode.OkCancel, navigationParameters,null, cancelAction => this.Vertragspositionen.Remove(newLine));
            _aggregator.GetEvent<VertragPositionPostedEvent>().Publish(messageObj);
        }

        private void NewBewertung() 
        {
            wt2_honorarkraft_bewertung newLine = new wt2_honorarkraft_bewertung();
            newLine.hkb_zeile = EditViewBewertungen.Max(a => a.hkb_zeile) + 1;
            newLine.hkb_lfdnr = EditViewBewertungen.Max(a => a.hkb_lfdnr) + 1;
            newLine.hkb_datum = DateTime.Now;
            EditViewBewertungen.Add(newLine);
        }

        private void DeleteBewertung() 
        {
            EditViewBewertungen.Remove(SelectedItem_Bewertung_Grid);
        }
        private void EditVertragsPosition()
        {
            NavigationParameters navigationParameters = new NavigationParameters();
            navigationParameters.Add("guid", this.ViewModelGuid);

            Message<wt2_honorarkraft_vertrag_position> messageObj = new Message<wt2_honorarkraft_vertrag_position>(SelectedItem_VertragPosition_Grid, this.ViewModelGuid);

            // Das Eventpublishing muss nach dem Aufruf des Dialogfensters stattfinden damit die neue View das Event empfangen kann.
            ViewService.Current.ShowInlineDialog(this.DialogManager, "AddVertragspositionView", "Vertragsposition bearbeiten", DialogMode.OkCancel, navigationParameters);
            _aggregator.GetEvent<VertragPositionPostedEvent>().Publish(messageObj);
        }

        private void NewZahlungsanweisungsPosition() 
        {
            NavigationParameters navigationParameters = new NavigationParameters();
            navigationParameters.Add("guid", this.ViewModelGuid);

            // Neue Zahlungsanweisungsposition generieren
            wt2_honorarkraft_zahlungsanweisung_position newLine = hDAO.NeueZahlungsanweisungsposition();
            newLine.hkzp_hkz_ident = SelectedItem_Zahlung_Grid.hkz_ident;

            if (Zahlungspositionen.Count > 0)
            {
                newLine.hkzp_lfdnr = Zahlungspositionen.Max(a => a.hkzp_lfdnr) + 1;
            }
            else newLine.hkzp_lfdnr = 1;

            Zahlungspositionen.Add(newLine);

            // Neue Zahlungsposition wird als Messageobjekt übergeben.
            Message<wt2_honorarkraft_zahlungsanweisung_position> messageObj = new Message<wt2_honorarkraft_zahlungsanweisung_position>(newLine, this.ViewModelGuid);

            // Das Eventpublishing muss nach dem Aufruf des Dialogfensters stattfinden damit die neue View das Event empfangen kann.
            ViewService.Current.ShowInlineDialog(this.DialogManager, "AddZahlungsanweisungspositionView", "Zahlungsanweisungsposition hinzufügen", DialogMode.OkCancel, navigationParameters, null, cancelAction => this.Zahlungspositionen.Remove(newLine));
            _aggregator.GetEvent<ZahlungsanweisungsPositionPostEvent>().Publish(messageObj);
        }

        private void EditZahlungsanweisungsPosition()
        {
            NavigationParameters navigationParameters = new NavigationParameters();
            navigationParameters.Add("guid", this.ViewModelGuid);

            Message<wt2_honorarkraft_zahlungsanweisung_position> messageObj = new Message<wt2_honorarkraft_zahlungsanweisung_position>(SelectedItem_ZahlungsanweisungsPosition_Grid, this.ViewModelGuid);

            ViewService.Current.ShowInlineDialog(this.DialogManager, "AddZahlungsanweisungspositionView", "Zahlungsanweisungsposition bearbeiten", DialogMode.OkCancel, navigationParameters);
            _aggregator.GetEvent<ZahlungsanweisungsPositionPostEvent>().Publish(messageObj);
        }

        private void DeleteVertrag()
        {
            EditViewVerträge.Remove(SelectedItem_Vertrag_Grid);
        }

        private void DeleteZahlungsanweisung()
        {
            EditViewZahlungsanweisungen.Remove(SelectedItem_Zahlung_Grid);
        }
        private void DeleteVertragPosition()
        {
            Vertragspositionen.Remove(SelectedItem_VertragPosition_Grid);
        }

        private void DeleteZahlungsanweisungsPosition()
        {
            Zahlungspositionen.Remove(SelectedItem_ZahlungsanweisungsPosition_Grid);
        }

        # region Prism-Navigation-Einstellungen
        public override bool IsNavigationTarget(NavigationContext navigationContext)
        {
            // Ist der Datensatz schon als View göffnet
            if ((navigationContext.Parameters["ID"]).ToString() == personId) 
                return true; // Es muss keine neue View göffnet werden
            else
                return false; // Es muss eine neue View geöffnet werden
        }

        // BLZ holen und pruefen
        public string Check_BLZ(string blz)
        {
            if (blz != null && blz.Length == 8)
            {
                if (LookRepo.GetBank(blz) != null)
                    return LookRepo.GetBank(blz).Name;
                return "";
            }
            else return "";
        }

        public override void ApplyNavigationParameters(NavigationParameters navigationParameters)
        {
                personId = (navigationParameters["ID"]).ToString();

                // Empfangenen Datensatz in Property ablegen
                EditViewDatensatz = hDAO.FillEditView(personId, this.ViewModelGuid);

                // Initiales laden des Banknamens
                Lbl_Personal_Bank = Check_BLZ(EditViewDatensatz.hk_blz);

                // Verträge des Empfangenendatensatzes in Property ablegen
                EditViewVerträge = new ObservableCollection<wt2_honorarkraft_vertrag>(EditViewDatensatz.wt2_honorarkraft_vertrag);

                // Zahlungsanweisungen des Empfangsdatensatzes in Property ablegen
                EditViewZahlungsanweisungen = new ObservableCollection<wt2_honorarkraft_zahlungsanweisung>(EditViewDatensatz.wt2_honorarkraft_zahlungsanweisung);
                
                // Bewertungen des Empfangsdatensatzes in Property ablegen
                EditViewBewertungen = new ObservableCollection<wt2_honorarkraft_bewertung>(EditViewDatensatz.wt2_honorarkraft_bewertung);

                SelectedItem_Personal_Anrede = hDAO.ConvertAnrede(EditViewDatensatz.hk_anrede);

                Current_Personal_Titel = EditViewDatensatz.hk_titel;

                Lbx_Personal_Teams = hDAO.CheckTeams(personId, Lbx_Personal_Teams, this.ViewModelGuid);
                

                Tv_Themen = hDAO.HoleThemaListe(this.ViewModelGuid, EditViewDatensatz);
                Tv_Themen.ToList().ForEach(a => a.ThemeGroup.ItemCheckedChanged += ThemeGroup_ItemCheckedChanged);

                Lbx_Einsatzgebiete = hDAO.HoleEinsatzgebieteListe(this.ViewModelGuid, EditViewDatensatz);
                // Methode mit dem ItemCheckedChanged Event verbinden um auf Änderungen zu reagieren
                Lbx_Einsatzgebiete.ItemCheckedChanged += Lbx_Einsatzgebiete_ItemCheckedChanged;  

                Lbx_Personal_Abteilung = hDAO.HoleAbteilungsListe(false, EditViewDatensatz);
                // Methode mit dem ItemCheckedChanged Event verbinden um auf Änderungen zu reagieren
                Lbx_Personal_Abteilung.ItemCheckedChanged += Lbx_Personal_Abteilung_ItemCheckedChanged;

                Lbx_Personal_Teams = hDAO.HoleTeamListe(true, "", false, true, EditViewDatensatz);
                // Methode mit dem ItemCheckedChanged Event verbinden um auf Änderungen zu reagieren
                Lbx_Personal_Teams.ItemCheckedChanged += Lbx_Personal_Teams_ItemCheckedChanged;

                // Um Abteilungsnamen zuzuweisen, muss HoleVerträge nach der initialisierung von Lbx_Personal_Abteilung aufgerufen werden
                hDAO.HoleVertraege(EditViewDatensatz, Lbx_Personal_Abteilung, this.ViewModelGuid);
                hDAO.HoleZahlungsanweisungen(EditViewDatensatz, this.ViewModelGuid);
                hDAO.HoleBewertungen(EditViewDatensatz, this.ViewModelGuid);
                Honorarkraft_Status = hDAO.HoleStatus(EditViewDatensatz, this.ViewModelGuid);
        }

        private void ThemeGroup_ItemCheckedChanged(wt2_konst_honorarkraft_thema item)
        {
            if (item.IsChecked && EditViewDatensatz.wt2_konst_honorarkraft_thema.Any(a => a.khkth_ident.Equals(item.khkth_ident)) == false)
            {
                EditViewDatensatz.wt2_konst_honorarkraft_thema.Add(item);
            }
            if (!item.IsChecked)
            {
                wt2_konst_honorarkraft_thema del = EditViewDatensatz.wt2_konst_honorarkraft_thema.Where(a => a.khkth_ident.Equals(item.khkth_ident)).FirstOrDefault();
                if (del != null)
                    EditViewDatensatz.wt2_konst_honorarkraft_thema.Remove(del);
            }
        }

        private void Lbx_Einsatzgebiete_ItemCheckedChanged(wt2_konst_honorarkraft_einsatzgebiet item)
        {
            /* 
             * Es wird ein Item übergeben das geändert wurde. Je nachdem ob ein Harken gesetzt wurde
             * oder entfernt wird der Honorarkraft eine Abteilung hinzugefügt oder entfernt.
             */
            if (item.IsChecked && EditViewDatensatz.wt2_konst_honorarkraft_einsatzgebiet.Any(a => a.khke_ident.Equals(item.khke_ident)) == false)
            {
                EditViewDatensatz.wt2_konst_honorarkraft_einsatzgebiet.Add(item);
            }
            if (!item.IsChecked)
            {
                wt2_konst_honorarkraft_einsatzgebiet del = EditViewDatensatz.wt2_konst_honorarkraft_einsatzgebiet.Where(a => a.khke_ident.Equals(item.khke_ident)).FirstOrDefault();
                if (del != null)
                EditViewDatensatz.wt2_konst_honorarkraft_einsatzgebiet.Remove(del);
            }
        }

        private void Lbx_Personal_Abteilung_ItemCheckedChanged(LookupBO item)
        {
            /* 
             * Es wird ein Item übergeben das geändert wurde. Je nachdem ob ein Harken gesetzt wurde
             * oder entfernt wird der Honorarkraft eine Abteilung hinzugefügt oder entfernt.
             */
            if (item.IsChecked && EditViewDatensatz.wt2_honorarkraft_cluster.Any(a => a.hkc_cl_ident.Equals(item.ID)) == false)
            {
                wt2_honorarkraft_cluster abteilung = new wt2_honorarkraft_cluster();
                abteilung.hkc_hk_ident = EditViewDatensatz.hk_ident;
                abteilung.hkc_cl_ident = Convert.ToInt32(item.ID);
                EditViewDatensatz.wt2_honorarkraft_cluster.Add(abteilung);
            }
            if (!item.IsChecked)
            {
                wt2_honorarkraft_cluster del = EditViewDatensatz.wt2_honorarkraft_cluster.Where(a => a.hkc_cl_ident == Convert.ToInt32(item.ID)).FirstOrDefault();
                if (del != null)
                EditViewDatensatz.wt2_honorarkraft_cluster.Remove(del);
            }
        }

        private void Lbx_Personal_Teams_ItemCheckedChanged(LookupBO item)
        {
            /* 
             * Es wird ein Item übergeben das geändert wurde. Je nachdem ob ein Harken gesetzt wurde
             * oder entfernt wird der Honorarkraft ein Team hinzugefügt oder entfernt.
             */
            if (item.IsChecked && EditViewDatensatz.wt2_honorarkraft_team.Any(a => a.hkt_team_nr.Equals(item.ID)) == false)
            {
                wt2_honorarkraft_team team = new wt2_honorarkraft_team();
                team.hkt_hk_ident = EditViewDatensatz.hk_ident;
                team.hkt_team_nr = item.ID;
                EditViewDatensatz.wt2_honorarkraft_team.Add(team);
            }
            if (!item.IsChecked)
            {
                wt2_honorarkraft_team del = EditViewDatensatz.wt2_honorarkraft_team.Where(a => a.hkt_team_nr == item.ID).FirstOrDefault();
                if(del!=null)
                EditViewDatensatz.wt2_honorarkraft_team.Remove(del);
            }            
        }
        # endregion
    }
}
