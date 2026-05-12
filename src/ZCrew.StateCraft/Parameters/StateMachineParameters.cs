using System.Diagnostics;
using ZCrew.StateCraft.Parameters.Contracts;

namespace ZCrew.StateCraft.Parameters;

/// <inheritdoc />
internal class StateMachineParameters : IStateMachineParameters
{
    private readonly Parameters previousParameters = new("Previous");
    private readonly Parameters currentParameters = new("Current");
    private readonly Parameters nextParameters = new("Next");

    /// <inheritdoc />
    public bool IsPreviousSet => this.previousParameters.IsSet;

    /// <inheritdoc />
    public bool IsCurrentSet => this.currentParameters.IsSet;

    /// <inheritdoc />
    public bool IsNextSet => this.nextParameters.IsSet;

    /// <inheritdoc />
    public ReadOnlySpan<Type> PreviousParameterTypes => this.previousParameters.Types;

    /// <inheritdoc />
    public ReadOnlySpan<Type> CurrentParameterTypes => this.currentParameters.Types;

    /// <inheritdoc />
    public ReadOnlySpan<Type> NextParameterTypes => this.nextParameters.Types;

    /// <inheritdoc />
    public void SetEmptyNextParameters()
    {
        this.nextParameters.Set();
    }

    /// <inheritdoc />
    public void SetNextParameter<T>(T parameter)
    {
        this.nextParameters.Set(parameter);
    }

    /// <inheritdoc />
    public void SetNextParameters<T1, T2>(T1 parameter1, T2 parameter2)
    {
        this.nextParameters.Set(parameter1, parameter2);
    }

    /// <inheritdoc />
    public void SetNextParameters<T1, T2, T3>(T1 parameter1, T2 parameter2, T3 parameter3)
    {
        this.nextParameters.Set(parameter1, parameter2, parameter3);
    }

    /// <inheritdoc />
    public void SetNextParameters<T1, T2, T3, T4>(T1 parameter1, T2 parameter2, T3 parameter3, T4 parameter4)
    {
        this.nextParameters.Set(parameter1, parameter2, parameter3, parameter4);
    }

    /// <inheritdoc />
    public T GetPreviousParameter<T>()
    {
        return this.previousParameters.Get<T>();
    }

    /// <inheritdoc />
    public (T1, T2) GetPreviousParameters<T1, T2>()
    {
        return this.previousParameters.Get<T1, T2>();
    }

    /// <inheritdoc />
    public (T1, T2, T3) GetPreviousParameters<T1, T2, T3>()
    {
        return this.previousParameters.Get<T1, T2, T3>();
    }

    /// <inheritdoc />
    public (T1, T2, T3, T4) GetPreviousParameters<T1, T2, T3, T4>()
    {
        return this.previousParameters.Get<T1, T2, T3, T4>();
    }

    /// <inheritdoc />
    public T GetCurrentParameter<T>()
    {
        return this.currentParameters.Get<T>();
    }

    /// <inheritdoc />
    public (T1, T2) GetCurrentParameters<T1, T2>()
    {
        return this.currentParameters.Get<T1, T2>();
    }

    /// <inheritdoc />
    public (T1, T2, T3) GetCurrentParameters<T1, T2, T3>()
    {
        return this.currentParameters.Get<T1, T2, T3>();
    }

    /// <inheritdoc />
    public (T1, T2, T3, T4) GetCurrentParameters<T1, T2, T3, T4>()
    {
        return this.currentParameters.Get<T1, T2, T3, T4>();
    }

    /// <inheritdoc />
    public T GetNextParameter<T>()
    {
        return this.nextParameters.Get<T>();
    }

    /// <inheritdoc />
    public (T1, T2) GetNextParameters<T1, T2>()
    {
        return this.nextParameters.Get<T1, T2>();
    }

    /// <inheritdoc />
    public (T1, T2, T3) GetNextParameters<T1, T2, T3>()
    {
        return this.nextParameters.Get<T1, T2, T3>();
    }

    /// <inheritdoc />
    public (T1, T2, T3, T4) GetNextParameters<T1, T2, T3, T4>()
    {
        return this.nextParameters.Get<T1, T2, T3, T4>();
    }

    /// <inheritdoc />
    public void BeginTransition()
    {
        // Transfer from Current -> Previous, Next is empty
        this.currentParameters.TransferTo(this.previousParameters);
    }

