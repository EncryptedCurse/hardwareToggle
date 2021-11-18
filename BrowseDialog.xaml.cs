using System.Windows;
using System.Collections.Generic;
using HardwareManagementLib;

namespace hardwareToggle {
    public partial class BrowseDialog : Window {
        public bool ShowInstancePath { get; private set; }
        public List<Device> DeviceList => ConfigManager.GetAllDevices();
        public Device SelectedDevice { get; private set; }

        public BrowseDialog(bool showInstancePath) {
            ShowInstancePath = showInstancePath;
            InitializeComponent();
        }

        private void SelectButton_Clicked(object sender, RoutedEventArgs e) {
            SelectedDevice = (Device) AllDevicesListView.SelectedItem;
            Close();
        }

        private void RefreshButton_Clicked(object sender, RoutedEventArgs e) => AllDevicesListView.ItemsSource = DeviceList;
    }
}
