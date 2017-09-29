using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinTIS30db_entwModel.Honorarkraefte;

namespace TIS3_WPF_TestMusterAddIn.Infrastructure
{
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
                             orderby hThema.khkth_bezeichnung
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
