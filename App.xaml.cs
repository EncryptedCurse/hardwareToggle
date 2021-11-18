using System;
using System.Windows;
using System.Reflection;
using System.ComponentModel;
using System.Security.Principal;
using MenuItem = System.Windows.Forms.MenuItem;
using ContextMenu = System.Windows.Forms.ContextMenu;
using NotifyIcon = System.Windows.Forms.NotifyIcon;
using hardwareToggle.Properties;

namespace hardwareToggle {
    public partial class App : Application {
        public static readonly string programName = ((AssemblyTitleAttribute) Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(AssemblyTitleAttribute), false)).Title;
        public readonly bool isElevated;
        private NotifyIcon trayIcon;
        private ConfigureWindow configureWindow;

        public App() {
            using (WindowsIdentity identity = WindowsIdentity.GetCurrent()) {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                isElevated = principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
        }

        private void App_Startup(object sender, StartupEventArgs e) {
            if (!isElevated) {
                new ErrorMsgBox($"{programName} must be run as administrator to work!");
                Current.Shutdown();
            } else {
                Settings.Default.PropertyChanged += Settings_Changed;

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
                trayIcon.DoubleClick += TrayIcon_DoubleClicked;
                UpdateTrayIcon();

                DeviceStateHandler.Set();
            }
        }

        private void TrayIcon_DoubleClicked(object sender, EventArgs e) {
            Settings.Default.deviceEnabled = !Settings.Default.deviceEnabled;
            Settings.Default.Save();
            DeviceStateHandler.Set();
        }

        private void MenuConfigure_Clicked(object sender, EventArgs e) {
            if (configureWindow == null) {
                configureWindow = new ConfigureWindow();
                configureWindow.ShowDialog();
                configureWindow = null;
            } else {
                configureWindow.Focus();
            }
        }

        private void MenuExit_Clicked(object sender, EventArgs e) => Current.Shutdown();

        private void Settings_Changed(object sender, PropertyChangedEventArgs e) {
            switch (e.PropertyName) {
                case "deviceId": DeviceStateHandler.Set(false); break;
                case "deviceEnabled": UpdateTrayIcon(); break;
            }
        }

        private void UpdateTrayIcon() {
            trayIcon.Text = $"{programName}: {(Settings.Default.deviceEnabled ? "enabled" : "disabled")}";
            trayIcon.Icon =
                Settings.Default.deviceEnabled
                    ? hardwareToggle.Properties.Resources.EnabledTrayIcon
                    : hardwareToggle.Properties.Resources.DisabledTrayIcon;
        }
    }
}
