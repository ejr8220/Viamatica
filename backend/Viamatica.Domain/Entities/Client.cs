using System.Text.RegularExpressions;
using Viamatica.Domain.Common;

namespace Viamatica.Domain.Entities;

public class Client : SoftDeletableEntity
{
    public int ClientId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string Identification { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string PhoneNumber { get; private set; } = string.Empty;
    public string Address { get; private set; } = string.Empty;
    public string ReferenceAddress { get; private set; } = string.Empty;

    // Navigation
    public ICollection<Payment> Payments { get; private set; } = new List<Payment>();
    public ICollection<Contract> Contracts { get; private set; } = new List<Contract>();
    public ICollection<Attention> Attentions { get; private set; } = new List<Attention>();

    private Client() { } // EF Constructor

    public Client(string name, string lastName, string identification, string email, 
                  string phoneNumber, string address, string referenceAddress)
    {
        ValidateName(name);
        ValidateLastName(lastName);
        ValidateIdentification(identification);
        ValidateEmail(email);
        ValidatePhoneNumber(phoneNumber);
        ValidateAddress(address);
        ValidateReferenceAddress(referenceAddress);

        Name = name;
        LastName = lastName;
        Identification = identification;
        Email = email;
        PhoneNumber = phoneNumber;
        Address = address;
        ReferenceAddress = referenceAddress;
    }

    public void Update(string name, string lastName, string identification, string email,
        string phoneNumber, string address, string referenceAddress)
    {
        ValidateName(name);
        ValidateLastName(lastName);
        ValidateIdentification(identification);
        ValidateEmail(email);
        ValidatePhoneNumber(phoneNumber);
        ValidateAddress(address);
        ValidateReferenceAddress(referenceAddress);

        Name = name;
        LastName = lastName;
        Identification = identification;
        Email = email;
        PhoneNumber = phoneNumber;
        Address = address;
        ReferenceAddress = referenceAddress;
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required", nameof(name));
    }

    private static void ValidateLastName(string lastName)
    {
        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name is required", nameof(lastName));
    }

    private static void ValidateIdentification(string identification)
    {
        if (string.IsNullOrWhiteSpace(identification))
            throw new ArgumentException("Identification is required", nameof(identification));

        if (identification.Length < 10 || identification.Length > 13)
            throw new ArgumentException("Identification must be between 10 and 13 characters", nameof(identification));

        if (!Regex.IsMatch(identification, @"^\d+$"))
            throw new ArgumentException("Identification must contain only numbers", nameof(identification));
    }

    private static void ValidateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required", nameof(email));

        if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            throw new ArgumentException("Invalid email format", nameof(email));
    }

    private static void ValidatePhoneNumber(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            throw new ArgumentException("Phone number is required", nameof(phoneNumber));

        if (phoneNumber.Length < 10)
            throw new ArgumentException("Phone number must be at least 10 characters", nameof(phoneNumber));

        if (!Regex.IsMatch(phoneNumber, @"^\d+$"))
            throw new ArgumentException("Phone number must contain only numbers", nameof(phoneNumber));

        if (!phoneNumber.StartsWith("09"))
            throw new ArgumentException("Phone number must start with 09", nameof(phoneNumber));
    }

    private static void ValidateAddress(string address)
    {
        if (string.IsNullOrWhiteSpace(address))
            throw new ArgumentException("Address is required", nameof(address));

        if (address.Length < 20 || address.Length > 100)
            throw new ArgumentException("Address must be between 20 and 100 characters", nameof(address));
    }

    private static void ValidateReferenceAddress(string referenceAddress)
    {
        if (string.IsNullOrWhiteSpace(referenceAddress))
            throw new ArgumentException("Reference address is required", nameof(referenceAddress));

        if (referenceAddress.Length < 20 || referenceAddress.Length > 100)
            throw new ArgumentException("Reference address must be between 20 and 100 characters", nameof(referenceAddress));
    }
}
