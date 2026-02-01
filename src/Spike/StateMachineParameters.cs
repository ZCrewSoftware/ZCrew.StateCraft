using Spike.Contracts;

namespace Spike;

public class StateMachineParameters : IStateMachineParameters
{
    private readonly object?[] previousParameters;
    private readonly HashSet<int> previousParametersSet;

    private readonly object?[] nextParameters;
    private readonly HashSet<int> nextParametersSet;

    public StateMachineParameters(object?[] previousParameters, object?[] nextParameters)
    {
        this.previousParameters = previousParameters;
        this.previousParametersSet = new HashSet<int>(Enumerable.Range(0, previousParameters.Length));
        this.nextParameters = nextParameters;
        this.nextParametersSet = new HashSet<int>(Enumerable.Range(0, nextParameters.Length));
    }

    public void SetPreviousParameter<T>(int index, T value)
    {
        this.previousParameters[index] = value;
        this.previousParametersSet.Add(index);
    }

    public T GetPreviousParameter<T>(int index)
    {
        if (!this.previousParametersSet.Contains(index))
        {
            throw new ArgumentException($"Previous parameter {index} has not been set");
        }
        var parameter = this.previousParameters[index];
        if (parameter is T typedParameter)
        {
            return typedParameter;
        }
        if (parameter is null)
        {
            return default!;
        }
        throw new ArgumentException($"Previous parameter {index} is not of type {typeof(T)}");
    }

    public void SetNextParameter<T>(int index, T value)
    {
        this.nextParameters[index] = value;
        this.nextParametersSet.Add(index);
    }

    public T GetNextParameter<T>(int index)
    {
        if (!this.nextParametersSet.Contains(index))
        {
            throw new ArgumentException($"Next parameter {index} has not been set");
        }
        var parameter = this.nextParameters[index];
        if (parameter is T typedParameter)
        {
            return typedParameter;
        }
        if (parameter is null)
        {
            return default!;
        }
        throw new ArgumentException($"Next parameter {index} is not of type {typeof(T)}");
    }
}
