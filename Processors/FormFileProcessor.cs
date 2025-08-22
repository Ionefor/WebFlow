using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using WebFlow.Models;

namespace WebFlow.Processors;

public class FormFileProcessor : IAsyncDisposable
{
    private CreateFile _file;
    
    public async ValueTask DisposeAsync()
    {
        await _file.Content.DisposeAsync();
    }

    public CreateFile Process(IFormFile file)
    {
        var stream = file.OpenReadStream();
        _file = new CreateFile(stream, file.FileName);
        
        return _file;
    }
}
