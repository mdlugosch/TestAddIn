using BFZ_Common_Lib.MVVM;
using Syncfusion.UI.Xaml.Grid;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
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
    /// Interaktionslogik für VertragsdatenView.xaml
    /// </summary>
    [Export("VertragsdatenView")] 
    public partial class VertragsdatenView : TIS3_Base.TIS3ActiveView
    {
        public RelayCommand OpenEditViewCommand { get; set; }
        public VertragsdatenView()
        {
            InitializeComponent();
            this.OpenEditViewCommand = new RelayCommand(_execute => this.OpenEditViewCommandMethode(), _canExecute => true);
        }

        private void Row_Loaded(object sender, RoutedEventArgs e)
        {
            var row = sender as VirtualizingCellsControl;
            if (row != null)
            {
                row.InputBindings.Add(new MouseBinding(OpenEditViewCommand, new MouseGesture() { MouseAction = MouseAction.LeftDoubleClick }));
                row.InputBindings.Add(new KeyBinding(OpenEditViewCommand, new KeyGesture(Key.Return)));
            }
        }

        private void OpenEditViewCommandMethode()
        {
            Type ViewModelType = this.DataContext.GetType();
            PropertyInfo CommandPropertyInfo = ViewModelType.GetProperty("OpenEditViewCommand");
            ICommand command = (ICommand)CommandPropertyInfo.GetValue(DataContext);
            if (command != null)
            {
                command.Execute(this.dg_Vertragsdaten.SelectedItem);
            }
        }
    }
}
