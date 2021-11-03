using System.Windows;
using System.Collections.Generic;
using HardwareManagementLib;

namespace hardwareToggle {
    public partial class DeviceListMessageBox : Window {
        private class FoundDevicesListViewItem {
            public string Description { get; set; }
            public string FriendlyName { get; set; }
            public string InstancePath { get; set; }
        }

        public DeviceListMessageBox(List<Device> deviceList, string message) {
            InitializeComponent();
            DeviceCountTextBlock.Text = message;
            foreach (Device device in deviceList) {
                FoundDevicesListView.Items.Add(new FoundDevicesListViewItem {
                    Description = device.description,
                    FriendlyName = device.friendlyName,
                    InstancePath = device.instancePath
                });
            }
            ShowDialog();
        }

        private void OKButton_Clicked(object sender, RoutedEventArgs e) {
            Close();
        }
    }
}
