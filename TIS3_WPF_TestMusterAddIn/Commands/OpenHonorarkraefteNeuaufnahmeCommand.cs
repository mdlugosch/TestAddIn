using BFZ_Common_Lib.MVVM;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TIS3_Base.Security;

namespace TIS3_WPF_TestMusterAddIn.Commands
{
    // Name und Typ der Exportiert werden soll
    [Export("OpenHonorarkraefteNeuaufnahmeCommand", typeof(ICommand))]
    class OpenHonorarkraefteNeuaufnahmeCommand : RelayCommand
    {
        public OpenHonorarkraefteNeuaufnahmeCommand() : base()
        {
            // Execute
            this.SetAction(this.OpenHonorarkraefteNeuaufnahmeCommandMethode);
            // CanExecute
            this.SetCanDoAction(this.CanOpenOpenHonorarkraefteNeuaufnahme);
        }

        public void OpenHonorarkraefteNeuaufnahmeCommandMethode(object param)
        {
            // ServiceLocator sucht den passenden RegionManager
            IRegionManager regionManager = ServiceLocator.Current.GetInstance<IRegionManager>();
            // Pfad zur View zusammenbauen relativ zur Position an der man sich befindet
            Uri newView = new Uri("NeuaufnahmeView", UriKind.Relative);

            // Anmelden der View an einer Region z.B. MainWorkspace.
            regionManager.RequestNavigate(CompositionPoints.Regions.MainWorkspace, newView);
        }

        // Methode die für CanExecute genutzt wird und auf Benutzerberechtigungen validiert
        public bool CanOpenOpenHonorarkraefteNeuaufnahme(object param)
        {
            return AuthProvider.Instance.Authorize(ACLKeys.Andere.IS_DEVELOPER);
        }
    }
}
