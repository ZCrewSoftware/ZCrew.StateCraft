using NSubstitute;
using ZCrew.StateCraft.Extensions;

namespace ZCrew.StateCraft.IntegrationTests.ExceptionHandling;

public class TryTransitionExceptionTests
{
    [Fact]
    public async Task TryTransition_WhenOnExitThrowsException_ShouldThrowAndRollback()
    {
        // Arrange
        var exception = new InvalidOperationException("Test exception");
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.OnExit(() => throw exception).WithTransition("To B", "B"))
            .WithState("B", state => state)
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var thrownException = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.TryTransition("To B", TestContext.Current.CancellationToken)
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
    public async Task TryTransition_WhenOnExitThrowsExceptionOnce_ShouldRetrySuccessfully()
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
            .WithState("A", state => state.OnExit(onExit).WithTransition("To B", "B"))
            .WithState("B", state => state)
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.TryTransition("To B", TestContext.Current.CancellationToken)
        );

        // Act
        var result = await stateMachine.TryTransition("To B", TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result);
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("B", stateMachine.CurrentState.StateValue);
    }

    [Fact]
    public async Task TryTransition_WhenOnExitThrowsExceptionOnce_ShouldTransitionDifferentlySuccessfully()
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
            .WithState("A", state => state.OnExit(onExit).WithTransition("To B", "B").WithTransition("To C", "C"))
            .WithState("B", state => state)
            .WithState("C", state => state)
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.TryTransition("To B", TestContext.Current.CancellationToken)
        );

        // Act
        var result = await stateMachine.TryTransition("To C", TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result);
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("C", stateMachine.CurrentState.StateValue);
    }

    [Fact]
    public async Task TryTransition_WhenStateMachineOnStateChangeThrowsException_ShouldThrowAndRollback()
    {
        // Arrange
        var exception = new InvalidOperationException("Test exception");
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .OnStateChange((_, _, _) => throw exception)
            .WithState("A", state => state.WithTransition("To B", "B"))
            .WithState("B", state => state)
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var thrownException = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.TryTransition("To B", TestContext.Current.CancellationToken)
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
    public async Task TryTransition_WhenStateMachineOnStateChangeThrowsExceptionOnce_ShouldRetrySuccessfully()
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
            .WithState("A", state => state.WithTransition("To B", "B"))
            .WithState("B", state => state)
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.TryTransition("To B", TestContext.Current.CancellationToken)
        );

        // Act
        var result = await stateMachine.TryTransition("To B", TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result);
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("B", stateMachine.CurrentState.StateValue);
    }

    [Fact]
    public async Task TryTransition_WhenStateMachineOnStateChangeThrowsExceptionOnce_ShouldTransitionDifferentlySuccessfully()
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
            .WithState("A", state => state.WithTransition("To B", "B").WithTransition("To C", "C"))
            .WithState("B", state => state)
            .WithState("C", state => state)
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.TryTransition("To B", TestContext.Current.CancellationToken)
        );

        // Act
        var result = await stateMachine.TryTransition("To C", TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result);
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("C", stateMachine.CurrentState.StateValue);
    }

    [Fact]
    public async Task TryTransition_WhenStateOnStateChangeThrowsException_ShouldThrowAndRollback()
    {
        // Arrange
        var exception = new InvalidOperationException("Test exception");
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition("To B", "B"))
            .WithState("B", state => state.OnStateChange((_, _, _) => throw exception))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var thrownException = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.TryTransition("To B", TestContext.Current.CancellationToken)
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
    public async Task TryTransition_WhenStateOnStateChangeThrowsExceptionOnce_ShouldRetrySuccessfully()
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
            .WithState("A", state => state.WithTransition("To B", "B"))
            .WithState("B", state => state.OnStateChange(onStateChange))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.TryTransition("To B", TestContext.Current.CancellationToken)
        );

        // Act
        var result = await stateMachine.TryTransition("To B", TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result);
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("B", stateMachine.CurrentState.StateValue);
    }

    [Fact]
    public async Task TryTransition_WhenStateOnStateChangeThrowsExceptionOnce_ShouldTransitionDifferentlySuccessfully()
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
            .WithState("A", state => state.WithTransition("To B", "B").WithTransition("To C", "C"))
            .WithState("B", state => state.OnStateChange(onStateChange))
            .WithState("C", state => state)
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.TryTransition("To B", TestContext.Current.CancellationToken)
        );

        // Act
        var result = await stateMachine.TryTransition("To C", TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result);
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("C", stateMachine.CurrentState.StateValue);
    }

    [Fact]
    public async Task TryTransition_WhenOnEntryThrowsException_ShouldThrowAndCommit()
    {
        // Arrange
        var exception = new InvalidOperationException("Test exception");
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition("To B", "B"))
            .WithState("B", state => state.OnEntry(() => throw exception))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var thrownException = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.TryTransition("To B", TestContext.Current.CancellationToken)
        );

        // Assert
        Assert.Same(exception, thrownException);
        Assert.Null(stateMachine.CurrentState);
        Assert.NotNull(stateMachine.NextState);
        Assert.Equal("B", stateMachine.NextState.StateValue);
        Assert.NotNull(stateMachine.PreviousState);
        Assert.Equal("A", stateMachine.PreviousState.StateValue);
        Assert.False(stateMachine.Parameters.IsCurrentSet);
        Assert.True(stateMachine.Parameters.IsPreviousSet);
        Assert.True(stateMachine.Parameters.IsNextSet);
        Assert.Null(stateMachine.CurrentTransition);
    }

    [Fact]
    public async Task TryTransition_WhenOnEntryThrowsExceptionOnce_ShouldRetrySuccessfully()
    {
        // Arrange
        var callCount = 0;
        var onEntry = Substitute.For<Action>();
        onEntry
            .When(x => x.Invoke())
            .Do(_ =>
            {
                if (Interlocked.Increment(ref callCount) == 1)
                    throw new InvalidOperationException();
            });
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition("To B", "B"))
            .WithState("B", state => state.OnEntry(onEntry).WithTransition("To C", "C"))
            .WithState("C", state => state)
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.TryTransition("To B", TestContext.Current.CancellationToken)
        );

        // Act
        var result = await stateMachine.TryTransition("To C", TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result);
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("C", stateMachine.CurrentState.StateValue);
    }

    [Fact]
    public async Task TryTransition_WhenConditionThrowsException_ShouldThrow()
    {
        // Arrange
        var exception = new InvalidOperationException("Test exception");
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition("To B", t => t.If(() => throw exception).To("B")))
            .WithState("B", state => state)
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var thrownException = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.TryTransition("To B", TestContext.Current.CancellationToken)
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
    public async Task TryTransition_WhenConditionThrowsExceptionOnce_ShouldRetrySuccessfully()
    {
        // Arrange
        var callCount = 0;
        var condition = Substitute.For<Func<bool>>();
        condition
            .Invoke()
            .Returns(_ =>
            {
                if (Interlocked.Increment(ref callCount) == 1)
                    throw new InvalidOperationException();
                return true;
            });

        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition("To B", t => t.If(condition).To("B")))
            .WithState("B", state => state)
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.TryTransition("To B", TestContext.Current.CancellationToken)
        );

        // Act
        var result = await stateMachine.TryTransition("To B", TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result);
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("B", stateMachine.CurrentState.StateValue);
    }
}
