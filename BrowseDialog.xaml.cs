using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using HardwareManagementLib;
using hardwareToggle.Properties;

namespace hardwareToggle {
    public partial class BrowseDialog : Window {
        private readonly bool useInstancePath;
        private List<Device> deviceList;

        private class DeviceListViewItem {
            public string Description { get; set; }
            public string FriendlyName { get; set; }
            public string Id { get; set; }
            public DeviceStatus Status { get; set; }
        }

        public BrowseDialog(bool useInstancePath) {
            this.useInstancePath = useInstancePath;
            InitializeComponent();
            ((GridView) AllDevicesListView.View).Columns[2].Header = useInstancePath ? "Instance path" : "Hardware ID";
            RefreshDeviceList();
        }

        private void SelectButton_Clicked(object sender, RoutedEventArgs e) {
            DeviceListViewItem selectedItem = (DeviceListViewItem) AllDevicesListView.SelectedItem;
            if (selectedItem != null) {
                Settings.Default.deviceId = selectedItem.Id;
                Close();
            }
        }

        private void RefreshButton_Clicked(object sender, RoutedEventArgs e) {
            RefreshDeviceList();
        }

        private void RefreshDeviceList() {
            AllDevicesListView.Items.Clear();
            deviceList = ConfigManager.GetAllDevices();
            if (deviceList != null) {
                foreach (Device device in deviceList) {
                    string deviceId = useInstancePath ? device.instancePath : device.hardwareId;
                    if (!string.IsNullOrWhiteSpace(deviceId)) {
                        AllDevicesListView.Items.Add(new DeviceListViewItem {
                            Description = device.description,
                            FriendlyName = device.friendlyName,
                            Id = deviceId,
                            Status = device.status
                        });
                    }
                }
            }
        }
    }
}
