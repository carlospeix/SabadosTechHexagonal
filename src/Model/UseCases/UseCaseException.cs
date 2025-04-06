namespace Model.UseCases;

public abstract class UseCaseException : Exception
{
    public UseCaseException(string message) : base(message) { }
}

public class InvalidParameterException : UseCaseException
{
    public InvalidParameterException(string message) : base(message) { }
}
