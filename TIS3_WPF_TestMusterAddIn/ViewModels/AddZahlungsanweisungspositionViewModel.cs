using BFZ_Common_Lib.MVVM;
using Microsoft.Practices.Prism.PubSubEvents;
using PostSharp.Patterns.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    class AddZahlungsanweisungspositionViewModel : TIS3ActiveViewModel, ITIS3DialogViewModel
    {
        # region Undo-Backup-Struktur
        private struct BackupData
        {
            public decimal? hkzp_unterrichtseinheiten;
            public int? hkzp_hkv_ident;
            public int hkzp_hkvp_lfdnr;
        }

        BackupData azp_backup;
        # endregion
        
        public RelayCommand ResetCommand { get; set; }

        // Zugriff auf EventaggregatorListe
        readonly IEventAggregator _aggregator = HonorarAddInAggregatorList.AggregatorFactory;

        wt2_honorarkraft_zahlungsanweisung_position receiveObj = new wt2_honorarkraft_zahlungsanweisung_position();
        public wt2_honorarkraft_zahlungsanweisung_position ReceiveObj { get { return receiveObj; } set { receiveObj = value; } }

        # region Data Access Objecte für Honorarkraefte Tabellen
        HonorarkraefteDAO hDAO = HonorarkraefteDAO.DAOFactory();
        # endregion

        public ObservableCollection<wt2_honorarkraft_vertrag_position> Dg_Vertraege 
        { 
            get; protected set;
        }

        wt2_honorarkraft_vertrag_position selectedItem_Dg_Vertraege=null;
        public wt2_honorarkraft_vertrag_position SelectedItem_Dg_Vertraege 
        {
            get { return selectedItem_Dg_Vertraege; }
            set { selectedItem_Dg_Vertraege = value;
                if(value != null)
                {
                    this.receiveObj.hkzp_hkv_ident = selectedItem_Dg_Vertraege.hkvp_hkv_ident;
                    this.receiveObj.hkzp_hkvp_lfdnr = selectedItem_Dg_Vertraege.hkvp_lfdnr;
                    RemoveZahlungsposition();
                }
            } 
        }

        public string Lbl_Azp_Meldung { get; protected set; }

        public string Tbx_Azp_Aussteller
        {
            get
            {
                if (Depends.Guard)
                {
                    Depends.On
                      (
                        receiveObj.wt2_honorarkraft_zahlungsanweisung.wt2_honorarkraft.hk_bildungstraeger
                      );
                }
                string aussteller = "";
                if (receiveObj != null &&
                    receiveObj.wt2_honorarkraft_zahlungsanweisung != null &&
                    receiveObj.wt2_honorarkraft_zahlungsanweisung.hkz_abteilung != null)
                {
                    int? traegerID = receiveObj.wt2_honorarkraft_zahlungsanweisung.hkz_bildungstraeger;
                    int? abteilungID = receiveObj.wt2_honorarkraft_zahlungsanweisung.hkz_abteilung;

                    if (traegerID != null)
                    {
                        int btID = (int)traegerID;
                        string name1 = LookupHelper.Bildungstraeger(btID).Name1;
                        string name2 = LookupHelper.Bildungstraeger(btID).Name2;
                        string abteilung = LookupHelper.Abteilung(abteilungID.Value).Displayname;
                        string strasse = LookupHelper.Bildungstraeger(btID).Strasse;
                        string ort = LookupHelper.Bildungstraeger(btID).Ort;
                        string plz = LookupHelper.Bildungstraeger(btID).PLZ;

                        // String zusammenbauen
                        aussteller += name1.Trim() + " "
                                    + name2.Trim() + "\n"
                                    + abteilung.Trim() + "\n"
                                    + strasse.Trim() + "\n"
                                    + plz.Trim() + " " + ort.Trim();
                    }

                }
                return aussteller;
            }
        }

        public string Tbx_Azp_Auftragnehmer
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
                          receiveObj.wt2_honorarkraft_zahlungsanweisung.wt2_honorarkraft.hk_nachname,
                          receiveObj.wt2_honorarkraft_zahlungsanweisung.wt2_honorarkraft.hk_plz
                          );
                    }
                    if (receiveObj.wt2_honorarkraft_zahlungsanweisung != null && receiveObj.wt2_honorarkraft_zahlungsanweisung.wt2_honorarkraft != null)
                    {
                        if (receiveObj.wt2_honorarkraft_zahlungsanweisung.wt2_honorarkraft.hk_vorname != null && receiveObj.wt2_honorarkraft_zahlungsanweisung.wt2_honorarkraft.hk_nachname != null) auftragnehmer += receiveObj.wt2_honorarkraft_zahlungsanweisung.wt2_honorarkraft.hk_vorname.Trim() + " " + receiveObj.wt2_honorarkraft_zahlungsanweisung.wt2_honorarkraft.hk_nachname;
                        if (receiveObj.wt2_honorarkraft_zahlungsanweisung.wt2_honorarkraft.hk_firma != null) auftragnehmer +=  "\n" + receiveObj.wt2_honorarkraft_zahlungsanweisung.wt2_honorarkraft.hk_firma + "\n";
                        if (receiveObj.wt2_honorarkraft_zahlungsanweisung.wt2_honorarkraft.hk_strasse != null) auftragnehmer += receiveObj.wt2_honorarkraft_zahlungsanweisung.wt2_honorarkraft.hk_strasse + "\n";
                        if (receiveObj.wt2_honorarkraft_zahlungsanweisung.wt2_honorarkraft.hk_plz != null && receiveObj.wt2_honorarkraft_zahlungsanweisung.wt2_honorarkraft.hk_ort != null) auftragnehmer += receiveObj.wt2_honorarkraft_zahlungsanweisung.wt2_honorarkraft.hk_plz.Trim() + " " + receiveObj.wt2_honorarkraft_zahlungsanweisung.wt2_honorarkraft.hk_ort;
                    }
                }
                return auftragnehmer;
            }
        }

        public decimal? Tbx_Azp_UE 
        { 
            get 
            {
                if (receiveObj != null)
                    return receiveObj.hkzp_unterrichtseinheiten;
                else return null;
            } 
            set 
            { 
                receiveObj.hkzp_unterrichtseinheiten = value;  
            } 
        }

        public decimal? Tbx_Azp_Auszahlung
        {
            get;
            set;
        }

        public override void Init()
        {
            // Meldungsfeld ohne Inhalt initialisieren
            Lbl_Azp_Meldung = " ";
            // Event subscribtion
            _aggregator.GetEvent<ZahlungsanweisungsPositionPostEvent>().Subscribe(GetDataMessage);

            ResetCommand = new RelayCommand(_execute => this.Reset_AZP_View(), _canExecute => true);
        }

        public void Load_Dg_Vertraege()
        {
            ObservableCollection<wt2_honorarkraft_vertrag_position> result;
            if (receiveObj != null && receiveObj.wt2_honorarkraft_zahlungsanweisung != null)
            {
                result = hDAO.Hole_AZP_Vertragspositionen(receiveObj.wt2_honorarkraft_zahlungsanweisung.hkz_datum.Value, receiveObj.wt2_honorarkraft_zahlungsanweisung.hkz_hk_ident.Value);

                // Aktuell zugeordnete Vertragsposition holen.
                var query = (from row in result
                             where receiveObj.hkzp_hkv_ident == row.hkvp_hkv_ident && receiveObj.hkzp_hkvp_lfdnr == row.hkvp_lfdnr
                             select row).FirstOrDefault();

                SelectedItem_Dg_Vertraege = query;
                
                Dg_Vertraege = result;
            }
        }

        public void RemoveZahlungsposition()
        {
            // Zur aktuellen Vertragsposition die Zahlungsanweisungsposition suchen die gelöscht werden soll
            if (this.SelectedItem_Dg_Vertraege != null && this.SelectedItem_Dg_Vertraege.wt2_honorarkraft_zahlungsanweisung_position != null)
            {
                var queryZa = (from row in this.SelectedItem_Dg_Vertraege.wt2_honorarkraft_zahlungsanweisung_position
                               where row.hkzp_hkz_ident == receiveObj.hkzp_hkz_ident && row.hkzp_lfdnr == receiveObj.hkzp_lfdnr
                               select row).FirstOrDefault();

                // Zahlungsanweisungsposition löschen.
                if (queryZa != null)
                    this.SelectedItem_Dg_Vertraege.wt2_honorarkraft_zahlungsanweisung_position.Remove(queryZa);
            }
        }

        # region Backup & Reset
        public void ReceiveObjBackup()
        {
            if (ReceiveObj != null)
            {
                azp_backup.hkzp_hkv_ident = this.receiveObj.hkzp_hkv_ident;
                azp_backup.hkzp_hkvp_lfdnr = this.receiveObj.hkzp_hkvp_lfdnr;
                azp_backup.hkzp_unterrichtseinheiten = this.receiveObj.hkzp_unterrichtseinheiten;
            }
        }

        private void Reset_AZP_View()
        {
            // Fehlermeldung im Labelfeld löschen
            Lbl_Azp_Meldung = " ";
            // alte Daten wieder zurückschreiben
            this.receiveObj.hkzp_hkv_ident = azp_backup.hkzp_hkv_ident;
            this.receiveObj.hkzp_hkvp_lfdnr = azp_backup.hkzp_hkvp_lfdnr;
            this.receiveObj.hkzp_unterrichtseinheiten = azp_backup.hkzp_unterrichtseinheiten;
        }
        # endregion

        // Parameter des Events auslesen
        private void GetDataMessage(Message<wt2_honorarkraft_zahlungsanweisung_position> messageObj)
        {
            if (messageObj.Sender == ParentViewModelGuid)
            {
                receiveObj = messageObj.Payload;

                ReceiveObjBackup();

                Load_Dg_Vertraege();
            }
        }

        public override void ApplyNavigationParameters(Microsoft.Practices.Prism.Regions.NavigationParameters navigationParameters)
        {
            if (navigationParameters != null)
            {
                this.ParentViewModelGuid = navigationParameters["guid"] as string;
            }
        }  

        public bool CancelAction()
        {
            Reset_AZP_View();

            return true;
        }

        public bool OKAction()
        {
            if (SelectedItem_Dg_Vertraege!=null)
            if (ReceiveObj.hkzp_unterrichtseinheiten <= SelectedItem_Dg_Vertraege.hkvp_rest_UE && ReceiveObj.hkzp_unterrichtseinheiten >= 0) 
            {
                Lbl_Azp_Meldung = " ";
                return true;
            }
            else
            {
                Lbl_Azp_Meldung = "ACHTUNG: Unzulässiger Wert bei den Unterrichtseinheiten";
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
    }
}
