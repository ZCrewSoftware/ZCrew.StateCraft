using NSubstitute;
using ZCrew.StateCraft.Extensions;

namespace ZCrew.StateCraft.IntegrationTests.ExceptionHandling;

public class TransitionExceptionTests_T1_T2_T3
{
    [Fact]
    public async Task Transition_T1_T2_T3_WhenOnExitThrowsException_ShouldRollbackAndHaveExpectedProperties()
    {
        // Arrange
        var exception = new InvalidOperationException("Test exception");
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState(
                "A",
                state => state.OnExit(() => throw exception).WithTransition<int, string, double>("To B", "B")
            )
            .WithState("B", state => state.WithParameters<int, string, double>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var thrownException = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.Transition("To B", 42, "hello", 3.14, TestContext.Current.CancellationToken)
        );

        // Assert
        Assert.Same(exception, thrownException);
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("A", stateMachine.CurrentState.StateValue);
        Assert.Null(stateMachine.PreviousState);
        Assert.Null(stateMachine.NextState);
        Assert.Empty(stateMachine.Parameters.CurrentParameterTypes);
        Assert.False(stateMachine.Parameters.IsPreviousSet);
        Assert.False(stateMachine.Parameters.IsNextSet);
        Assert.Null(stateMachine.CurrentTransition);
    }

    [Fact]
    public async Task Transition_T1_T2_T3_WhenOnExitThrowsExceptionOnce_ShouldRetrySuccessfully()
    {
        // Arrange
        var callCount = 0;
        var onExit = Substitute.For<Action>();
        onExit
            .When(x => x.Invoke())
            .Do(_ =>
            {
                if (Interlocked.Increment(ref callCount) == 1)
                    throw new InvalidOperationException();
            });
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.OnExit(onExit).WithTransition<int, string, double>("To B", "B"))
            .WithState("B", state => state.WithParameters<int, string, double>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.Transition("To B", 42, "hello", 3.14, TestContext.Current.CancellationToken)
        );

        // Act
        await stateMachine.Transition("To B", 99, "world", 2.72, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("B", stateMachine.CurrentState.StateValue);
    }

