namespace Mops.Contracts.Commands;

public interface ICommand
{
    bool CanExecute(string[] context);
    Task Execute(string[] context);
}