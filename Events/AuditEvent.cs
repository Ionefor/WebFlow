using System;
using WebFlow.Abstractions.Interfaces;
using WebFlow.Models;

namespace WebFlow.Events;

public class AuditEvent : IEvent
{
    public Guid Id { get; set; }
    
    public string UserId { get; set; } = string.Empty;

    public DateTime OccurredOn { get; set; }

    public string EntityName { get; set; } = string.Empty;
    
    public string EntityId { get; set; } = string.Empty;
    
    public OperationType Operation { get; set; }
    
    public object? NewValues { get; set; }
    
    public object? OldValues { get; set; }
}