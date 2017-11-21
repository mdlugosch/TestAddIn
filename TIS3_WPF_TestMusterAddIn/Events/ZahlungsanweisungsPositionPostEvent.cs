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
    class ZahlungsanweisungsPositionPostEvent : PubSubEvent<Message<wt2_honorarkraft_zahlungsanweisung_position>>
    {
    }
}
