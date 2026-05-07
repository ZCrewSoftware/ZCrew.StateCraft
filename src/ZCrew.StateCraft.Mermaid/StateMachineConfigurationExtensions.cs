using ZCrew.StateCraft.Rendering;

namespace ZCrew.StateCraft.Mermaid;

/// <summary>
///     Provides extensions on <see cref="IStateMachineConfiguration{TState, TTransition}"/> for producing a Mermaid
///     <c>stateDiagram-v2</c> representation of a state machine.
/// </summary>
public static class StateMachineConfigurationExtensions
{
    /// <summary>
    ///     Renders <paramref name="configuration"/> as a Mermaid <c>stateDiagram-v2</c> diagram string.
    /// </summary>
    /// <typeparam name="TState">The type of the state.</typeparam>
    /// <typeparam name="TTransition">The type of the transition.</typeparam>
    /// <param name="configuration">The state machine configuration to render.</param>
    /// <returns>The rendered Mermaid diagram.</returns>
    public static string ToMermaidDiagram<TState, TTransition>(
        this IStateMachineConfiguration<TState, TTransition> configuration
    )
        where TState : notnull
        where TTransition : notnull
    {
        using var stringWriter = new StringWriter();

        configuration.ToMermaidDiagram(stringWriter, new MermaidOptions());
        return stringWriter.ToString();
    }

    /// <summary>
    ///     Renders <paramref name="configuration"/> as a Mermaid <c>stateDiagram-v2</c> diagram string.
    /// </summary>
    /// <typeparam name="TState">The type of the state.</typeparam>
    /// <typeparam name="TTransition">The type of the transition.</typeparam>
    /// <param name="configuration">The state machine configuration to render.</param>
    /// <param name="options">The Mermaid rendering options to apply.</param>
    /// <returns>The rendered Mermaid diagram.</returns>
    public static string ToMermaidDiagram<TState, TTransition>(
        this IStateMachineConfiguration<TState, TTransition> configuration,
        MermaidOptions? options
    )
        where TState : notnull
        where TTransition : notnull
    {
        using var stringWriter = new StringWriter();
        options ??= new MermaidOptions();

        configuration.ToMermaidDiagram(stringWriter, options);
        return stringWriter.ToString();
    }

    /// <summary>
    ///     Renders <paramref name="configuration"/> as a Mermaid <c>stateDiagram-v2</c> diagram string, building the
    ///     options inline via <paramref name="configureOptions"/>.
    /// </summary>
    /// <typeparam name="TState">The type of the state.</typeparam>
    /// <typeparam name="TTransition">The type of the transition.</typeparam>
    /// <param name="configuration">The state machine configuration to render.</param>
    /// <param name="configureOptions">
    ///     A callback that mutates a fresh <see cref="MermaidOptions"/> instance, or <see langword="null"/> to use
    ///     defaults.
    /// </param>
    /// <returns>The rendered Mermaid diagram.</returns>
    public static string ToMermaidDiagram<TState, TTransition>(
        this IStateMachineConfiguration<TState, TTransition> configuration,
        Action<MermaidOptions>? configureOptions
    )
        where TState : notnull
        where TTransition : notnull
    {
        using var stringWriter = new StringWriter();
        var options = new MermaidOptions();
        configureOptions?.Invoke(options);

        configuration.ToMermaidDiagram(stringWriter, options);
        return stringWriter.ToString();
    }

    private static void ToMermaidDiagram<TState, TTransition>(
        this IStateMachineConfiguration<TState, TTransition> configuration,
        TextWriter writer,
        MermaidOptions options
    )
        where TState : notnull
        where TTransition : notnull
    {
        var rendering = StateMachineRenderingContext.Render(configuration);
        var title = rendering.StateMachine?.Descriptor.EncodeForMermaid(options) ?? string.Empty;

        writer.WriteLine("---");
        writer.WriteLine($"title: {title}");
        writer.WriteLine("---");
        writer.WriteLine("stateDiagram-v2");
        writer.WriteLine($"    direction {GetDirectionToken(options.Direction)}");
        writer.WriteLine();

        foreach (var state in rendering.States)
        {
            writer.WriteLine($"    {state.Identifier}: {state.Descriptor.EncodeForMermaid(options)}");
        }

        if (rendering.States.Count > 0 && rendering.Transitions.Count > 0)
        {
            writer.WriteLine();
        }

        foreach (var transition in rendering.Transitions)
        {
            var descriptor = transition.Descriptor.EncodeForMermaid(options);
            writer.Write($"    {transition.PreviousState} --> {transition.NextState} : {descriptor}");
            WriteConditions(transition.Conditions, writer, options);
            writer.WriteLine();
        }
    }

    private static void WriteConditions(IReadOnlyList<string> conditions, TextWriter writer, MermaidOptions options)
    {
        if (conditions.Count == 0)
        {
            return;
        }

        for (var i = 0; i < conditions.Count; i++)
        {
            var prefix = i == 0 ? "If" : "And";
            writer.Write($" <br/> {prefix}: {conditions[i].EncodeForMermaid(options)}");
        }
    }

    private static string GetDirectionToken(MermaidDirection direction)
    {
        return direction switch
        {
            MermaidDirection.TopToBottom => "TB",
            MermaidDirection.LeftToRight => "LR",
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, "Unknown Mermaid direction."),
        };
    }
}
