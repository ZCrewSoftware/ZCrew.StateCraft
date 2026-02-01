using Spike.Contracts;

namespace Spike;

public class MappingFunction<TIn, TOut> : IMappingFunction
{
    private readonly Func<TIn, TOut> function;

    public MappingFunction(Func<TIn, TOut> function)
    {
        this.function = function;
    }

    public void Map(IStateMachineParameters parameters)
    {
        var parameter = parameters.GetPreviousParameter<TIn>(0);
        var output = this.function(parameter);
        parameters.SetNextParameter(0, output);
    }
}

public class MappingFunction<TIn1, TIn2, TOut> : IMappingFunction
{
    private readonly Func<TIn1, TIn2, TOut> function;

    public MappingFunction(Func<TIn1, TIn2, TOut> function)
    {
        this.function = function;
    }

    public void Map(IStateMachineParameters parameters)
    {
        var parameter1 = parameters.GetPreviousParameter<TIn1>(0);
        var parameter2 = parameters.GetPreviousParameter<TIn2>(1);
        var output = this.function(parameter1, parameter2);
        parameters.SetNextParameter(0, output);
    }
}

public class MappingFunction<TIn1, TIn2, TIn3, TOut> : IMappingFunction
{
    private readonly Func<TIn1, TIn2, TIn3, TOut> function;

    public MappingFunction(Func<TIn1, TIn2, TIn3, TOut> function)
    {
        this.function = function;
    }

    public void Map(IStateMachineParameters parameters)
    {
        var parameter1 = parameters.GetPreviousParameter<TIn1>(0);
        var parameter2 = parameters.GetPreviousParameter<TIn2>(1);
        var parameter3 = parameters.GetPreviousParameter<TIn3>(2);
        var output = this.function(parameter1, parameter2, parameter3);
        parameters.SetNextParameter(0, output);
    }
}

public class MappingFunction<TIn1, TIn2, TIn3, TIn4, TOut> : IMappingFunction
{
    private readonly Func<TIn1, TIn2, TIn3, TIn4, TOut> function;

    public MappingFunction(Func<TIn1, TIn2, TIn3, TIn4, TOut> function)
    {
        this.function = function;
    }

    public void Map(IStateMachineParameters parameters)
    {
        var parameter1 = parameters.GetPreviousParameter<TIn1>(0);
        var parameter2 = parameters.GetPreviousParameter<TIn2>(1);
        var parameter3 = parameters.GetPreviousParameter<TIn3>(2);
        var parameter4 = parameters.GetPreviousParameter<TIn4>(3);
        var output = this.function(parameter1, parameter2, parameter3, parameter4);
        parameters.SetNextParameter(0, output);
    }
}

public class MappingFunctionValueTuple2<TIn, TOut1, TOut2> : IMappingFunction
{
    private readonly Func<TIn, (TOut1, TOut2)> function;

    public MappingFunctionValueTuple2(Func<TIn, (TOut1, TOut2)> function)
    {
        this.function = function;
    }

    public void Map(IStateMachineParameters parameters)
    {
        var parameter = parameters.GetPreviousParameter<TIn>(0);
        var output = this.function(parameter);
        parameters.SetNextParameter(0, output.Item1);
        parameters.SetNextParameter(1, output.Item2);
    }
}

public class MappingFunctionValueTuple3<TIn, TOut1, TOut2, TOut3> : IMappingFunction
{
    private readonly Func<TIn, (TOut1, TOut2, TOut3)> function;

    public MappingFunctionValueTuple3(Func<TIn, (TOut1, TOut2, TOut3)> function)
    {
        this.function = function;
    }

    public void Map(IStateMachineParameters parameters)
    {
        var parameter = parameters.GetPreviousParameter<TIn>(0);
        var output = this.function(parameter);
        parameters.SetNextParameter(0, output.Item1);
        parameters.SetNextParameter(1, output.Item2);
        parameters.SetNextParameter(2, output.Item3);
    }
}

public class MappingFunctionValueTuple4<TIn, TOut1, TOut2, TOut3, TOut4> : IMappingFunction
{
    private readonly Func<TIn, (TOut1, TOut2, TOut3, TOut4)> function;

    public MappingFunctionValueTuple4(Func<TIn, (TOut1, TOut2, TOut3, TOut4)> function)
    {
        this.function = function;
    }

