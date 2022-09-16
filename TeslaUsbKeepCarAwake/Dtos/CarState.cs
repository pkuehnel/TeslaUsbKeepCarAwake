namespace TeslaUsbKeepCarAwake.Dtos;

public class CarState
{
    public string? Geofence { get; set; }
    public string? LastGeofence { get; set; }
    public string? State { get; set; }
    public string? LastState { get; set; }
    public DateTime? HomeGeofenceSince { get; set; }
}