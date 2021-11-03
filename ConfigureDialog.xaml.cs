using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using hardwareToggle.Properties;
using HardwareManagementLib;

namespace hardwareToggle {
    public partial class ConfigureDialog : Window {
        public ConfigureDialog() {
            InitializeComponent();
            IdTypeComboBox.SelectedIndex = Settings.Default.useInstancePath ? 0 : 1;
            IdTextBox.Text = Settings.Default.deviceId;
        }

        private void IdTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (SaveButton != null) {
                SaveButton.IsEnabled = IdTypeComboBox.SelectedIndex == 0 != Settings.Default.useInstancePath;
            }
        }

        private void BrowseButton_Clicked(object sender, RoutedEventArgs e) {
            new BrowseDialog(IdTypeComboBox.SelectedIndex == 0).ShowDialog();
            IdTextBox.Text = Settings.Default.deviceId;
        }

        private void IdTextBox_TextChanged(object sender, TextChangedEventArgs e) {
            if (string.IsNullOrEmpty(IdTextBox.Text)) {
                ClearButton.IsEnabled = false;
                ClearIcon.Opacity = 0.5;
            } else {
                ClearButton.IsEnabled = true;
                ClearIcon.Opacity = 1;
            }
            SaveButton.IsEnabled = true;
        }

        private void ClearButton_Clicked(object sender, RoutedEventArgs e) {
            IdTextBox.Text = null;
        }

        private void SaveButton_Clicked(object sender, RoutedEventArgs e) {
            SaveButton.IsEnabled = false;
            Settings.Default.deviceId = IdTextBox.Text;
            Settings.Default.useInstancePath = IdTypeComboBox.SelectedIndex == 0;
            Settings.Default.Save();
            if (!string.IsNullOrWhiteSpace(Settings.Default.deviceId)) {
                if (Settings.Default.useInstancePath) {
                    Device device = ConfigManager.GetDeviceByInstancePath(IdTextBox.Text);
                    if (device != null) {
                        new DeviceListMessageBox(new List<Device> { device }, "1 device will be affected");
                    } else {
                        new ErrorMessageBox($"No device found with instance path '{IdTextBox.Text}'");
                    }
                } else {
                    List<Device> deviceList = ConfigManager.GetDevicesByHardwareId(IdTextBox.Text);
                    if (deviceList != null) {
                        new DeviceListMessageBox(deviceList, $"{deviceList.Count} {(deviceList.Count == 1 ? "device" : "devices")} will be affected");
                    } else {
                        new ErrorMessageBox($"No device found with hardware ID '{IdTextBox.Text}'");
                    }
                }
            }
        }
    }
}
