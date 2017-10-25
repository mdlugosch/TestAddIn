using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TIS3_LookupBL;
using TIS3_WPF_TestMusterAddIn.ViewModels;
using WinTIS30db_entwModel.Honorarkraefte;
using WinTIS30db_entwModel.Lookup;

namespace TIS3_WPF_TestMusterAddIn.Infrastructure
{
    /*
     * Data Access Object - HonorarkraefteDAO
     * Sammelklasse für LinQ-Befehle die die Honorarkräftetabellen nutzen
     */
    public class HonorarkraefteDAO 
    {
        LookupRepository LookRepo = new LookupRepository();

        # region Context für die Honorarkräfte-Tabellen holen
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
        # endregion

        # region Abfragen die Daten für Steuerelemente liefern
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
        public ObservableCollection<Einsatzgebiet_Check> HoleEinsatzgebieteListe(wt2_honorarkraft honorarkraftDaten = null)
        {
            ObservableCollection<Einsatzgebiet_Check> result = new ObservableCollection<Einsatzgebiet_Check>();

            CreateContext();
            using (context)
            {
                // ID-Liste mit den Einsatzgebiete der HK
                List<int> auswahl = new List<int>();
                if (honorarkraftDaten != null)
                    auswahl.AddRange(from eg in honorarkraftDaten.wt2_konst_honorarkraft_einsatzgebiet select eg.khke_ident);

                // Wenn die ID der Standartliste gleich der ID-Liste ist wird IsChecked wahr
                var query = (from einsatzGebiete in context.wt2_konst_honorarkraft_einsatzgebiet
                             orderby einsatzGebiete.khke_bezeichnung
                             select new Einsatzgebiet_Check()
                             {
                                 IsChecked = auswahl.Contains(einsatzGebiete.khke_ident),
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

        public LookupCollectionBO HoleAbteilungsListe(bool leeresElement, wt2_honorarkraft honorarkraftDaten = null)
        {
            LookupCollectionBO boResult = new LookupCollectionBO();
            boResult = LookRepo.GetAbteilungen(leeresElement); 

            List<int> auswahl = new List<int>();
            if (honorarkraftDaten != null) 
            { 
                auswahl.AddRange(from abteilung in honorarkraftDaten.wt2_honorarkraft_cluster select abteilung.hkc_cl_ident);

            foreach (int id in auswahl)
            {
                foreach (LookupBO element in boResult) 
                {
                    if (Convert.ToInt32(element.ID) == id) element.IsChecked = true;
                }
            }
            }

            return boResult;
        }

        # region (EditView) Ablegen von temporären Daten die zur Laufzeit ermittelt werden.
        public void HoleVertraege(wt2_honorarkraft honorarkraftDaten,LookupCollectionBO abteilungsListe)
        {
            int i = 1;

            CreateContext();
            
            using (context)
            {
                honorarkraftDaten.wt2_honorarkraft_vertrag.ToList().ForEach(a => 
                { a.hkv_zeile = i++;
                a.hkv_abt_bez = (from element in abteilungsListe
                                where Convert.ToInt32(element.ID) == a.hkv_abteilung
                                select element.Bezeichnung).FirstOrDefault();
                  a.hkv_summe = (from element in context.wt2_honorarkraft_vertrag_position
                                where element.hkvp_hkv_ident == a.hkv_ident
                                select element.hkvp_honorar * element.hkvp_unterrichtseinheiten).Sum();
                });

            }
        }

        public void HoleBewertungen(wt2_honorarkraft honorarkraftDaten)
        {
            int i = 1;

            CreateContext();

            using (context)
            {
                honorarkraftDaten.wt2_honorarkraft_bewertung.ToList().ForEach(a =>
                {
                    a.hkb_zeile = i++;
                    a.hkb_abt_bez = (LookupHelper.Abteilung(Convert.ToInt32(a.hkb_abteilung))).Displayname;
                });

            }
        }

        public void HoleZahlungsanweisungen(wt2_honorarkraft honorarkraftDaten)
        {
            int i = 1;

            CreateContext();

            using (context)
            {
                honorarkraftDaten.wt2_honorarkraft_zahlungsanweisung.ToList().ForEach(a =>
                {
                    a.hkz_zeile = i++;
                    a.hkz_summe = (from element in context.wt2_honorarkraft_zahlungsanweisung_position
                                   where element.hkzp_hkz_ident == a.hkz_ident
                                   select element.hkzp_unterrichtseinheiten * element.wt2_honorarkraft_vertrag_position.hkvp_honorar).Sum();
                    if (!a.hkz_summe.HasValue) a.hkz_summe = 0;
                });

            }
        }
        # endregion

        public ObservableCollection<wt2_honorarkraft_vertrag_position> HoleVertragsPositionen(wt2_honorarkraft_vertrag vertragsdaten)
        {
            CreateContext();
            ObservableCollection<wt2_honorarkraft_vertrag_position> result;

            using (context)
            {
                if (vertragsdaten != null)
                {
                    IEnumerable<wt2_honorarkraft_vertrag_position> query = from positionen in vertragsdaten.wt2_honorarkraft_vertrag_position
                                                                           select positionen;
                    result = new ObservableCollection<wt2_honorarkraft_vertrag_position>(query);
                }
                else return null;
            }

            return result;
        }

        public ObservableCollection<wt2_honorarkraft_zahlungsanweisung_position> HoleZahlungsPositionen(wt2_honorarkraft_zahlungsanweisung zahlungsdaten)
        {
            CreateContext();
            ObservableCollection<wt2_honorarkraft_zahlungsanweisung_position> result;
            int i = 1;

            using (context)
            {
                if (zahlungsdaten != null)
                {
                    IEnumerable<wt2_honorarkraft_zahlungsanweisung_position> query = from positionen in zahlungsdaten.wt2_honorarkraft_zahlungsanweisung_position
                                                                             select positionen;


                    query.ToList().ForEach(a =>
                    {
                        a.hkzp_zeile = i++;

                        a.hkzp_auszahlung = ((from hkvp in context.wt2_honorarkraft_vertrag_position
                                              where hkvp.hkvp_hkv_ident == a.hkzp_hkv_ident
                                              select hkvp.hkvp_honorar).FirstOrDefault()) * a.hkzp_unterrichtseinheiten;
                        if (!a.hkzp_auszahlung.HasValue) a.hkzp_auszahlung = 0;
                    });
                                                                               
                    result = new ObservableCollection<wt2_honorarkraft_zahlungsanweisung_position>(query);
                }
                else return null;
            }

            return result;
        }

        public LookupCollectionBO HoleTeamListe(bool nurGueltig, string aktuellesTeam, bool leeresElement,  bool mitNummerInBezeichnung = false, wt2_honorarkraft honorarkraftDaten = null)
        {
            LookupCollectionBO boResult = new LookupCollectionBO();
            boResult = LookRepo.GetTeams(nurGueltig, aktuellesTeam, leeresElement, mitNummerInBezeichnung);

            List<string> auswahl = new List<string>();
            if (honorarkraftDaten != null)
            {
                auswahl.AddRange(from team in honorarkraftDaten.wt2_honorarkraft_team select team.hkt_team_nr);

                foreach (string id in auswahl)
                {
                    foreach (LookupBO element in boResult)
                    {
                        if (element.ID.Equals(id)) element.IsChecked = true;
                    }
                }
            }

            return boResult;
        }

        // ThemaListe aufbereiten für TreeView
        public ObservableCollection<SortedList_Thema> HoleThemaListe(wt2_honorarkraft honorarkraftDaten=null)
        {
            // Result als sortierte Liste
            ObservableCollection<SortedList_Thema> sortedResult = new ObservableCollection<SortedList_Thema>();

            CreateContext();

            using (context)
            {
                List<Thema_Check> result;

                List<int> auswahl = new List<int>();
                if(honorarkraftDaten != null)
                    auswahl.AddRange(from thema in honorarkraftDaten.wt2_konst_honorarkraft_thema select thema.khkth_ident);

                // Erstellen einer unsortierten Liste mit zusätzlicher IsChecked Property
                result = (from hThema in context.wt2_konst_honorarkraft_thema
                         where !String.IsNullOrEmpty(hThema.khkth_gruppe) ||
                               !String.IsNullOrEmpty(hThema.khkth_bezeichnung)
                                      orderby hThema.khkth_bezeichnung
                                      select new Thema_Check()
                                        {
                                        IsChecked = auswahl.Contains(hThema.khkth_ident),
                                        Thema = hThema
                                        }).ToList();



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
        # endregion

        # region LinQ-Abfragen der verschiedenen Suchmasken
        public ObservableCollection<wt2_honorarkraft> PersonendatenSuche(PersonaldatenViewModel vm)
        {
            ObservableCollection<wt2_honorarkraft> result = null;
            CreateContext();

            vm.HonorarListe = null;

            using (context)
            {
                # region Standardabfrage
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
                # region Standardabfrage
                IEnumerable<wt2_honorarkraft_vertrag> query = from vertrag in context.wt2_honorarkraft_vertrag
                                                          .Include("wt2_honorarkraft.wt2_honorarkraft_team")
                                                          .Include("wt2_honorarkraft.wt2_honorarkraft_cluster")
                                                          .Include("wt2_honorarkraft")
                                                          .Include("wt2_honorarkraft_vertrag_position")
                                                          .Include("wt2_honorarkraft.wt2_konst_honorarkraft_thema")     
                                                      select vertrag;
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
                    query = query.Where(row => row.hkv_bildungstraeger.Equals(vm.Cbx_Vertrag_Bildungstraeger[vm.SelectedItem_Vertrag_Bildungstraeger].ID));
                }
                if (vm.SelectedItem_Vertrag_Teams != 0)
                {
                    query = query.Where(row => row.wt2_honorarkraft_vertrag_position.Any(a => a.hkvp_kostenstelle.Equals(vm.Cbx_Vertrag_Teams[vm.SelectedItem_Vertrag_Teams].ID)));
                }
                if (vm.SelectedItem_Vertrag_Abteilung != 0)
                {
                    query = query.Where(row => row.hkv_abteilung.Equals(Convert.ToInt32(vm.Cbx_Vertrag_Abteilung[vm.SelectedItem_Vertrag_Abteilung].ID)));
                }
                if (!string.IsNullOrWhiteSpace(vm.Tbx_Vertrag_Thema))
                {
                    query = query.Where(row => row.wt2_honorarkraft_vertrag_position.Any(a => a.hkvp_thema.Equals(vm.Tbx_Vertrag_Thema)));
                }
                if (!string.IsNullOrWhiteSpace(vm.Dp_Vertrag_Datum))
                {
                    DateTime dt = DateTime.Parse(vm.Dp_Vertrag_Datum);
                    query = query.Where(row => row.hkv_datum.Equals(dt));
                }
                # endregion

                # region Prüfe Checkboxen auf Suchparameter
                if (vm.Chkbx_Vertrag_400 == true)
                {
                    query = query.Where(row => row.hkv_400EuroBasis.Contains("J"));
                }
                if (vm.Chkbx_Vertrag_Gedruckt == true)
                {
                    query = query.Where(row => string.IsNullOrWhiteSpace((row.hkv_druckdatum.ToString())));
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

        public ObservableCollection<wt2_honorarkraft_zahlungsanweisung> ZahlungsanweisungSuche(ZahlungsanweisungViewModel vm)
        {
            ObservableCollection<wt2_honorarkraft_zahlungsanweisung> result = null;
            CreateContext();

            vm.HonorarListe = null;

            using (context)
            {
                # region Standardabfrage
                IEnumerable<wt2_honorarkraft_zahlungsanweisung> query = from zahlung in context.wt2_honorarkraft_zahlungsanweisung
                                                          .Include("wt2_honorarkraft")
                                                          .Include("wt2_honorarkraft_zahlungsanweisung_position.wt2_honorarkraft_vertrag_position")
                                                          select zahlung;  
                # endregion

                # region Prüfe Textboxen auf Suchparameter
                if (!string.IsNullOrWhiteSpace(vm.Tbx_Zahlung_Name))
                    query = query.Where(row => row.wt2_honorarkraft.hk_nachname.Contains(vm.Tbx_Zahlung_Name));
                if (!string.IsNullOrWhiteSpace(vm.Tbx_Zahlung_Vorname))
                    query = query.Where(row => row.wt2_honorarkraft.hk_vorname.Contains(vm.Tbx_Zahlung_Vorname));
                if (!string.IsNullOrWhiteSpace(vm.Tbx_Zahlung_Firma))
                    query = query.Where(row => row.wt2_honorarkraft.hk_firma.Contains(vm.Tbx_Zahlung_Firma));
                if (!string.IsNullOrWhiteSpace(vm.Tbx_Zahlung_Nummer))
                    query = query.Where(row => row.hkz_ident.Equals(Convert.ToInt32(vm.Tbx_Zahlung_Nummer)));
                if (!string.IsNullOrWhiteSpace(vm.Dp_Zahlung_Datum))
                {
                    DateTime dt = DateTime.Parse(vm.Dp_Zahlung_Datum);
                    query = query.Where(row => row.hkz_datum.Equals(dt));
                }
                if (!string.IsNullOrWhiteSpace(vm.Tbx_Zahlung_Auftrag))
                    query = query.Where(row => row.wt2_honorarkraft_zahlungsanweisung_position.Any(a => 
                    {
                        if (a.wt2_honorarkraft_vertrag_position != null)
                            return a.wt2_honorarkraft_vertrag_position.hkvp_auftrag.Contains(vm.Tbx_Zahlung_Auftrag);
                        else return false;
                    }));

                # endregion

                # region Prüfe Comboboxen auf Suchparameter
                if (vm.SelectedItem_Zahlung_Thema!=1)
                    query = query.Where(row => row.wt2_honorarkraft_zahlungsanweisung_position.Any(a =>
                    {
                        if (a.wt2_honorarkraft_vertrag_position != null)
                            return a.wt2_honorarkraft_vertrag_position.hkvp_thema.Contains(vm.Cbx_Zahlung_Thema[vm.SelectedItem_Zahlung_Thema].khkth_bezeichnung);
                        else return false;
                    }));

                if (vm.SelectedItem_Zahlung_Abteilung != 0)
                    query = query.Where(row => row.hkz_abteilung.Equals(Convert.ToInt32(vm.Cbx_Zahlung_Abteilung[vm.SelectedItem_Zahlung_Abteilung].ID)));
                if (vm.SelectedItem_Zahlung_Bildungstraeger != 0)
                    query = query.Where(row => row.hkz_bildungstraeger.Equals(Convert.ToInt32(vm.Cbx_Zahlung_Bildungstraeger[vm.SelectedItem_Zahlung_Bildungstraeger].ID)));
                if (vm.SelectedItem_Zahlung_Teams != 0)
                    query = query.Where(row => row.wt2_honorarkraft_zahlungsanweisung_position.Any(a =>
                    {
                        if (a.wt2_honorarkraft_vertrag_position != null)
                            return a.wt2_honorarkraft_vertrag_position.hkvp_kostenstelle.Contains(vm.Cbx_Zahlung_Teams[vm.SelectedItem_Zahlung_Teams].ID);
                        else return false;
                    }));
                # endregion

                # region Prüfe Checkboxen auf Suchparameter
                if (vm.Chkbx_Zahlung_Gedruckt == true)
                {
                    query = query.Where(row => string.IsNullOrWhiteSpace((row.hkz_druckdatum.ToString())));
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

                result = new ObservableCollection<wt2_honorarkraft_zahlungsanweisung>(query);
            }
            return result;
        }

        public ObservableCollection<wt2_honorarkraft_bewertung> BewertungbogenSuche(BewertungsbogenViewModel vm)
        {
            ObservableCollection<wt2_honorarkraft_bewertung> result = null;
            CreateContext();

            vm.HonorarListe = null;

            using (context)
            {
                # region Standardabfrage
                IEnumerable<wt2_honorarkraft_bewertung> query = from bewertung in context.wt2_honorarkraft_bewertung
                                                          .Include("wt2_honorarkraft")
                                                          select bewertung;
                # endregion

                # region Prüfe Textboxen auf Suchparameter
                if (!string.IsNullOrWhiteSpace(vm.Tbx_Bewertung_Name))
                    query = query.Where(row => row.wt2_honorarkraft.hk_nachname.Contains(vm.Tbx_Bewertung_Name));
                if (!string.IsNullOrWhiteSpace(vm.Tbx_Bewertung_Vorname))
                    query = query.Where(row => row.wt2_honorarkraft.hk_vorname.Contains(vm.Tbx_Bewertung_Vorname));
                if (!string.IsNullOrWhiteSpace(vm.Tbx_Bewertung_Firma))
                    query = query.Where(row => row.wt2_honorarkraft.hk_firma.Contains(vm.Tbx_Bewertung_Firma));
                # endregion

                # region Prüfe Comboboxen auf Suchparameter
                if (vm.SelectedItem_Bewertung_Bildungstraeger != 0)
                {
                    query = query.Where(row => row.hkb_bildungstraeger.Equals(vm.Cbx_Bewertung_Bildungstraeger[vm.SelectedItem_Bewertung_Bildungstraeger].ID));
                }
                if (vm.SelectedItem_Bewertung_Abteilung != 0)
                {
                    query = query.Where(row => row.hkb_abteilung.Equals(Convert.ToInt32(vm.Cbx_Bewertung_Abteilung[vm.SelectedItem_Bewertung_Abteilung].ID)));
                }
                # endregion

                # region DatePicker und Decimal-Berechnung
                if (!string.IsNullOrWhiteSpace(vm.Dp_Bewertung_TerminVon) && !string.IsNullOrWhiteSpace(vm.Dp_Bewertung_TerminBis))
                {
                    DateTime TerminVon = DateTime.Parse(vm.Dp_Bewertung_TerminVon);
                    DateTime TerminBis = DateTime.Parse(vm.Dp_Bewertung_TerminBis);
                    if(TerminVon<=TerminBis)
                        query = query.Where(row => row.hkb_verbesserung_termin >= TerminVon && row.hkb_verbesserung_termin <= TerminBis); 
                }
                if (!string.IsNullOrWhiteSpace(vm.Tbx_Bewertung_TLVon) && !string.IsNullOrWhiteSpace(vm.Tbx_Bewertung_TLBis))
                {
                    Decimal TLVon = Parse(vm.Tbx_Bewertung_TLVon);
                    Decimal TLBis = Parse(vm.Tbx_Bewertung_TLBis);
                    if (TLVon <= TLBis)
                        query = query.Where(row => row.hkb_befragung_tl >= TLVon && row.hkb_befragung_tl <= TLBis); 
                }
                if (!string.IsNullOrWhiteSpace(vm.Tbx_Bewertung_TNVon) && !string.IsNullOrWhiteSpace(vm.Tbx_Bewertung_TNBis))
                {
                    Decimal TNVon = Parse(vm.Tbx_Bewertung_TNVon);
                    Decimal TNBis = Parse(vm.Tbx_Bewertung_TNBis);
                    if (TNVon <= TNBis)
                        query = query.Where(row => row.hkb_befragung_tl >= TNVon && row.hkb_befragung_tl <= TNBis); 
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

                result = new ObservableCollection<wt2_honorarkraft_bewertung>(query);
            }
            return result;
        }

        public ObservableCollection<wt2_honorarkraft> UeberpruefungsSuche(UeberpruefungsViewModel vm)
        {
            ObservableCollection<wt2_honorarkraft> result = null;
            CreateContext();

            using (context)
            {
                if (vm.SelectedItem_Pruefung_Jahr != 0)
                {
                    int auswahlJahr = Convert.ToInt32(vm.Cbx_Pruefung_Jahr[vm.SelectedItem_Pruefung_Jahr]);

                    # region ID-Liste mit Honorarkräften die in dem angegebenen Jahr nicht bewertet wurden.
                    IEnumerable<int> unbewertet = from row in context.wt2_honorarkraft
                                                  .Include("wt2_honorarkraft_bewertung")
                                             where row.wt2_honorarkraft_bewertung.Any(a => a.hkb_datum.Value.Year.Equals(auswahlJahr)) == false
                                             select row.hk_ident;
                    # endregion

                    # region Honorarkräften im gewählten Jahr mit gültigen Vertrag mit Unterrichtsstunden >= 40
                    var stunden = from row in context.wt2_honorarkraft_vertrag_position
                                              .Include("wt2_honorarkraft_vertrag")
                                  where unbewertet.Contains(row.wt2_honorarkraft_vertrag.hkv_hk_ident) && row.hkvp_datum_beginn.Value.Year <= auswahlJahr && row.hkvp_datum_ende.Value.Year >= auswahlJahr
                                  group row by row.wt2_honorarkraft_vertrag.hkv_hk_ident into g
                                  select new { Id = g.Key, Count = g.Sum(item => item.hkvp_unterrichtseinheiten) };
                    # endregion

                    # region Filtern der Honorarkraefte die 40 oder mehr Unterrichtsstunden im Vertragszeitrum hatten
                    IEnumerable<wt2_honorarkraft> honorar = from row in context.wt2_honorarkraft
                                                            where stunden.Any(a => a.Id == row.hk_ident && a.Count >= 40)
                                                            select row;
                    # endregion

                    # region Daten Vorsortieren
                    /* 
                     * Schon beim Laden soll vorsortiert werden.
                     * Ein "Order by" direkt im LinQ Statment bringt LinQ durcheinander.
                     * LinQ ist der Meinung das eine Spalte zweimal vorkommt.
                     */
                    honorar = honorar.OrderBy(o => o.hk_nachname).ThenBy(t => t.hk_vorname);
                    # endregion

                    result = new ObservableCollection<wt2_honorarkraft>(honorar);
                }           
            }
            return result;
        }
        # endregion

        # region LinQ-Abfragen für die EditView
        public wt2_honorarkraft FillEditView(string personId)
        {
            wt2_honorarkraft result = null;
            int id = int.Parse(personId);

            CreateContext();

                var query = (from row in context.wt2_honorarkraft
                                         where row.hk_ident.Equals(id)
                                         select row).SingleOrDefault();

                result = (wt2_honorarkraft)query;

            return result;
        }

        // Setzen der Checkboxharken-Einsatzgebiete in der EditView
        public ObservableCollection<Einsatzgebiet_Check> CheckEinsatzgebiete(string personId, ObservableCollection<Einsatzgebiet_Check> checkList)
        {
            int id = int.Parse(personId);

            CreateContext();

            using (context)
            {

                var query = (from row in context.wt2_honorarkraft
                            where row.hk_ident.Equals(id)
                            select row).SingleOrDefault();
 
                foreach (Einsatzgebiet_Check element in checkList)
                {
                    foreach (wt2_konst_honorarkraft_einsatzgebiet eg in query.wt2_konst_honorarkraft_einsatzgebiet) 
                    {
                        if (eg.khke_ident == element.Einsatzgebiet.khke_ident) 
                            element.IsChecked = true;
                    }


                }
            }
            return checkList;
        }

        // Setzen der Checkboxharken-Teams in der EditView
        public LookupCollectionBO CheckTeams(string personId, LookupCollectionBO checkList)
        {
            int id = int.Parse(personId);

            CreateContext();

            using (context)
            {

                var query = (from row in context.wt2_honorarkraft
                             where row.hk_ident.Equals(id)
                             select row).SingleOrDefault(); ;

                foreach (LookupBO element in checkList)
                {
                    foreach (wt2_honorarkraft_team eg in query.wt2_honorarkraft_team)
                    {
                        if (eg.hkt_team_nr == element.ID) element.IsChecked = true;
                    }

                }
            }
            return checkList;
        }

        # endregion

        # region Hilfsmethoden

        public int GetBildungstraegerNummer(int? flag, ObservableCollection<Bildungstraeger> checkList)
        {
            int result=0;
            if (flag != null) 
            { 
            foreach (Bildungstraeger element in checkList)
            {
                if (element.ID == Convert.ToInt32(flag)) return result;
                result++;
            }
            }
            return result;
        }

        public int ConvertAnrede(String flag)
        {
            if (!string.IsNullOrWhiteSpace(flag))
            {
                if (flag.Equals("W")) return 1;
                else if (flag.Equals("M")) return 2;
                else return 3;
            }
            else return 0;
        }

        public string DecimalToString(Decimal number)
        {
            return string.Format("{0:C}", number);
        }

        // Umwandlung deutsche dezimalnotation in englische
        private static decimal Parse(string s)
        {
            s = s.Replace(",", ".");
            return decimal.Parse(s);
        }

# endregion

    }
}
