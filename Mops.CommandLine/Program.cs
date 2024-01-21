using System.Reflection;
using Microsoft.Extensions.Logging;
using Mops.Contracts;
using Mops.Contracts.Base;
using Mops.Contracts.Base.Commands;
using Mops.Contracts.Commands;

namespace Mops.CommandLine;

internal class Program
{
    internal static Settings Settings = new();

    private static async Task Main(string[] args)
    {
        var assemblyDir = new FileInfo(Assembly.GetExecutingAssembly().Location);
        Settings.RootFolder = assemblyDir.Directory!;
        if (Environment.GetEnvironmentVariables().Contains("MOPS_ROOT"))
            Settings.RootFolder = new DirectoryInfo(Environment.GetEnvironmentVariable("MOPS_ROOT")!);
        //Settings.RootFolder = new DirectoryInfo(@"c:\mops2");

        // lets see if any command can use the parameter for execution
        var factory = new CommandFactory();
        factory.AddAssembly(typeof(ReindexCommand).Assembly); // add this assembly which contains the commands
        factory.Register<ILogger>(() => new Logger<Program>(new LoggerFactory()));
        factory.RegisterSingleton<IStorageEndpoint>(() =>
        {
            var connection = new Uri(Settings.RootFolder.FullName);
            return new FileSystemStorageEndpoint(connection.AbsoluteUri);
        });

        // lets get the commands
        var command = factory.GetCommand(args);
        if (command != null)
        {
            await command.Execute(args);
            return;
        }

        foreach (var anArg in args)
        {
            // if just a file is provided, we simply add this one
            if (File.Exists(anArg))
                factory.GetCommand(new[] { "addfile", anArg })?.Execute(new[] { "addfile", anArg });

            // if just a folder is provided, we simply add this one
            else if (Directory.Exists(anArg))
                factory.GetCommand(new[] { "addfolder", anArg })?.Execute(new[] { "addfolder", anArg });
        }
    }
}