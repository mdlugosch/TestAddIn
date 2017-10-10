using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TIS3_WPF_TestMusterAddIn.ViewModels;
using WinTIS30db_entwModel.Honorarkraefte;

namespace TIS3_WPF_TestMusterAddIn.Infrastructure
{
    /*
     * Data Access Object - HonorarkraefteDAO
     * Sammelklasse für LinQ-Befehle die die Honorarkräftetabellen nutzen
     */
    public class HonorarkraefteDAO 
    {
        public static HonorarkraefteDAO hDAO;

        // Datenbank-Context für die Honorarkräfte-Tabellen
        protected static WinTIS30db_entwModel.Honorarkraefte.WinTIS30db_entwEntities context;
                                                  
        // Alle Elemente sollen die Inhalte über die selbe Klasse bzw. den selben Context beziehen
        public static HonorarkraefteDAO DAOFactory()
        {
            if (hDAO == null)
            {
                hDAO = new HonorarkraefteDAO();
                return hDAO;
            }
            else return hDAO;
        }

        // Über den Konstruktor einen neuen Context laden.
        private HonorarkraefteDAO()
        {
            LookupRepository(new WinTIS30db_entwModel.Honorarkraefte.WinTIS30db_entwEntities());
        }

        // Schlägt die initialisierung des Context fehl, führt dies zu einem Fehler.
        public void LookupRepository(WinTIS30db_entwModel.Honorarkraefte.WinTIS30db_entwEntities newContext)
        {
            if (newContext == null)
            {
                Debug.WriteLine("DB-Context ist null!");
                throw new ArgumentNullException("context");
            }
    
                context = newContext;    
        }

        public void CreateContext()
        {
            LookupRepository(new WinTIS30db_entwModel.Honorarkraefte.WinTIS30db_entwEntities());
        }

        // Auslesen von Daten mit Hilfe des Context
        public ObservableCollection<wt2_konst_honorarkraft_einsatzgebiet> HoleEinsatzgebiete(Boolean leeresElement)
        {
            ObservableCollection<wt2_konst_honorarkraft_einsatzgebiet> result = null;

            CreateContext();

            using (context)
            {
                var query = (from einsatzGebiete in context.wt2_konst_honorarkraft_einsatzgebiet
                             orderby einsatzGebiete.khke_bezeichnung
                             select einsatzGebiete).ToList();


                if (leeresElement)
                {
                    wt2_konst_honorarkraft_einsatzgebiet einsatzGebiete = new wt2_konst_honorarkraft_einsatzgebiet();
                    einsatzGebiete.khke_ident = -1;
                    query.Insert(0, einsatzGebiete);
                }

                result = new ObservableCollection<wt2_konst_honorarkraft_einsatzgebiet>(query);
            }
            return result;
        }

        // Abruf Einsatzgebiete für ListBox mit Checkboxfunktion
        public ObservableCollection<Einsatzgebiet_Check> HoleEinsatzgebieteListe()
        {
            ObservableCollection<Einsatzgebiet_Check> result = new ObservableCollection<Einsatzgebiet_Check>();

            CreateContext();

            using (context)
            {
                var query = (from einsatzGebiete in context.wt2_konst_honorarkraft_einsatzgebiet
                             orderby einsatzGebiete.khke_bezeichnung
                             select new Einsatzgebiet_Check()
                             {
                                 IsChecked = false,
                                 Einsatzgebiet = einsatzGebiete
                             }).ToList();

                foreach (Einsatzgebiet_Check c in query)
                {
                    result.Add(c);
                }
            }
            return result;
        }

        public ObservableCollection<wt2_konst_honorarkraft_thema> HoleThema(Boolean leeresElement)
        {
            ObservableCollection<wt2_konst_honorarkraft_thema> result = null;
            CreateContext();

            using (context)
            {
                var query = (from hThema in context.wt2_konst_honorarkraft_thema
                             orderby hThema.khkth_gruppe
                             select hThema).ToList();

                if (leeresElement)
                {
                    wt2_konst_honorarkraft_thema thema = new wt2_konst_honorarkraft_thema();
                    thema.khkth_ident = -1;
                    query.Insert(0, thema);
                }
                result = new ObservableCollection<wt2_konst_honorarkraft_thema>(query);
            }
            return result;
        }

        // Holt Bezeichnungen für Themengruppen als Stringliste
        public ObservableCollection<string> HoleThemaGruppen()
        {
            // Result mit IsChecked Property
            ObservableCollection<string> result = new ObservableCollection<string>();

            result.Clear();

            CreateContext();

            using (context)
            {
                // Gruppenliste für anschließende Sortierung erstellen
                var groupQuery = from themaTab in context.wt2_konst_honorarkraft_thema
                                 group themaTab by themaTab.khkth_gruppe;

                // Ablegen der LinQ Ergebnisse in die Result Collection
                foreach (var t in groupQuery) 
                {
                    result.Add(t.Key);
                }
            }

            return result;
        }

