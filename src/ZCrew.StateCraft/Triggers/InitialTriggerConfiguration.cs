namespace ZCrew.StateCraft.Triggers;

/// <inheritdoc />
internal class InitialTriggerConfiguration<TState, TTransition> : IInitialTriggerConfiguration<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    /// <inheritdoc />
    public IScheduledTriggerConfiguration<TState, TTransition> Once()
    {
        return new RunOnceTriggerConfiguration<TState, TTransition>();
    }

    /// <inheritdoc />
    public IScheduledTriggerConfiguration<TState, TTransition> Repeat()
    {
        return new RepeatingTriggerConfiguration<TState, TTransition>();
    }
}
