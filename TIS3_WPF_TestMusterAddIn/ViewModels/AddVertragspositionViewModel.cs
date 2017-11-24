using BFZ_Common_Lib.MVVM;
using BFZ_WPF_Lib.BusinessObjects;
using Microsoft.Practices.Prism.PubSubEvents;
using Microsoft.Practices.Prism.Regions;
using PostSharp.Patterns.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TIS3_Base;
using TIS3_Base.Prism;
using TIS3_LookupBL;
using TIS3_WPF_TestMusterAddIn.Events;
using TIS3_WPF_TestMusterAddIn.Infrastructure;
using WinTIS30db_entwModel.Honorarkraefte;
using WinTIS30db_entwModel.Lookup;

namespace TIS3_WPF_TestMusterAddIn.ViewModels
{
    [NotifyPropertyChanged]
    class AddVertragspositionViewModel : TIS3ActiveViewModel, ITIS3DialogViewModel
    {
        # region Undo-Backup-Variables
        private struct BackupData
        {
            public int? avp_unterrichtseinheiten_zeiteinheit;
            public bool avp_Selbstlernphase;
            public string hkvp_kostenstelle;
            public string hkvp_auftrag;
            public DateTime? hkvp_datum_beginn;
            public DateTime? hkvp_datum_ende;
            public string hkvp_thema;
            public Decimal hkvp_unterrichtseinheiten;
            public Decimal hkvp_honorar;
        }

        BackupData avp_backup;
        # endregion

        public RelayCommand ResetCommand { get; set; }

        // Zugriff auf EventaggregatorListe
        readonly IEventAggregator _aggregator = HonorarAddInAggregatorList.AggregatorFactory;

        wt2_honorarkraft_vertrag_position receiveObj = new wt2_honorarkraft_vertrag_position();
        public wt2_honorarkraft_vertrag_position ReceiveObj { get { return receiveObj; } set { receiveObj = value; } }

        public Decimal tbx_Avp_Gesamt=0;
        public Decimal Tbx_Avp_Gesamt { 
            get 
            {
                if (ReceiveObj != null)
                {
                    tbx_Avp_Gesamt = ReceiveObj.hkvp_honorar * ReceiveObj.hkvp_unterrichtseinheiten;
                }
                return tbx_Avp_Gesamt;
            }
        }

        public bool Chkbx_Avp_Selbstlernphase { 
        get 
        { 
            if (Depends.Guard)
            { Depends.On(this.receiveObj.hkvp_info); }
            if (receiveObj!=null && receiveObj.hkvp_info != null)
            if (receiveObj.hkvp_info.ToLower().Contains("selbstlernphase")) 
            {
                return true;
            }
            else 
            {       
                return false; 
            }
            return false;
        } 
        set 
        {
            if (value == true)
            {
                receiveObj.hkvp_info = "Selbstlernphase";
            }
            else { receiveObj.hkvp_info = ""; }
        } 
        }

        public string Tbx_Avp_Aussteller 
        { 
            get 
            {
                if (Depends.Guard)
                {
                    Depends.On
                      (
                        ReceiveObj.wt2_honorarkraft_vertrag.wt2_honorarkraft.hk_bildungstraeger
                      );
                }
                string aussteller = "";
                if (ReceiveObj != null &&
                    ReceiveObj.wt2_honorarkraft_vertrag != null &&
                    ReceiveObj.wt2_honorarkraft_vertrag.hkv_abteilung != null)
                {
                    int? traegerID = ReceiveObj.wt2_honorarkraft_vertrag.hkv_bildungstraeger;
                    int? abteilungID = ReceiveObj.wt2_honorarkraft_vertrag.hkv_abteilung;

                    if (traegerID != null) 
                    { 
                        int btID = (int)traegerID;
                        string ausstellerName = "<Platzhalter> Ausstellername";
                        string name1 = LookupHelper.Bildungstraeger(btID).Name1;
                        string name2 = LookupHelper.Bildungstraeger(btID).Name2;
                        string abteilung = LookupHelper.Abteilung(abteilungID.Value).Displayname;
                        string strasse = LookupHelper.Bildungstraeger(btID).Strasse;
                        string ort = LookupHelper.Bildungstraeger(btID).Ort;
                        string plz = LookupHelper.Bildungstraeger(btID).PLZ;

                        // String zusammenbauen
                        aussteller  += ausstellerName + "\n"
                                    + name1.Trim() + " " 
                                    + name2.Trim() + "\n"
                                    + abteilung.Trim() + "\n" 
                                    + strasse.Trim() + "\n"
                                    + plz.Trim() + " " + ort.Trim();
                    }
                    
                }
                return aussteller;
            } 
        }