    public void Map(IStateMachineParameters parameters)
    {
        var parameter = parameters.GetPreviousParameter<TIn>(0);
        var output = this.function(parameter);
        parameters.SetNextParameter(0, output.Item1);
        parameters.SetNextParameter(1, output.Item2);
        parameters.SetNextParameter(2, output.Item3);
        parameters.SetNextParameter(3, output.Item4);
    }
}

public class MappingFunction2ValueTuple2<TIn1, TIn2, TOut1, TOut2> : IMappingFunction
{
    private readonly Func<TIn1, TIn2, (TOut1, TOut2)> function;

    public MappingFunction2ValueTuple2(Func<TIn1, TIn2, (TOut1, TOut2)> function)
    {
        this.function = function;
    }

    public void Map(IStateMachineParameters parameters)
    {
        var parameter1 = parameters.GetPreviousParameter<TIn1>(0);
        var parameter2 = parameters.GetPreviousParameter<TIn2>(1);
        var output = this.function(parameter1, parameter2);
        parameters.SetNextParameter(0, output.Item1);
        parameters.SetNextParameter(1, output.Item2);
    }
}

public class MappingFunction2ValueTuple3<TIn1, TIn2, TOut1, TOut2, TOut3> : IMappingFunction
{
    private readonly Func<TIn1, TIn2, (TOut1, TOut2, TOut3)> function;

    public MappingFunction2ValueTuple3(Func<TIn1, TIn2, (TOut1, TOut2, TOut3)> function)
    {
        this.function = function;
    }

    public void Map(IStateMachineParameters parameters)
    {
        var parameter1 = parameters.GetPreviousParameter<TIn1>(0);
        var parameter2 = parameters.GetPreviousParameter<TIn2>(1);
        var output = this.function(parameter1, parameter2);
        parameters.SetNextParameter(0, output.Item1);
        parameters.SetNextParameter(1, output.Item2);
        parameters.SetNextParameter(2, output.Item3);
    }
}

public class MappingFunction2ValueTuple4<TIn1, TIn2, TOut1, TOut2, TOut3, TOut4> : IMappingFunction
{
    private readonly Func<TIn1, TIn2, (TOut1, TOut2, TOut3, TOut4)> function;

    public MappingFunction2ValueTuple4(Func<TIn1, TIn2, (TOut1, TOut2, TOut3, TOut4)> function)
    {
        this.function = function;
    }

    public void Map(IStateMachineParameters parameters)
    {
        var parameter1 = parameters.GetPreviousParameter<TIn1>(0);
        var parameter2 = parameters.GetPreviousParameter<TIn2>(1);
        var output = this.function(parameter1, parameter2);
        parameters.SetNextParameter(0, output.Item1);
        parameters.SetNextParameter(1, output.Item2);
        parameters.SetNextParameter(2, output.Item3);
        parameters.SetNextParameter(3, output.Item4);
    }
}

public class MappingFunction3ValueTuple2<TIn1, TIn2, TIn3, TOut1, TOut2> : IMappingFunction
{
    private readonly Func<TIn1, TIn2, TIn3, (TOut1, TOut2)> function;

    public MappingFunction3ValueTuple2(Func<TIn1, TIn2, TIn3, (TOut1, TOut2)> function)
    {
        this.function = function;
    }

    public void Map(IStateMachineParameters parameters)
    {
        var parameter1 = parameters.GetPreviousParameter<TIn1>(0);
        var parameter2 = parameters.GetPreviousParameter<TIn2>(1);
        var parameter3 = parameters.GetPreviousParameter<TIn3>(2);
        var output = this.function(parameter1, parameter2, parameter3);
        parameters.SetNextParameter(0, output.Item1);
        parameters.SetNextParameter(1, output.Item2);
    }
}

public class MappingFunction3ValueTuple3<TIn1, TIn2, TIn3, TOut1, TOut2, TOut3> : IMappingFunction
{
    private readonly Func<TIn1, TIn2, TIn3, (TOut1, TOut2, TOut3)> function;

    public MappingFunction3ValueTuple3(Func<TIn1, TIn2, TIn3, (TOut1, TOut2, TOut3)> function)
    {
        this.function = function;
    }

    public void Map(IStateMachineParameters parameters)
    {
        var parameter1 = parameters.GetPreviousParameter<TIn1>(0);
        var parameter2 = parameters.GetPreviousParameter<TIn2>(1);
        var parameter3 = parameters.GetPreviousParameter<TIn3>(2);
        var output = this.function(parameter1, parameter2, parameter3);
        parameters.SetNextParameter(0, output.Item1);
        parameters.SetNextParameter(1, output.Item2);
        parameters.SetNextParameter(2, output.Item3);
    }
}

