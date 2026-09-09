namespace WalletApi.Exceptions;

// Services throw these - they describe *what went wrong* in domain terms.
// ExceptionMiddleware is the only place that translates them into HTTP status codes.
// This keeps your service layer completely unaware that HTTP even exists.

public class ConflictException : Exception
{
    public ConflictException(string message) : base(message) { }
}

public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message) { }
}

public class BadRequestException : Exception
{
    public BadRequestException(string message) : base(message) { }
}

public class UnauthorizedException : Exception
{
    public UnauthorizedException(string message) : base(message) { }
}