using BFZ_Common_Lib.MVVM;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Input;
using TIS3_Base;
using TIS3_WPF_TestMusterAddIn.Commands;
using WinTIS30db_entwModel.Honorarkraefte;

namespace TIS3_WPF_TestMusterAddIn.ViewModels
{
    // TIS3ViewModelBase Basisklasse für ViewModels mit notwendigen Schnittstellen aus Prism usw.
    class TestMusterAppViewModel : TIS3ActiveViewModel
    {
        public List<wt2_honorarkraft> HonorarListe { get; set; }
        public List<wt2_honorarkraft> HonorarLocation { get; set; }
        public List<wt2_honorarkraft> SFHonorarLocation { get; set; }
        public List<String> LocList { get; set; }
        public List<String> SFLocList { get; set; }

        [Import("OpenHonorarkraefteAppCommand")]
        public ICommand OpenHonorar { get; set; }
        public RelayCommand LoadTableData { get; set; }

        public string loc;
        public string Loc
        {
            get { return loc; }
            set
            {
                if (loc != value) { loc = value; LoadFilterTable(loc); }
            }
        }

        // Seperate Location Variable für die SyncFusion ComboBox
        public string sfloc;
        public string SFLoc
        {
            get { return sfloc; }
            set
            {
                if (sfloc != value) { sfloc = value; SFFilterTable(sfloc); }
            }
        }

        public override void Init()
        {
            Header.Title = "TestApp";       // Header Attribut zum bestimmen des Tab-Namens
            Header.Group = "Demonstration"; // Gruppenname der Anwendungs-Tabs

            using (var context = new WinTIS30db_entwEntities())
            {
                LocList = (from row in context.wt2_honorarkraft
                           where row.hk_ort!=""
                           select row.hk_ort).Distinct().ToList();

                SFLocList = LocList;
            }

            LoadTableData = new RelayCommand(_execute => { LoadTable(); }, _canExecute => { return true; });
        }

        public TestMusterAppViewModel()
        {
            OpenTestMusterAppCommand otm = new OpenTestMusterAppCommand();
        }

        public void LoadTable()
        {
            using (var context = new WinTIS30db_entwEntities())
            {
                //HonorarListe = context.wt2_honorarkraft.ToList();

                HonorarListe =
                (from row in context.wt2_honorarkraft
                 where row.hk_nachname != "" && row.hk_vorname != "" 
                orderby row.hk_nachname
                select row).ToList();
                
            }
        }

        public void LoadFilterTable(string location)
        {
            using (var context = new WinTIS30db_entwEntities())
            {
                HonorarLocation =
                (from row in context.wt2_honorarkraft
                 where row.hk_nachname != "" && row.hk_vorname != "" && row.hk_ort == location
                 orderby row.hk_nachname
                 select row).ToList();

            }
        }

        // SyncFusion Filter Methode
        public void SFFilterTable(string location)
        {
            using (var context = new WinTIS30db_entwEntities())
            {
                SFHonorarLocation =
                (from row in context.wt2_honorarkraft
                 where row.hk_nachname != "" && row.hk_vorname != "" && row.hk_ort == location
                 orderby row.hk_nachname
                 select row).ToList();

            }
        }

    }
}