public class MappingFunction3ValueTuple4<TIn1, TIn2, TIn3, TOut1, TOut2, TOut3, TOut4> : IMappingFunction
{
    private readonly Func<TIn1, TIn2, TIn3, (TOut1, TOut2, TOut3, TOut4)> function;

    public MappingFunction3ValueTuple4(Func<TIn1, TIn2, TIn3, (TOut1, TOut2, TOut3, TOut4)> function)
    {
        this.function = function;
    }

    public void Map(IStateMachineParameters parameters)
    {
        var parameter1 = parameters.GetPreviousParameter<TIn1>(0);
        var parameter2 = parameters.GetPreviousParameter<TIn2>(1);
        var parameter3 = parameters.GetPreviousParameter<TIn3>(2);
        var output = this.function(parameter1, parameter2, parameter3);
        parameters.SetNextParameter(0, output.Item1);
        parameters.SetNextParameter(1, output.Item2);
        parameters.SetNextParameter(2, output.Item3);
        parameters.SetNextParameter(3, output.Item4);
    }
}

public class MappingFunction4ValueTuple2<TIn1, TIn2, TIn3, TIn4, TOut1, TOut2> : IMappingFunction
{
    private readonly Func<TIn1, TIn2, TIn3, TIn4, (TOut1, TOut2)> function;

    public MappingFunction4ValueTuple2(Func<TIn1, TIn2, TIn3, TIn4, (TOut1, TOut2)> function)
    {
        this.function = function;
    }

    public void Map(IStateMachineParameters parameters)
    {
        var parameter1 = parameters.GetPreviousParameter<TIn1>(0);
        var parameter2 = parameters.GetPreviousParameter<TIn2>(1);
        var parameter3 = parameters.GetPreviousParameter<TIn3>(2);
        var parameter4 = parameters.GetPreviousParameter<TIn4>(3);
        var output = this.function(parameter1, parameter2, parameter3, parameter4);
        parameters.SetNextParameter(0, output.Item1);
        parameters.SetNextParameter(1, output.Item2);
    }
}

public class MappingFunction4ValueTuple3<TIn1, TIn2, TIn3, TIn4, TOut1, TOut2, TOut3> : IMappingFunction
{
    private readonly Func<TIn1, TIn2, TIn3, TIn4, (TOut1, TOut2, TOut3)> function;

    public MappingFunction4ValueTuple3(Func<TIn1, TIn2, TIn3, TIn4, (TOut1, TOut2, TOut3)> function)
    {
        this.function = function;
    }

    public void Map(IStateMachineParameters parameters)
    {
        var parameter1 = parameters.GetPreviousParameter<TIn1>(0);
        var parameter2 = parameters.GetPreviousParameter<TIn2>(1);
        var parameter3 = parameters.GetPreviousParameter<TIn3>(2);
        var parameter4 = parameters.GetPreviousParameter<TIn4>(3);
        var output = this.function(parameter1, parameter2, parameter3, parameter4);
        parameters.SetNextParameter(0, output.Item1);
        parameters.SetNextParameter(1, output.Item2);
        parameters.SetNextParameter(2, output.Item3);
    }
}

public class MappingFunction4ValueTuple4<TIn1, TIn2, TIn3, TIn4, TOut1, TOut2, TOut3, TOut4> : IMappingFunction
{
    private readonly Func<TIn1, TIn2, TIn3, TIn4, (TOut1, TOut2, TOut3, TOut4)> function;

    public MappingFunction4ValueTuple4(Func<TIn1, TIn2, TIn3, TIn4, (TOut1, TOut2, TOut3, TOut4)> function)
    {
        this.function = function;
    }

    public void Map(IStateMachineParameters parameters)
    {
        var parameter1 = parameters.GetPreviousParameter<TIn1>(0);
        var parameter2 = parameters.GetPreviousParameter<TIn2>(1);
        var parameter3 = parameters.GetPreviousParameter<TIn3>(2);
        var parameter4 = parameters.GetPreviousParameter<TIn4>(3);
        var output = this.function(parameter1, parameter2, parameter3, parameter4);
        parameters.SetNextParameter(0, output.Item1);
        parameters.SetNextParameter(1, output.Item2);
        parameters.SetNextParameter(2, output.Item3);
        parameters.SetNextParameter(3, output.Item4);
    }
}
