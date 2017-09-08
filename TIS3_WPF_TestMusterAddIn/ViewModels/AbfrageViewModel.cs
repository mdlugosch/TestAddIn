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
        [Import(AllowRecomposition = false)]
        public IRegionManager regionManager;

        [Import(typeof(RegionNames))]
        RegionNames regionNames;

        public RelayCommand OpenPersonalSuche { get; set; }
        public RelayCommand OpenVertragsSuche { get; set; }
        public RelayCommand OpenZahlungsanweisungsSuche { get; set; }
        public RelayCommand OpenBewertungsSuche { get; set; }

        private static Uri StartViewUri = new Uri("/PersonaldatenView", UriKind.Relative);
        private static Uri PersonaldatenViewUri = new Uri("/PersonaldatenView", UriKind.Relative);
        private static Uri VertragsdatenViewUri = new Uri("/VertragsdatenView", UriKind.Relative);
        private static Uri ZahlungsanweisungViewUri = new Uri("/ZahlungsanweisungView", UriKind.Relative);
        private static Uri BewertungsbogenViewUri = new Uri("/BewertungsbogenView", UriKind.Relative);

        public override void Init()
        {
            Header.Title = "Suchmaske";                         // Header Attribut zum bestimmen des Tab-Namens
            Header.Group = "Honorarkräfteverwaltung";           // Gruppenname der Anwendungs-Tabs

            OpenPersonalSuche = new RelayCommand(_execute => { LadePersonaldatenView(); }, _canExecute => { return true; });
            OpenVertragsSuche = new RelayCommand(_execute => { LadeVertragsdatenView(); }, _canExecute => { return true; });
            OpenZahlungsanweisungsSuche = new RelayCommand(_execute => { LadeZahlungsanweisungView(); }, _canExecute => { return true; });
            OpenBewertungsSuche = new RelayCommand(_execute => { LadeBewertungsbogenView(); }, _canExecute => { return true; });    
             }

        public override void OnImportsSatisfied()
        {
           regionManager.RegisterViewWithRegion(regionNames.SearchRegion, () => GetStartView("PersonaldatenView"));
        }

        private object GetStartView(string viewName)
        {
            var startView = (TIS3ViewBase)ServiceLocator.Current.GetInstance<object>(viewName);
            return startView;
        }

        public void LadePersonaldatenView() 
        {

            this.regionManager.RequestNavigate(regionNames.SearchRegion, PersonaldatenViewUri);
        }

        public void LadeVertragsdatenView() 
        {
            this.regionManager.RequestNavigate(regionNames.SearchRegion, VertragsdatenViewUri);
        }

        public void LadeZahlungsanweisungView() 
        {
            this.regionManager.RequestNavigate(regionNames.SearchRegion, ZahlungsanweisungViewUri);
        }

        public void LadeBewertungsbogenView() 
        {
            this.regionManager.RequestNavigate(regionNames.SearchRegion, BewertungsbogenViewUri);
        }

    }
}
