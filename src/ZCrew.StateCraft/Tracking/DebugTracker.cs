using System.Diagnostics;
using ZCrew.StateCraft.Tracking.Contracts;
using ZCrew.StateCraft.Transitions.Contracts;

namespace ZCrew.StateCraft.Tracking;

/// <inheritdoc />
[DebuggerDisplay("Count={Count}")]
internal class DebugTracker<TState, TTransition> : ITracker<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    private readonly LinkedList<string> records = [];

    public int Count => this.records.Count;

    /// <inheritdoc />
    public void Activated(IState<TState, TTransition> initialState)
    {
        AddRecord($"Activated at {initialState}");
    }

    /// <inheritdoc />
    public void Activated<T>(IState<TState, TTransition> initialState, T initialParameter)
    {
        AddRecord($"Activated at {initialState}");
    }

    /// <inheritdoc />
    public void Deactivated(IState<TState, TTransition> finalState)
    {
        AddRecord($"Deactivated at {finalState}");
    }

    /// <inheritdoc />
    public void Deactivated<T>(IState<TState, TTransition> finalState, T parameter)
    {
        AddRecord($"Deactivated at {finalState}");
    }

    /// <inheritdoc />
    public void Transitioned(ITransition<TState, TTransition> transition)
    {
        AddRecord($"Performing {transition}");
    }

    /// <inheritdoc />
    public void Transitioned<T>(ITransition<TState, TTransition> transition, T parameter)
    {
        AddRecord($"Performing {transition}");
    }

    /// <inheritdoc />
    public void Entered(IState<TState, TTransition> state)
    {
        AddRecord($"Entered {state}");
    }

    /// <inheritdoc />
    public void Entered<T>(IState<TState, TTransition> state, T parameter)
    {
        AddRecord($"Entered {state}");
    }

    /// <inheritdoc />
    public void Exited(IState<TState, TTransition> state)
    {
        AddRecord($"Exited {state}");
    }

    /// <inheritdoc />
    public void Exited<T>(IState<TState, TTransition> state, T parameter)
    {
        AddRecord($"Exited {state}");
    }

    private void AddRecord(string message)
    {
        var record = $"[{DateTime.Now:O}]: {message}";
        this.records.AddLast(record);

        // Limit the records to just a constant value for now
        if (this.records.Count > 100)
        {
            this.records.RemoveFirst();
        }
    }
}
