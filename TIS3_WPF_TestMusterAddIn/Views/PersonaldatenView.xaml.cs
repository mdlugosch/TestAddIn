using Microsoft.Practices.Prism.Regions;
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
using TIS3_WPF_TestMusterAddIn.ViewModels;

namespace TIS3_WPF_TestMusterAddIn.Views
{
    /// <summary>
    /// Interaktionslogik für PersonaldatenView.xaml
    /// </summary>
    [Export("PersonaldatenView"), PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class PersonaldatenView : TIS3_Base.TIS3ActiveView
    {
        public PersonaldatenView()
        {
            InitializeComponent();
            this.DataContext = new PersonaldatenViewModel();
        }
    }
}