    [Fact]
    public async Task Transition_T1_T2_T3_WhenOnExitThrowsExceptionOnce_ShouldTransitionDifferentlySuccessfully()
    {
        // Arrange
        var callCount = 0;
        var onExit = Substitute.For<Action>();
        onExit
            .When(x => x.Invoke())
            .Do(_ =>
            {
                if (Interlocked.Increment(ref callCount) == 1)
                    throw new InvalidOperationException();
            });
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState(
                "A",
                state =>
                    state
                        .OnExit(onExit)
                        .WithTransition<int, string, double>("To B", "B")
                        .WithTransition<int, string, double>("To C", "C")
            )
            .WithState("B", state => state.WithParameters<int, string, double>())
            .WithState("C", state => state.WithParameters<int, string, double>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.Transition("To B", 42, "hello", 3.14, TestContext.Current.CancellationToken)
        );

        // Act
        await stateMachine.Transition("To C", 99, "world", 2.72, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("C", stateMachine.CurrentState.StateValue);
    }

    [Fact]
    public async Task Transition_T1_T2_T3_WhenStateMachineOnStateChangeThrowsException_ShouldRollbackAndHaveExpectedProperties()
    {
        // Arrange
        var exception = new InvalidOperationException("Test exception");
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .OnStateChange((_, _, _) => throw exception)
            .WithState("A", state => state.WithTransition<int, string, double>("To B", "B"))
            .WithState("B", state => state.WithParameters<int, string, double>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var thrownException = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.Transition("To B", 42, "hello", 3.14, TestContext.Current.CancellationToken)
        );

        // Assert
        Assert.Same(exception, thrownException);
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("A", stateMachine.CurrentState.StateValue);
        Assert.Null(stateMachine.PreviousState);
        Assert.Null(stateMachine.NextState);
        Assert.Empty(stateMachine.Parameters.CurrentParameterTypes);
        Assert.False(stateMachine.Parameters.IsPreviousSet);
        Assert.False(stateMachine.Parameters.IsNextSet);
        Assert.Null(stateMachine.CurrentTransition);
    }

    [Fact]
    public async Task Transition_T1_T2_T3_WhenStateMachineOnStateChangeThrowsExceptionOnce_ShouldRetrySuccessfully()
    {
        // Arrange
        var callCount = 0;
        var onStateChange = Substitute.For<Action<string, string, string>>();
        onStateChange
            .When(x => x.Invoke(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>()))
            .Do(_ =>
            {
                if (Interlocked.Increment(ref callCount) == 1)
                    throw new InvalidOperationException();
            });
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .OnStateChange(onStateChange)
            .WithState("A", state => state.WithTransition<int, string, double>("To B", "B"))
            .WithState("B", state => state.WithParameters<int, string, double>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.Transition("To B", 42, "hello", 3.14, TestContext.Current.CancellationToken)
        );

        // Act
        await stateMachine.Transition("To B", 99, "world", 2.72, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("B", stateMachine.CurrentState.StateValue);
    }

    [Fact]
    public async Task Transition_T1_T2_T3_WhenStateMachineOnStateChangeThrowsExceptionOnce_ShouldTransitionDifferentlySuccessfully()
    {
        // Arrange
        var callCount = 0;
        var onStateChange = Substitute.For<Action<string, string, string>>();
        onStateChange
            .When(x => x.Invoke(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>()))
            .Do(_ =>
            {
                if (Interlocked.Increment(ref callCount) == 1)
                    throw new InvalidOperationException();
            });
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .OnStateChange(onStateChange)
            .WithState(
                "A",
                state =>
                    state
                        .WithTransition<int, string, double>("To B", "B")
                        .WithTransition<int, string, double>("To C", "C")
            )
            .WithState("B", state => state.WithParameters<int, string, double>())
            .WithState("C", state => state.WithParameters<int, string, double>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.Transition("To B", 42, "hello", 3.14, TestContext.Current.CancellationToken)
        );

        // Act
        await stateMachine.Transition("To C", 99, "world", 2.72, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("C", stateMachine.CurrentState.StateValue);
    }

    [Fact]
    public async Task Transition_T1_T2_T3_WhenStateOnStateChangeThrowsException_ShouldRollbackAndHaveExpectedProperties()
    {
        // Arrange
        var exception = new InvalidOperationException("Test exception");
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition<int, string, double>("To B", "B"))
            .WithState(
                "B",
                state =>
                    state.WithParameters<int, string, double>().OnStateChange((_, _, _, _, _, _) => throw exception)
            )
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var thrownException = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.Transition("To B", 42, "hello", 3.14, TestContext.Current.CancellationToken)
        );

        // Assert
        Assert.Same(exception, thrownException);
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("A", stateMachine.CurrentState.StateValue);
        Assert.Null(stateMachine.PreviousState);
        Assert.Null(stateMachine.NextState);
        Assert.Empty(stateMachine.Parameters.CurrentParameterTypes);
        Assert.False(stateMachine.Parameters.IsPreviousSet);
        Assert.False(stateMachine.Parameters.IsNextSet);
        Assert.Null(stateMachine.CurrentTransition);
    }

    [Fact]
    public async Task Transition_T1_T2_T3_WhenStateOnStateChangeThrowsExceptionOnce_ShouldRetrySuccessfully()
    {
        // Arrange
        var callCount = 0;
        var onStateChange = Substitute.For<Action<string, string, string, int, string, double>>();
        onStateChange
            .When(x =>
                x.Invoke(
                    Arg.Any<string>(),
                    Arg.Any<string>(),
                    Arg.Any<string>(),
                    Arg.Any<int>(),
                    Arg.Any<string>(),
                    Arg.Any<double>()
                )
            )
            .Do(_ =>
            {
                if (Interlocked.Increment(ref callCount) == 1)
                    throw new InvalidOperationException();
            });
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition<int, string, double>("To B", "B"))
            .WithState("B", state => state.WithParameters<int, string, double>().OnStateChange(onStateChange))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.Transition("To B", 42, "hello", 3.14, TestContext.Current.CancellationToken)
        );

