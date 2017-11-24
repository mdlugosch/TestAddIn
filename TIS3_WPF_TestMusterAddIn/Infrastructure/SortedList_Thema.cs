using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinTIS30db_entwModel.Honorarkraefte;

namespace TIS3_WPF_TestMusterAddIn.Infrastructure
{
    /*
     * Für die TreeView muss eine Struktur erstellt werden in der Themen den
     * Gruppen zugeordnet werden. Da die Datenbank eine solche Struktur für Themen und
     * Gruppen nicht bereitstellt ist eine eigene Struktur nötig in der die Datensätze
     * nach der Verarbeitung getrennt nach Gruppen abgelegt werden. SortedList_Thema
     * enthält pro Gruppe eine Liste mit Themen die der Gruppe zugeordnet sind.
     */
    public class SortedList_Thema
    {
        public string Gruppe { get; set; }
        public CheckedLookupCollection<wt2_konst_honorarkraft_thema> ThemeGroup { get; set; }

    }
}
