﻿using BPA.ViewModels.Pages;
using System;
using System.Collections.Generic;
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
using Wpf.Ui.Abstractions.Controls;
using Wpf.Ui.Controls;

namespace BPA.Views.Pages
{
    /// <summary>
    /// BPAHomePage.xaml 的交互逻辑
    /// </summary>
    public partial class BPAHomePage : INavigableView<BPAHomeViewModel>
    {
        public BPAHomeViewModel ViewModel { get; }
        public BPAHomePage(BPAHomeViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;

            InitializeComponent();
        }
    }
}
