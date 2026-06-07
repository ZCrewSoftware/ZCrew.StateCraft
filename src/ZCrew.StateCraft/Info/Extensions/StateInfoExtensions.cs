using System.Diagnostics.CodeAnalysis;
using ZCrew.StateCraft.Identities;
using ZCrew.StateCraft.Identities.Extensions;

namespace ZCrew.StateCraft.Info.Extensions;

/// <summary>
///     Extension methods over <see cref="IStateInfo{TState, TTransition}"/> for comparing states and navigating the
///     configured transition graph (the transitions leaving a state, the states it leads to, and reachability across
///     the whole machine).
/// </summary>
public static class StateInfoExtensions
{
    extension<TState, TTransition>(IStateInfo<TState, TTransition> stateInfo)
        where TState : notnull
        where TTransition : notnull
    {
        /// <summary>
        ///     Gets the transitions whose source is this state, drawn from
        ///     <see cref="IStateMachineInfo{TState, TTransition}.Transitions"/>. Inverted (<c>From</c>) transitions
        ///     are included whenever this state is not excluded from them.
        /// </summary>
        /// <returns>The transitions that can be taken from this state, in declaration order.</returns>
        public IEnumerable<ITransitionInfo<TState, TTransition>> GetTransitions()
        {
            foreach (var transition in stateInfo.StateMachine.Transitions)
            {
                if (transition.IsTransitionFrom(stateInfo))
                {
                    yield return transition;
                }
            }
        }

        /// <summary>
        ///     Gets the states reachable from this state in a single transition — the destination of every transition
        ///     returned by <c>GetTransitions</c>. The same state may appear more than once when multiple
        ///     transitions lead to it.
        /// </summary>
        /// <returns>The immediate successor states of this state.</returns>
        public IEnumerable<IStateInfo<TState, TTransition>> GetNextStates()
        {
            var transitions = stateInfo.GetTransitions();
            foreach (var transition in transitions)
            {
                foreach (var nextState in transition.GetNextStates())
                {
                    yield return nextState;
                }
            }
        }

        /// <summary>
        ///     Gets every state reachable from this state by following one or more transitions (the transitive
        ///     closure of <c>GetNextStates</c>), in breadth-first order with duplicates removed.
        /// </summary>
        /// <remarks>
        ///     This state is itself included only when a transition path leads back to it — for example a reentrant
        ///     self-transition or a cycle through other states. Its presence in the result is therefore meaningful:
        ///     it signals that such a return path exists.
        /// </remarks>
        /// <returns>The distinct states reachable from this state via at least one transition.</returns>
        public IEnumerable<IStateInfo<TState, TTransition>> ReachableStates()
        {
            var visited = new HashSet<IStateInfo<TState, TTransition>>(StateIdentityEqualityComparer<TState>.Instance);
            var result = new List<IStateInfo<TState, TTransition>>();
            var queue = new Queue<IStateInfo<TState, TTransition>>();

            foreach (var next in stateInfo.GetNextStates())
            {
                Enqueue(next);
            }

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                result.Add(current);

                foreach (var next in current.GetNextStates())
                {
                    Enqueue(next);
                }
            }

            return result;

            void Enqueue(IStateInfo<TState, TTransition> state)
            {
                if (visited.Add(state))
                {
                    queue.Enqueue(state);
                }
            }
        }

        /// <summary>
        ///     Determines whether <paramref name="target"/> can be reached from this state by following one or more
        ///     transitions. Equivalent to <c>TryFindPath(target, out _)</c> but without building the path.
        /// </summary>
        /// <param name="target">The state to reach.</param>
        /// <returns>
        ///     <see langword="true"/> if a transition path from this state to <paramref name="target"/> exists;
        ///     otherwise <see langword="false"/>. Reaching this state from itself requires a return path (a reentrant
        ///     self-transition or a cycle).
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="target"/> is <see langword="null"/>.</exception>
        public bool CanReach(IStateInfo<TState, TTransition> target)
        {
            ArgumentNullException.ThrowIfNull(target);

            // TODO: this can be improved by just breaking out early instead of querying all reachable states
            return stateInfo.ReachableStates().Contains(target, StateIdentityEqualityComparer<TState>.Instance);
        }

