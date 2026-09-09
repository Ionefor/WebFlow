using System;
using WebFlow.Models;

namespace WebFlow.Abstractions.Interfaces;

public interface IEvent
{
    Guid Id { get; set; }
    
    DateTime OccurredOn { get; set; }
    
    OperationType Operation { get; set; }
    
    string EntityId { get; set; } 
}