namespace Viamatica.Application.Common;

public sealed class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message)
    {
    }
}

public sealed class ConflictException : Exception
{
    public ConflictException(string message) : base(message)
    {
    }
}

public sealed class ForbiddenOperationException : Exception
{
    public ForbiddenOperationException(string message) : base(message)
    {
    }
}

public sealed class BusinessRuleException : Exception
{
    public BusinessRuleException(string message) : base(message)
    {
    }
}
