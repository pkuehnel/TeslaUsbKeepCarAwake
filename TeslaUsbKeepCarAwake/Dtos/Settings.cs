﻿namespace TeslaUsbKeepCarAwake.Dtos;

public class Settings
{
    public int CarId { get; set; }
    public int MinimumSoC { get; set; }
    public string TeslaMateApiBaseUrl { get; set; } = null!;
    public string TeslaMateBaseUrl { get; set; } = null!;
    public string MqttUrl { get; set; } = null!;
    public string RelevantGeofence { get; set; } = null!;
    public string ArchiveUploadPath { get; set; } = null!;
    public int JobIntervallSeconds { get; set; }
}