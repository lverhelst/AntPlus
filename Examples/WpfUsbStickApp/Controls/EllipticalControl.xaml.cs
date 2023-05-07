﻿using SmallEarthTech.AntPlus.DeviceProfiles.FitnessEquipment;
using System.Windows.Controls;

namespace WpfUsbStickApp.Controls
{
    /// <summary>
    /// Interaction logic for EllipticalControl.xaml
    /// </summary>
    public partial class EllipticalControl : UserControl
    {
        public EllipticalControl(FitnessEquipment fitnessEquipment)
        {
            InitializeComponent();
            DataContext = fitnessEquipment.Elliptical;
        }
    }
}
