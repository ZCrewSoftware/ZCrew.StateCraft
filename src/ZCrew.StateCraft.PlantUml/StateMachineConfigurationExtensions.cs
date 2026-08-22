namespace ZCrew.StateCraft.PlantUml;

/// <summary>
///     Provides extensions on <see cref="IStateMachineConfiguration{TState, TTransition}"/> for producing a PlantUML
///     state diagram representation of a state machine.
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
        ///     Renders <paramref name="configuration"/> as a PlantUML state diagram string.
        /// </summary>
        /// <returns>The rendered PlantUML diagram.</returns>
        public string ToPlantUmlDiagram()
        {
            return configuration.ToPlantUmlDiagram(options: null);
        }

        /// <summary>
        ///     Renders <paramref name="configuration"/> as a PlantUML state diagram string.
        /// </summary>
        /// <param name="options">The PlantUML rendering options to apply.</param>
        /// <returns>The rendered PlantUML diagram.</returns>
        public string ToPlantUmlDiagram(PlantUmlOptions? options)
        {
            return configuration.ToPlantUmlDiagramCore(options ?? new PlantUmlOptions());
        }

        /// <summary>
        ///     Renders <paramref name="configuration"/> as a PlantUML state diagram string, building the options
        ///     inline via <paramref name="configureOptions"/>.
        /// </summary>
        /// <param name="configureOptions">
        ///     A callback that mutates a fresh <see cref="PlantUmlOptions"/> instance, or <see langword="null"/> to
        ///     use defaults.
        /// </param>
        /// <returns>The rendered PlantUML diagram.</returns>
        public string ToPlantUmlDiagram(Action<PlantUmlOptions>? configureOptions)
        {
            var options = new PlantUmlOptions();
            configureOptions?.Invoke(options);
            return configuration.ToPlantUmlDiagramCore(options);
        }

        private string ToPlantUmlDiagramCore(PlantUmlOptions options)
        {
            var stateMachine = configuration.GetInfo();
            using var stringWriter = new StringWriter();
            var plantUmlWriter = DefaultPlantUmlWriter.Instance; // TODO: allow overrides
            plantUmlWriter.Write(stateMachine, stringWriter, options);

            return stringWriter.ToString();
        }
    }
}
