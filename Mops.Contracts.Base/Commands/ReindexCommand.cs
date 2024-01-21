using Microsoft.Extensions.Logging;
using Mops.Contracts;
using Mops.Contracts.Commands;

namespace Mops.Contracts.Base.Commands;

public class ReindexCommand : ICommand
{
    private readonly ILogger _logger;
    private readonly IStorageEndpoint _storage;

    public ReindexCommand(ILogger logger, IStorageEndpoint storage)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _storage = storage ?? throw new ArgumentNullException(nameof(storage));
    }
    public bool CanExecute(string[] context)
    {
        return context.Length == 1
               && string.Compare(context[0], "reindex", StringComparison.InvariantCultureIgnoreCase) == 0;
    }

    public async Task Execute(string[] context)
    {
        _logger.LogTrace("Reindexing the Storage Endpoint");

        await _storage.InitStorageEndpoint();
    }
}