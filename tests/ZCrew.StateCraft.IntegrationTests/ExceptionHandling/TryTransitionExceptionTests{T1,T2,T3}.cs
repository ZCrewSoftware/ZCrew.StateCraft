using NSubstitute;
using ZCrew.StateCraft.Extensions;

namespace ZCrew.StateCraft.IntegrationTests.ExceptionHandling;

public class TryTransitionExceptionTests_T1_T2_T3
{
    [Fact]
    public async Task TryTransition_T1_T2_T3_WhenOnExitThrowsException_ShouldThrowAndRollback()
    {
        // Arrange
        var exception = new InvalidOperationException("Test exception");
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.OnExit(() => throw exception).WithTransition<int, string, bool>("To B", "B"))
            .WithState("B", state => state.WithParameters<int, string, bool>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var thrownException = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.TryTransition("To B", 42, "hello", true, TestContext.Current.CancellationToken)
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
    public async Task TryTransition_T1_T2_T3_WhenOnExitThrowsExceptionOnce_ShouldRetrySuccessfully()
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
            .WithState("A", state => state.OnExit(onExit).WithTransition<int, string, bool>("To B", "B"))
            .WithState("B", state => state.WithParameters<int, string, bool>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.TryTransition("To B", 42, "hello", true, TestContext.Current.CancellationToken)
        );

        // Act
        var result = await stateMachine.TryTransition(
            "To B",
            99,
            "world",
            false,
            TestContext.Current.CancellationToken
        );

        // Assert
        Assert.True(result);
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("B", stateMachine.CurrentState.StateValue);
    }

