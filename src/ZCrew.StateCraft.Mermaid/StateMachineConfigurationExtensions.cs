namespace ZCrew.StateCraft.Mermaid;

/// <summary>
///     Provides extensions on <see cref="IStateMachineConfiguration{TState, TTransition}"/> for producing a Mermaid
///     <c>stateDiagram-v2</c> representation of a state machine.
/// </summary>
public static class StateMachineConfigurationExtensions
{
    /// <param name="configuration">The state machine configuration to render.</param>
    /// <typeparam name="TState">The type of the state.</typeparam>
    /// <typeparam name="TTransition">The type of the transition.</typeparam>
    extension<TState, TTransition>(IStateMachineConfiguration<TState, TTransition> configuration)
        where TState : notnull
        where TTransition : notnull
    {
        /// <summary>
        ///     Renders <paramref name="configuration"/> as a Mermaid <c>stateDiagram-v2</c> diagram string.
        /// </summary>
        /// <returns>The rendered Mermaid diagram.</returns>
        public string ToMermaidDiagram()
        {
            return configuration.ToMermaidDiagram(options: null);
        }

        /// <summary>
        ///     Renders <paramref name="configuration"/> as a Mermaid <c>stateDiagram-v2</c> diagram string.
        /// </summary>
        /// <param name="options">The Mermaid rendering options to apply.</param>
        /// <returns>The rendered Mermaid diagram.</returns>
        public string ToMermaidDiagram(MermaidOptions? options)
        {
            return configuration.ToMermaidDiagramCore(options ?? new MermaidOptions());
        }

        /// <summary>
        ///     Renders <paramref name="configuration"/> as a Mermaid <c>stateDiagram-v2</c> diagram string, building
        ///     the options inline via <paramref name="configureOptions"/>.
        /// </summary>
        /// <param name="configureOptions">
        ///     A callback that mutates a fresh <see cref="MermaidOptions"/> instance, or <see langword="null"/> to use
        ///     defaults.
        /// </param>
        /// <returns>The rendered Mermaid diagram.</returns>
        public string ToMermaidDiagram(Action<MermaidOptions>? configureOptions)
        {
            var options = new MermaidOptions();
            configureOptions?.Invoke(options);
            return configuration.ToMermaidDiagramCore(options);
        }

        private string ToMermaidDiagramCore(MermaidOptions options)
        {
            var stateMachine = configuration.GetInfo();
            using var stringWriter = new StringWriter();
            var mermaidWriter = DefaultMermaidWriter.Instance; // TODO: allow overrides
            mermaidWriter.Write(stateMachine, stringWriter, options);

            return stringWriter.ToString();
        }
    }
}