        // Act
        await stateMachine.Transition("To B", 99, "world", 2.72, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("B", stateMachine.CurrentState.StateValue);
    }

    [Fact]
    public async Task Transition_T1_T2_T3_WhenStateOnStateChangeThrowsExceptionOnce_ShouldTransitionDifferentlySuccessfully()
    {
        // Arrange
        var callCount = 0;
        var onStateChange = Substitute.For<Action<string, string, string, int, string, double>>();
        onStateChange
            .When(x =>
                x.Invoke(
                    Arg.Any<string>(),
                    Arg.Any<string>(),
                    Arg.Any<string>(),
                    Arg.Any<int>(),
                    Arg.Any<string>(),
                    Arg.Any<double>()
                )
            )
            .Do(_ =>
            {
                if (Interlocked.Increment(ref callCount) == 1)
                    throw new InvalidOperationException();
            });
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState(
                "A",
                state =>
                    state
                        .WithTransition<int, string, double>("To B", "B")
                        .WithTransition<int, string, double>("To C", "C")
            )
            .WithState("B", state => state.WithParameters<int, string, double>().OnStateChange(onStateChange))
            .WithState("C", state => state.WithParameters<int, string, double>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.Transition("To B", 42, "hello", 3.14, TestContext.Current.CancellationToken)
        );

        // Act
        await stateMachine.Transition("To C", 99, "world", 2.72, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("C", stateMachine.CurrentState.StateValue);
    }

    [Fact]
    public async Task Transition_T1_T2_T3_WhenOnEntryThrowsException_ShouldRollbackAndHaveExpectedProperties()
    {
        // Arrange
        var exception = new InvalidOperationException("Test exception");
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition<int, string, double>("To B", "B"))
            .WithState("B", state => state.WithParameters<int, string, double>().OnEntry((_, _, _) => throw exception))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var thrownException = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.Transition("To B", 42, "hello", 3.14, TestContext.Current.CancellationToken)
        );

