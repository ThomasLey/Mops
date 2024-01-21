namespace Mops.Contracts;

public interface IStorageEndpointFactory
{
    IStorageEndpoint GetStorage(string connectionString);
}