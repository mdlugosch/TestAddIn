using Microsoft.Practices.Prism.PubSubEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TIS3_Base.Prism;
using TIS3_WPF_TestMusterAddIn.Infrastructure;
using WinTIS30db_entwModel.Honorarkraefte;

namespace TIS3_WPF_TestMusterAddIn.Events
{
    class ThemePostedEvent : PubSubEvent<Message<wt2_konst_honorarkraft_thema>>
    {
        /*
         * Das Event dient zur aktualisierung der EditViewTabs wenn in der Thema-Tree-View
         * ein neues Thema hinzugefügt wurde.
         * Das Event erwartet ein MessageObjekt vom Typ wt2_konst_honorarkraft_thema.
         * Der ursprüngliche Plan sah vor ein solches Object mit dem Event zusammen zu
         * senden. Für die Aktualisierungsweitergabe war dies letztlich nicht notwendig.
         * Das MessageObjekt wurde trotzdem beibehalten um es vielleicht in anderer Form
         * später nutzen zu können. Im Code wird als Objekt jedoch erstmal nur ein Null-Wert
         * übergeben.
         */
        internal void Subscribe()
        {
            throw new NotImplementedException();
        }
    }
}
