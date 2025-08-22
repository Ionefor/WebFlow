using System.IO;

namespace WebFlow.Models;

public record CreateFile(Stream Content, string FileName);