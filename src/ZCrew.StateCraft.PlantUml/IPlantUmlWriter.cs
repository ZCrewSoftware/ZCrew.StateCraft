namespace ZCrew.StateCraft.PlantUml;

// TODO: make public and allow overriding options
/// <summary>
///     Utility responsible for converting a <see cref="IStateMachineInfo{TState, TTransition}"/> into a PlantUML
///     diagram and writing it to a stream.
/// </summary>
internal interface IPlantUmlWriter
{
    /// <summary>
    ///     Writes the <paramref name="stateMachine"/> to the <paramref name="writer"/> based on the configured
    ///     <paramref name="options"/>.
    /// </summary>
    /// <param name="stateMachine">The state machine info.</param>
    /// <param name="writer">The writer to output to.</param>
    /// <param name="options">The configured PlantUML options.</param>
    /// <typeparam name="TState">The type of the state.</typeparam>
    /// <typeparam name="TTransition">The type of the transition.</typeparam>
    void Write<TState, TTransition>(
        IStateMachineInfo<TState, TTransition> stateMachine,
        TextWriter writer,
        PlantUmlOptions options
    )
        where TState : notnull
        where TTransition : notnull;
}
