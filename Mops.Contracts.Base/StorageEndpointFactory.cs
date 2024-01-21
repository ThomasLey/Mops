using System.Reflection;
using Mops.Contracts.Attributes;
using Mops.Contracts.Exceptions;

namespace Mops.Contracts.Base;

public class StorageEndpointFactory : IStorageEndpointFactory
{
    private readonly Dictionary<string, Type> _storageTypes = new();

    public IStorageEndpoint GetStorage(string connectionString)
    {
        var prefix = connectionString.Split("://")[0] + "://";

        if (!_storageTypes.TryGetValue(prefix, out var storageType))
            throw new StorageNotSupportedException(prefix);

        var storage =
            Activator.CreateInstance(storageType, connectionString.Substring(prefix.Length)) as IStorageEndpoint;
        return storage ?? throw new StorageNotSupportedException(prefix);
    }

    public void Add(Assembly assembly)
    {
        var types = assembly.GetTypes().Where(x => x.IsAssignableTo(typeof(IStorageEndpoint)) && !x.IsAbstract);

        foreach (var type in types)
        {
            var prefix = type.GetCustomAttribute<StorageEndpointPrefixAttribute>();
            if (prefix == null) continue;
            _storageTypes.Add(prefix.Prefix, type);
        }
    }
}