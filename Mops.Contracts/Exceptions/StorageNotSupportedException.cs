namespace Mops.Contracts.Exceptions;

public class StorageNotSupportedException : MopsException
{
    public StorageNotSupportedException(string connectionString)
    {
        ConnectionString = connectionString;
    }

    public string ConnectionString { get; }
}