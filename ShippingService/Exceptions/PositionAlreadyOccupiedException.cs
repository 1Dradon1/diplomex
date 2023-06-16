namespace ShippingService.Exceptions;

public class PositionAlreadyOccupiedException : Exception
{
    public PositionAlreadyOccupiedException()
    {
    }

    public PositionAlreadyOccupiedException(string? message)
        : base(message)
    {
    }

    public PositionAlreadyOccupiedException(string? message, Exception? inner)
        : base(message, inner)
    {
    }
}