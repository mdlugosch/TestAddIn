using BFZ_Common_Lib.MVVM;
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
using WinTIS30db_entwModel.Lookup;

namespace TIS3_WPF_TestMusterAddIn.ViewModels
{
     [NotifyPropertyChanged]
    class NeuaufnahmeViewModel : TIS3ActiveViewModel
    {
        # region Data Access Objecte für Honorarkraefte Tabellen
        LookupRepository LookRepo = new LookupRepository();
        HonorarkraefteDAO hDAO = HonorarkraefteDAO.DAOFactory();
        # endregion

        #region Listen für Anrede und Titel die direkt in dieser Klasse generiert werden.
        public List<string> anredeListe = new List<string>() { "", "Frau", "Herr", "Firma" };
        public List<string> titelListe = new List<string>() { "", "Dr.", "Prof." };
        public List<string> Cbx_Neu_Anrede { get { return anredeListe; } }
        public List<string> Cbx_Neu_Titel { get { return titelListe; } }
        #endregion

        # region Elemente der Neuaufnahmemaske 
        public ObservableCollection<SortedList_Thema> Tv_Neu_Themen { get; set; }
        public ObservableCollection<Einsatzgebiet_Check> Lbx_Neu_Einsatzgebiete { get; set; }
        public int SelectedItem_Neu_Abteilung { get; set; }
        public LookupCollectionBO Cbx_Neu_Abteilung { get; set; }
        public int SelectedItem_Neu_Bildungstraeger { get; set; }
        public ObservableCollection<Bildungstraeger> Cbx_Neu_Bildungstraeger { get; set; }
        public int SelectedItem_Neu_Teams { get; set; }
        public LookupCollectionBO Cbx_Neu_Teams { get; set; }
        # endregion

        # region Commands-Neuaufnahme
        public RelayCommand SaveCommand { get; set; }
        public RelayCommand ResetCommand { get; set; }
        # endregion

        public override void Init()
        {
            Header.Title = "Neuaufnahme";                         // Header Attribut zum bestimmen des Tab-Namens
            Header.Group = "Honorarkräfteverwaltung";             // Gruppenname der Anwendungs-Tabs
            SaveCommand = new RelayCommand(_execute => this.Neuanlegen(), _canExecute => true);
            ResetCommand = new RelayCommand(_execute => this.Zureucksetzen(), _canExecute => true);

            InitComboBoxes();
        }

        public void InitComboBoxes()
        {
            Cbx_Neu_Teams = new LookupCollectionBO();
            Cbx_Neu_Abteilung = new LookupCollectionBO();
            Cbx_Neu_Bildungstraeger = new ObservableCollection<Bildungstraeger>();
            Tv_Neu_Themen = new ObservableCollection<SortedList_Thema>();
            Lbx_Neu_Einsatzgebiete = new ObservableCollection<Einsatzgebiet_Check>();

            # region Daten in Comboboxen laden
            Tv_Neu_Themen = hDAO.HoleThemaListe();
            Lbx_Neu_Einsatzgebiete = hDAO.HoleEinsatzgebieteListe();
            Cbx_Neu_Teams = LookRepo.GetTeams(true, "", true);
            Cbx_Neu_Abteilung = LookRepo.GetAbteilungen(true);
            Cbx_Neu_Bildungstraeger = LookRepo.GetBildungstraeger(true);
            # endregion

            ResetComboBoxes();
        }

        public void ResetComboBoxes()
        {
            # region Comboboxen auf den ersten Eintrag stellen
            SelectedItem_Neu_Teams = 0;
            SelectedItem_Neu_Abteilung = 0;
            SelectedItem_Neu_Bildungstraeger = 0;
            # endregion
        }

        private void Neuanlegen()
        {
            MessageBox.Show("It works! - Neuanlage");
        }

        private void Zureucksetzen()
        {
            MessageBox.Show("It works! - zurücksetzen");
        }
    }
}
