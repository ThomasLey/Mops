namespace Mops.Contracts.Exceptions;

public class DocumentNotExistException : MopsException
{
    public DocumentNotExistException(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; }
}