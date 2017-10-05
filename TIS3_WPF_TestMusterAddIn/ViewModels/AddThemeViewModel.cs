using BFZ_Common_Lib.MVVM;
using PostSharp.Patterns.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TIS3_Base;
using TIS3_Base.Commands;
using TIS3_Base.Services;
using TIS3_LookupBL;
using TIS3_WPF_TestMusterAddIn.Infrastructure;
using TIS3_WPF_TestMusterAddIn.Views;

namespace TIS3_WPF_TestMusterAddIn.ViewModels
{
    [NotifyPropertyChanged]
    class AddThemeViewModel : TIS3ActiveViewModel, ITIS3DialogViewModel
    {
        # region Data Access Objecte für Honorarkraefte Tabellen
        LookupRepository LookRepo = new LookupRepository();
        HonorarkraefteDAO hDAO = HonorarkraefteDAO.DAOFactory();
        # endregion

        # region Elemente der Neuaufnahmemaske
        public int SelectedItem_AddTheme_Gruppe { get; set; }
        public ObservableCollection<string> Cbx_AddTheme_Gruppe { get; set; }
        public string Tbx_AddTheme_Thema { get; set; }
        # endregion

        public override void Init()
        {
            InitComboBoxes();  
        }

        public void InitComboBoxes()
        {
            Cbx_AddTheme_Gruppe = new ObservableCollection<string>();

            Cbx_AddTheme_Gruppe = hDAO.HoleThemaGruppen();
        }

        public bool CancelAction()
        {
            MessageBox.Show("It works! - MessageBox Cancel");
            return true;
        }

        public bool OKAction()
        {
            MessageBox.Show("It works! - MessageBox Ok");
            return true;
        }


        public bool YesAction()
        {
            throw new NotImplementedException();
        }

        public bool NoAction()
        {
            throw new NotImplementedException();
        }

        public dynamic ReturnValue
        {
            get { throw new NotImplementedException(); }
        }
    }
}
