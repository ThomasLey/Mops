using System.Reflection;

namespace Mops.Contracts.Commands;

public class CommandFactory
{
    #region Simple DI Container (Service registration/resolve functions)

    private readonly Dictionary<Type, Func<object>> _registrations = new();

    public void Register<TService, TImpl>() where TImpl : TService
    {
        if (_lazyCommands.IsValueCreated) throw new Exception("Cannot register service after commands are created");
        _registrations.Add(typeof(TService), () => this.GetInstance(typeof(TImpl)));
    }

    public void Register<TService>(Func<TService> factory)
    {
        if (_lazyCommands.IsValueCreated) throw new Exception("Cannot register service after commands are created");
        _registrations.Add(typeof(TService), () => factory()!);
    }

    public void RegisterInstance<TService>(TService instance)
    {
        if (_lazyCommands.IsValueCreated) throw new Exception("Cannot register service after commands are created");
        _registrations.Add(typeof(TService), () => instance!);
    }

    public void RegisterSingleton<TService>(Func<TService> factory)
    {
        if (_lazyCommands.IsValueCreated) throw new Exception("Cannot register service after commands are created");
        var lazy = new Lazy<TService>(factory);
        Register(() => lazy.Value);
    }

    private object GetInstance(Type type)
    {
        if (_registrations.TryGetValue(type, out var fac)) return fac();
        else if (!type.IsAbstract) return CreateInstance(type);
        throw new InvalidOperationException("No registration for " + type);

        // inline (recursive) function to create the instance with the first (and only) constructor
        object CreateInstance(Type implementationType)
        {
            var ctor = implementationType.GetConstructors().Single();
            var paramTypes = ctor.GetParameters().Select(p => p.ParameterType);
            var dependencies = paramTypes.Select(GetInstance).ToArray();
            return Activator.CreateInstance(implementationType, dependencies)!;
        }
    }

    #endregion

    #region Lazy command types & constructor

    private readonly Lazy<IEnumerable<ICommand>> _lazyCommands;
    private readonly List<Assembly> _assemblies = new();

    private IEnumerable<ICommand> CreateCommands()
    {
        var types = _assemblies.SelectMany(x =>
            x.GetTypes().Where(y => y.IsAssignableTo(typeof(ICommand)) && !y.IsAbstract));

        // this is a little workaround hence this simple DO cannot register the same interface multiple times.
        return types.Select(x => (ICommand)GetInstance(x));
        // todo: discuss it the commands are registered as class (not interface) and ask for all "ICommand"
    }

    public CommandFactory()
    {
        _lazyCommands = new Lazy<IEnumerable<ICommand>>(CreateCommands);
    }

    #endregion

    public ICommand? GetCommand(string[] context)
    {
        return _lazyCommands.Value.SingleOrDefault(x => x.CanExecute(context));
    }

    public void AddAssembly(Assembly assembly)
    {
        _assemblies.Add(assembly); ;
    }
}