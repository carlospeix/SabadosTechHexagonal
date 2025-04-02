namespace Model.UseCases;

public abstract class UseCaseException : Exception
{
    public UseCaseException(string message) : base(message) { }

    public UseCaseException(Exception e, string message) : base(message, e) { }
}
