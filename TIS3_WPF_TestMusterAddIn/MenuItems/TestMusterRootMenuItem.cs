using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using TIS3_Base.MenuItems;

namespace TIS3_WPF_TestMusterAddIn.MenuItems
{
    [Export(CompositionPoints.ExtensionPoints.MainMenu.Self, typeof(MenuItem))]
    [ExportMetadata("Parent", "")] // Oberer MenuPunkt hat kein Elternelement
    [ExportMetadata("Position", 100)]
    [DesignTimeVisible(false)]
    class TestMusterRootMenuItem : RootMenuItem
    {
        public TestMusterRootMenuItem()
        {
            this.Header = "Honorarkräfte (Demo)"; // Name der unter dem Icon angezeigt wird
            this.Name = "TestMusterRoot"; // Damit das Element als Elternteil verwendet werden kann wird hier ein Name vergeben.
            this.Icon = new Image { Source = new BitmapImage(new Uri("pack://application:,,,/TIS3_WPF_Styles;component/Icons/test.png")) };
        }
    }
}
