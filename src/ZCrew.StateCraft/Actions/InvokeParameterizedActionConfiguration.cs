using ZCrew.Extensions.Tasks;
using ZCrew.StateCraft.Actions.Contracts;

namespace ZCrew.StateCraft.Actions;

/// <inheritdoc />
internal class InvokeParameterizedActionConfiguration<T> : IFinalParameterizedActionConfiguration<T>
{
    private readonly IAsyncAction<T> action;

    internal InvokeParameterizedActionConfiguration(IAsyncAction<T> action)
    {
        this.action = action;
    }

    /// <inheritdoc />
    public IParameterizedAction<T> Build()
    {
        return new InvokeParameterizedAction<T>(this.action);
    }
}