        // Assert
        Assert.Same(exception, thrownException);
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("A", stateMachine.CurrentState.StateValue);
        Assert.Null(stateMachine.PreviousState);
        Assert.Null(stateMachine.NextState);
        Assert.Empty(stateMachine.Parameters.CurrentParameterTypes);
        Assert.False(stateMachine.Parameters.IsPreviousSet);
        Assert.False(stateMachine.Parameters.IsNextSet);
        Assert.Null(stateMachine.CurrentTransition);
    }

    [Fact]
    public async Task Transition_T1_T2_T3_WhenOnEntryThrowsExceptionOnce_ShouldRetrySuccessfully()
    {
        // Arrange
        var callCount = 0;
        var onEntry = Substitute.For<Action<int, string, double>>();
        onEntry
            .When(x => x.Invoke(Arg.Any<int>(), Arg.Any<string>(), Arg.Any<double>()))
            .Do(_ =>
            {
                if (Interlocked.Increment(ref callCount) == 1)
                    throw new InvalidOperationException();
            });
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition<int, string, double>("To B", "B"))
            .WithState("B", state => state.WithParameters<int, string, double>().OnEntry(onEntry))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.Transition("To B", 42, "hello", 3.14, TestContext.Current.CancellationToken)
        );

        // Act
        await stateMachine.Transition("To B", 99, "world", 2.72, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("B", stateMachine.CurrentState.StateValue);
    }

    [Fact]
    public async Task Transition_T1_T2_T3_WhenOnEntryThrowsExceptionOnce_ShouldTransitionDifferentlySuccessfully()
    {
        // Arrange
        var callCount = 0;
        var onEntry = Substitute.For<Action<int, string, double>>();
        onEntry
            .When(x => x.Invoke(Arg.Any<int>(), Arg.Any<string>(), Arg.Any<double>()))
            .Do(_ =>
            {
                if (Interlocked.Increment(ref callCount) == 1)
                    throw new InvalidOperationException();
            });
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState(
                "A",
                state =>
                    state
                        .WithTransition<int, string, double>("To B", "B")
                        .WithTransition<int, string, double>("To C", "C")
            )
            .WithState("B", state => state.WithParameters<int, string, double>().OnEntry(onEntry))
            .WithState("C", state => state.WithParameters<int, string, double>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.Transition("To B", 42, "hello", 3.14, TestContext.Current.CancellationToken)
        );

        // Act
        await stateMachine.Transition("To C", 99, "world", 2.72, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("C", stateMachine.CurrentState.StateValue);
    }

    [Fact]
    public async Task Transition_T1_T2_T3_WhenNextParameterConditionThrowsException_ShouldThrowAndRemainInCurrentState()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState(
                "A",
                state =>
                    state.WithTransition(
                        "To B",
                        t =>
                            t.WithParameters<int, string, double>()
                                .If((_, _, _) => throw new InvalidOperationException())
                                .To("B")
                    )
            )
            .WithState("B", state => state.WithParameters<int, string, double>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.Transition("To B", 42, "hello", 3.14, TestContext.Current.CancellationToken)
        );

        // Assert
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("A", stateMachine.CurrentState.StateValue);
        Assert.Empty(stateMachine.Parameters.CurrentParameterTypes);
        Assert.Null(stateMachine.PreviousState);
        Assert.Null(stateMachine.NextState);
        Assert.False(stateMachine.Parameters.IsPreviousSet);
        Assert.False(stateMachine.Parameters.IsNextSet);
    }

    [Fact]
    public async Task Transition_T1_T2_T3_WhenNextParameterConditionThrowsExceptionOnce_ShouldRetrySuccessfully()
    {
        // Arrange
        var callCount = 0;
        var condition = Substitute.For<Func<int, string, double, bool>>();
        condition
            .Invoke(Arg.Any<int>(), Arg.Any<string>(), Arg.Any<double>())
            .Returns(_ =>
            {
                if (Interlocked.Increment(ref callCount) == 1)
                    throw new InvalidOperationException();
                return true;
            });

        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState(
                "A",
                state =>
                    state.WithTransition("To B", t => t.WithParameters<int, string, double>().If(condition).To("B"))
            )
            .WithState("B", state => state.WithParameters<int, string, double>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.Transition("To B", 42, "hello", 3.14, TestContext.Current.CancellationToken)
        );

        // Act
        await stateMachine.Transition("To B", 99, "world", 2.72, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("B", stateMachine.CurrentState.StateValue);
    }

    [Fact]
    public async Task Transition_T1_T2_T3_WhenNextParameterConditionThrowsExceptionOnce_ShouldTransitionDifferentlySuccessfully()
    {
        // Arrange
        var callCount = 0;
        var condition = Substitute.For<Func<int, string, double, bool>>();
        condition
            .Invoke(Arg.Any<int>(), Arg.Any<string>(), Arg.Any<double>())
            .Returns(_ =>
            {
                if (Interlocked.Increment(ref callCount) == 1)
                    throw new InvalidOperationException();
                return true;
            });

        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState(
                "A",
                state =>
                    state
                        .WithTransition("To B", t => t.WithParameters<int, string, double>().If(condition).To("B"))
                        .WithTransition<int, string, double>("To C", "C")
            )
            .WithState("B", state => state.WithParameters<int, string, double>())
            .WithState("C", state => state.WithParameters<int, string, double>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.Transition("To B", 42, "hello", 3.14, TestContext.Current.CancellationToken)
        );

        // Act
        await stateMachine.Transition("To C", 99, "world", 2.72, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("C", stateMachine.CurrentState.StateValue);
    }
}
