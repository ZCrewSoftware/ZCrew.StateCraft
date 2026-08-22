using System.Text;
using ZCrew.StateCraft.Identities;
using ZCrew.StateCraft.Identities.Extensions;
using ZCrew.StateCraft.Info.Extensions;

namespace ZCrew.StateCraft.PlantUml;

internal class DefaultPlantUmlWriter : IPlantUmlWriter
{
    public static readonly DefaultPlantUmlWriter Instance = new();

    public void Write<TState, TTransition>(
        IStateMachineInfo<TState, TTransition> stateMachine,
        TextWriter writer,
        PlantUmlOptions options
    )
        where TState : notnull
        where TTransition : notnull
    {
        var aliases = BuildAliases(stateMachine);

        writer.WriteLine("@startuml");
        writer.WriteLine("title State Machine");
        writer.WriteLine();
        writer.WriteLine(GetDirectionToken(options.Direction));
        writer.WriteLine();

        foreach (var state in stateMachine.States)
        {
            var label = state.ToDisplayString().EncodeForPlantUml(options);
            writer.WriteLine($"state \"{label}\" as {GetAlias(aliases, state)}");
        }

        var initialState = GetInitialState(stateMachine);
        if (stateMachine.States.Count > 0 && (initialState != null || stateMachine.Transitions.Count > 0))
        {
            writer.WriteLine();
        }

        if (initialState != null)
        {
            writer.WriteLine($"[*] --> {GetAlias(aliases, initialState)}");
        }

        foreach (var transition in stateMachine.Transitions)
        {
            WriteTransition(transition, aliases, writer, options);
        }

        writer.WriteLine("@enduml");
    }

    private static void WriteTransition<TState, TTransition>(
        ITransitionInfo<TState, TTransition> transition,
        Dictionary<IStateInfo<TState, TTransition>, string> aliases,
        TextWriter writer,
        PlantUmlOptions options
    )
        where TState : notnull
        where TTransition : notnull
    {
        var descriptor = transition.ToDisplayString().EncodeForPlantUml(options);

        // GetPreviousStates expands inverted transitions across every non-excluded state and throws on an
        // unrecognized variant, so no per-variant switch is needed here
        foreach (var previous in transition.GetPreviousStates())
        {
            foreach (var next in transition.GetNextStates())
            {
                WriteTransitionFromTo(previous, next, descriptor, aliases, writer, options);
            }
        }
    }

    private static void WriteTransitionFromTo<TState, TTransition>(
        IStateInfo<TState, TTransition> previous,
        IStateInfo<TState, TTransition> next,
        string descriptor,
        Dictionary<IStateInfo<TState, TTransition>, string> aliases,
        TextWriter writer,
        PlantUmlOptions options
    )
        where TState : notnull
        where TTransition : notnull
    {
        var conditions = default(List<IConditionInfo>);
        AppendConditions(previous);
        AppendConditions(next);

        writer.Write($"{GetAlias(aliases, previous)} --> {GetAlias(aliases, next)} : {descriptor}");
        if (conditions != null)
        {
            WriteConditions(conditions, writer, options);
        }
        writer.WriteLine();

        void AppendConditions(IStateInfo<TState, TTransition> state)
        {
            if (state is IConditionalStateInfo<TState, TTransition> { Conditions.Count: > 0 } conditionalState)
            {
                conditions ??= [];
                conditions.AddRange(conditionalState.Conditions);
            }
        }
    }

    private static void WriteConditions(
        IEnumerable<IConditionInfo> conditions,
        TextWriter writer,
        PlantUmlOptions options
    )
    {
        var first = true;
        foreach (var condition in conditions)
        {
            if (condition.Descriptor == null)
            {
                continue;
            }

            var prefix = first ? "If" : "And";
            writer.Write($"\\n{prefix}: {condition.Descriptor.EncodeForPlantUml(options)}");
            first = false;
        }
    }

    /// <summary>
    ///     Resolves the configured state the machine starts in, or <see langword="null"/> when no start marker can be
    ///     drawn. A dynamic initial state is only known once the machine activates, and an unvalidated configuration
    ///     can name a static initial state that was never configured.
    /// </summary>
    private static IStateInfo<TState, TTransition>? GetInitialState<TState, TTransition>(
        IStateMachineInfo<TState, TTransition> stateMachine
    )
        where TState : notnull
        where TTransition : notnull
    {
        if (stateMachine.InitialState is not IStaticInitialStateInfo<TState, TTransition> staticInitialState)
        {
            return null;
        }

        return stateMachine.GetStateOrDefault(
            staticInitialState.InitialStateValue,
            [.. staticInitialState.InitialParameterTypes]
        );
    }

    /// <summary>
    ///     Assigns every configured state a PlantUML-safe alias, in declaration order, suffixing on collision so two
    ///     distinct states never share a node.
    /// </summary>
    private static Dictionary<IStateInfo<TState, TTransition>, string> BuildAliases<TState, TTransition>(
        IStateMachineInfo<TState, TTransition> stateMachine
    )
        where TState : notnull
        where TTransition : notnull
    {
        var aliases = new Dictionary<IStateInfo<TState, TTransition>, string>(
            StateIdentityEqualityComparer<TState>.Instance
        );
        var usedAliases = new HashSet<string>(StringComparer.Ordinal);

        foreach (var state in stateMachine.States)
        {
            var alias = BuildAlias(state);
            if (!usedAliases.Add(alias))
            {
                var suffix = 2;
                string candidate;
                do
                {
                    candidate = $"{alias}_{suffix++}";
                } while (!usedAliases.Add(candidate));

                alias = candidate;
            }

            aliases[state] = alias;
        }

        return aliases;
    }

    private static string GetAlias<TState, TTransition>(
        Dictionary<IStateInfo<TState, TTransition>, string> aliases,
        IStateInfo<TState, TTransition> state
    )
        where TState : notnull
        where TTransition : notnull
    {
        // A transition can reference a state that was never configured; PlantUML still renders the node, so fall back
        // to the unreserved alias rather than failing the whole diagram
        return aliases.TryGetValue(state, out var alias) ? alias : BuildAlias(state);
    }

    /// <summary>
    ///     Builds the unreserved alias for a state: its value followed by each parameter type, reduced to characters
    ///     PlantUML accepts in an alias.
    /// </summary>
    private static string BuildAlias<TState>(IStateIdentity<TState> state)
        where TState : notnull
    {
        var builder = new StringBuilder();
        AppendSanitized(state.StateValue.ToString());
        foreach (var parameterType in state.StateParameterTypes)
        {
            builder.Append('_');
            AppendSanitized(parameterType.FriendlyName);
        }

        if (builder.Length == 0)
        {
            return "_";
        }

        // A PlantUML alias cannot start with a digit
        return char.IsAsciiDigit(builder[0]) ? builder.Insert(0, '_').ToString() : builder.ToString();

        void AppendSanitized(string? text)
        {
            foreach (var c in text ?? string.Empty)
            {
                builder.Append(char.IsAsciiLetterOrDigit(c) ? c : '_');
            }
        }
    }

    private static string GetDirectionToken(PlantUmlDirection direction)
    {
        return direction switch
        {
            PlantUmlDirection.TopToBottom => "top to bottom direction",
            PlantUmlDirection.LeftToRight => "left to right direction",
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, "Unknown PlantUML direction."),
        };
    }
}
