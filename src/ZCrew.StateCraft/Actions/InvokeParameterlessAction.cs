using ZCrew.Extensions.Tasks;
using ZCrew.StateCraft.Actions.Contracts;

namespace ZCrew.StateCraft.Actions;

/// <inheritdoc />
internal class InvokeParameterlessAction : IParameterlessAction
{
    private readonly IAsyncAction action;

    internal InvokeParameterlessAction(IAsyncAction action)
    {
        this.action = action;
    }

    /// <inheritdoc />
    public IReadOnlyList<Type> TypeParameters { get; } = [];

    /// <inheritdoc />
    public Task Invoke(CancellationToken token)
    {
        return this.action.InvokeAsync(token);
    }
}
