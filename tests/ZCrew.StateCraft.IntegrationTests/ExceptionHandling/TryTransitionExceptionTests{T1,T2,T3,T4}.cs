using NSubstitute;
using ZCrew.StateCraft.Extensions;

namespace ZCrew.StateCraft.IntegrationTests.ExceptionHandling;

public class TryTransitionExceptionTests_T1_T2_T3_T4
{
    [Fact]
    public async Task TryTransition_T1_T2_T3_T4_WhenOnExitThrowsException_ShouldThrowAndRollback()
    {
        // Arrange
        var exception = new InvalidOperationException("Test exception");
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState(
                "A",
                state => state.OnExit(() => throw exception).WithTransition<int, string, double, bool>("To B", "B")
            )
            .WithState("B", state => state.WithParameters<int, string, double, bool>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var thrownException = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.TryTransition("To B", 42, "hello", 3.14, true, TestContext.Current.CancellationToken)
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
    public async Task TryTransition_T1_T2_T3_T4_WhenOnExitThrowsExceptionOnce_ShouldRetrySuccessfully()
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
            .WithState("A", state => state.OnExit(onExit).WithTransition<int, string, double, bool>("To B", "B"))
            .WithState("B", state => state.WithParameters<int, string, double, bool>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.TryTransition("To B", 42, "hello", 3.14, true, TestContext.Current.CancellationToken)
        );

        // Act
        var result = await stateMachine.TryTransition(
            "To B",
            99,
            "world",
            2.72,
            false,
            TestContext.Current.CancellationToken
        );

        // Assert
        Assert.True(result);
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("B", stateMachine.CurrentState.StateValue);
    }

    [Fact]
    public async Task TryTransition_T1_T2_T3_T4_WhenOnExitThrowsExceptionOnce_ShouldTransitionDifferentlySuccessfully()
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
                        .WithTransition<int, string, double, bool>("To B", "B")
                        .WithTransition<int, string, double, bool>("To C", "C")
            )
            .WithState("B", state => state.WithParameters<int, string, double, bool>())
            .WithState("C", state => state.WithParameters<int, string, double, bool>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.TryTransition("To B", 42, "hello", 3.14, true, TestContext.Current.CancellationToken)
        );

        // Act
        var result = await stateMachine.TryTransition(
            "To C",
            99,
            "world",
            2.72,
            false,
            TestContext.Current.CancellationToken
        );

