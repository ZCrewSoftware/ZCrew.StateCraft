using NSubstitute;
using ZCrew.StateCraft.Extensions;

namespace ZCrew.StateCraft.IntegrationTests.ExceptionHandling;

public class TransitionExceptionTests_T
{
    [Fact]
    public async Task Transition_T_WhenOnExitThrowsException_ShouldRollbackAndHaveExpectedProperties()
    {
        // Arrange
        var exception = new InvalidOperationException("Test exception");
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42)
            .WithState(
                "A",
                state => state.WithParameter<int>().OnExit(_ => throw exception).WithTransition<string>("To B", "B")
            )
            .WithState("B", state => state.WithParameter<string>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var thrownException = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.Transition("To B", "hello", TestContext.Current.CancellationToken)
        );

        // Assert
        Assert.Same(exception, thrownException);
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("A", stateMachine.CurrentState.StateValue);
        Assert.Null(stateMachine.PreviousState);
        Assert.Null(stateMachine.NextState);
        Assert.Equal(42, stateMachine.Parameters.GetCurrentParameter<int>());
        Assert.False(stateMachine.Parameters.IsPreviousSet);
        Assert.False(stateMachine.Parameters.IsNextSet);
        Assert.Null(stateMachine.CurrentTransition);
    }

    [Fact]
    public async Task Transition_T_WhenOnExitThrowsExceptionOnce_ShouldRetrySuccessfully()
    {
        // Arrange
        var callCount = 0;
        var onExit = Substitute.For<Action<int>>();
        onExit
            .When(x => x.Invoke(Arg.Any<int>()))
            .Do(_ =>
            {
                if (Interlocked.Increment(ref callCount) == 1)
                    throw new InvalidOperationException();
            });
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42)
            .WithState("A", state => state.WithParameter<int>().OnExit(onExit).WithTransition<string>("To B", "B"))
            .WithState("B", state => state.WithParameter<string>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.Transition("To B", "hello", TestContext.Current.CancellationToken)
        );

        // Act
        await stateMachine.Transition("To B", "world", TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("B", stateMachine.CurrentState.StateValue);
        Assert.Equal("world", stateMachine.Parameters.GetCurrentParameter<string>());
    }

    [Fact]
    public async Task Transition_T_WhenOnExitThrowsExceptionOnce_ShouldTransitionDifferentlySuccessfully()
    {
        // Arrange
        var callCount = 0;
        var onExit = Substitute.For<Action<int>>();
        onExit
            .When(x => x.Invoke(Arg.Any<int>()))
            .Do(_ =>
            {
                if (Interlocked.Increment(ref callCount) == 1)
                    throw new InvalidOperationException();
            });
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42)
            .WithState(
                "A",
                state =>
                    state
                        .WithParameter<int>()
                        .OnExit(onExit)
                        .WithTransition<string>("To B", "B")
                        .WithTransition<string>("To C", "C")
            )
            .WithState("B", state => state.WithParameter<string>())
            .WithState("C", state => state.WithParameter<string>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.Transition("To B", "hello", TestContext.Current.CancellationToken)
        );

        // Act
        await stateMachine.Transition("To C", "world", TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("C", stateMachine.CurrentState.StateValue);
        Assert.Equal("world", stateMachine.Parameters.GetCurrentParameter<string>());
    }

    [Fact]
    public async Task Transition_T_WhenStateMachineOnStateChangeThrowsException_ShouldRollbackAndHaveExpectedProperties()
    {
        // Arrange
        var exception = new InvalidOperationException("Test exception");
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42)
            .OnStateChange((_, _, _) => throw exception)
            .WithState("A", state => state.WithParameter<int>().WithTransition<string>("To B", "B"))
            .WithState("B", state => state.WithParameter<string>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var thrownException = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.Transition("To B", "hello", TestContext.Current.CancellationToken)
        );

        // Assert
        Assert.Same(exception, thrownException);
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("A", stateMachine.CurrentState.StateValue);
        Assert.Null(stateMachine.PreviousState);
        Assert.Null(stateMachine.NextState);
        Assert.Equal(42, stateMachine.Parameters.GetCurrentParameter<int>());
        Assert.False(stateMachine.Parameters.IsPreviousSet);
        Assert.False(stateMachine.Parameters.IsNextSet);
        Assert.Null(stateMachine.CurrentTransition);
    }

    [Fact]
    public async Task Transition_T_WhenStateMachineOnStateChangeThrowsExceptionOnce_ShouldRetrySuccessfully()
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
            .WithInitialState("A", 42)
            .OnStateChange(onStateChange)
            .WithState("A", state => state.WithParameter<int>().WithTransition<string>("To B", "B"))
            .WithState("B", state => state.WithParameter<string>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.Transition("To B", "hello", TestContext.Current.CancellationToken)
        );

        // Act
        await stateMachine.Transition("To B", "world", TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("B", stateMachine.CurrentState.StateValue);
        Assert.Equal("world", stateMachine.Parameters.GetCurrentParameter<string>());
    }

    [Fact]
    public async Task Transition_T_WhenStateMachineOnStateChangeThrowsExceptionOnce_ShouldTransitionDifferentlySuccessfully()
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
            .WithInitialState("A", 42)
            .OnStateChange(onStateChange)
            .WithState(
                "A",
                state =>
                    state.WithParameter<int>().WithTransition<string>("To B", "B").WithTransition<string>("To C", "C")
            )
            .WithState("B", state => state.WithParameter<string>())
            .WithState("C", state => state.WithParameter<string>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.Transition("To B", "hello", TestContext.Current.CancellationToken)
        );

        // Act
        await stateMachine.Transition("To C", "world", TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("C", stateMachine.CurrentState.StateValue);
        Assert.Equal("world", stateMachine.Parameters.GetCurrentParameter<string>());
    }

    [Fact]
    public async Task Transition_T_WhenStateOnStateChangeThrowsException_ShouldRollbackAndHaveExpectedProperties()
    {
        // Arrange
        var exception = new InvalidOperationException("Test exception");
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42)
            .WithState("A", state => state.WithParameter<int>().WithTransition<string>("To B", "B"))
            .WithState("B", state => state.WithParameter<string>().OnStateChange((_, _, _, _) => throw exception))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var thrownException = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.Transition("To B", "hello", TestContext.Current.CancellationToken)
        );

        // Assert
        Assert.Same(exception, thrownException);
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("A", stateMachine.CurrentState.StateValue);
        Assert.Null(stateMachine.PreviousState);
        Assert.Null(stateMachine.NextState);
        Assert.Equal(42, stateMachine.Parameters.GetCurrentParameter<int>());
        Assert.False(stateMachine.Parameters.IsPreviousSet);
        Assert.False(stateMachine.Parameters.IsNextSet);
        Assert.Null(stateMachine.CurrentTransition);
    }

    [Fact]
    public async Task Transition_T_WhenStateOnStateChangeThrowsExceptionOnce_ShouldRetrySuccessfully()
    {
        // Arrange
        var callCount = 0;
        var onStateChange = Substitute.For<Action<string, string, string, string>>();
        onStateChange
            .When(x => x.Invoke(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>()))
            .Do(_ =>
            {
                if (Interlocked.Increment(ref callCount) == 1)
                    throw new InvalidOperationException();
            });
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42)
            .WithState("A", state => state.WithParameter<int>().WithTransition<string>("To B", "B"))
            .WithState("B", state => state.WithParameter<string>().OnStateChange(onStateChange))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.Transition("To B", "hello", TestContext.Current.CancellationToken)
        );

        // Act
        await stateMachine.Transition("To B", "world", TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("B", stateMachine.CurrentState.StateValue);
        Assert.Equal("world", stateMachine.Parameters.GetCurrentParameter<string>());
    }

    [Fact]
    public async Task Transition_T_WhenStateOnStateChangeThrowsExceptionOnce_ShouldTransitionDifferentlySuccessfully()
    {
        // Arrange
        var callCount = 0;
        var onStateChange = Substitute.For<Action<string, string, string, string>>();
        onStateChange
            .When(x => x.Invoke(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>()))
            .Do(_ =>
            {
                if (Interlocked.Increment(ref callCount) == 1)
                    throw new InvalidOperationException();
            });
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42)
            .WithState(
                "A",
                state =>
                    state.WithParameter<int>().WithTransition<string>("To B", "B").WithTransition<string>("To C", "C")
            )
            .WithState("B", state => state.WithParameter<string>().OnStateChange(onStateChange))
            .WithState("C", state => state.WithParameter<string>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.Transition("To B", "hello", TestContext.Current.CancellationToken)
        );

        // Act
        await stateMachine.Transition("To C", "world", TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("C", stateMachine.CurrentState.StateValue);
        Assert.Equal("world", stateMachine.Parameters.GetCurrentParameter<string>());
    }

    [Fact]
    public async Task Transition_T_WhenOnEntryThrowsException_ShouldThrowAndCommit()
    {
        // Arrange
        var exception = new InvalidOperationException("Test exception");
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42)
            .WithState("A", state => state.WithParameter<int>().WithTransition<string>("To B", "B"))
            .WithState("B", state => state.WithParameter<string>().OnEntry(_ => throw exception))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var thrownException = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.Transition("To B", "hello", TestContext.Current.CancellationToken)
        );

        // Assert
        Assert.Same(exception, thrownException);
        Assert.Null(stateMachine.CurrentState);
        Assert.NotNull(stateMachine.NextState);
        Assert.Equal("B", stateMachine.NextState.StateValue);
        Assert.NotNull(stateMachine.PreviousState);
        Assert.Equal("A", stateMachine.PreviousState.StateValue);
        Assert.False(stateMachine.Parameters.IsCurrentSet);
        Assert.Equal("hello", stateMachine.Parameters.GetNextParameter<string>());
        Assert.True(stateMachine.Parameters.IsPreviousSet);
        Assert.True(stateMachine.Parameters.IsNextSet);
        Assert.Null(stateMachine.CurrentTransition);
    }

    [Fact]
    public async Task Transition_T_WhenOnEntryThrowsExceptionOnce_ShouldRetrySuccessfully()
    {
        // Arrange
        var callCount = 0;
        var onEntry = Substitute.For<Action<string>>();
        onEntry
            .When(x => x.Invoke(Arg.Any<string>()))
            .Do(_ =>
            {
                if (Interlocked.Increment(ref callCount) == 1)
                    throw new InvalidOperationException();
            });
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42)
            .WithState("A", state => state.WithParameter<int>().WithTransition<string>("To B", "B"))
            .WithState("B", state => state.WithParameter<string>().OnEntry(onEntry).WithTransition("To C", "C"))
            .WithState("C", state => state)
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.Transition("To B", "hello", TestContext.Current.CancellationToken)
        );

        // Act
        await stateMachine.Transition("To C", TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("C", stateMachine.CurrentState.StateValue);
    }

    [Fact]
    public async Task Transition_T_WhenPreviousParameterConditionThrowsException_ShouldThrowAndRemainInCurrentState()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42)
            .WithState(
                "A",
                state =>
                    state
                        .WithParameter<int>()
                        .WithTransition(
                            "To B",
                            t => t.If(_ => throw new InvalidOperationException()).WithParameter<string>().To("B")
                        )
            )
            .WithState("B", state => state.WithParameter<string>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.Transition("To B", "hello", TestContext.Current.CancellationToken)
        );

        // Assert
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("A", stateMachine.CurrentState.StateValue);
        Assert.Equal(42, stateMachine.Parameters.GetCurrentParameter<int>());
        Assert.Null(stateMachine.PreviousState);
        Assert.Null(stateMachine.NextState);
        Assert.False(stateMachine.Parameters.IsPreviousSet);
        Assert.False(stateMachine.Parameters.IsNextSet);
    }

    [Fact]
    public async Task Transition_T_WhenPreviousParameterConditionThrowsExceptionOnce_ShouldRetrySuccessfully()
    {
        // Arrange
        var callCount = 0;
        var condition = Substitute.For<Func<int, bool>>();
        condition
            .Invoke(Arg.Any<int>())
            .Returns(_ =>
            {
                if (Interlocked.Increment(ref callCount) == 1)
                    throw new InvalidOperationException();
                return true;
            });

        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42)
            .WithState(
                "A",
                state =>
                    state
                        .WithParameter<int>()
                        .WithTransition("To B", t => t.If(condition).WithParameter<string>().To("B"))
            )
            .WithState("B", state => state.WithParameter<string>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.Transition("To B", "hello", TestContext.Current.CancellationToken)
        );

        // Act
        await stateMachine.Transition("To B", "world", TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("B", stateMachine.CurrentState.StateValue);
        Assert.Equal("world", stateMachine.Parameters.GetCurrentParameter<string>());
    }

    [Fact]
    public async Task Transition_T_WhenPreviousParameterConditionThrowsExceptionOnce_ShouldTransitionDifferentlySuccessfully()
    {
        // Arrange
        var callCount = 0;
        var condition = Substitute.For<Func<int, bool>>();
        condition
            .Invoke(Arg.Any<int>())
            .Returns(_ =>
            {
                if (Interlocked.Increment(ref callCount) == 1)
                    throw new InvalidOperationException();
                return true;
            });

        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42)
            .WithState(
                "A",
                state =>
                    state
                        .WithParameter<int>()
                        .WithTransition("To B", t => t.If(condition).WithParameter<string>().To("B"))
                        .WithTransition<string>("To C", "C")
            )
            .WithState("B", state => state.WithParameter<string>())
            .WithState("C", state => state.WithParameter<string>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.Transition("To B", "hello", TestContext.Current.CancellationToken)
        );

        // Act
        await stateMachine.Transition("To C", "world", TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("C", stateMachine.CurrentState.StateValue);
        Assert.Equal("world", stateMachine.Parameters.GetCurrentParameter<string>());
    }

    [Fact]
    public async Task Transition_T_WhenNextParameterConditionThrowsException_ShouldThrowAndRemainInCurrentState()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42)
            .WithState(
                "A",
                state =>
                    state
                        .WithParameter<int>()
                        .WithTransition(
                            "To B",
                            t => t.WithParameter<string>().If(_ => throw new InvalidOperationException()).To("B")
                        )
            )
            .WithState("B", state => state.WithParameter<string>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.Transition("To B", "hello", TestContext.Current.CancellationToken)
        );

        // Assert
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("A", stateMachine.CurrentState.StateValue);
        Assert.Equal(42, stateMachine.Parameters.GetCurrentParameter<int>());
        Assert.Null(stateMachine.PreviousState);
        Assert.Null(stateMachine.NextState);
        Assert.False(stateMachine.Parameters.IsPreviousSet);
        Assert.False(stateMachine.Parameters.IsNextSet);
    }

    [Fact]
    public async Task Transition_T_WhenNextParameterConditionThrowsExceptionOnce_ShouldRetrySuccessfully()
    {
        // Arrange
        var callCount = 0;
        var condition = Substitute.For<Func<string, bool>>();
        condition
            .Invoke(Arg.Any<string>())
            .Returns(_ =>
            {
                if (Interlocked.Increment(ref callCount) == 1)
                    throw new InvalidOperationException();
                return true;
            });

        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42)
            .WithState(
                "A",
                state =>
                    state
                        .WithParameter<int>()
                        .WithTransition("To B", t => t.WithParameter<string>().If(condition).To("B"))
            )
            .WithState("B", state => state.WithParameter<string>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.Transition("To B", "hello", TestContext.Current.CancellationToken)
        );

        // Act
        await stateMachine.Transition("To B", "world", TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("B", stateMachine.CurrentState.StateValue);
        Assert.Equal("world", stateMachine.Parameters.GetCurrentParameter<string>());
    }

    [Fact]
    public async Task Transition_T_WhenNextParameterConditionThrowsExceptionOnce_ShouldTransitionDifferentlySuccessfully()
    {
        // Arrange
        var callCount = 0;
        var condition = Substitute.For<Func<string, bool>>();
        condition
            .Invoke(Arg.Any<string>())
            .Returns(_ =>
            {
                if (Interlocked.Increment(ref callCount) == 1)
                    throw new InvalidOperationException();
                return true;
            });

        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42)
            .WithState(
                "A",
                state =>
                    state
                        .WithParameter<int>()
                        .WithTransition("To B", t => t.WithParameter<string>().If(condition).To("B"))
                        .WithTransition<string>("To C", "C")
            )
            .WithState("B", state => state.WithParameter<string>())
            .WithState("C", state => state.WithParameter<string>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.Transition("To B", "hello", TestContext.Current.CancellationToken)
        );

        // Act
        await stateMachine.Transition("To C", "world", TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("C", stateMachine.CurrentState.StateValue);
        Assert.Equal("world", stateMachine.Parameters.GetCurrentParameter<string>());
    }
}
