# TeslaUsbKeepCarAwake
There needs to be a file named `settings.json` in the same directory as the executable.
Example content:

```JSON
{
  "CarId": 1,
  "MinimumSoC": 20,
  "TeslaMateBaseUrl": "http://192.168.1.50:4000/",
  "MqttUrl": "192.168.1.50:1883",
  "RelevantGeofence": "Home",
  "ArchiveUploadPath": "Z:\\Sicherungskopieen\\TeslaVideo\\ARCHIVE_UPLOADED"
}
```

## How it works:
If the car with specified Id (e.g. `1`) enters the Geofence (e.g. `Home`) the time of entering that geofence is compared to creation DateTime of the specified ArchiveUploadPath file (e.g. `Z:\\Sicherungskopieen\\TeslaVideo\\ARCHIVE_UPLOADED` Note: backslashes need to be escaped in JSON that's the reason for the double `\\`). If the entering time of the geofence is later than the creation date time of the specified file, and TeslaMate stopped logging for the car to go to sleep (state: `suspended`) the car is kept awake (trying to sleep is interrupted for TeslaMate). Note: If SoC below `MinimumSoC` the car is not kept awake.
## Known issues:
* After a restart of TeslaUsbKeepCarAwake it is not tried to keep the car awake, the car has to reenter the Geofence after a restart of TeslaUsbKeepCarAwake
* After a restart of TeslaMate under certain curcumstances the car is kept awake until a new `ARCHIVE_UPLOADED` file is created (or TeslaUsbKeepCarAwake is restarted).