using ZCrew.Extensions.Tasks;
using ZCrew.StateCraft.Mapping.Contracts;
using ZCrew.StateCraft.Parameters.Contracts;

namespace ZCrew.StateCraft.Mapping;

/// <inheritdoc />
internal class MappingFunctionValueTuple3<TIn, TOut1, TOut2, TOut3> : IMappingFunction
{
    private readonly IAsyncFunc<TIn, (TOut1, TOut2, TOut3)> function;

    public MappingFunctionValueTuple3(IAsyncFunc<TIn, (TOut1, TOut2, TOut3)> function)
    {
        this.function = function;
    }

    /// <inheritdoc />
    public async Task Map(IStateMachineParameters parameters, CancellationToken token)
    {
        var parameter = parameters.GetPreviousParameter<TIn>(0);
        var output = await this.function.InvokeAsync(parameter, token);
        parameters.SetNextParameters([output.Item1, output.Item2, output.Item3]);
    }
}

/// <inheritdoc />
internal class MappingFunctionValueTuple3<TIn1, TIn2, TOut1, TOut2, TOut3> : IMappingFunction
{
    private readonly IAsyncFunc<TIn1, TIn2, (TOut1, TOut2, TOut3)> function;

    public MappingFunctionValueTuple3(IAsyncFunc<TIn1, TIn2, (TOut1, TOut2, TOut3)> function)
    {
        this.function = function;
    }

    /// <inheritdoc />
    public async Task Map(IStateMachineParameters parameters, CancellationToken token)
    {
        var parameter1 = parameters.GetPreviousParameter<TIn1>(0);
        var parameter2 = parameters.GetPreviousParameter<TIn2>(1);
        var output = await this.function.InvokeAsync(parameter1, parameter2, token);
        parameters.SetNextParameters([output.Item1, output.Item2, output.Item3]);
    }
}

/// <inheritdoc />
internal class MappingFunctionValueTuple3<TIn1, TIn2, TIn3, TOut1, TOut2, TOut3> : IMappingFunction
{
    private readonly IAsyncFunc<TIn1, TIn2, TIn3, (TOut1, TOut2, TOut3)> function;

    public MappingFunctionValueTuple3(IAsyncFunc<TIn1, TIn2, TIn3, (TOut1, TOut2, TOut3)> function)
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
        parameters.SetNextParameters([output.Item1, output.Item2, output.Item3]);
    }
}

/// <inheritdoc />
internal class MappingFunctionValueTuple3<TIn1, TIn2, TIn3, TIn4, TOut1, TOut2, TOut3> : IMappingFunction
{
    private readonly IAsyncFunc<TIn1, TIn2, TIn3, TIn4, (TOut1, TOut2, TOut3)> function;

    public MappingFunctionValueTuple3(IAsyncFunc<TIn1, TIn2, TIn3, TIn4, (TOut1, TOut2, TOut3)> function)
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
        parameters.SetNextParameters([output.Item1, output.Item2, output.Item3]);
    }
}
