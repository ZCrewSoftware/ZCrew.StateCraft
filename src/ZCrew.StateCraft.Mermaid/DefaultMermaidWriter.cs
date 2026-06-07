using System.Text;
using ZCrew.StateCraft.Identities;
using ZCrew.StateCraft.Identities.Extensions;

namespace ZCrew.StateCraft.Mermaid;

internal class DefaultMermaidWriter : IMermaidWriter
{
    public static readonly DefaultMermaidWriter Instance = new();

    public void Write<TState, TTransition>(
        IStateMachineInfo<TState, TTransition> stateMachine,
        TextWriter writer,
        MermaidOptions options
    )
        where TState : notnull
        where TTransition : notnull
    {
        writer.WriteLine("---");
        writer.WriteLine("title: State Machine");
        writer.WriteLine("---");
        writer.WriteLine("stateDiagram-v2");
        writer.WriteLine($"    direction {GetDirectionToken(options.Direction)}");
        writer.WriteLine();

        foreach (var state in stateMachine.States)
        {
            var identifier = GetStateIdentifier(state);
            writer.WriteLine($"    {identifier}: {state.ToDisplayString().EncodeForMermaid(options)}");
        }

        if (stateMachine.States.Count > 0 && stateMachine.Transitions.Count > 0)
        {
            writer.WriteLine();
        }

        foreach (var transition in stateMachine.Transitions)
        {
            WriteTransition(stateMachine, transition, writer, options);
        }
    }

    private void WriteTransition<TState, TTransition>(
        IStateMachineInfo<TState, TTransition> stateMachine,
        ITransitionInfo<TState, TTransition> transition,
        TextWriter writer,
        MermaidOptions options
    )
        where TState : notnull
        where TTransition : notnull
    {
        var descriptor = transition.ToDisplayString().EncodeForMermaid(options);

        switch (transition)
        {
            case IDirectTransitionInfo<TState, TTransition> directTransition:
                WriteTransitionFromTo(
                    directTransition.PreviousState,
                    directTransition.NextState,
                    descriptor,
                    writer,
                    options
                );
                break;

            case IMappedTransitionInfo<TState, TTransition> mappedTransition:
                WriteTransitionFromTo(
                    mappedTransition.PreviousState,
                    mappedTransition.NextState,
                    descriptor,
                    writer,
                    options
                );
                break;

            case IFromTransitionInfo<TState, TTransition> fromTransition:
                // Use except by here so 'fromStates' is still a IStateInfo
                var fromStates = stateMachine.States.ExceptBy(
                    fromTransition.ExcludedStates,
                    state => state,
                    StateIdentityEqualityComparer<TState>.Instance
                );
                foreach (var fromState in fromStates)
                {
                    WriteTransitionFromTo(fromState, fromTransition.NextState, descriptor, writer, options);
                }
                break;
        }
    }

    private void WriteTransitionFromTo<TState, TTransition>(
        IStateInfo<TState, TTransition> previous,
        IStateInfo<TState, TTransition> next,
        string descriptor,
        TextWriter writer,
        MermaidOptions options
    )
        where TState : notnull
        where TTransition : notnull
    {
        var conditions = default(List<IConditionInfo>);
        AppendConditions(previous);
        AppendConditions(next);

        var previousStateIdentifier = GetStateIdentifier(previous).EncodeForMermaid(options);
        var nextStateIdentifier = GetStateIdentifier(next).EncodeForMermaid(options);

        writer.Write($"    {previousStateIdentifier} --> {nextStateIdentifier} : {descriptor}");
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

    private static string GetStateIdentifier<TState>(IStateIdentity<TState> state)
        where TState : notnull
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.Append(state.StateValue);
        foreach (var parameterType in state.StateParameterTypes)
        {
            stringBuilder.Append('_').Append(GetTypeIdentifier(parameterType));
        }
        return stringBuilder.ToString();
    }

    private static string GetTypeIdentifier(Type type)
    {
        return type.FullName ?? type.Name;
    }

    private static void WriteConditions(
        IEnumerable<IConditionInfo> conditions,
        TextWriter writer,
        MermaidOptions options
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
            writer.Write($" <br/> {prefix}: {condition.Descriptor.EncodeForMermaid(options)}");
            first = false;
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
