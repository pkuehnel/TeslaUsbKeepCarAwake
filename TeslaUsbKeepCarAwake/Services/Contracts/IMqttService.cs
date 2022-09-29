namespace TeslaUsbKeepCarAwake.Services.Contracts;

public interface IMqttService
{
    bool IsConnected { get; }
    Task ConnectMqttClient();
    Task ConnectClientIfNotConnected();
}