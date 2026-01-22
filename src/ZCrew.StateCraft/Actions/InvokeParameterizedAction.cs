using ZCrew.Extensions.Tasks;
using ZCrew.StateCraft.Actions.Contracts;

namespace ZCrew.StateCraft.Actions;

/// <inheritdoc />
internal class InvokeParameterizedAction<T> : IParameterizedAction<T>
{
    private readonly IAsyncAction<T> action;

    internal InvokeParameterizedAction(IAsyncAction<T> action)
    {
        this.action = action;
    }

    /// <inheritdoc />
    public IReadOnlyList<Type> TypeParameters { get; } = [];

    /// <inheritdoc />
    public Task Invoke(T parameter, CancellationToken token)
    {
        return this.action.InvokeAsync(parameter, token);
    }
}
