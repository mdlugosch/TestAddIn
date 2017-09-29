using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace TIS3_WPF_TestMusterAddIn.MenuItems
{
    // Verweis zu Hauptmenu, Muss von Typ MenuItem sein. Wird als MenuItem exportiert.
    [Export(CompositionPoints.ExtensionPoints.MainMenu.Self, typeof(MenuItem))]
    [ExportMetadata("Parent", "TestMusterRoot")] // Hat ein Elternelement. Namensangabe des Elternelements 
    [ExportMetadata("Position", 995)] // Position des Menupunktes
    [DesignTimeVisible(false)]
    class HonorarkraefteNeuaufnahmeMenuItem : MenuItem, IPartImportsSatisfiedNotification
    {

        [Import("OpenHonorarkraefteNeuaufnahmeCommand")]
        private ICommand MyCommand { get; set; }

        public HonorarkraefteNeuaufnahmeMenuItem()
        {
            this.Header = "Honorarkraft-Neuaufnahme"; // Name unter dem Icon
            this.Icon = new Image { Source = new BitmapImage(new Uri("pack://application:,,,/TIS3_WPF_Styles;component/Icons/test.png")) };
        }

        // Neues Command dem Command Attribut von MenuItem zuweisen.
        public void OnImportsSatisfied()
        {
            this.Command = this.MyCommand;
        }
    }
}
