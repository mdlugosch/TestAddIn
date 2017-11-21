using Microsoft.Practices.Prism.PubSubEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TIS3_Base.Prism;
using WinTIS30db_entwModel.Honorarkraefte;

namespace TIS3_WPF_TestMusterAddIn.Events
{
    /*
     * VertragPositionPostedEvent das ein wt2_honorarkraft_vertrag_position entgegenimmt
     * Events dienen zum austauch zwischen Views und werden in die EventAggregatorListe eingetragen.
     */
    class VertragPositionPostedEvent : PubSubEvent<Message<wt2_honorarkraft_vertrag_position>>
    {
    }
}
