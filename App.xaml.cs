using System;
using System.Windows;
using System.Reflection;
using System.ComponentModel;
using System.Collections.Generic;
using System.Security.Principal;
using MenuItem = System.Windows.Forms.MenuItem;
using ContextMenu = System.Windows.Forms.ContextMenu;
using NotifyIcon = System.Windows.Forms.NotifyIcon;
using hardwareToggle.Properties;
using HardwareManagementLib;

namespace hardwareToggle {
    public partial class App : Application {
        public static readonly string programName = ((AssemblyTitleAttribute) Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(AssemblyTitleAttribute), false)).Title;
        private NotifyIcon trayIcon;
        private readonly bool isElevated;
        private ConfigureDialog configureDialog;

        public App() {
            using (WindowsIdentity identity = WindowsIdentity.GetCurrent()) {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                isElevated = principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            Settings.Default.PropertyChanged += Settings_Changed;
        }

        private void Settings_Changed(object sender, PropertyChangedEventArgs e) {
            if (e.PropertyName == "deviceId") {
                SetDeviceState(Settings.Default.deviceEnabled, false);
            }
        }

        private void Application_Startup(object sender, StartupEventArgs e) {
            if (!isElevated) {
                new ErrorMessageBox($"{programName} must be run as administrator to work!");
                Current.Shutdown();
            }

            ContextMenu contextMenu = new ContextMenu(new MenuItem[] {
                new MenuItem("Configure...", MenuConfigure_Clicked),
                new MenuItem("-"),
                new MenuItem("Exit", MenuExit_Clicked)
            });
            trayIcon = new NotifyIcon() {
                Icon = hardwareToggle.Properties.Resources.AppIcon,
                ContextMenu = contextMenu,
                Text = programName,
                Visible = true
            };
            trayIcon.DoubleClick += TrayIcon_Clicked;

            SetDeviceState(Settings.Default.deviceEnabled);
        }

        private void TrayIcon_Clicked(object sender, EventArgs e) {
            SetDeviceState(!Settings.Default.deviceEnabled);
        }

        private void MenuConfigure_Clicked(object sender, EventArgs e) {
            if (configureDialog == null) {
                configureDialog = new ConfigureDialog();
                configureDialog.ShowDialog();
                configureDialog = null;
            } else {
                configureDialog.Focus();
            }
        }

        private void MenuExit_Clicked(object sender, EventArgs e) {
            Current.Shutdown();
        }

        public void SetDeviceState(bool enable, bool showErrors = true) {
            Settings.Default.deviceEnabled = enable;
            Settings.Default.Save();

            trayIcon.Icon = enable ? hardwareToggle.Properties.Resources.EnabledTrayIcon : hardwareToggle.Properties.Resources.DisabledTrayIcon;
            trayIcon.Text = $"{programName}: {(enable ? "enabled" : "disabled")}";

            if (!string.IsNullOrWhiteSpace(Settings.Default.deviceId)) {
                Native.ReturnCode returnCode;
                if (Settings.Default.useInstancePath) {
                    Device device = ConfigManager.GetDeviceByInstancePath(Settings.Default.deviceId);
                    if (device != null) {
                        returnCode = enable ? device.Enable() : device.Disable();
                        if (returnCode != Native.ReturnCode.CR_SUCCESS) {
                            new DeviceListMessageBox(new List<Device> { device }, $"Failed to {(enable ? "enable" : "disable")} 1 device");
                        }
                    } else if (showErrors) {
                        new ErrorMessageBox($"No device found with instance path '{Settings.Default.deviceId}'");
                    }
                } else {
                    List<Device> deviceList = ConfigManager.GetDevicesByHardwareId(Settings.Default.deviceId);
                    List<Device> failedDeviceList = new List<Device>();
                    if (deviceList != null) {
                        foreach (Device device in deviceList) {
                            returnCode = enable ? device.Enable() : device.Disable();
                            if (returnCode != Native.ReturnCode.CR_SUCCESS) {
                                failedDeviceList.Add(device);
                            }
                        }
                        if (showErrors && failedDeviceList.Count != 0) {
                            new DeviceListMessageBox(failedDeviceList, $"Failed to {(enable ? "enable" : "disable")} {failedDeviceList.Count} {(failedDeviceList.Count == 1 ? "device" : "devices")}");
                        }
                    } else if (showErrors) {
                        new ErrorMessageBox($"No device found with hardware ID '{Settings.Default.deviceId}'");
                    }
                }
            } else if (showErrors) {
                new ErrorMessageBox("No device ID configured");
            }
        }
    }
}
