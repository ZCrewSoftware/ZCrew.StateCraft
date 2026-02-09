using ZCrew.StateCraft.Parameters.Contracts;

namespace ZCrew.StateCraft.Parameters;

/// <inheritdoc />
internal class StateMachineParameters : IStateMachineParameters
{
    private const int MaxParameterCount = 4;

    private readonly Type?[] previousParameterTypes = new Type?[MaxParameterCount];
    private readonly object?[] previousParameters = new object?[MaxParameterCount];
    private int previousParameterCount;

    private readonly Type?[] currentParameterTypes = new Type?[MaxParameterCount];
    private readonly object?[] currentParameters = new object?[MaxParameterCount];
    private int currentParameterCount;

    private readonly Type?[] nextParameterTypes = new Type?[MaxParameterCount];
    private readonly object?[] nextParameters = new object?[MaxParameterCount];
    private int nextParameterCount;

    /// <inheritdoc />
    public StateMachineParametersFlags Status { get; private set; }

    /// <inheritdoc />
    public IReadOnlyList<Type> PreviousParameterTypes
    {
        get
        {
            if (!Status.HasFlag(StateMachineParametersFlags.PreviousParametersSet))
            {
                throw new InvalidOperationException("Previous parameters have not been set");
            }

            return this.previousParameterTypes.Take(this.previousParameterCount).OfType<Type>().ToArray();
        }
    }

    /// <inheritdoc />
    public IReadOnlyList<Type> CurrentParameterTypes
    {
        get
        {
            if (!Status.HasFlag(StateMachineParametersFlags.CurrentParametersSet))
            {
                throw new InvalidOperationException("Current parameters have not been set");
            }

            return this.currentParameterTypes.Take(this.currentParameterCount).OfType<Type>().ToArray();
        }
    }

    /// <inheritdoc />
    public IReadOnlyList<Type> NextParameterTypes
    {
        get
        {
            if (!Status.HasFlag(StateMachineParametersFlags.NextParametersSet))
            {
                throw new InvalidOperationException("Next parameters have not been set");
            }

            return this.nextParameterTypes.Take(this.nextParameterCount).OfType<Type>().ToArray();
        }
    }

    /// <inheritdoc />
    public void SetEmptyNextParameters()
    {
        Status |= StateMachineParametersFlags.NextParametersSet;
        this.nextParameterCount = 0;
        Clear(this.nextParameters);
        Clear(this.nextParameterTypes);
    }

    /// <inheritdoc />
    public void SetNextParameter<T>(T nextParameter)
    {
        Status |= StateMachineParametersFlags.NextParametersSet;
        this.nextParameterCount = 1;

        this.nextParameters[0] = nextParameter;
        this.nextParameterTypes[0] = typeof(T);
    }

    /// <inheritdoc />
    public void SetNextParameters<T1, T2>(T1 parameter1, T2 parameter2)
    {
        Status |= StateMachineParametersFlags.NextParametersSet;
        this.nextParameterCount = 2;

        this.nextParameters[0] = parameter1;
        this.nextParameters[1] = parameter2;
        this.nextParameterTypes[0] = typeof(T1);
        this.nextParameterTypes[1] = typeof(T2);
    }

    /// <inheritdoc />
    public void SetNextParameters<T1, T2, T3>(T1 parameter1, T2 parameter2, T3 parameter3)
    {
        Status |= StateMachineParametersFlags.NextParametersSet;
        this.nextParameterCount = 3;

        this.nextParameters[0] = parameter1;
        this.nextParameters[1] = parameter2;
        this.nextParameters[2] = parameter3;
        this.nextParameterTypes[0] = typeof(T1);
        this.nextParameterTypes[1] = typeof(T2);
        this.nextParameterTypes[2] = typeof(T3);
    }

    /// <inheritdoc />
    public void SetNextParameters<T1, T2, T3, T4>(T1 parameter1, T2 parameter2, T3 parameter3, T4 parameter4)
    {
        Status |= StateMachineParametersFlags.NextParametersSet;
        this.nextParameterCount = 4;

        this.nextParameters[0] = parameter1;
        this.nextParameters[1] = parameter2;
        this.nextParameters[2] = parameter3;
        this.nextParameters[3] = parameter4;
        this.nextParameterTypes[0] = typeof(T1);
        this.nextParameterTypes[1] = typeof(T2);
        this.nextParameterTypes[2] = typeof(T3);
        this.nextParameterTypes[3] = typeof(T4);
    }

    /// <inheritdoc />
    public T GetPreviousParameter<T>()
    {
        if (!Status.HasFlag(StateMachineParametersFlags.PreviousParametersSet))
        {
            throw new InvalidOperationException("Previous parameters have not been set");
        }
        if (this.previousParameterCount < 1)
        {
            throw new InvalidOperationException("Previous parameter count is less than 1");
        }
        return CastParameter<T>(this.previousParameters[0]);
    }