    [Fact]
    public async Task TryTransition_T1_T2_T3_WhenOnExitThrowsExceptionOnce_ShouldTransitionDifferentlySuccessfully()
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
                        .WithTransition<int, string, bool>("To B", "B")
                        .WithTransition<int, string, bool>("To C", "C")
            )
            .WithState("B", state => state.WithParameters<int, string, bool>())
            .WithState("C", state => state.WithParameters<int, string, bool>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.TryTransition("To B", 42, "hello", true, TestContext.Current.CancellationToken)
        );

        // Act
        var result = await stateMachine.TryTransition(
            "To C",
            99,
            "world",
            false,
            TestContext.Current.CancellationToken
        );

        // Assert
        Assert.True(result);
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("C", stateMachine.CurrentState.StateValue);
    }

    [Fact]
    public async Task TryTransition_T1_T2_T3_WhenStateMachineOnStateChangeThrowsException_ShouldThrowAndRollback()
    {
        // Arrange
        var exception = new InvalidOperationException("Test exception");
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .OnStateChange((_, _, _) => throw exception)
            .WithState("A", state => state.WithTransition<int, string, bool>("To B", "B"))
            .WithState("B", state => state.WithParameters<int, string, bool>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var thrownException = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.TryTransition("To B", 42, "hello", true, TestContext.Current.CancellationToken)
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
    public async Task TryTransition_T1_T2_T3_WhenStateMachineOnStateChangeThrowsExceptionOnce_ShouldRetrySuccessfully()
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
            .WithState("A", state => state.WithTransition<int, string, bool>("To B", "B"))
            .WithState("B", state => state.WithParameters<int, string, bool>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.TryTransition("To B", 42, "hello", true, TestContext.Current.CancellationToken)
        );

        // Act
        var result = await stateMachine.TryTransition(
            "To B",
            99,
            "world",
            false,
            TestContext.Current.CancellationToken
        );

        // Assert
        Assert.True(result);
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("B", stateMachine.CurrentState.StateValue);
    }

    [Fact]
    public async Task TryTransition_T1_T2_T3_WhenStateMachineOnStateChangeThrowsExceptionOnce_ShouldTransitionDifferentlySuccessfully()
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
                    state.WithTransition<int, string, bool>("To B", "B").WithTransition<int, string, bool>("To C", "C")
            )
            .WithState("B", state => state.WithParameters<int, string, bool>())
            .WithState("C", state => state.WithParameters<int, string, bool>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.TryTransition("To B", 42, "hello", true, TestContext.Current.CancellationToken)
        );

        // Act
        var result = await stateMachine.TryTransition(
            "To C",
            99,
            "world",
            false,
            TestContext.Current.CancellationToken
        );

        // Assert
        Assert.True(result);
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("C", stateMachine.CurrentState.StateValue);
    }

    [Fact]
    public async Task TryTransition_T1_T2_T3_WhenStateOnStateChangeThrowsException_ShouldThrowAndRollback()
    {
        // Arrange
        var exception = new InvalidOperationException("Test exception");
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition<int, string, bool>("To B", "B"))
            .WithState(
                "B",
                state => state.WithParameters<int, string, bool>().OnStateChange((_, _, _, _, _, _) => throw exception)
            )
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var thrownException = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.TryTransition("To B", 42, "hello", true, TestContext.Current.CancellationToken)
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
    public async Task TryTransition_T1_T2_T3_WhenStateOnStateChangeThrowsExceptionOnce_ShouldRetrySuccessfully()
    {
        // Arrange
        var callCount = 0;
        var onStateChange = Substitute.For<Action<string, string, string, int, string, bool>>();
        onStateChange
            .When(x =>
                x.Invoke(
                    Arg.Any<string>(),
                    Arg.Any<string>(),
                    Arg.Any<string>(),
                    Arg.Any<int>(),
                    Arg.Any<string>(),
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
            .WithState("A", state => state.WithTransition<int, string, bool>("To B", "B"))
            .WithState("B", state => state.WithParameters<int, string, bool>().OnStateChange(onStateChange))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.TryTransition("To B", 42, "hello", true, TestContext.Current.CancellationToken)
        );

        // Act
        var result = await stateMachine.TryTransition(
            "To B",
            99,
            "world",
            false,
            TestContext.Current.CancellationToken
        );

        // Assert
        Assert.True(result);
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("B", stateMachine.CurrentState.StateValue);
    }

    [Fact]
    public async Task TryTransition_T1_T2_T3_WhenStateOnStateChangeThrowsExceptionOnce_ShouldTransitionDifferentlySuccessfully()
    {
        // Arrange
        var callCount = 0;
        var onStateChange = Substitute.For<Action<string, string, string, int, string, bool>>();
        onStateChange
            .When(x =>
                x.Invoke(
                    Arg.Any<string>(),
                    Arg.Any<string>(),
                    Arg.Any<string>(),
                    Arg.Any<int>(),
                    Arg.Any<string>(),
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
                    state.WithTransition<int, string, bool>("To B", "B").WithTransition<int, string, bool>("To C", "C")
            )
            .WithState("B", state => state.WithParameters<int, string, bool>().OnStateChange(onStateChange))
            .WithState("C", state => state.WithParameters<int, string, bool>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.TryTransition("To B", 42, "hello", true, TestContext.Current.CancellationToken)
        );

        // Act
        var result = await stateMachine.TryTransition(
            "To C",
            99,
            "world",
            false,
            TestContext.Current.CancellationToken
        );

        // Assert
        Assert.True(result);
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("C", stateMachine.CurrentState.StateValue);
    }

    [Fact]
    public async Task TryTransition_T1_T2_T3_WhenOnEntryThrowsException_ShouldThrowAndRollback()
    {
        // Arrange
        var exception = new InvalidOperationException("Test exception");
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition<int, string, bool>("To B", "B"))
            .WithState("B", state => state.WithParameters<int, string, bool>().OnEntry((_, _, _) => throw exception))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var thrownException = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.TryTransition("To B", 42, "hello", true, TestContext.Current.CancellationToken)
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
    public async Task TryTransition_T1_T2_T3_WhenOnEntryThrowsExceptionOnce_ShouldRetrySuccessfully()
    {
        // Arrange
        var callCount = 0;
        var onEntry = Substitute.For<Action<int, string, bool>>();
        onEntry
            .When(x => x.Invoke(Arg.Any<int>(), Arg.Any<string>(), Arg.Any<bool>()))
            .Do(_ =>
            {
                if (Interlocked.Increment(ref callCount) == 1)
                    throw new InvalidOperationException();
            });
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition<int, string, bool>("To B", "B"))
            .WithState("B", state => state.WithParameters<int, string, bool>().OnEntry(onEntry))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.TryTransition("To B", 42, "hello", true, TestContext.Current.CancellationToken)
        );

        // Act
        var result = await stateMachine.TryTransition(
            "To B",
            99,
            "world",
            false,
            TestContext.Current.CancellationToken
        );

        // Assert
        Assert.True(result);
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("B", stateMachine.CurrentState.StateValue);
    }

    [Fact]
    public async Task TryTransition_T1_T2_T3_WhenOnEntryThrowsExceptionOnce_ShouldTransitionDifferentlySuccessfully()
    {
        // Arrange
        var callCount = 0;
        var onEntry = Substitute.For<Action<int, string, bool>>();
        onEntry
            .When(x => x.Invoke(Arg.Any<int>(), Arg.Any<string>(), Arg.Any<bool>()))
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
                    state.WithTransition<int, string, bool>("To B", "B").WithTransition<int, string, bool>("To C", "C")
            )
            .WithState("B", state => state.WithParameters<int, string, bool>().OnEntry(onEntry))
            .WithState("C", state => state.WithParameters<int, string, bool>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.TryTransition("To B", 42, "hello", true, TestContext.Current.CancellationToken)
        );

        // Act
        var result = await stateMachine.TryTransition(
            "To C",
            99,
            "world",
            false,
            TestContext.Current.CancellationToken
        );

        // Assert
        Assert.True(result);
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("C", stateMachine.CurrentState.StateValue);
    }

    [Fact]
    public async Task TryTransition_T1_T2_T3_WhenNextParameterConditionThrowsException_ShouldThrowAndRemainInCurrentState()
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
                        t => t.WithParameters<int, string, bool>().If((_, _, _) => throw exception).To("B")
                    )
            )
            .WithState("B", state => state.WithParameters<int, string, bool>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var thrownException = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.TryTransition("To B", 42, "hello", true, TestContext.Current.CancellationToken)
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
    public async Task TryTransition_T1_T2_T3_WhenNextParameterConditionThrowsExceptionOnce_ShouldRetrySuccessfully()
    {
        // Arrange
        var callCount = 0;
        var condition = Substitute.For<Func<int, string, bool, bool>>();
        condition
            .Invoke(Arg.Any<int>(), Arg.Any<string>(), Arg.Any<bool>())
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
                state => state.WithTransition("To B", t => t.WithParameters<int, string, bool>().If(condition).To("B"))
            )
            .WithState("B", state => state.WithParameters<int, string, bool>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.TryTransition("To B", 42, "hello", true, TestContext.Current.CancellationToken)
        );

        // Act
        var result = await stateMachine.TryTransition(
            "To B",
            99,
            "world",
            false,
            TestContext.Current.CancellationToken
        );

        // Assert
        Assert.True(result);
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("B", stateMachine.CurrentState.StateValue);
    }

    [Fact]
    public async Task TryTransition_T1_T2_T3_WhenNextParameterConditionThrowsExceptionOnce_ShouldTransitionDifferentlySuccessfully()
    {
        // Arrange
        var callCount = 0;
        var condition = Substitute.For<Func<int, string, bool, bool>>();
        condition
            .Invoke(Arg.Any<int>(), Arg.Any<string>(), Arg.Any<bool>())
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
                        .WithTransition("To B", t => t.WithParameters<int, string, bool>().If(condition).To("B"))
                        .WithTransition<int, string, bool>("To C", "C")
            )
            .WithState("B", state => state.WithParameters<int, string, bool>())
            .WithState("C", state => state.WithParameters<int, string, bool>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.TryTransition("To B", 42, "hello", true, TestContext.Current.CancellationToken)
        );

        // Act
        var result = await stateMachine.TryTransition(
            "To C",
            99,
            "world",
            false,
            TestContext.Current.CancellationToken
        );

        // Assert
        Assert.True(result);
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("C", stateMachine.CurrentState.StateValue);
    }
}
