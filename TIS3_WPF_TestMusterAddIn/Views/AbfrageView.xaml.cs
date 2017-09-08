using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Practices.Prism.Regions;
using TIS3_WPF_TestMusterAddIn.Infrastructure;

namespace TIS3_WPF_TestMusterAddIn.Views
{
    /// <summary>
    /// Interaktionslogik für AbfrageView.xaml
    /// </summary>
    [Export("AbfrageView"), PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class AbfrageView : TIS3_Base.TIS3ActiveView
    {
        private static Int64 idNr = 0;

       [Export]
       RegionNames regionNames = new RegionNames();

        public AbfrageView()
        {
            regionNames.SearchRegion="SearchMaskRegion" + ++idNr;
            InitializeComponent();
            if (String.IsNullOrWhiteSpace(regionNames.SearchRegion)) MessageBox.Show("Boom");
            RegionManager.SetRegionName(SearchMask, regionNames.SearchRegion);
        }

    }
}
