using Microsoft.Extensions.Logging;
using Mops.Contracts;
using Mops.Contracts.Commands;

namespace Mops.Contracts.Base.Commands;

internal class AddFolderCommand : ICommand
{
    private readonly ILogger _logger;
    private readonly IStorageEndpoint _storage;

    public AddFolderCommand(ILogger logger, IStorageEndpoint storage)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _storage = storage ?? throw new ArgumentNullException(nameof(storage));
    }

    public bool CanExecute(string[] context)
    {
        return context.Length == 2
               && string.Compare(context[0], "addfolder", StringComparison.InvariantCultureIgnoreCase) == 0;
    }

    public async Task Execute(string[] context)
    {
        if (!CanExecute(context)) return;

        var files = new DirectoryInfo(context[1]).GetFiles("*.*", SearchOption.AllDirectories);
        var command = new AddFileCommand(_logger, _storage);

        foreach (var file in files) await command.Execute(new[] { "addfile", file.FullName });
    }
}