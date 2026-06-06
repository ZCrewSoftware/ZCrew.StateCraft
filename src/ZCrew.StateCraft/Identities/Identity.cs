using ZCrew.StateCraft.Identities;

namespace ZCrew.StateCraft;

// TODO: add overloads for generic types like <TState, T1, T2>
// TODO: document
public static class Identity
{
    public static IStateIdentity<TState> ForState<TState>(TState stateValue)
        where TState : notnull
    {
        return new StateIdentity<TState>(stateValue, []);
    }

    public static IStateIdentity<TState> ForState<TState>(TState stateValue, params Type[] stateParameterTypes)
        where TState : notnull
    {
        return new StateIdentity<TState>(stateValue, stateParameterTypes);
    }

    public static IStateIdentity<TState> ForState<TState>(TState stateValue, IEnumerable<Type> stateParameterTypes)
        where TState : notnull
    {
        if (stateParameterTypes is IReadOnlyList<Type> stateParameterTypesList)
        {
            return new StateIdentity<TState>(stateValue, stateParameterTypesList);
        }
        return new StateIdentity<TState>(stateValue, stateParameterTypes.ToArray());
    }

    public static ITransitionIdentity<TTransition> ForTransition<TTransition>(TTransition transitionValue)
        where TTransition : notnull
    {
        return new TransitionIdentity<TTransition>(transitionValue, []);
    }

    public static ITransitionIdentity<TTransition> ForTransition<TTransition>(
        TTransition transitionValue,
        params Type[] transitionParameterTypes
    )
        where TTransition : notnull
    {
        return new TransitionIdentity<TTransition>(transitionValue, transitionParameterTypes);
    }

    public static ITransitionIdentity<TTransition> ForTransition<TTransition>(
        TTransition transitionValue,
        IEnumerable<Type> transitionParameterTypes
    )
        where TTransition : notnull
    {
        if (transitionParameterTypes is IReadOnlyList<Type> transitionParameterTypesList)
        {
            return new TransitionIdentity<TTransition>(transitionValue, transitionParameterTypesList);
        }
        return new TransitionIdentity<TTransition>(transitionValue, transitionParameterTypes.ToArray());
    }
}
