﻿using SmallEarthTech.AntPlus.DeviceProfiles.BicyclePower;
using System.Windows.Controls;

namespace WpfUsbStickApp.Controls
{
    /// <summary>
    /// Interaction logic for BicycleWheelTorqueControl.xaml
    /// </summary>
    public partial class BicycleWheelTorqueControl : UserControl
    {
        public BicycleWheelTorqueControl(StandardWheelTorqueSensor wts)
        {
            InitializeComponent();
            DataContext = wts;
        }
    }
}
