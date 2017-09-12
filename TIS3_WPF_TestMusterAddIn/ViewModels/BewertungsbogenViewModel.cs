using PostSharp.Patterns.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TIS3_Base;
using TIS3_LookupBL;
using WinTIS30db_entwModel.Lookup;

namespace TIS3_WPF_TestMusterAddIn.ViewModels
{
    [NotifyPropertyChanged]
    class BewertungsbogenViewModel : TIS3ActiveViewModel
    {
        public LookupCollectionBO Cbx_Bewertung_Abteilung { get; set; }
        public ObservableCollection<Bildungstraeger> Cbx_Bewertung_Bildungstraeger { get; set; }
        public ObservableCollection<string> Cbx_Bewertung_Jahr { get; set; }
        public override void Init()
        {
            InitComboBoxes();
        }

        public void InitComboBoxes()
        {
            Cbx_Bewertung_Abteilung = new LookupCollectionBO();
            Cbx_Bewertung_Bildungstraeger = new ObservableCollection<Bildungstraeger>();


            LookupRepository LookRepo = new LookupRepository();

            Cbx_Bewertung_Abteilung = LookRepo.GetAbteilungen(true);
            Cbx_Bewertung_Bildungstraeger = LookRepo.GetBildungstraeger(true);
            Cbx_Bewertung_Jahr = HoleJahresliste(true);
        }

        public ObservableCollection<string> HoleJahresliste(Boolean leeresElement)
        {
            ObservableCollection<string> result = new ObservableCollection<string>();

            if (leeresElement)
            {
                result.Add("");
            }

            DateTime localDate = DateTime.Now;

            for (int i = 2014; i <= (localDate.Year) + 2; i++)
            {
                result.Add(i.ToString());
            }

            return result;
        }
    }
}
