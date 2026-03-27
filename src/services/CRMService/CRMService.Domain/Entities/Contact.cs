using CRM.Shared.Domain;
using CRMService.Domain.Enums;
using CRMService.Domain.ValueObjects;

namespace CRMService.Domain.Entities;

/// <summary>
/// Contact aggregate — a confirmed person/company in the CRM.
/// </summary>
public sealed class Contact : AggregateRoot<Guid>
{
    public string   FirstName   { get; private set; } = default!;
    public string   LastName    { get; private set; } = default!;
    public string   Email       { get; private set; } = default!;
    public string?  Phone       { get; private set; }
    public string?  Company     { get; private set; }
    public string?  JobTitle    { get; private set; }
    public Address? Address     { get; private set; }
    public string?  LinkedIn    { get; private set; }
    public string?  Website     { get; private set; }
    public string?  Notes       { get; private set; }
    public Guid     OwnerId     { get; private set; }
    public DateTime CreatedAt   { get; private set; }
    public DateTime? LastContactedAt { get; private set; }

    private readonly List<Guid> _dealIds = new();
    public IReadOnlyCollection<Guid> DealIds => _dealIds.AsReadOnly();

    private Contact() { }

    public static Contact Create(
        string firstName, string lastName, string email,
        Guid ownerId,
        string? phone = null, string? company = null, string? jobTitle = null)
    {
        return new Contact
        {
            Id        = Guid.NewGuid(),
            FirstName = firstName,
            LastName  = lastName,
            Email     = email.ToLowerInvariant().Trim(),
            Phone     = phone,
            Company   = company,
            JobTitle  = jobTitle,
            OwnerId   = ownerId,
            CreatedAt = DateTime.UtcNow
        };
    }

    public string FullName => $"{FirstName} {LastName}";

    public void Update(string firstName, string lastName, string email,
        string? phone, string? company, string? jobTitle)
    {
        FirstName = firstName;
        LastName  = lastName;
        Email     = email.ToLowerInvariant().Trim();
        Phone     = phone;
        Company   = company;
        JobTitle  = jobTitle;
        LastContactedAt = DateTime.UtcNow;
    }

    public void UpdateProfile(string firstName, string lastName,
        string? phone, string? company, string? jobTitle,
        string? linkedin, string? website, string? notes)
    {
        Update(firstName, lastName, Email, phone, company, jobTitle);
        LinkedIn  = linkedin;
        Website   = website;
        Notes     = notes;
    }

    public void SetAddress(string street, string city, string state, string country, string postalCode) =>
        Address = new Address(street, city, state, country, postalCode);

    public void LinkDeal(Guid dealId)
    {
        if (!_dealIds.Contains(dealId))
            _dealIds.Add(dealId);
    }

    public void RecordInteraction() => LastContactedAt = DateTime.UtcNow;
}
