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
        var parameter = parameters.GetPreviousParameter<TIn>();
        var output = await this.function.InvokeAsync(parameter, token);
        parameters.SetNextParameter(output);
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
        var (parameter1, parameter2) = parameters.GetPreviousParameters<TIn1, TIn2>();
        var output = await this.function.InvokeAsync(parameter1, parameter2, token);
        parameters.SetNextParameter(output);
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
        var (parameter1, parameter2, parameter3) = parameters.GetPreviousParameters<TIn1, TIn2, TIn3>();
        var output = await this.function.InvokeAsync(parameter1, parameter2, parameter3, token);
        parameters.SetNextParameter(output);
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
        var (parameter1, parameter2, parameter3, parameter4) = parameters.GetPreviousParameters<
            TIn1,
            TIn2,
            TIn3,
            TIn4
        >();
        var output = await this.function.InvokeAsync(parameter1, parameter2, parameter3, parameter4, token);
        parameters.SetNextParameter(output);
    }
}
