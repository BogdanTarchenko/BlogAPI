namespace HitsBackend.Application.Common.Exceptions;

public class UnauthorizedException : ApiException
{
    public UnauthorizedException(string message) 
        : base(message, 401) { }
}