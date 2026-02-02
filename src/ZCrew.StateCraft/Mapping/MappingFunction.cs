using ZCrew.Extensions.Tasks;
using ZCrew.StateCraft.Mapping.Contracts;
using ZCrew.StateCraft.Parameters.Contracts;

namespace ZCrew.StateCraft.Mapping;

/// <inheritdoc />
internal class MappingFunction<TIn, TOut> : IMappingFunction
{
    private readonly IAsyncFunc<TIn, TOut> function;

    public MappingFunction(IAsyncFunc<TIn, TOut> function)
    {
        this.function = function;
    }

    /// <inheritdoc />
    public async Task Map(IStateMachineParameters parameters, CancellationToken token)
    {
        var parameter = parameters.GetPreviousParameter<TIn>(0);
        var output = await this.function.InvokeAsync(parameter, token);
        parameters.SetNextParameters([output]);
    }
}

/// <inheritdoc />
internal class MappingFunction<TIn1, TIn2, TOut> : IMappingFunction
{
    private readonly IAsyncFunc<TIn1, TIn2, TOut> function;

    public MappingFunction(IAsyncFunc<TIn1, TIn2, TOut> function)
    {
        this.function = function;
    }

    /// <inheritdoc />
    public async Task Map(IStateMachineParameters parameters, CancellationToken token)
    {
        var parameter1 = parameters.GetPreviousParameter<TIn1>(0);
        var parameter2 = parameters.GetPreviousParameter<TIn2>(1);
        var output = await this.function.InvokeAsync(parameter1, parameter2, token);
        parameters.SetNextParameters([output]);
    }
}

/// <inheritdoc />
internal class MappingFunction<TIn1, TIn2, TIn3, TOut> : IMappingFunction
{
    private readonly IAsyncFunc<TIn1, TIn2, TIn3, TOut> function;

    public MappingFunction(IAsyncFunc<TIn1, TIn2, TIn3, TOut> function)
    {
        this.function = function;
    }

    /// <inheritdoc />
    public async Task Map(IStateMachineParameters parameters, CancellationToken token)
    {
        var parameter1 = parameters.GetPreviousParameter<TIn1>(0);
        var parameter2 = parameters.GetPreviousParameter<TIn2>(1);
        var parameter3 = parameters.GetPreviousParameter<TIn3>(2);
        var output = await this.function.InvokeAsync(parameter1, parameter2, parameter3, token);
        parameters.SetNextParameters([output]);
    }
}

/// <inheritdoc />
internal class MappingFunction<TIn1, TIn2, TIn3, TIn4, TOut> : IMappingFunction
{
    private readonly IAsyncFunc<TIn1, TIn2, TIn3, TIn4, TOut> function;

    public MappingFunction(IAsyncFunc<TIn1, TIn2, TIn3, TIn4, TOut> function)
    {
        this.function = function;
    }

    /// <inheritdoc />
    public async Task Map(IStateMachineParameters parameters, CancellationToken token)
    {
        var parameter1 = parameters.GetPreviousParameter<TIn1>(0);
        var parameter2 = parameters.GetPreviousParameter<TIn2>(1);
        var parameter3 = parameters.GetPreviousParameter<TIn3>(2);
        var parameter4 = parameters.GetPreviousParameter<TIn4>(3);
        var output = await this.function.InvokeAsync(parameter1, parameter2, parameter3, parameter4, token);
        parameters.SetNextParameters([output]);
    }
}
