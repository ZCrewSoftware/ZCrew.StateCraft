using ZCrew.StateCraft.Async;
using ZCrew.StateCraft.Mapping.Contracts;
using ZCrew.StateCraft.Parameters.Contracts;

namespace ZCrew.StateCraft.Mapping;

/// <inheritdoc />
internal class MappingFunctionValueTuple2<TIn, TOut1, TOut2> : IMappingFunction
{
    private readonly AsyncMapValueTuple2<TIn, TOut1, TOut2> map;

    public MappingFunctionValueTuple2(AsyncMapValueTuple2<TIn, TOut1, TOut2> map)
    {
        this.map = map;
    }

    /// <inheritdoc />
    public string? Descriptor => this.map.Descriptor;

    /// <inheritdoc />
    public async Task Map(IStateMachineParameters parameters, CancellationToken token)
    {
        var parameter = parameters.GetPreviousParameter<TIn>();
        var output = await this.map.Invoke(parameter, token);
        parameters.SetNextParameters(output.Item1, output.Item2);
    }
}

/// <inheritdoc />
internal class MappingFunctionValueTuple2<TIn1, TIn2, TOut1, TOut2> : IMappingFunction
{
    private readonly AsyncMapValueTuple2<TIn1, TIn2, TOut1, TOut2> map;

    public MappingFunctionValueTuple2(AsyncMapValueTuple2<TIn1, TIn2, TOut1, TOut2> map)
    {
        this.map = map;
    }

    /// <inheritdoc />
    public string? Descriptor => this.map.Descriptor;

    /// <inheritdoc />
    public async Task Map(IStateMachineParameters parameters, CancellationToken token)
    {
        var (parameter1, parameter2) = parameters.GetPreviousParameters<TIn1, TIn2>();
        var output = await this.map.Invoke(parameter1, parameter2, token);
        parameters.SetNextParameters(output.Item1, output.Item2);
    }
}

/// <inheritdoc />
internal class MappingFunctionValueTuple2<TIn1, TIn2, TIn3, TOut1, TOut2> : IMappingFunction
{
    private readonly AsyncMapValueTuple2<TIn1, TIn2, TIn3, TOut1, TOut2> map;

    public MappingFunctionValueTuple2(AsyncMapValueTuple2<TIn1, TIn2, TIn3, TOut1, TOut2> map)
    {
        this.map = map;
    }

    /// <inheritdoc />
    public string? Descriptor => this.map.Descriptor;

    /// <inheritdoc />
    public async Task Map(IStateMachineParameters parameters, CancellationToken token)
    {
        var (parameter1, parameter2, parameter3) = parameters.GetPreviousParameters<TIn1, TIn2, TIn3>();
        var output = await this.map.Invoke(parameter1, parameter2, parameter3, token);
        parameters.SetNextParameters(output.Item1, output.Item2);
    }
}

/// <inheritdoc />
internal class MappingFunctionValueTuple2<TIn1, TIn2, TIn3, TIn4, TOut1, TOut2> : IMappingFunction
{
    private readonly AsyncMapValueTuple2<TIn1, TIn2, TIn3, TIn4, TOut1, TOut2> map;

    public MappingFunctionValueTuple2(AsyncMapValueTuple2<TIn1, TIn2, TIn3, TIn4, TOut1, TOut2> map)
    {
        this.map = map;
    }

    /// <inheritdoc />
    public string? Descriptor => this.map.Descriptor;

    /// <inheritdoc />
    public async Task Map(IStateMachineParameters parameters, CancellationToken token)
    {
        var (parameter1, parameter2, parameter3, parameter4) = parameters.GetPreviousParameters<
            TIn1,
            TIn2,
            TIn3,
            TIn4
        >();
        var output = await this.map.Invoke(parameter1, parameter2, parameter3, parameter4, token);
        parameters.SetNextParameters(output.Item1, output.Item2);
    }
}
