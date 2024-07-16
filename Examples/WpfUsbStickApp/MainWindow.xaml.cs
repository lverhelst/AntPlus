﻿using SmallEarthTech.AntPlus;
using SmallEarthTech.AntPlus.DeviceProfiles;
using SmallEarthTech.AntPlus.DeviceProfiles.AssetTracker;
using SmallEarthTech.AntPlus.DeviceProfiles.BicyclePower;
using SmallEarthTech.AntPlus.DeviceProfiles.BikeSpeedAndCadence;
using SmallEarthTech.AntPlus.DeviceProfiles.FitnessEquipment;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using WpfUsbStickApp.ViewModels;
using WpfUsbStickApp.Views;

namespace WpfUsbStickApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainWindowViewModel? viewModel;

        public MainWindow()
        {
            InitializeComponent();

            // the view model may throw an exception if ANT radio is not available
            try
            {
                viewModel = new MainWindowViewModel();
                BindingOperations.EnableCollectionSynchronization(viewModel.AntDevices, viewModel.AntDevices.CollectionLock);
                DataContext = viewModel;
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.Message, "Unable to initialize!", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
            }
        }

        private void AntDevices_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            AntDevice? antDevice = ((ListView)sender).SelectedItem as AntDevice;
            switch (antDevice?.ChannelId.DeviceType)
            {
                case HeartRate.DeviceClass:
                    new HeartRateWindow((HeartRate)antDevice).Show();
                    break;
                case BicyclePower.DeviceClass:
                    if (antDevice is CrankTorqueFrequencySensor sensor)
                    {
                        new CTFWindow(sensor).Show();
                    }
                    else
                    {
                        new StandardPowerWindow((StandardPowerSensor)antDevice).Show();
                    }
                    break;
                case BikeSpeedSensor.DeviceClass:
                    new BikeSpeedWindow((BikeSpeedSensor)antDevice).Show();
                    break;
                case BikeCadenceSensor.DeviceClass:
                    new BikeCadenceWindow((BikeCadenceSensor)antDevice).Show();
                    break;
                case CombinedSpeedAndCadenceSensor.DeviceClass:
                    new BikeSpeedAndCadenceWindow((CombinedSpeedAndCadenceSensor)antDevice).Show();
                    break;
                case FitnessEquipment.DeviceClass:
                    new FitnessEquipmentWindow((FitnessEquipment)antDevice).Show();
                    break;
                case MuscleOxygen.DeviceClass:
                    new MuscleOxygenWindow((MuscleOxygen)antDevice).Show();
                    break;
                case Geocache.DeviceClass:
                    new GeocacheWindow((Geocache)antDevice).Show();
                    break;
                case Tracker.DeviceClass:
                    new AssetTrackerWindow((Tracker)antDevice).Show();
                    break;
                case StrideBasedSpeedAndDistance.DeviceClass:
                    new StrideBasedMonitorWindow((StrideBasedSpeedAndDistance)antDevice).Show();
                    break;
                default:
                    // unknown device
                    if (antDevice is AntDevice ad)
                    {
                        UnknownDeviceWindow unknownDeviceWindow = new((UnknownDevice)ad);
                        unknownDeviceWindow.Show();
                    }
                    break;
            }
        }

        private async void Capabilities_Click(object sender, RoutedEventArgs e)
        {
            if (viewModel != null)
            {
                CapabilitiesWindow capabilitiesWindow = new(await viewModel.UsbAntRadio.GetDeviceCapabilities())
                {
                    Icon = Icon
                };
                capabilitiesWindow.Show();
            }
        }
    }
}
