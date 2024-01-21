namespace Mops.Contracts.EmptyObjects;

/// <summary>
///     Empty Object Pattern implementation for the <see cref="IStorageEndpointFactory" /> which always returns an
///     <see cref="EmptyStorageEndpoint" />
/// </summary>
public class EmptyStorageEndpointFactory : IStorageEndpointFactory
{
    /// <returns>returns an<see cref="EmptyStorageEndpoint" /></returns>
    public IStorageEndpoint GetStorage(string connectionString)
    {
        return new EmptyStorageEndpoint();
    }
}