using Microsoft.AspNetCore.Mvc;
using WebFlow.Models;

namespace WebFlow.Controller;

[ApiController]
[Route("[controller]")]
public partial class ApplicationController : ControllerBase
{
    public override OkObjectResult Ok(object? value)
    {
        var envelope = Envelope.Ok(value);
        return base.Ok(envelope);
    }
}