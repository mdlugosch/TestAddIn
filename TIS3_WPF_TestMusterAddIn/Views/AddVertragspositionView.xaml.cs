﻿using System;
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

namespace TIS3_WPF_TestMusterAddIn.Views
{
    /// <summary>
    /// Interaktionslogik für AddVertragspositionView.xaml
    /// </summary>
    [Export("AddVertragspositionView"), PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class AddVertragspositionView : TIS3_Base.TIS3ActiveView
    {
        public AddVertragspositionView()
        {
            InitializeComponent();
        }
    }
}