    /// <inheritdoc />
    public (T1, T2) GetPreviousParameters<T1, T2>()
    {
        if (!Status.HasFlag(StateMachineParametersFlags.PreviousParametersSet))
        {
            throw new InvalidOperationException("Previous parameters have not been set");
        }
        if (this.previousParameterCount < 2)
        {
            throw new InvalidOperationException("Previous parameter count is less than 2");
        }
        return (CastParameter<T1>(this.previousParameters[0]), CastParameter<T2>(this.previousParameters[1]));
    }

    /// <inheritdoc />
    public (T1, T2, T3) GetPreviousParameters<T1, T2, T3>()
    {
        if (!Status.HasFlag(StateMachineParametersFlags.PreviousParametersSet))
        {
            throw new InvalidOperationException("Previous parameters have not been set");
        }
        if (this.previousParameterCount < 3)
        {
            throw new InvalidOperationException("Previous parameter count is less than 3");
        }
        return (
            CastParameter<T1>(this.previousParameters[0]),
            CastParameter<T2>(this.previousParameters[1]),
            CastParameter<T3>(this.previousParameters[2])
        );
    }

    /// <inheritdoc />
    public (T1, T2, T3, T4) GetPreviousParameters<T1, T2, T3, T4>()
    {
        if (!Status.HasFlag(StateMachineParametersFlags.PreviousParametersSet))
        {
            throw new InvalidOperationException("Previous parameters have not been set");
        }
        if (this.previousParameterCount < 4)
        {
            throw new InvalidOperationException("Previous parameter count is less than 4");
        }
        return (
            CastParameter<T1>(this.previousParameters[0]),
            CastParameter<T2>(this.previousParameters[1]),
            CastParameter<T3>(this.previousParameters[2]),
            CastParameter<T4>(this.previousParameters[3])
        );
    }

    /// <inheritdoc />
    public T GetCurrentParameter<T>()
    {
        if (!Status.HasFlag(StateMachineParametersFlags.CurrentParametersSet))
        {
            throw new InvalidOperationException("Current parameters have not been set");
        }
        if (this.currentParameterCount < 1)
        {
            throw new InvalidOperationException("Current parameter count is less than 1");
        }
        return CastParameter<T>(this.currentParameters[0]);
    }

    /// <inheritdoc />
    public (T1, T2) GetCurrentParameters<T1, T2>()
    {
        if (!Status.HasFlag(StateMachineParametersFlags.CurrentParametersSet))
        {
            throw new InvalidOperationException("Current parameters have not been set");
        }
        if (this.currentParameterCount < 2)
        {
            throw new InvalidOperationException("Current parameter count is less than 2");
        }
        return (CastParameter<T1>(this.currentParameters[0]), CastParameter<T2>(this.currentParameters[1]));
    }

    /// <inheritdoc />
    public (T1, T2, T3) GetCurrentParameters<T1, T2, T3>()
    {
        if (!Status.HasFlag(StateMachineParametersFlags.CurrentParametersSet))
        {
            throw new InvalidOperationException("Current parameters have not been set");
        }
        if (this.currentParameterCount < 3)
        {
            throw new InvalidOperationException("Current parameter count is less than 3");
        }
        return (
            CastParameter<T1>(this.currentParameters[0]),
            CastParameter<T2>(this.currentParameters[1]),
            CastParameter<T3>(this.currentParameters[2])
        );
    }

    /// <inheritdoc />
    public (T1, T2, T3, T4) GetCurrentParameters<T1, T2, T3, T4>()
    {
        if (!Status.HasFlag(StateMachineParametersFlags.CurrentParametersSet))
        {
            throw new InvalidOperationException("Current parameters have not been set");
        }
        if (this.currentParameterCount < 4)
        {
            throw new InvalidOperationException("Current parameter count is less than 4");
        }
        return (
            CastParameter<T1>(this.currentParameters[0]),
            CastParameter<T2>(this.currentParameters[1]),
            CastParameter<T3>(this.currentParameters[2]),
            CastParameter<T4>(this.currentParameters[3])
        );
    }

    /// <inheritdoc />
    public T GetNextParameter<T>()
    {
        if (!Status.HasFlag(StateMachineParametersFlags.NextParametersSet))
        {
            throw new InvalidOperationException("Next parameters have not been set");
        }
        if (this.nextParameterCount < 1)
        {
            throw new InvalidOperationException("Next parameter count is less than 1");
        }
        return CastParameter<T>(this.nextParameters[0]);
    }

