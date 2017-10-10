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
using Technewlogic.WpfDialogManagement;
using TIS3_Base;
using TIS3_Base.Services;
using TIS3_LookupBL;
using TIS3_WPF_TestMusterAddIn.Infrastructure;
using TIS3_WPF_TestMusterAddIn.Views;
using WinTIS30db_entwModel.Honorarkraefte;
using WinTIS30db_entwModel.Lookup;

namespace TIS3_WPF_TestMusterAddIn.ViewModels
{
    [NotifyPropertyChanged]
    class EditViewModel : TIS3ActiveViewModel, INavigationAware
    {
        # region Data Access Objecte für Honorarkraefte Tabellen
        LookupRepository LookRepo = new LookupRepository();
        HonorarkraefteDAO hDAO = HonorarkraefteDAO.DAOFactory();
        # endregion

        // Personal-Id zur Dublikatvermeidung 
        string personId;

        # region Commands-Neuaufnahme
        public RelayCommand AddThemeCommand { get; set; }
        # endregion

        # region EditView Properties

        #region Listen für Anrede und Titel die direkt in dieser Klasse generiert werden.
        public List<string> anredeListe = new List<string>() { "","Frau","Herr","Firma"};
        public List<string> titelListe = new List<string>() { "","Dr.","Prof."};
        public List<string> Cbx_Personal_Anrede { get { return anredeListe; } }
        public List<string> Cbx_Personal_Titel { get { return titelListe; } }
        #endregion

        # region Textboxen EditView
        public string Tbx_Personal_Vorname { get; set; }
        public string Tbx_Personal_Nachname { get; set; }
        public string Tbx_Personal_Firma { get; set; }
        public string Tbx_Personal_Strasse { get; set; }
        public string Tbx_Personal_Plz { get; set; }
        public string Tbx_Personal_Ort { get; set; }
        public string Tbx_Personal_PTel { get; set; }
        public string Tbx_Personal_Fax { get; set; }
        public string Tbx_Personal_FTel { get; set; }
        public string Tbx_Personal_Mobil { get; set; }
        public string Tbx_Personal_EMail { get; set; }
        public string Tbx_Personal_Blz { get; set; }
        public string Tbx_Personal_Bank { get; set; }
        public string Tbx_Personal_Konto { get; set; }
        public string Tbx_Personal_Ue { get; set; }
        public string Tbx_Personal_Ausbildung { get; set; }
        public string Tbx_Personal_Studium { get; set; }
        public string Tbx_Personal_Situation { get; set; }
        public string Tbx_Personal_Fachgebiet { get; set; }
        public string Tbx_Vertrag_Nummer { get; set; }
        public string Tbx_Vertrag_Datum { get; set; }
        public string Tbx_Vertrag_Druckdatum { get; set; }
        public string Tbx_Vertrag_Unterschriftdatum { get; set; }
        public string Tbx_Zahlung_Nummer { get; set; }
        public string Tbx_Zahlung_Monat { get; set; }
        public string Tbx_Zahlung_Druckdatum { get; set; }
        public string Tbx_Bewertung_Nummer { get; set; }
        public string Tbx_Bewertung_Datum { get; set; }
        public string Tbx_Bewertung_TLvon { get; set; }
        public string Tbx_Bewertung_TLbis { get; set; }
        public string Tbx_Bewertung_TNvon { get; set; }
        public string Tbx_Bewertung_TNbis { get; set; }
        public string Tbx_Bewertung_Kommentar { get; set; }
        public string Tbx_Bewertung_Was { get; set; }
        public string Tbx_Bewertung_Wer { get; set; }
        public string Tbx_Bewertung_Wann { get; set; }
        public string Tbx_Bewertung_Info { get; set; }
        public string Tbx_Bewertung_Strasse { get; set; }
        public string Tbx_Bewertung_Plz { get; set; }
        public string Tbx_Bewertung_Ort { get; set; }
        public string Tbx_Info_2Mail { get; set; }
        public string Tbx_Info_3Mail { get; set; }
        public string Tbx_Info_Web { get; set; }
        public string Tbx_Info_Abteilung { get; set; }
        public string Tbx_Info_Position { get; set; }
        public string Tbx_Info_BogenVon { get; set; }
        public string Tbx_Info_CheckVon { get; set; }
        public string Tbx_Info_Rechtsform { get; set; }
        public string Tbx_Status_Info { get; set; }
        public string Tbx_Info_Verfolgung { get; set; }
        public string Tbx_Info_OffzVerfolgung { get; set; }
        public string Tbx_Info_DatumUederpruefung { get; set; }
        # endregion

