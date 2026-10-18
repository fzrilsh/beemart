namespace BeeMart.Api.Services;

public class ValidationException : Exception
{
    public ValidationException(string message) : base(message)
    {
    }
}

public class ConflictException : Exception
{
    public string Code { get; }

    public ConflictException(string code, string message) : base(message)
    {
        Code = code;
    }
}

public class UnauthorizedException : Exception
{
    public string Code { get; }

    public UnauthorizedException(string code, string message) : base(message)
    {
        Code = code;
    }
}

public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message)
    {
    }
}
