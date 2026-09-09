using System;
using WebFlow.Abstractions.Interfaces;
using WebFlow.Models;

namespace WebFlow.Events;

public class CorporateAccountEvent : IEvent
{
    public Guid Id { get; set; }
    
    public DateTime OccurredOn { get; set; }
    
    public OperationType Operation { get; set; }
    
    public string EntityId { get; set; } = string.Empty;
    
    public string UserId { get; set; } = string.Empty;
    
    public string Email { get; set; } = string.Empty;
    
    public string CompanyName { get; set; } = string.Empty;
    
    public string TaxId { get; set; } = string.Empty;
    
    public string Country { get; set; } = string.Empty;
    
    public string City { get; set; } = string.Empty;
    
    public string Street { get; set; } = string.Empty;
    
    public string HouseNumber { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;
}