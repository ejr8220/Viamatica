namespace Viamatica.Domain.Entities;

public class Service
{
    public int ServiceId { get; private set; }
    public string ServiceName { get; private set; } = string.Empty;
    public string ServiceDescription { get; private set; } = string.Empty;
    public decimal Price { get; private set; }

    // Navigation
    public ICollection<Device> Devices { get; private set; } = new List<Device>();
    public ICollection<Contract> Contracts { get; private set; } = new List<Contract>();

    private Service() { } // EF Constructor

    public Service(string serviceName, string serviceDescription, decimal price)
    {
        if (string.IsNullOrWhiteSpace(serviceName))
            throw new ArgumentException("Service name is required", nameof(serviceName));

        if (string.IsNullOrWhiteSpace(serviceDescription))
            throw new ArgumentException("Service description is required", nameof(serviceDescription));

        if (price < 0)
            throw new ArgumentException("Price cannot be negative", nameof(price));

        ServiceName = serviceName;
        ServiceDescription = serviceDescription;
        Price = price;
    }

    public void Update(string serviceName, string serviceDescription, decimal price)
    {
        if (string.IsNullOrWhiteSpace(serviceName))
            throw new ArgumentException("Service name is required", nameof(serviceName));

        if (string.IsNullOrWhiteSpace(serviceDescription))
            throw new ArgumentException("Service description is required", nameof(serviceDescription));

        if (price < 0)
            throw new ArgumentException("Price cannot be negative", nameof(price));

        ServiceName = serviceName;
        ServiceDescription = serviceDescription;
        Price = price;
    }
}
