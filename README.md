# <img src="Resources/AppIcon.svg" height="64" valign="middle" /> hardwareToggle
Simple Windows tray utility to quickly enable or disable a hardware device.
* Requires administrator privileges.
* Uses [HardwareManagementLib](https://github.com/EncryptedCurse/HardwareManagementLib).


## Usage
1. Download the latest release from [Releases](https://github.com/EncryptedCurse/hardwareToggle/releases) as an archive and extract it; ensure that `hardwareToggle.exe`, `HardwareManagementLib.dll`, and `hardwareToggle.exe.config` are (and stay) in the same directory
2. Right click the tray icon and click `Configure...`
3. Choose either `Instance path` or `Hardware ID` from the dropdown menu; an instance path is unique to every individual device, while a hardware ID may be shared by multiple devices (e.g., same manufacturer/model)
4. Determine the ID by using either the built-in browser or Device Manager
5. Click `Save`
6. Double click the tray icon to toggle the device's state
