using ZCrew.Extensions.Tasks;
using ZCrew.StateCraft.Mapping.Contracts;
using ZCrew.StateCraft.Parameters.Contracts;

namespace ZCrew.StateCraft.Mapping;

/// <inheritdoc />
internal class MappingFunctionValueTuple4<TIn, TOut1, TOut2, TOut3, TOut4> : IMappingFunction
{
    private readonly IAsyncFunc<TIn, (TOut1, TOut2, TOut3, TOut4)> function;

    public MappingFunctionValueTuple4(IAsyncFunc<TIn, (TOut1, TOut2, TOut3, TOut4)> function)
    {
        this.function = function;
    }

    /// <inheritdoc />
    public async Task Map(IStateMachineParameters parameters, CancellationToken token)
    {
        var parameter = parameters.GetPreviousParameter<TIn>();
        var output = await this.function.InvokeAsync(parameter, token);
        parameters.SetNextParameters<TOut1, TOut2, TOut3, TOut4>(
            output.Item1,
            output.Item2,
            output.Item3,
            output.Item4
        );
    }
}

/// <inheritdoc />
internal class MappingFunctionValueTuple4<TIn1, TIn2, TOut1, TOut2, TOut3, TOut4> : IMappingFunction
{
    private readonly IAsyncFunc<TIn1, TIn2, (TOut1, TOut2, TOut3, TOut4)> function;

    public MappingFunctionValueTuple4(IAsyncFunc<TIn1, TIn2, (TOut1, TOut2, TOut3, TOut4)> function)
    {
        this.function = function;
    }

    /// <inheritdoc />
    public async Task Map(IStateMachineParameters parameters, CancellationToken token)
    {
        var (parameter1, parameter2) = parameters.GetPreviousParameters<TIn1, TIn2>();
        var output = await this.function.InvokeAsync(parameter1, parameter2, token);
        parameters.SetNextParameters<TOut1, TOut2, TOut3, TOut4>(
            output.Item1,
            output.Item2,
            output.Item3,
            output.Item4
        );
    }
}

/// <inheritdoc />
internal class MappingFunctionValueTuple4<TIn1, TIn2, TIn3, TOut1, TOut2, TOut3, TOut4> : IMappingFunction
{
    private readonly IAsyncFunc<TIn1, TIn2, TIn3, (TOut1, TOut2, TOut3, TOut4)> function;

    public MappingFunctionValueTuple4(IAsyncFunc<TIn1, TIn2, TIn3, (TOut1, TOut2, TOut3, TOut4)> function)
    {
        this.function = function;
    }

    /// <inheritdoc />
    public async Task Map(IStateMachineParameters parameters, CancellationToken token)
    {
        var (parameter1, parameter2, parameter3) = parameters.GetPreviousParameters<TIn1, TIn2, TIn3>();
        var output = await this.function.InvokeAsync(parameter1, parameter2, parameter3, token);
        parameters.SetNextParameters<TOut1, TOut2, TOut3, TOut4>(
            output.Item1,
            output.Item2,
            output.Item3,
            output.Item4
        );
    }
}

/// <inheritdoc />
internal class MappingFunctionValueTuple4<TIn1, TIn2, TIn3, TIn4, TOut1, TOut2, TOut3, TOut4> : IMappingFunction
{
    private readonly IAsyncFunc<TIn1, TIn2, TIn3, TIn4, (TOut1, TOut2, TOut3, TOut4)> function;

    public MappingFunctionValueTuple4(IAsyncFunc<TIn1, TIn2, TIn3, TIn4, (TOut1, TOut2, TOut3, TOut4)> function)
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
        parameters.SetNextParameters<TOut1, TOut2, TOut3, TOut4>(
            output.Item1,
            output.Item2,
            output.Item3,
            output.Item4
        );
    }
}
