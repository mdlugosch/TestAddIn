using BFZ_Common_Lib.MVVM;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;
using PostSharp.Patterns.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TIS3_Base;
using TIS3_Base.Services;
using TIS3_LookupBL;
using TIS3_WPF_TestMusterAddIn.Infrastructure;
using TIS3_WPF_TestMusterAddIn.Views;
using WinTIS30db_entwModel.Lookup;

namespace TIS3_WPF_TestMusterAddIn.ViewModels
{
    class AbfrageViewModel : TIS3ActiveViewModel
    {
        // Laden des aktuellen RegionManagers
        public IRegionManager regionManager = ServiceLocator.Current.GetInstance<IRegionManager>();

# region Zuweisung: Regionname + GUID
        /*
         * Wenn der Benutzer mehrfach eine Instanz der Shell öffnet,
         * dient die GUID zur Unterscheidung welche Regions zu welcher
         * Shell gehören.
         */

        // Region ID für den Suchbereich einer Shell
        string searchPanel;
        public string SearchPanel
        {
            get
            {
                if (String.IsNullOrWhiteSpace(searchPanel))
                {
                    searchPanel = "SearchMaskRegion-" + Guid.NewGuid().ToString();
                    return searchPanel;
                }
                else return searchPanel;
            }
        }

        // Region ID für den Bereich der die Ergebnistabelle einer Shell darstellt
        string resultPanel;
        public string ResultPanel
        {
            get
            {
                if (String.IsNullOrWhiteSpace(resultPanel))
                {
                    searchPanel = "ResultMaskRegion-" + Guid.NewGuid().ToString();
                    return resultPanel;
                }
                else return resultPanel;
            }
        }
# endregion

# region Definition der Hauptmenu RelayCommands
        public RelayCommand OpenPersonalSuche { get; set; }
        public RelayCommand OpenVertragsSuche { get; set; }
        public RelayCommand OpenZahlungsanweisungsSuche { get; set; }
        public RelayCommand OpenBewertungsSuche { get; set; }
# endregion

# region Uri`s zu den versch. Suchmasken(SuchmaskenViews)
        private static Uri StartViewUri = new Uri("/PersonaldatenView", UriKind.Relative);
        private static Uri PersonaldatenViewUri = new Uri("/PersonaldatenView", UriKind.Relative);
        private static Uri VertragsdatenViewUri = new Uri("/VertragsdatenView", UriKind.Relative);
        private static Uri ZahlungsanweisungViewUri = new Uri("/ZahlungsanweisungView", UriKind.Relative);
        private static Uri BewertungsbogenViewUri = new Uri("/BewertungsbogenView", UriKind.Relative);
# endregion


# region Init() - Ausführung wenn Klasse Instanziiert wird
        public override void Init()
        {
            Header.Title = "Suchmaske";                         // Header Attribut zum bestimmen des Tab-Namens
            Header.Group = "Honorarkräfteverwaltung";           // Gruppenname der Anwendungs-Tabs

            /*
             * Implementation der Hauptmenu RelayCommands
             */
            OpenPersonalSuche = new RelayCommand(_execute => { LadePersonaldatenView(); }, _canExecute => { return true; });
            OpenVertragsSuche = new RelayCommand(_execute => { LadeVertragsdatenView(); }, _canExecute => { return true; });
            OpenZahlungsanweisungsSuche = new RelayCommand(_execute => { LadeZahlungsanweisungView(); }, _canExecute => { return true; });
            OpenBewertungsSuche = new RelayCommand(_execute => { LadeBewertungsbogenView(); }, _canExecute => { return true; });    
             }
# endregion

# region  OnImportsSatisfied() - Ausführung nachdem alle Imports durchgeführt wurden
        /*
         * Ladet als Stadart die Personaldatensuchmaske wenn die App gestartet wird.
         */
        public override void OnImportsSatisfied()
        {
            regionManager.RegisterViewWithRegion(SearchPanel, () => GetStartView("PersonaldatenView"));
        }
# endregion

# region GetStartView - Holt den Pfad zur ersten Suchmaskenview
        private object GetStartView(string viewName)
        {
            var startView = (TIS3ViewBase)ServiceLocator.Current.GetInstance<object>(viewName);
   
            return startView;
        }

# endregion

# region Methoden der Menu Commands
        /*
         * Die MenuCommand Methoden lösen ein RequestNavigate aus
         * und laden damit die vom Benutzer gewählte Suchmaske in die
         * Suchmaskenregion der Shell.
         */
        public void LadePersonaldatenView() 
        {
            this.regionManager.RequestNavigate(SearchPanel, PersonaldatenViewUri);
        }

        public void LadeVertragsdatenView() 
        {
            this.regionManager.RequestNavigate(SearchPanel, VertragsdatenViewUri);
        }

        public void LadeZahlungsanweisungView() 
        {
            this.regionManager.RequestNavigate(SearchPanel, ZahlungsanweisungViewUri);
        }

        public void LadeBewertungsbogenView() 
        {
            this.regionManager.RequestNavigate(SearchPanel, BewertungsbogenViewUri);
        }
# endregion
    }
}
