﻿using AntPlusUsbClient.ViewModels;
using DeviceProfiles.BicyclePower;
using System.Windows;

namespace AntPlusUsbClient.Views
{
    /// <summary>
    /// Interaction logic for BicyclePowerWindow.xaml
    /// </summary>
    public partial class BicyclePowerWindow : Window
    {
        public BicyclePowerWindow(BicyclePower bicyclePower)
        {
            InitializeComponent();
            BicyclePowerViewModel bicyclePowerViewModel = new BicyclePowerViewModel(bicyclePower);
            CommandBindings.AddRange(bicyclePowerViewModel.CommandBindings);
            DataContext = bicyclePowerViewModel;
        }
    }
}