        /// <summary>
        ///     Finds the shortest path of transitions from this state to <paramref name="target"/>.
        /// </summary>
        /// <param name="target">The state to reach.</param>
        /// <returns>
        ///     A <see cref="PathInfo{TState, TTransition}"/> whose steps lead from this state to
        ///     <paramref name="target"/>. When <paramref name="target"/> is this state, the path describes the return
        ///     path to it (a single reentrant step or a longer cycle).
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="target"/> is <see langword="null"/>.</exception>
        /// <exception cref="UnreachableStateException">
        ///     <paramref name="target"/> cannot be reached from this state. Use <c>TryFindPath</c> or <c>CanReach</c>
        ///     to test reachability without throwing.
        /// </exception>
        public PathInfo<TState, TTransition> FindPathTo(IStateInfo<TState, TTransition> target)
        {
            if (!stateInfo.TryFindPathTo(target, out var path))
            {
                throw new UnreachableStateException();
            }

            return path;
        }

        /// <summary>
        ///     Attempts to find the shortest path of transitions from this state to <paramref name="target"/>.
        /// </summary>
        /// <param name="target">The state to reach.</param>
        /// <param name="path">
        ///     When this method returns <see langword="true"/>, the path leading from this state to
        ///     <paramref name="target"/>; otherwise <see langword="null"/>.
        /// </param>
        /// <returns>
        ///     <see langword="true"/> if a path was found; otherwise <see langword="false"/>. When
        ///     <paramref name="target"/> is this state, a path is found only when a return path exists (a reentrant
        ///     self-transition or a cycle).
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="target"/> is <see langword="null"/>.</exception>
        public bool TryFindPathTo(
            IStateInfo<TState, TTransition> target,
            [NotNullWhen(true)] out PathInfo<TState, TTransition>? path
        )
        {
            ArgumentNullException.ThrowIfNull(target);

            var visited = new HashSet<IStateInfo<TState, TTransition>>(StateIdentityEqualityComparer<TState>.Instance)
            {
                stateInfo,
            };
            var parents = new Dictionary<
                IStateInfo<TState, TTransition>,
                (ITransitionInfo<TState, TTransition> Transition, IStateInfo<TState, TTransition> Previous)
            >(StateIdentityEqualityComparer<TState>.Instance);
            var queue = new Queue<IStateInfo<TState, TTransition>>();
            queue.Enqueue(stateInfo);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();

                foreach (var transition in current.GetTransitions())
                {
                    foreach (var next in transition.GetNextStates())
                    {
                        // Test the target before the visited check so a return path to this state (which is seeded as
                        // visited) is still found.
                        if (target.Matches(next))
                        {
                            path = BuildPath(current, transition, next);
                            return true;
                        }

                        if (!visited.Add(next))
                        {
                            continue;
                        }

                        parents[next] = (transition, current);
                        queue.Enqueue(next);
                    }
                }
            }

            path = null;
            return false;

            PathInfo<TState, TTransition> BuildPath(
                IStateInfo<TState, TTransition> finalPrevious,
                ITransitionInfo<TState, TTransition> finalTransition,
                IStateInfo<TState, TTransition> finalState
            )
            {
                var steps = new List<PathStepInfo<TState, TTransition>> { new(finalTransition, finalState) };

                var cursor = finalPrevious;
                while (parents.TryGetValue(cursor, out var parent))
                {
                    steps.Add(new PathStepInfo<TState, TTransition>(parent.Transition, cursor));
                    cursor = parent.Previous;
                }

                steps.Reverse();
                return new PathInfo<TState, TTransition>(steps);
            }
        }
    }
}
