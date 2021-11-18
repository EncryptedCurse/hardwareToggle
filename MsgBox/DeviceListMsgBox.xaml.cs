using System.Windows;
using System.Collections.Generic;
using HardwareManagementLib;

namespace hardwareToggle {
    public partial class DeviceListMsgBox : Window {
        public string Message { get; private set; }
        public List<Device> DeviceList { get; private set; }

        public DeviceListMsgBox(List<Device> deviceList, string message) {
            Message = message;
            DeviceList = deviceList;
            InitializeComponent();
            ShowDialog();
        }
    }
}