    /// <inheritdoc />
    public void RollbackTransition()
    {
        // Transfer from Previous -> Current and clear Next
        this.previousParameters.TransferTo(this.currentParameters);
        this.nextParameters.Clear();
    }

    /// <inheritdoc />
    public void CommitTransition()
    {
        // Transfer from Next -> Current and clear Previous
        this.nextParameters.TransferTo(this.currentParameters);
        this.previousParameters.Clear();
    }

    /// <inheritdoc />
    public void Clear()
    {
        this.previousParameters.Clear();
        this.currentParameters.Clear();
        this.nextParameters.Clear();
    }

    [DebuggerDisplay("{name}: IsSet={IsSet}, Count={Count}")]
    private class Parameters
    {
        private readonly Type?[] types = new Type?[4];
        private readonly object?[] values = new object?[4];
        private readonly string name;

        public Parameters(string name)
        {
            this.name = name;
        }

        public int Count { get; private set; }

        public bool IsSet { get; private set; }

        public ReadOnlySpan<Type> Types
        {
            get
            {
                VerifySet();
                return this.types.AsSpan()[..Count]!;
            }
        }

        public void Set()
        {
            Count = 0;
            IsSet = true;
        }

        public void Set<T>(T value)
        {
            Count = 1;
            IsSet = true;
            this.values[0] = value;
            this.types[0] = typeof(T);
        }

        public void Set<T1, T2>(T1 value1, T2 value2)
        {
            Count = 2;
            IsSet = true;
            this.values[0] = value1;
            this.values[1] = value2;
            this.types[0] = typeof(T1);
            this.types[1] = typeof(T2);
        }

        public void Set<T1, T2, T3>(T1 value1, T2 value2, T3 value3)
        {
            Count = 3;
            IsSet = true;
            this.values[0] = value1;
            this.values[1] = value2;
            this.values[2] = value3;
            this.types[0] = typeof(T1);
            this.types[1] = typeof(T2);
            this.types[2] = typeof(T3);
        }

        public void Set<T1, T2, T3, T4>(T1 value1, T2 value2, T3 value3, T4 value4)
        {
            Count = 4;
            IsSet = true;
            this.values[0] = value1;
            this.values[1] = value2;
            this.values[2] = value3;
            this.values[3] = value4;
            this.types[0] = typeof(T1);
            this.types[1] = typeof(T2);
            this.types[2] = typeof(T3);
            this.types[3] = typeof(T4);
        }

        public T Get<T>()
        {
            VerifySet();
            VerifyParameterCount(1);
            return CastParameter<T>(this.values[0]);
        }

        public (T1, T2) Get<T1, T2>()
        {
            VerifySet();
            VerifyParameterCount(2);
            return (CastParameter<T1>(this.values[0]), CastParameter<T2>(this.values[1]));
        }

        public (T1, T2, T3) Get<T1, T2, T3>()
        {
            VerifySet();
            VerifyParameterCount(3);
            return (
                CastParameter<T1>(this.values[0]),
                CastParameter<T2>(this.values[1]),
                CastParameter<T3>(this.values[2])
            );
        }

        public (T1, T2, T3, T4) Get<T1, T2, T3, T4>()
        {
            VerifySet();
            VerifyParameterCount(4);
            return (
                CastParameter<T1>(this.values[0]),
                CastParameter<T2>(this.values[1]),
                CastParameter<T3>(this.values[2]),
                CastParameter<T4>(this.values[3])
            );
        }

        public void Clear()
        {
            if (Count != 0)
            {
                Array.Fill(this.types, null, 0, Count);
                Array.Fill(this.values, null, 0, Count);
                Count = 0;
            }

            IsSet = false;
        }

        public void TransferTo(Parameters other)
        {
            other.Count = Count;
            if (Count != 0)
            {
                Array.Copy(this.types, other.types, Count);
                Array.Copy(this.values, other.values, Count);

                Array.Fill(this.types, null, 0, Count);
                Array.Fill(this.values, null, 0, Count);
                Count = 0;
            }

            other.IsSet = true;
            IsSet = false;
        }

        private void VerifySet()
        {
            if (!IsSet)
            {
                throw new InvalidOperationException($"{this.name} parameters have not been set");
            }
        }

        private void VerifyParameterCount(int expectedCount)
        {
            if (Count != expectedCount)
            {
                throw new InvalidOperationException($"{this.name} parameter count is not {expectedCount}");
            }
        }

        [System.Runtime.CompilerServices.MethodImpl(
            System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining
        )]
        private static T CastParameter<T>(object? parameter)
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
}
