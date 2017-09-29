using Microsoft.Practices.Prism.Regions;
using PostSharp.Patterns.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TIS3_Base;
using TIS3_LookupBL;
using TIS3_WPF_TestMusterAddIn.Infrastructure;
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

        // Personal-Id zur Dublikat vermeidung 
        string personId;

        # region EditView Properties

        #region Listen für Anrede und Titel die direkt in dieser Klasse generiert werden.
        public List<string> anredeListe = new List<string>() { "","Frau","Herr","Firma"};
        public List<string> titelListe = new List<string>() { "","Dr.","Prof."};
        public List<string> Cbx_Personal_Anrede { get { return anredeListe; } }
        public List<string> Cbx_Personal_Titel { get { return titelListe; } }
        #endregion

        public string Tbx_Personal_Vorname { get; set; }
        public string Tbx_Personal_Nachname { get; set; }
        public string Tbx_Personal_Firma { get; set; }

        public ObservableCollection<SortedList_Thema> Tv_Themen { get; set; }
        public ObservableCollection<Einsatzgebiet_Check> Lbx_Einsatzgebiete { get; set; }
        public int SelectedItem_Personal_Bildungstraeger { get; set; }
        public ObservableCollection<Bildungstraeger> Cbx_Personal_Bildungstraeger { get; set; }
        public int SelectedItem_Personal_Abteilung { get; set; }
        public LookupCollectionBO Cbx_Personal_Abteilung { get; set; }
        public int SelectedItem_Personal_Team { get; set; }
        public LookupCollectionBO Cbx_Personal_Teams { get; set; }
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

        public bool Chkbx_Info_DRV { get; set; }
        public bool Chkbx_Info_SVBefreiung { get; set; }

        public ObservableCollection<wt2_honorarkraft> HonorarListe { get; set; }
        # endregion

        # region Editview Init
        public override void Init()
        {
            Header.Title = "Änderungsbogen";                         // Header Attribut zum bestimmen des Tab-Namens
            Header.Group = "Honorarkräfteverwaltung";                // Gruppenname der Anwendungs-Tabs

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
            Cbx_Personal_Abteilung = new LookupCollectionBO();
            Cbx_Personal_Teams = new LookupCollectionBO();
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
            Cbx_Personal_Abteilung = LookRepo.GetAbteilungen(true);
            Cbx_Personal_Teams = LookRepo.GetTeams(true, "", true);
            Cbx_Vertrag_Bildungstraeger = LookRepo.GetBildungstraeger(true);
            Cbx_Vertrag_Abteilung = LookRepo.GetAbteilungen(true);
            Cbx_Zahlung_Abteilung = LookRepo.GetAbteilungen(true);
            Cbx_Zahlung_Bildungstraeger = LookRepo.GetBildungstraeger(true);
            Cbx_Bewertung_Abteilung = LookRepo.GetAbteilungen(true);
            Cbx_Bewertung_Bildungstraeger = LookRepo.GetBildungstraeger(true);
            # endregion

            //foreach (SortedList_Thema e in Tv_Themen) { Console.WriteLine(e.Gruppe); foreach (Thema_Check t in e.ThemeGroup) { Console.WriteLine("--- " + t.khkth_bezeichnung); } }
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
