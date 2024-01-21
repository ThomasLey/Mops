namespace Mops.Contracts.Base;

internal interface ISuggest<out T>
{
    T Suggest(string source);
}