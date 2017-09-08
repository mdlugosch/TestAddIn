using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TIS3_Base.Commands;

namespace TIS3_WPF_TestMusterAddIn.Commands
{
    // Name und Typ der Exportiert werden soll
   [Export("OpenHonorarkraefteAppCommand", typeof(ICommand))]
    public class OpenHonorarkraefteAppCommand : OpenExternalWinTIS20ProgrammCommand
    {
        public OpenHonorarkraefteAppCommand() : base()
        {
        }

        protected override void Init()
        {
            base.Init();
            this.programmName = "I:\\Bfz Service\\Programme\\WinTIS20\\bin\\wtHonorarkraefte.exe";
            this.arguments = " ";
        }

        protected override bool IstErlaubt(object param)
        {
            // Validation wurde entfernt da hier ansonsten auf ein anderes
            // Projekt zugegriffen werden muss.
            return true;
        }
    }
}
