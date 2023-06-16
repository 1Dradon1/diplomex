namespace ShippingService.Exceptions;

public class BestShipFindingException : Exception
{
    public BestShipFindingException()
    {
    }

    public BestShipFindingException(string? message)
        : base(message)
    {
    }

    public BestShipFindingException(string? message, Exception? inner)
        : base(message, inner)
    {
    }
}