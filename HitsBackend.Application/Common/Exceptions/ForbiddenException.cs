namespace HitsBackend.Application.Common.Exceptions;

public class ForbiddenException : ApiException
{
    public ForbiddenException(string message) 
        : base(message, 403) { }
} 