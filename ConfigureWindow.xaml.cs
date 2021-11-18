using System;
using System.Windows;
using hardwareToggle.Properties;

namespace hardwareToggle {
    public partial class ConfigureWindow : Window {
        public ConfigureWindow() {
            InitializeComponent();
            IdTypeComboBox.SelectedIndex = Settings.Default.isInstancePath ? 0 : 1;
            IdTextBox.Text = Settings.Default.deviceId;
            SaveButton.IsEnabled = false;
        }

        private void BrowseButton_Clicked(object sender, RoutedEventArgs e) {
            bool showInstancePath = IdTypeComboBox.SelectedIndex == 0;
            BrowseDialog browseDialog = new BrowseDialog(showInstancePath);
            browseDialog.ShowDialog();
            if (browseDialog.SelectedDevice != null) {
                IdTextBox.Text = showInstancePath ? browseDialog.SelectedDevice.instancePath : browseDialog.SelectedDevice.hardwareId;
            }
        }

        private void SaveButton_Clicked(object sender, RoutedEventArgs e) {
            SaveButton.IsEnabled = false;
            Settings.Default.deviceId = IdTextBox.Text.Trim();
            Settings.Default.isInstancePath = IdTypeComboBox.SelectedIndex == 0;
            Settings.Default.Save();
            DeviceStateHandler.Test();
        }

        private void Settings_Changed(object sender, EventArgs e) {
            if (IsInitialized) {
                bool isIdTypeChanged = IdTypeComboBox.SelectedIndex == 0 != Settings.Default.isInstancePath;
                bool isIdChanged = IdTextBox.Text != Settings.Default.deviceId;
                SaveButton.IsEnabled = isIdTypeChanged || isIdChanged;
            }
        }
    }
}
