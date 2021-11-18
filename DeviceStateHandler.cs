using System.Collections.Generic;
using hardwareToggle.Properties;
using HardwareManagementLib;

namespace hardwareToggle {
    internal static class DeviceStateHandler {
        public static void Test() => HandleDeviceState(null, true);

        public static void Set(bool showErrors = true) => HandleDeviceState(Settings.Default.deviceEnabled, showErrors);

        private static void HandleDeviceState(bool? enable, bool showErrors) {
            if (!string.IsNullOrWhiteSpace(Settings.Default.deviceId)) {
                List<Device> deviceList = null;
                if (Settings.Default.isInstancePath) {
                    Device device = ConfigManager.GetDeviceByInstancePath(Settings.Default.deviceId);
                    if (device != null) deviceList = new List<Device> { device };
                } else {
                    deviceList = ConfigManager.GetDevicesByHardwareId(Settings.Default.deviceId);
                }
                if (deviceList != null) {
                    string devicesString = $"{deviceList.Count} device{(deviceList.Count != 1 ? "s" : "")}";
                    if (enable == null) {
                        new DeviceListMsgBox(deviceList, $"{devicesString} will be affected");
                    } else {
                        Native.ReturnCode returnCode;
                        deviceList.RemoveAll(device => {
                            returnCode = (bool) enable ? device.Enable() : device.Disable();
                            return returnCode == Native.ReturnCode.CR_SUCCESS;
                        });
                        if (showErrors && deviceList.Count != 0) {
                            new DeviceListMsgBox(deviceList, $"Failed to {((bool) enable ? "enable" : "disable")} {devicesString}");
                        }
                    }
                } else if (showErrors) {
                    new ErrorMsgBox($"No device found with {(Settings.Default.isInstancePath ? "instance path" : "hardware ID")}", Settings.Default.deviceId);
                }
            } else if (showErrors && enable != null) {
                new ErrorMsgBox("No device ID configured");
            }
        }
    }
}
