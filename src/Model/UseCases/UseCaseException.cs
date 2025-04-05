namespace Model.UseCases;

public abstract class UseCaseException : Exception
{
    public UseCaseException(string message) : base(message) { }

    public UseCaseException(string message, Exception e) : base(message, e) { }
}
