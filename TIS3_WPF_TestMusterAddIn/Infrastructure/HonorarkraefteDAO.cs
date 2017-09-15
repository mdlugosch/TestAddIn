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

        // Auslesen von Daten mit hilfe des Context
        public ObservableCollection<wt2_konst_honorarkraft_einsatzgebiet> HoleEinsatzgebiete(Boolean leeresElement)
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
            return new ObservableCollection<wt2_konst_honorarkraft_einsatzgebiet>(query);
        }

        public ObservableCollection<wt2_konst_honorarkraft_thema> HoleThema(Boolean leeresElement)
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

            return new ObservableCollection<wt2_konst_honorarkraft_thema>(query);
        }

        public static void DisposeContext() 
        {
            context.Dispose();
        }

# region Testdaten generieren
        public ObservableCollection<wt2_honorarkraft> LoadTestdata()
        {
                var query =
                (from row in context.wt2_honorarkraft
                 where row.hk_nachname != "" && row.hk_vorname != ""
                 orderby row.hk_nachname
                 select row).ToList();
                return new ObservableCollection<wt2_honorarkraft>(query);   
        }
# endregion
    }
}
