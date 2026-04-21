namespace Viamatica.Domain.Entities;

public class Device
{
    public int DeviceId { get; private set; }
    public string DeviceName { get; private set; } = string.Empty;
    public int ServiceId { get; private set; }

    // Navigation
    public Service Service { get; private set; } = null!;

    private Device() { } // EF Constructor

    public Device(string deviceName, int serviceId)
    {
        if (string.IsNullOrWhiteSpace(deviceName))
            throw new ArgumentException("Device name is required", nameof(deviceName));

        DeviceName = deviceName;
        ServiceId = serviceId;
    }
}
