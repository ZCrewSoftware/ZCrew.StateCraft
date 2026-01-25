using ZCrew.Extensions.Tasks;
using ZCrew.StateCraft.Actions.Contracts;

namespace ZCrew.StateCraft.Actions;

/// <inheritdoc />
internal class InvokeParameterlessActionConfiguration : IFinalParameterlessActionConfiguration
{
    private readonly IAsyncAction action;

    internal InvokeParameterlessActionConfiguration(IAsyncAction action)
    {
        this.action = action;
    }

    /// <inheritdoc />
    public IParameterlessAction Build()
    {
        return new InvokeParameterlessAction(this.action);
    }
}
