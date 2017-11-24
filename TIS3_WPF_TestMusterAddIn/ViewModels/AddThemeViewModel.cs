using BFZ_Common_Lib.MVVM;
using Microsoft.Practices.Prism.PubSubEvents;
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
using TIS3_Base.Prism;
using TIS3_Base.Services;
using TIS3_LookupBL;
using TIS3_WPF_TestMusterAddIn.Events;
using TIS3_WPF_TestMusterAddIn.Infrastructure;
using TIS3_WPF_TestMusterAddIn.Views;
using WinTIS30db_entwModel.Honorarkraefte;

namespace TIS3_WPF_TestMusterAddIn.ViewModels
{
    [NotifyPropertyChanged]
    class AddThemeViewModel : TIS3ActiveViewModel, ITIS3DialogViewModel
    {
        // Zugriff auf EventaggregatorListe
        readonly IEventAggregator _aggregator = HonorarAddInAggregatorList.AggregatorFactory;

        wt2_konst_honorarkraft_thema receiveObj = new wt2_konst_honorarkraft_thema();

        # region Data Access Objecte für Honorarkraefte Tabellen
        LookupRepository LookRepo = new LookupRepository();
        HonorarkraefteDAO hDAO;
        # endregion

        # region Elemente der Neuaufnahmemaske
        public string SelectedItem_AddTheme_Gruppe { get; set; }
        public ObservableCollection<string> Cbx_AddTheme_Gruppe { get; set; }
        public string Tbx_AddTheme_Thema { get; set; }
        # endregion

        public AddThemeViewModel() : base()
        {
            // Aktuelle Guid übergeben um ViewModel und Dao mit einem Context zu verbinden.
            hDAO = new HonorarkraefteDAO(this.ViewModelGuid);

            /*
             * InitComboBoxes muss im Konstruktor stehen damit die Guid nicht null ist.
             * Die Init-Methode wäre der falsche Ort für InitComboBoxes da diese zu früh
             * initialisiert werden würde zu einem Zeitpunkt an dem die Guid noch Null ist.
             */
            InitComboBoxes();
        }

        public override void Init()
        {
            // Event subscribtion
            //_aggregator.GetEvent<ThemePostedEvent>().Subscribe(GetDataMessage);
        }

        public void InitComboBoxes()
        {
            Cbx_AddTheme_Gruppe = new ObservableCollection<string>();

            Cbx_AddTheme_Gruppe = hDAO.HoleThemaGruppen(this.ViewModelGuid);
        }

        public bool CancelAction()
        {
            MessageBox.Show("It works! - MessageBox Cancel");
            return true;
        }

        public bool OKAction()
        {
            wt2_konst_honorarkraft_thema newLine = new wt2_konst_honorarkraft_thema();
            if (!string.IsNullOrWhiteSpace(SelectedItem_AddTheme_Gruppe) ||
                !string.IsNullOrWhiteSpace(Tbx_AddTheme_Thema)) 
                {
                    newLine.khkth_gruppe = SelectedItem_AddTheme_Gruppe;
                    newLine.khkth_bezeichnung = Tbx_AddTheme_Thema;
                    hDAO.AddNewTheme(newLine, this.ViewModelGuid);
                    _aggregator.GetEvent<ThemePostedEvent>().Publish(null);
                    return true;
                }
            return false;
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

        // Parameter des Events auslesen
        //private void GetDataMessage(Message<wt2_konst_honorarkraft_thema> messageObj)
        //{
        //    if (messageObj.Sender == ParentViewModelGuid)
        //    {
        //        receiveObj = messageObj.Payload;
        //    }
        //}

        public override void ApplyNavigationParameters(Microsoft.Practices.Prism.Regions.NavigationParameters navigationParameters)
        {
            if (navigationParameters != null)
            {
                this.ParentViewModelGuid = navigationParameters["guid"] as string;
            }
        }  
    }
}