        # region Checkboxen EditView
        public bool Chkbx_Vertrag_400{ get; set; }
        public bool Chkbx_Vertrag_AZWV{ get; set; }
        public bool Chkbx_Vertrag_Vermittlungsagentur{ get; set; }
        public bool Chkbx_Info_DRV{ get; set; }
        public bool Chkbx_Info_SVBefreiung{ get; set; }
        public bool Chkbx_Info_Selbst{ get; set; }
        public bool Chkbx_Info_Verfolgen{ get; set; }
        public bool Chkbx_Info_OffzVerfolgen{ get; set; }
        public bool Chkbx_Info_Bedenklich { get; set; }
        # endregion

        #region Radiobuttons EditView
        public bool Rb_Regelung_Ja { get; set; }     
        public bool Rb_Regelung_Nein { get; set; }      
        public bool Rb_Regelung_ka { get; set; }        
        public bool Rb_ArbN_Ja { get; set; }        
        public bool Rb_ArbN_Nein { get; set; }      
        public bool Rb_ArbN_ka { get; set; }        
        public bool Rb_SVPflicht_Ja  { get; set; }       
        public bool Rb_SVPflicht_Nein { get; set; }      
        public bool Rb_SVPflicht_ka { get; set; }
        public bool Rb_SVBefreiung_beantragt { get; set; }
        public bool Rb_SVBefreiung_liegtvor { get; set; }
        public bool Rb_Einsatzvolument_Ja { get; set; }
        public bool Rb_Einsatzvolumen_Nein { get; set; }
        public bool Rb_Einsatzvolumen_ka { get; set; }
        public bool Rb_Umst_Ja { get; set; }
        public bool Rb_Umst_Nein { get; set; }
        public bool Rb_Umst_ka { get; set; }  
        #endregion

        public ObservableCollection<SortedList_Thema> Tv_Themen { get; set; }
        public ObservableCollection<Einsatzgebiet_Check> Lbx_Einsatzgebiete { get; set; }
        public int SelectedItem_Personal_Bildungstraeger { get; set; }
        public ObservableCollection<Bildungstraeger> Cbx_Personal_Bildungstraeger { get; set; }
        public int SelectedItem_Personal_Abteilung { get; set; }
        public LookupCollectionBO Lbx_Personal_Abteilung { get; set; }
        public int SelectedItem_Personal_Team { get; set; }
        public LookupCollectionBO Lbx_Personal_Teams { get; set; }
        public int SelectedItem_Vertrag_Bildungstraeger { get; set; }
        public ObservableCollection<Bildungstraeger> Cbx_Vertrag_Bildungstraeger { get; set; }
        public int SelectedItem_Vertrag_Abteilung { get; set; }
        public LookupCollectionBO Cbx_Vertrag_Abteilung { get; set; }
        public int SelectedItem_Zahlung_Abteilung { get; set; }
        public LookupCollectionBO Cbx_Zahlung_Abteilung { get; set; }
        public int SelectedItem_Zahlung_Bildungstraeger { get; set; }
        public ObservableCollection<Bildungstraeger> Cbx_Zahlung_Bildungstraeger { get; set; }
        public int SelectedItem_Bewertung_Abteilung { get; set; }
        public LookupCollectionBO Cbx_Bewertung_Abteilung { get; set; }
        public int SelectedItem_Bewertung_Bildungstraeger { get; set; }
        public ObservableCollection<Bildungstraeger> Cbx_Bewertung_Bildungstraeger { get; set; }