        // Assert
        Assert.True(result);
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("C", stateMachine.CurrentState.StateValue);
    }

    [Fact]
    public async Task TryTransition_T1_T2_T3_T4_WhenStateMachineOnStateChangeThrowsException_ShouldThrowAndRollback()
    {
        // Arrange
        var exception = new InvalidOperationException("Test exception");
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .OnStateChange((_, _, _) => throw exception)
            .WithState("A", state => state.WithTransition<int, string, double, bool>("To B", "B"))
            .WithState("B", state => state.WithParameters<int, string, double, bool>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var thrownException = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.TryTransition("To B", 42, "hello", 3.14, true, TestContext.Current.CancellationToken)
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
    public async Task TryTransition_T1_T2_T3_T4_WhenStateMachineOnStateChangeThrowsExceptionOnce_ShouldRetrySuccessfully()
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
            .WithState("A", state => state.WithTransition<int, string, double, bool>("To B", "B"))
            .WithState("B", state => state.WithParameters<int, string, double, bool>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.TryTransition("To B", 42, "hello", 3.14, true, TestContext.Current.CancellationToken)
        );

        // Act
        var result = await stateMachine.TryTransition(
            "To B",
            99,
            "world",
            2.72,
            false,
            TestContext.Current.CancellationToken
        );

        // Assert
        Assert.True(result);
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("B", stateMachine.CurrentState.StateValue);
    }

    [Fact]
    public async Task TryTransition_T1_T2_T3_T4_WhenStateMachineOnStateChangeThrowsExceptionOnce_ShouldTransitionDifferentlySuccessfully()
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
                        .WithTransition<int, string, double, bool>("To B", "B")
                        .WithTransition<int, string, double, bool>("To C", "C")
            )
            .WithState("B", state => state.WithParameters<int, string, double, bool>())
            .WithState("C", state => state.WithParameters<int, string, double, bool>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.TryTransition("To B", 42, "hello", 3.14, true, TestContext.Current.CancellationToken)
        );

        // Act
        var result = await stateMachine.TryTransition(
            "To C",
            99,
            "world",
            2.72,
            false,
            TestContext.Current.CancellationToken
        );

        // Assert
        Assert.True(result);
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("C", stateMachine.CurrentState.StateValue);
    }

    [Fact]
    public async Task TryTransition_T1_T2_T3_T4_WhenStateOnStateChangeThrowsException_ShouldThrowAndRollback()
    {
        // Arrange
        var exception = new InvalidOperationException("Test exception");
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition<int, string, double, bool>("To B", "B"))
            .WithState(
                "B",
                state =>
                    state
                        .WithParameters<int, string, double, bool>()
                        .OnStateChange((_, _, _, _, _, _, _) => throw exception)
            )
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var thrownException = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.TryTransition("To B", 42, "hello", 3.14, true, TestContext.Current.CancellationToken)
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
    public async Task TryTransition_T1_T2_T3_T4_WhenStateOnStateChangeThrowsExceptionOnce_ShouldRetrySuccessfully()
    {
        // Arrange
        var callCount = 0;
        var onStateChange = Substitute.For<Action<string, string, string, int, string, double, bool>>();
        onStateChange
            .When(x =>
                x.Invoke(
                    Arg.Any<string>(),
                    Arg.Any<string>(),
                    Arg.Any<string>(),
                    Arg.Any<int>(),
                    Arg.Any<string>(),
                    Arg.Any<double>(),
                    Arg.Any<bool>()
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
            .WithState("A", state => state.WithTransition<int, string, double, bool>("To B", "B"))
            .WithState("B", state => state.WithParameters<int, string, double, bool>().OnStateChange(onStateChange))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.TryTransition("To B", 42, "hello", 3.14, true, TestContext.Current.CancellationToken)
        );

        // Act
        var result = await stateMachine.TryTransition(
            "To B",
            99,
            "world",
            2.72,
            false,
            TestContext.Current.CancellationToken
        );

        // Assert
        Assert.True(result);
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("B", stateMachine.CurrentState.StateValue);
    }

    [Fact]
    public async Task TryTransition_T1_T2_T3_T4_WhenStateOnStateChangeThrowsExceptionOnce_ShouldTransitionDifferentlySuccessfully()
    {
        // Arrange
        var callCount = 0;
        var onStateChange = Substitute.For<Action<string, string, string, int, string, double, bool>>();
        onStateChange
            .When(x =>
                x.Invoke(
                    Arg.Any<string>(),
                    Arg.Any<string>(),
                    Arg.Any<string>(),
                    Arg.Any<int>(),
                    Arg.Any<string>(),
                    Arg.Any<double>(),
                    Arg.Any<bool>()
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
                        .WithTransition<int, string, double, bool>("To B", "B")
                        .WithTransition<int, string, double, bool>("To C", "C")
            )
            .WithState("B", state => state.WithParameters<int, string, double, bool>().OnStateChange(onStateChange))
            .WithState("C", state => state.WithParameters<int, string, double, bool>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.TryTransition("To B", 42, "hello", 3.14, true, TestContext.Current.CancellationToken)
        );

        // Act
        var result = await stateMachine.TryTransition(
            "To C",
            99,
            "world",
            2.72,
            false,
            TestContext.Current.CancellationToken
        );

        // Assert
        Assert.True(result);
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("C", stateMachine.CurrentState.StateValue);
    }

    [Fact]
    public async Task TryTransition_T1_T2_T3_T4_WhenOnEntryThrowsException_ShouldThrowAndCommit()
    {
        // Arrange
        var exception = new InvalidOperationException("Test exception");
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition<int, string, double, bool>("To B", "B"))
            .WithState(
                "B",
                state => state.WithParameters<int, string, double, bool>().OnEntry((_, _, _, _) => throw exception)
            )
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var thrownException = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.TryTransition("To B", 42, "hello", 3.14, true, TestContext.Current.CancellationToken)
        );

        // Assert
        Assert.Same(exception, thrownException);
        Assert.Null(stateMachine.CurrentState);
        Assert.NotNull(stateMachine.NextState);
        Assert.Equal("B", stateMachine.NextState.StateValue);
        Assert.NotNull(stateMachine.PreviousState);
        Assert.Equal("A", stateMachine.PreviousState.StateValue);
        Assert.False(stateMachine.Parameters.IsCurrentSet);
        Assert.Equal((42, "hello", 3.14, true), stateMachine.Parameters.GetNextParameters<int, string, double, bool>());
        Assert.True(stateMachine.Parameters.IsPreviousSet);
        Assert.True(stateMachine.Parameters.IsNextSet);
        Assert.Null(stateMachine.CurrentTransition);
    }

    [Fact]
    public async Task TryTransition_T1_T2_T3_T4_WhenOnEntryThrowsExceptionOnce_ShouldRetrySuccessfully()
    {
        // Arrange
        var callCount = 0;
        var onEntry = Substitute.For<Action<int, string, double, bool>>();
        onEntry
            .When(x => x.Invoke(Arg.Any<int>(), Arg.Any<string>(), Arg.Any<double>(), Arg.Any<bool>()))
            .Do(_ =>
            {
                if (Interlocked.Increment(ref callCount) == 1)
                    throw new InvalidOperationException();
            });
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition<int, string, double, bool>("To B", "B"))
            .WithState(
                "B",
                state => state.WithParameters<int, string, double, bool>().OnEntry(onEntry).WithTransition("To C", "C")
            )
            .WithState("C", state => state)
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.TryTransition("To B", 42, "hello", 3.14, true, TestContext.Current.CancellationToken)
        );

        // Act
        var result = await stateMachine.TryTransition("To C", TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result);
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("C", stateMachine.CurrentState.StateValue);
    }

    [Fact]
    public async Task TryTransition_T1_T2_T3_T4_WhenNextParameterConditionThrowsException_ShouldThrowAndRemainInCurrentState()
    {
        // Arrange
        var exception = new InvalidOperationException("Test exception");
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState(
                "A",
                state =>
                    state.WithTransition(
                        "To B",
                        t => t.WithParameters<int, string, double, bool>().If((_, _, _, _) => throw exception).To("B")
                    )
            )
            .WithState("B", state => state.WithParameters<int, string, double, bool>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var thrownException = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.TryTransition("To B", 42, "hello", 3.14, true, TestContext.Current.CancellationToken)
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
    public async Task TryTransition_T1_T2_T3_T4_WhenNextParameterConditionThrowsExceptionOnce_ShouldRetrySuccessfully()
    {
        // Arrange
        var callCount = 0;
        var condition = Substitute.For<Func<int, string, double, bool, bool>>();
        condition
            .Invoke(Arg.Any<int>(), Arg.Any<string>(), Arg.Any<double>(), Arg.Any<bool>())
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
                    state.WithTransition(
                        "To B",
                        t => t.WithParameters<int, string, double, bool>().If(condition).To("B")
                    )
            )
            .WithState("B", state => state.WithParameters<int, string, double, bool>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.TryTransition("To B", 42, "hello", 3.14, true, TestContext.Current.CancellationToken)
        );

        // Act
        var result = await stateMachine.TryTransition(
            "To B",
            99,
            "world",
            2.72,
            false,
            TestContext.Current.CancellationToken
        );

        // Assert
        Assert.True(result);
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("B", stateMachine.CurrentState.StateValue);
    }

    [Fact]
    public async Task TryTransition_T1_T2_T3_T4_WhenNextParameterConditionThrowsExceptionOnce_ShouldTransitionDifferentlySuccessfully()
    {
        // Arrange
        var callCount = 0;
        var condition = Substitute.For<Func<int, string, double, bool, bool>>();
        condition
            .Invoke(Arg.Any<int>(), Arg.Any<string>(), Arg.Any<double>(), Arg.Any<bool>())
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
                        .WithTransition(
                            "To B",
                            t => t.WithParameters<int, string, double, bool>().If(condition).To("B")
                        )
                        .WithTransition<int, string, double, bool>("To C", "C")
            )
            .WithState("B", state => state.WithParameters<int, string, double, bool>())
            .WithState("C", state => state.WithParameters<int, string, double, bool>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.TryTransition("To B", 42, "hello", 3.14, true, TestContext.Current.CancellationToken)
        );

        // Act
        var result = await stateMachine.TryTransition(
            "To C",
            99,
            "world",
            2.72,
            false,
            TestContext.Current.CancellationToken
        );

        // Assert
        Assert.True(result);
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("C", stateMachine.CurrentState.StateValue);
    }
}