        public string Tbx_Avp_Auftragnehmer
        {
            get
            {
                string auftragnehmer = "";

                if (receiveObj != null) 
                {
                    if (Depends.Guard)
                    {
                        Depends.On
                          (
                          receiveObj.wt2_honorarkraft_vertrag.wt2_honorarkraft.hk_nachname,
                          receiveObj.wt2_honorarkraft_vertrag.wt2_honorarkraft.hk_plz
                          );
                    }
                    if (receiveObj.wt2_honorarkraft_vertrag != null && receiveObj.wt2_honorarkraft_vertrag.wt2_honorarkraft != null) 
                    { 
                        if (receiveObj.wt2_honorarkraft_vertrag.wt2_honorarkraft.hk_vorname != null && receiveObj.wt2_honorarkraft_vertrag.wt2_honorarkraft.hk_nachname != null) auftragnehmer += receiveObj.wt2_honorarkraft_vertrag.wt2_honorarkraft.hk_vorname.Trim() + " " + receiveObj.wt2_honorarkraft_vertrag.wt2_honorarkraft.hk_nachname + "\n";
                        if (receiveObj.wt2_honorarkraft_vertrag.wt2_honorarkraft.hk_firma != null) auftragnehmer += receiveObj.wt2_honorarkraft_vertrag.wt2_honorarkraft.hk_firma + "\n";
                        if (receiveObj.wt2_honorarkraft_vertrag.wt2_honorarkraft.hk_strasse != null) auftragnehmer += receiveObj.wt2_honorarkraft_vertrag.wt2_honorarkraft.hk_strasse + "\n";
                        if (receiveObj.wt2_honorarkraft_vertrag.wt2_honorarkraft.hk_plz != null && receiveObj.wt2_honorarkraft_vertrag.wt2_honorarkraft.hk_ort != null) auftragnehmer += receiveObj.wt2_honorarkraft_vertrag.wt2_honorarkraft.hk_plz.Trim() + " " + receiveObj.wt2_honorarkraft_vertrag.wt2_honorarkraft.hk_ort;
                    }
                }
                return auftragnehmer;
            }
        }

        # region Zeiteinheitenproperty
        public List<string> zeiteinheitenListe = new List<string>() { "", "Unterrichtsstunden", "Stunden" };
        public List<string> Cbx_Avp_Zeiteinheiten { get { return zeiteinheitenListe; } }

        public int? SelectedItem_avp_zeiteinheit
        {
            get 
            {
                if (receiveObj != null)
                    return receiveObj.hkvp_unterrichtseinheiten_zeiteinheit;
                else return 0;
            } 
            set 
            {
                switch (value)
                {
                    case 1: receiveObj.hkvp_unterrichtseinheiten_zeiteinheit = 1;  break;
                    case 2: receiveObj.hkvp_unterrichtseinheiten_zeiteinheit = 2; break;
                    default: receiveObj.hkvp_unterrichtseinheiten_zeiteinheit = 0; break;
                }
            } 
        }
        # endregion    

        public AddVertragspositionViewModel() : base()
        {
        }

        public override void Init()
        {
            // Event subscribtion
            _aggregator.GetEvent<VertragPositionPostedEvent>().Subscribe(GetDataMessage);

            ResetCommand = new RelayCommand(_execute => this.Reset_AVP_View(), _canExecute => true);
        }

        // Parameter des Events auslesen
        private void GetDataMessage(Message<wt2_honorarkraft_vertrag_position> messageObj)
        {
            if (messageObj.Sender == ParentViewModelGuid) 
            { 
                receiveObj = messageObj.Payload;
                // Daten sichern um Reset zu ermöglichen
                ReceiveObjBackup();
            }
        }

        public string Tbx_Avp_KstBez { 
            get 
            {
                if (Depends.Guard)
                { Depends.On(ReceiveObj.hkvp_kostenstelle); }
                if (ReceiveObj != null && ReceiveObj.hkvp_kostenstelle != null && LookupHelper.Team(ReceiveObj.hkvp_kostenstelle)!=null)
                    return LookupHelper.Team(ReceiveObj.hkvp_kostenstelle).Anzeigename;
                return "unbekannt";
            } 
        }