        //public bool Chkbx_Info_DRV { get; set; }
        //public bool Chkbx_Info_SVBefreiung { get; set; }

        public ObservableCollection<wt2_honorarkraft> HonorarListe { get; set; }
        # endregion

        # region Editview Init
        public override void Init()
        {
            Header.Title = "Änderungsbogen";                         // Header Attribut zum bestimmen des Tab-Namens
            Header.Group = "Honorarkräfteverwaltung";                // Gruppenname der Anwendungs-Tabs

            AddThemeCommand = new RelayCommand(_execute => this.AddTheme(), _canExecute => true);
            InitComboBoxes();

            # region Testdaten initializieren
            HonorarListe = hDAO.LoadTestdata();
            # endregion
        }
        # endregion


        public void InitComboBoxes()
        {
            # region Combobox-Properties deklarieren
            Tv_Themen = new ObservableCollection<SortedList_Thema>();
            Lbx_Einsatzgebiete = new ObservableCollection<Einsatzgebiet_Check>();
            Cbx_Personal_Bildungstraeger = new ObservableCollection<Bildungstraeger>();
            Lbx_Personal_Abteilung = new LookupCollectionBO();
            Lbx_Personal_Teams = new LookupCollectionBO();
            Cbx_Vertrag_Bildungstraeger = new ObservableCollection<Bildungstraeger>();
            Cbx_Vertrag_Abteilung = new LookupCollectionBO();
            Cbx_Zahlung_Abteilung = new LookupCollectionBO();
            Cbx_Zahlung_Bildungstraeger = new ObservableCollection<Bildungstraeger>();
            Cbx_Bewertung_Abteilung = new LookupCollectionBO();
            Cbx_Bewertung_Bildungstraeger = new ObservableCollection<Bildungstraeger>();      
            # endregion

            # region Daten in Comboboxen laden
            Tv_Themen = hDAO.HoleThemaListe();
            Lbx_Einsatzgebiete = hDAO.HoleEinsatzgebieteListe();
            Cbx_Personal_Bildungstraeger = LookRepo.GetBildungstraeger(true);
            Lbx_Personal_Abteilung = LookRepo.GetAbteilungen(false);
            Lbx_Personal_Teams = LookRepo.GetTeams(true, "", false);
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
            SelectedItem_Personal_Bildungstraeger = 0;
            SelectedItem_Personal_Abteilung = 0;
            SelectedItem_Personal_Team = 0;
            SelectedItem_Vertrag_Bildungstraeger = 0;
            SelectedItem_Zahlung_Abteilung = 0;
            SelectedItem_Zahlung_Bildungstraeger = 0;
            SelectedItem_Bewertung_Abteilung = 0;
            SelectedItem_Bewertung_Bildungstraeger = 0;
            # endregion
        }

        private void AddTheme()
        {
            ViewService.Current.ShowInlineDialog(this.DialogManager, "AddThemeView", "Thema hinzufügen", DialogMode.OkCancel);
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

        public override void ApplyNavigationParameters(NavigationParameters navigationParameters)
        {
                personId = (navigationParameters["ID"]).ToString();

                IEnumerable<wt2_honorarkraft> result = from tupel in HonorarListe
                                                       where tupel.hk_ident.Equals(int.Parse(personId))
                                                       select tupel;
                wt2_honorarkraft test = result.FirstOrDefault();

                Tbx_Personal_Vorname = test.hk_vorname;
                Tbx_Personal_Nachname = test.hk_nachname;
                Tbx_Personal_Firma = test.hk_firma; 
        }
        # endregion
    }
}
