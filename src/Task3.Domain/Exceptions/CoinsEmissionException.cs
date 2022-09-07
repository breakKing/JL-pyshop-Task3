using System.Runtime.Serialization;

namespace Task3.Domain.Exceptions;

public class CoinsEmissionException : Exception
{
    private const string INSUFFICIENT_AMOUNT_MESSAGE = "{1} coins is not enough for {2} users";
    public CoinsEmissionException() : base()
    {
    }

    protected CoinsEmissionException(SerializationInfo info, StreamingContext context) :
        base(info, context)
    {
    }

    public CoinsEmissionException(string? message) : base(message)
    {
    }

    public CoinsEmissionException(string? message, Exception? innerException) :
        base(message, innerException)
    {
    }

    public CoinsEmissionException(long coinsAmount, long usersCount) :
        base(string.Format(INSUFFICIENT_AMOUNT_MESSAGE, coinsAmount, usersCount))
    {
    }
}
