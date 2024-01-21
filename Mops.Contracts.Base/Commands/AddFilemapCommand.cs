using Microsoft.Extensions.Logging;
using Mops.Contracts;
using Mops.Contracts.Commands;
using Mops.Contracts.DomainModel;

namespace Mops.Contracts.Base.Commands;

internal class AddFilemapCommand : ICommand
{
    private readonly ILogger _logger;
    private readonly IStorageEndpoint _storage;

    public AddFilemapCommand(ILogger logger, IStorageEndpoint storage)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _storage = storage ?? throw new ArgumentNullException(nameof(storage));
    }

    public bool CanExecute(string[] context)
    {
        return context.Length == 2
               && string.Compare(context[0], "addfilemap", StringComparison.InvariantCultureIgnoreCase) == 0
               && !string.IsNullOrWhiteSpace(context[1]);
    }

    public async Task Execute(string[] context)
    {
        if (!CanExecute(context)) return;

        await _storage.AddFilemap(Filemap.Create(context[1]));
    }
}