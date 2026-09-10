using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebFlow.AspNetCore.Controllers;
using WebFlow.AspNetCore.Models;

namespace WebFlow.AspNetCore.Tests;

public sealed class ApplicationControllerTests
{
    [Fact]
    public void CreatedEnvelope_WhenCalled_ReturnsCreatedEnvelopeWithoutLocation()
    {
        var controller = new TestController();

        var result = controller.Create(Guid.Empty);

        Assert.Equal(StatusCodes.Status201Created, result.StatusCode);
        Assert.Null(result.Location);
        var envelope = Assert.IsType<Envelope<Guid>>(result.Value);
        Assert.Equal(Guid.Empty, envelope.Data);
        Assert.Null(envelope.Errors);
    }

    [Fact]
    public void AcceptedEnvelope_WhenCalled_ReturnsAcceptedEnvelope()
    {
        var controller = new TestController();

        var result = controller.Accept(Guid.Empty);

        Assert.Equal(StatusCodes.Status202Accepted, result.StatusCode);
        var envelope = Assert.IsType<Envelope<Guid>>(result.Value);
        Assert.Equal(Guid.Empty, envelope.Data);
        Assert.Null(envelope.Errors);
    }

    private sealed class TestController : ApplicationController
    {
        public CreatedResult Create<T>(T response) => CreatedEnvelope(response);

        public AcceptedResult Accept<T>(T response) => AcceptedEnvelope(response);
    }
}
