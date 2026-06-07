using ZCrew.StateCraft.Identities;

namespace ZCrew.StateCraft;

/// <summary>
///     Factory for creating <see cref="ITransitionIdentity{TTransition}"/> instances. Use the generic overloads
///     (<see cref="For{TTransition, T}"/> and friends) to declare the transition's parameter types at compile time, or
///     the <see cref="Type"/>-list overloads when the parameter types are only known at runtime.
/// </summary>
public static class TransitionIdentity
{
    /// <summary>
    ///     Creates the identity of a parameterless transition.
    /// </summary>
    /// <typeparam name="TTransition">The transition type.</typeparam>
    /// <param name="transitionValue">The transition value.</param>
    /// <returns>The identity of the transition.</returns>
    public static ITransitionIdentity<TTransition> For<TTransition>(TTransition transitionValue)
        where TTransition : notnull
    {
        return new TransitionIdentity<TTransition>(transitionValue, []);
    }

    /// <summary>
    ///     Creates the identity of a transition with one parameter.
    /// </summary>
    /// <typeparam name="TTransition">The transition type.</typeparam>
    /// <typeparam name="T">The type of the parameter for this transition.</typeparam>
    /// <param name="transitionValue">The transition value.</param>
    /// <returns>The identity of the transition.</returns>
    public static ITransitionIdentity<TTransition> For<TTransition, T>(TTransition transitionValue)
        where TTransition : notnull
    {
        return new TransitionIdentity<TTransition>(transitionValue, [typeof(T)]);
    }

    /// <summary>
    ///     Creates the identity of a transition with two parameters.
    /// </summary>
    /// <typeparam name="TTransition">The transition type.</typeparam>
    /// <typeparam name="T1">The type of the first parameter for this transition.</typeparam>
    /// <typeparam name="T2">The type of the second parameter for this transition.</typeparam>
    /// <param name="transitionValue">The transition value.</param>
    /// <returns>The identity of the transition.</returns>
    public static ITransitionIdentity<TTransition> For<TTransition, T1, T2>(TTransition transitionValue)
        where TTransition : notnull
    {
        return new TransitionIdentity<TTransition>(transitionValue, [typeof(T1), typeof(T2)]);
    }

    /// <summary>
    ///     Creates the identity of a transition with three parameters.
    /// </summary>
    /// <typeparam name="TTransition">The transition type.</typeparam>
    /// <typeparam name="T1">The type of the first parameter for this transition.</typeparam>
    /// <typeparam name="T2">The type of the second parameter for this transition.</typeparam>
    /// <typeparam name="T3">The type of the third parameter for this transition.</typeparam>
    /// <param name="transitionValue">The transition value.</param>
    /// <returns>The identity of the transition.</returns>
    public static ITransitionIdentity<TTransition> For<TTransition, T1, T2, T3>(TTransition transitionValue)
        where TTransition : notnull
    {
        return new TransitionIdentity<TTransition>(transitionValue, [typeof(T1), typeof(T2), typeof(T3)]);
    }

    /// <summary>
    ///     Creates the identity of a transition with four parameters.
    /// </summary>
    /// <typeparam name="TTransition">The transition type.</typeparam>
    /// <typeparam name="T1">The type of the first parameter for this transition.</typeparam>
    /// <typeparam name="T2">The type of the second parameter for this transition.</typeparam>
    /// <typeparam name="T3">The type of the third parameter for this transition.</typeparam>
    /// <typeparam name="T4">The type of the fourth parameter for this transition.</typeparam>
    /// <param name="transitionValue">The transition value.</param>
    /// <returns>The identity of the transition.</returns>
    public static ITransitionIdentity<TTransition> For<TTransition, T1, T2, T3, T4>(TTransition transitionValue)
        where TTransition : notnull
    {
        return new TransitionIdentity<TTransition>(transitionValue, [typeof(T1), typeof(T2), typeof(T3), typeof(T4)]);
    }

    /// <summary>
    ///     Creates the identity of a transition whose parameter types are supplied at runtime.
    /// </summary>
    /// <typeparam name="TTransition">The transition type.</typeparam>
    /// <param name="transitionValue">The transition value.</param>
    /// <param name="transitionParameterTypes">
    ///     The types of the parameters the caller must supply when invoking this transition, in declaration order.
    /// </param>
    /// <returns>The identity of the transition.</returns>
    public static ITransitionIdentity<TTransition> For<TTransition>(
        TTransition transitionValue,
        params Type[] transitionParameterTypes
    )
        where TTransition : notnull
    {
        return new TransitionIdentity<TTransition>(transitionValue, transitionParameterTypes);
    }

    /// <summary>
    ///     Creates the identity of a transition whose parameter types are supplied at runtime.
    /// </summary>
    /// <typeparam name="TTransition">The transition type.</typeparam>
    /// <param name="transitionValue">The transition value.</param>
    /// <param name="transitionParameterTypes">
    ///     The types of the parameters the caller must supply when invoking this transition, in declaration order.
    /// </param>
    /// <returns>The identity of the transition.</returns>
    public static ITransitionIdentity<TTransition> For<TTransition>(
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
