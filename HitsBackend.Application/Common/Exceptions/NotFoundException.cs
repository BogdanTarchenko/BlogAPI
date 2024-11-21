namespace HitsBackend.Application.Common.Exceptions;

public class NotFoundException : ApiException
{
    public NotFoundException(string name, object key) 
        : base($"Entity \"{name}\" ({key}) was not found.", 404) { }
}