        # region Backupproperties
        public string Tbx_Avp_Kostenstelle 
        { 
            get 
            {
                if (ReceiveObj != null)
                    return ReceiveObj.hkvp_kostenstelle;
                else return null;
            } 
            set 
            {
                if (ReceiveObj != null)
                ReceiveObj.hkvp_kostenstelle = value; 
            } 
        }
        public string Tbx_Avp_Auftrag 
        { 
            get 
            {
                if (ReceiveObj != null)
                return ReceiveObj.hkvp_auftrag;
                else return null;
            } 
            set 
            { 
                ReceiveObj.hkvp_auftrag = value; 
            } 
        }
        public DateTime? Dp_Avp_VonZeitr 
        {
            get 
            {
                if (ReceiveObj != null)
                return ReceiveObj.hkvp_datum_beginn;
                else return null;
            } 
            set 
            { 
                ReceiveObj.hkvp_datum_beginn = value; 
            } 
        }
        public DateTime? Dp_Avp_BisZeitr 
        { 
            get 
            {
                if (ReceiveObj != null)
                return ReceiveObj.hkvp_datum_ende;
                else return null;
            } 
            set 
            { 
                ReceiveObj.hkvp_datum_ende = value; 
            } 
        }
        public string Tbx_Avp_Thema 
        { 
            get
            {
                if (ReceiveObj != null)
                return ReceiveObj.hkvp_thema;
                else return null;
            } 
            set 
            { 
                ReceiveObj.hkvp_thema = value; 
            } 
        }
        public Decimal Tbx_Avp_UE 
        { 
            get
            {
                if (ReceiveObj != null)
                return ReceiveObj.hkvp_unterrichtseinheiten;
                else return 0;
            } 
            set 
            { 
                ReceiveObj.hkvp_unterrichtseinheiten = value; 
            } 
        }
        public Decimal Tbx_Avp_Honorar 
        { 
            get
            {
                if (ReceiveObj != null)
                return ReceiveObj.hkvp_honorar;
                else return 0;
            } 
            set 
            { 
                ReceiveObj.hkvp_honorar = value; 
            } 
        }
        # endregion

        # region Backup & Reset
        private void Reset_AVP_View()
        {
            
            Tbx_Avp_Kostenstelle = avp_backup.hkvp_kostenstelle;
            Tbx_Avp_Auftrag = avp_backup.hkvp_auftrag;
            Dp_Avp_VonZeitr = avp_backup.hkvp_datum_beginn;
            Dp_Avp_BisZeitr = avp_backup.hkvp_datum_ende;
            Tbx_Avp_Thema = avp_backup.hkvp_thema;
            Tbx_Avp_UE = avp_backup.hkvp_unterrichtseinheiten;
            Tbx_Avp_Honorar = avp_backup.hkvp_honorar;
            Chkbx_Avp_Selbstlernphase = avp_backup.avp_Selbstlernphase;
            SelectedItem_avp_zeiteinheit = avp_backup.avp_unterrichtseinheiten_zeiteinheit;
        }

        public void ReceiveObjBackup()
        {
            if (ReceiveObj != null) 
            {
            if (receiveObj.hkvp_info != null)
                if (receiveObj.hkvp_info.ToLower().Contains("selbstlernphase")) avp_backup.avp_Selbstlernphase = true; else avp_backup.avp_Selbstlernphase = false;

            avp_backup.avp_unterrichtseinheiten_zeiteinheit = receiveObj.hkvp_unterrichtseinheiten_zeiteinheit;
            avp_backup.hkvp_kostenstelle = ReceiveObj.hkvp_kostenstelle;
            avp_backup.hkvp_auftrag = ReceiveObj.hkvp_auftrag;
            avp_backup.hkvp_datum_beginn = ReceiveObj.hkvp_datum_beginn;
            avp_backup.hkvp_datum_ende = ReceiveObj.hkvp_datum_ende;
            avp_backup.hkvp_thema = ReceiveObj.hkvp_thema;
            avp_backup.hkvp_unterrichtseinheiten = ReceiveObj.hkvp_unterrichtseinheiten;
            avp_backup.hkvp_honorar = ReceiveObj.hkvp_honorar; 
            }
        }
        # endregion

        public bool CancelAction()
        {
            Reset_AVP_View();
            return true;
        }

        public bool OKAction()
        { 
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

        public override void ApplyNavigationParameters(Microsoft.Practices.Prism.Regions.NavigationParameters navigationParameters)
        {
            if (navigationParameters != null)
            {
                this.ParentViewModelGuid = navigationParameters["guid"] as string;
            }          
        }  
    }
}
