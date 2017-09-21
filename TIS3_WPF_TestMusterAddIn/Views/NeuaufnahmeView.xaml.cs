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
using TIS3_Base;

namespace TIS3_WPF_TestMusterAddIn.Views
{
    /// <summary>
    /// Interaktionslogik für NeuaufnahmeView.xaml
    /// </summary>
    [Export("EditView"), PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class NeuaufnahmeView : TIS3ActiveView
    {
        public NeuaufnahmeView()
        {
            InitializeComponent();
        }
    }
}