    /// <inheritdoc />
    public (T1, T2) GetNextParameters<T1, T2>()
    {
        if (!Status.HasFlag(StateMachineParametersFlags.NextParametersSet))
        {
            throw new InvalidOperationException("Next parameters have not been set");
        }
        if (this.nextParameterCount < 2)
        {
            throw new InvalidOperationException("Next parameter count is less than 2");
        }
        return (CastParameter<T1>(this.nextParameters[0]), CastParameter<T2>(this.nextParameters[1]));
    }

    /// <inheritdoc />
    public (T1, T2, T3) GetNextParameters<T1, T2, T3>()
    {
        if (!Status.HasFlag(StateMachineParametersFlags.NextParametersSet))
        {
            throw new InvalidOperationException("Next parameters have not been set");
        }
        if (this.nextParameterCount < 3)
        {
            throw new InvalidOperationException("Next parameter count is less than 3");
        }
        return (
            CastParameter<T1>(this.nextParameters[0]),
            CastParameter<T2>(this.nextParameters[1]),
            CastParameter<T3>(this.nextParameters[2])
        );
    }

    /// <inheritdoc />
    public (T1, T2, T3, T4) GetNextParameters<T1, T2, T3, T4>()
    {
        if (!Status.HasFlag(StateMachineParametersFlags.NextParametersSet))
        {
            throw new InvalidOperationException("Next parameters have not been set");
        }
        if (this.nextParameterCount < 4)
        {
            throw new InvalidOperationException("Next parameter count is less than 4");
        }
        return (
            CastParameter<T1>(this.nextParameters[0]),
            CastParameter<T2>(this.nextParameters[1]),
            CastParameter<T3>(this.nextParameters[2]),
            CastParameter<T4>(this.nextParameters[3])
        );
    }

    /// <inheritdoc />
    public void BeginTransition()
    {
        // Transfer from Current -> Previous, Next is empty
        Status |= StateMachineParametersFlags.PreviousParametersSet;
        Status &= ~StateMachineParametersFlags.CurrentParametersSet;
        this.previousParameterCount = this.currentParameterCount;
        this.currentParameterCount = 0;

        CopyTo(this.currentParameters, this.previousParameters);
        Clear(this.currentParameters);

        CopyTo(this.currentParameterTypes, this.previousParameterTypes);
        Clear(this.currentParameterTypes);
    }

    /// <inheritdoc />
    public void RollbackTransition()
    {
        // Transfer from Previous -> Current
        Status |= StateMachineParametersFlags.CurrentParametersSet;
        Status &= ~StateMachineParametersFlags.PreviousParametersSet;
        this.currentParameterCount = this.previousParameterCount;
        this.previousParameterCount = 0;

        CopyTo(this.previousParameters, this.currentParameters);
        Clear(this.previousParameters);

        CopyTo(this.previousParameterTypes, this.currentParameterTypes);
        Clear(this.previousParameterTypes);

        // Clear Next
        Status &= ~StateMachineParametersFlags.NextParametersSet;
        this.nextParameterCount = 0;
        Clear(this.nextParameters);
        Clear(this.nextParameterTypes);
    }

    /// <inheritdoc />
    public void CommitTransition()
    {
        // Transfer from Next -> Current
        Status |= StateMachineParametersFlags.CurrentParametersSet;
        Status &= ~StateMachineParametersFlags.NextParametersSet;
        this.currentParameterCount = this.nextParameterCount;
        this.nextParameterCount = 0;

        CopyTo(this.nextParameters, this.currentParameters);
        Clear(this.nextParameters);

        CopyTo(this.nextParameterTypes, this.currentParameterTypes);
        Clear(this.nextParameterTypes);

        // Clear Previous
        Status &= ~StateMachineParametersFlags.PreviousParametersSet;
        this.previousParameterCount = 0;
        Clear(this.previousParameters);
        Clear(this.previousParameterTypes);
    }

    /// <inheritdoc />
    public void Clear()
    {
        this.previousParameterCount = 0;
        this.currentParameterCount = 0;
        this.nextParameterCount = 0;

        Status &= ~StateMachineParametersFlags.PreviousParametersSet;
        Status &= ~StateMachineParametersFlags.CurrentParametersSet;
        Status &= ~StateMachineParametersFlags.NextParametersSet;

        Clear(this.previousParameters);
        Clear(this.currentParameters);
        Clear(this.nextParameters);

        Clear(this.previousParameterTypes);
        Clear(this.currentParameterTypes);
        Clear(this.nextParameterTypes);
    }

    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    private static void CopyTo<T>(T[] sourceArray, T[] destinationArray)
    {
        Array.Copy(sourceArray, destinationArray, sourceArray.Length);
        Array.Fill(destinationArray, default, sourceArray.Length, MaxParameterCount - sourceArray.Length);
    }

    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    private static void Clear<T>(T[] array)
    {
        Array.Fill(array, default);
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

        throw new InvalidCastException($"Cannot cast {parameter} to type {typeof(T)}");
    }
}
