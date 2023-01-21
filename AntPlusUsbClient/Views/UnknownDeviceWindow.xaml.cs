﻿using AntPlusUsbClient.ViewModels;
using SmallEarthTech.AntPlus.DeviceProfiles.UnknownDevice;
using System.Windows;
using System.Windows.Data;

namespace AntPlusUsbClient.Views
{
    /// <summary>
    /// Interaction logic for UnknownDeviceWindow.xaml
    /// </summary>
    public partial class UnknownDeviceWindow : Window
    {
        public UnknownDeviceWindow(UnknownDevice unknownDevice)
        {
            BindingOperations.EnableCollectionSynchronization(unknownDevice.DataPages, unknownDevice.DataPages.CollectionLock);
            InitializeComponent();
            DataContext = new UnknownDeviceViewModel(unknownDevice);
        }
    }
}