        // ThemaListe aufbereiten für TreeView
        public ObservableCollection<SortedList_Thema> HoleThemaListe()
        {
            // Result mit IsChecked Property
            ObservableCollection<Thema_Check>result = new ObservableCollection<Thema_Check>();
            // Result als sortierte Liste
            ObservableCollection<SortedList_Thema> sortedResult = new ObservableCollection<SortedList_Thema>();
            
            CreateContext();

            using (context)
            {
                // Erstellen einer unsortierten Liste mit zusätzlicher IsChecked Property
                var query = (from hThema in context.wt2_konst_honorarkraft_thema
                             where !String.IsNullOrEmpty(hThema.khkth_gruppe) || 
                                   !String.IsNullOrEmpty(hThema.khkth_bezeichnung)
                             orderby hThema.khkth_bezeichnung
                             select new Thema_Check() 
                             {
                                 IsChecked = false,
                                 Thema = hThema
                             }).ToList();

                // Ablegen der LinQ Ergebnisse in die Result Collection
                foreach (Thema_Check t in query) { result.Add(t); }

                // Gruppenliste für anschließende Sortierung erstellen
                var groupQuery = from q in result
                                 group q by q.Thema.khkth_gruppe;

                // Alle Gruppen durchgehen und Themen nach Gruppen sortiert in die Struktur speichern
                foreach (var element in groupQuery)
                {
                    var themeQuery = from q in result
                                     where q.Thema.khkth_gruppe == element.Key
                                     select q;
                    sortedResult.Add(new SortedList_Thema() { Gruppe = element.Key, ThemeGroup = new ObservableCollection<Thema_Check>(themeQuery) });
                } 
            }
            return sortedResult;
        }

