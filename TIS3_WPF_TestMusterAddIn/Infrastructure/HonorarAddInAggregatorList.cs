using Microsoft.Practices.Prism.PubSubEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TIS3_WPF_TestMusterAddIn.Infrastructure
{
    class HonorarAddInAggregatorList
    {
        /*
         * Statische Aggregatorliste die Events aufnimmt. Singelton-Entwurfsmuster
         * um zu gewehrleisten das alle Klassen auf die selbe Liste zugreifen.
         */
        public static IEventAggregator _aggregator;
        private HonorarAddInAggregatorList() { }       

        public static IEventAggregator AggregatorFactory
        {
            get
            {
                if (_aggregator == null)
                {
                    _aggregator = new EventAggregator();
                }
                return _aggregator;
            }
        }
    }
}
