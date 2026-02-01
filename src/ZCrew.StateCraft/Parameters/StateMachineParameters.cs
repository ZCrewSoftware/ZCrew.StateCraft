using ZCrew.StateCraft.Parameters.Contracts;

namespace ZCrew.StateCraft.Parameters;

/// <inheritdoc />
internal class StateMachineParameters : IStateMachineParameters
{
    private const int MaxParameterCount = 4;

    private readonly object?[] previousParameters = new object?[MaxParameterCount];
    private int previousParameterCount;

    private readonly object?[] currentParameters = new object?[MaxParameterCount];
    private int currentParameterCount;

    private readonly object?[] nextParameters = new object?[MaxParameterCount];
    private int nextParameterCount;
    private bool nextParametersSet;

    /// <inheritdoc />
    public T GetPreviousParameter<T>(int index)
    {
        if (index < 0 || index >= this.previousParameterCount)
        {
            throw new ArgumentOutOfRangeException(nameof(index), index, "Invalid previous parameter index");
        }
        return CastParameter<T>(this.previousParameters[index]);
    }

    /// <inheritdoc />
    public T GetCurrentParameter<T>(int index)
    {
        if (index < 0 || index >= this.currentParameterCount)
        {
            throw new ArgumentException($"Current parameter {index} has not been set");
        }
        return CastParameter<T>(this.currentParameters[index]);
    }

    /// <inheritdoc />
    public T GetNextParameter<T>(int index)
    {
        if (index < 0 || index >= this.nextParameterCount)
        {
            throw new ArgumentException($"Next parameter {index} has not been set");
        }
        return CastParameter<T>(this.nextParameters[index]);
    }

    /// <inheritdoc />
    public void SetNextParameters(object?[] nextParameters)
    {
        if (nextParameters.Length > MaxParameterCount)
        {
            throw new ArgumentException($"Setting {nextParameters.Length} next parameters exceeds parameter limit");
        }
        this.nextParametersSet = true;
        this.nextParameterCount = nextParameters.Length;

        CopyTo(nextParameters, this.nextParameters);
    }

    /// <inheritdoc />
    public void BeginTransition()
    {
        // Transfer from Current -> Previous, Next is empty
        this.previousParameterCount = this.currentParameterCount;
        this.currentParameterCount = 0;

        CopyTo(this.currentParameters, this.previousParameters);
        Clear(this.currentParameters);
    }

    /// <inheritdoc />
    public void RollbackTransition()
    {
        // Transfer from Previous -> Current
        this.currentParameterCount = this.previousParameterCount;
        this.previousParameterCount = 0;

        CopyTo(this.previousParameters, this.currentParameters);
        Clear(this.previousParameters);

        // Clear Next
        this.nextParameterCount = 0;
        this.nextParametersSet = false;
        Clear(this.nextParameters);
    }

    /// <inheritdoc />
    public void CommitTransition()
    {
        // Transfer from Next -> Current
        this.currentParameterCount = this.nextParameterCount;
        this.nextParameterCount = 0;
        this.nextParametersSet = false;

        CopyTo(this.nextParameters, this.currentParameters);
        Clear(this.nextParameters);

        // Clear Previous
        this.previousParameterCount = 0;
        Clear(this.previousParameters);
    }

    /// <inheritdoc />
    public bool CanCommitTransition()
    {
        return this.nextParametersSet;
    }

    /// <inheritdoc />
    public void Clear()
    {
        this.previousParameterCount = 0;
        this.currentParameterCount = 0;
        this.nextParameterCount = 0;
        this.nextParametersSet = false;

        Clear(this.previousParameters);
        Clear(this.currentParameters);
        Clear(this.nextParameters);
    }

    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    private static void CopyTo(object?[] sourceArray, object?[] destinationArray)
    {
        Array.Copy(sourceArray, destinationArray, sourceArray.Length);
        Array.Fill(destinationArray, null, sourceArray.Length, MaxParameterCount - sourceArray.Length);
    }

    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    private static void Clear(object?[] array)
    {
        Array.Fill(array, null);
    }

    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    private T CastParameter<T>(object? parameter)
    {
        if (parameter is T typedParameter)
        {
            return typedParameter;
        }
        if (parameter is null)
        {
            return default!;
        }

        throw new ArgumentException($"Parameter {parameter.GetType()} is not of type {typeof(T)}");
    }
}