        public ObservableCollection<wt2_honorarkraft> PersonendatenSuche(PersonaldatenViewModel vm)
        {
            ObservableCollection<wt2_honorarkraft> result = null;
            CreateContext();

            vm.HonorarListe = null;

            using (context)
            {
                # region Standartquery
                IEnumerable<wt2_honorarkraft> query = from perso in context.wt2_honorarkraft
                                                          .Include("wt2_honorarkraft_team")
                                                          .Include("wt2_honorarkraft_cluster")
                                                          .Include("wt2_konst_honorarkraft_einsatzgebiet")
                                                          .Include("wt2_konst_honorarkraft_thema")
                                                          .Include("wt2_honorarkraft_status")
                            select perso;
                # endregion

                # region Prüfe Textboxen auf Suchparameter
                if (!string.IsNullOrWhiteSpace(vm.Tbx_Personal_Name))
                    query = query.Where(row => row.hk_nachname.Contains(vm.Tbx_Personal_Name));
                if (!string.IsNullOrWhiteSpace(vm.Tbx_Personal_Vorname))
                    query = query.Where(row => row.hk_vorname.Contains(vm.Tbx_Personal_Vorname));
                if (!string.IsNullOrWhiteSpace(vm.Tbx_Personal_Firma))
                    query = query.Where(row => row.hk_firma.Contains(vm.Tbx_Personal_Firma));
                # endregion

                # region Prüfe Comboboxen auf Suchparameter
                if (vm.SelectedItem_Personal_Bildungstraeger != 0)
                { 
                    query = query.Where(row => row.hk_bildungstraeger.Equals(vm.Cbx_Personal_Bildungstraeger[vm.SelectedItem_Personal_Bildungstraeger].ID));
                }
                if (vm.SelectedItem_Personal_Teams != 0)
                {
                    query = query.Where(row => row.wt2_honorarkraft_team.Any(a => a.hkt_team_nr.Equals(vm.Cbx_Personal_Teams[vm.SelectedItem_Personal_Teams].ID)));
                }
                if (vm.SelectedItem_Personal_Abteilung != 0)
                {
                    query = query.Where(row => row.wt2_honorarkraft_cluster.Any(a => a.hkc_cl_ident.Equals(Convert.ToInt32(vm.Cbx_Personal_Abteilung[vm.SelectedItem_Personal_Abteilung].ID))));
                }
                if (vm.SelectedItem_Personal_Einsatzgebiet != 0)
                {
                    query = query.Where(row => row.wt2_konst_honorarkraft_einsatzgebiet.Any(a => a.khke_ident.Equals(Convert.ToInt32(vm.Cbx_Personal_Einsatzgebiet[vm.SelectedItem_Personal_Einsatzgebiet].khke_ident))));
                }
                if (vm.SelectedItem_Personal_Thema != 1)
                {
                    query = query.Where(row => row.wt2_konst_honorarkraft_thema.Any(a => a.khkth_ident.Equals(Convert.ToInt32(vm.Cbx_Personal_Thema[vm.SelectedItem_Personal_Thema].khkth_ident))));
                }
                # endregion

                # region Prüfe Checkboxen auf Suchparameter
                if (vm.Chkbx_Status_bedenklich == true)
                {
                    query = query.Where(row => row.wt2_honorarkraft_status.Any(a => a.hks_einsatz_bedenklich=="J"));
                }
                if (vm.Chkbx_Status_Pruefen == true)
                {
                    query = query.Where(row => row.wt2_honorarkraft_status.Any(a => a.hks_statusueberpruefung == "J"));
                }
                if (vm.Chkbx_Status_Selbstaendig == true)
                {
                    query = query.Where(row => row.wt2_honorarkraft_status.Any(a => a.hks_selbstaendigenstatus == "J"));
                }
                if (vm.Chkbx_Status_Verfolgen == true)
                {
                    query = query.Where(row => row.wt2_honorarkraft_status.Any(a => a.hks_weiterverfolgung == "J"));
                }
                # endregion

                # region Daten Vorsortieren
                /* 
                 * Schon beim Laden soll vorsortiert werden.
                 * Ein "Order by" direkt im LinQ Statment bringt LinQ durcheinander.
                 * LinQ ist der Meinung das eine Spalte zweimal vorkommt.
                 */
                query = query.OrderBy(o => o.hk_nachname).ThenBy(t => t.hk_vorname);
                # endregion

                result = new ObservableCollection<wt2_honorarkraft>(query);
            }
            return result;
        }
        public ObservableCollection<wt2_honorarkraft_vertrag> VertragsdatenSuche(VertragsdatenViewModel vm)
        {
            ObservableCollection<wt2_honorarkraft_vertrag> result = null;
            CreateContext();

            vm.HonorarListe = null;

            using (context)
            {
                # region Standartquery
                IEnumerable<wt2_honorarkraft_vertrag> query = from perso in context.wt2_honorarkraft_vertrag
                                                          .Include("wt2_honorarkraft.wt2_honorarkraft_team")
                                                          .Include("wt2_honorarkraft.wt2_honorarkraft_cluster")
                                                          .Include("wt2_honorarkraft")
                                                          .Include("wt2_honorarkraft_vertrag_position")
                                                          .Include("wt2_honorarkraft.wt2_konst_honorarkraft_thema")     
                                                      select perso;
                # endregion

                # region Prüfe Textboxen auf Suchparameter
                if (!string.IsNullOrWhiteSpace(vm.Tbx_Vertrag_Name))
                    query = query.Where(row => row.wt2_honorarkraft.hk_nachname.Contains(vm.Tbx_Vertrag_Name));
                if (!string.IsNullOrWhiteSpace(vm.Tbx_Vertrag_Vorname))
                    query = query.Where(row => row.wt2_honorarkraft.hk_vorname.Contains(vm.Tbx_Vertrag_Vorname));
                if (!string.IsNullOrWhiteSpace(vm.Tbx_Vertrag_Firma))
                    query = query.Where(row => row.wt2_honorarkraft.hk_firma.Contains(vm.Tbx_Vertrag_Firma));
                if (!string.IsNullOrWhiteSpace(vm.Tbx_Vertrag_Nummer))
                    query = query.Where(row => row.hkv_ident.Equals(Convert.ToInt32(vm.Tbx_Vertrag_Nummer)));
                if (!string.IsNullOrWhiteSpace(vm.Tbx_Vertrag_Auftrag))
                    query = query.Where(row => row.wt2_honorarkraft_vertrag_position.Any(a => a.hkvp_auftrag.Contains(vm.Tbx_Vertrag_Auftrag)));
                # endregion

                # region Prüfe Comboboxen auf Suchparameter
                if (vm.SelectedItem_Vertrag_Bildungstraeger != 0)
                {
                    query = query.Where(row => row.wt2_honorarkraft.hk_bildungstraeger.Equals(vm.Cbx_Vertrag_Bildungstraeger[vm.SelectedItem_Vertrag_Bildungstraeger].ID));
                }
                if (vm.SelectedItem_Vertrag_Teams != 0)
                {
                    query = query.Where(row => row.wt2_honorarkraft.wt2_honorarkraft_team.Any(a => a.hkt_team_nr.Equals(vm.Cbx_Vertrag_Teams[vm.SelectedItem_Vertrag_Teams].ID)));
                }
                # endregion

                # region Daten Vorsortieren
                /* 
                 * Schon beim Laden soll vorsortiert werden.
                 * Ein "Order by" direkt im LinQ Statment bringt LinQ durcheinander.
                 * LinQ ist der Meinung das eine Spalte zweimal vorkommt.
                */
                query = query.OrderBy(o => o.wt2_honorarkraft.hk_nachname).ThenBy(t => t.wt2_honorarkraft.hk_vorname);
                # endregion

                result = new ObservableCollection<wt2_honorarkraft_vertrag>(query);
            }

            return result;
        }

# region Testdaten generieren
        public ObservableCollection<wt2_honorarkraft> LoadTestdata()
        {
            ObservableCollection<wt2_honorarkraft> result = null;
            CreateContext();
     
            using (context)
            {
                var query =
                (from row in context.wt2_honorarkraft
                 where row.hk_nachname != "" && row.hk_vorname != ""
                 orderby row.hk_nachname
                 select row).ToList();

                result = new ObservableCollection<wt2_honorarkraft>(query);
            }
                return result;   
        }
# endregion
    }
}
