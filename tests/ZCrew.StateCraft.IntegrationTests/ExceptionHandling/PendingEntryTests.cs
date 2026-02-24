using NSubstitute;

namespace ZCrew.StateCraft.IntegrationTests.ExceptionHandling;

public class PendingEntryTests
{
    [Fact]
    public async Task Transition_WhenOnEntryThrows_ShouldCommitToTargetStateWithParameters()
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
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.Transition("To B", "hello", TestContext.Current.CancellationToken)
        );

        // Assert
        Assert.Null(stateMachine.CurrentState);
        Assert.NotNull(stateMachine.NextState);
        Assert.Equal("B", stateMachine.NextState.StateValue);
        Assert.Equal("hello", stateMachine.Parameters.GetNextParameter<string>());
        Assert.NotNull(stateMachine.PreviousState);
        Assert.Equal("A", stateMachine.PreviousState.StateValue);
    }

    [Fact]
    public async Task Transition_WhenInPendingEntry_ShouldRetryEntryThenTransition()
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
            stateMachine.Transition("To B", TestContext.Current.CancellationToken)
        );

        // Act
        await stateMachine.Transition("To C", TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("C", stateMachine.CurrentState.StateValue);
        Assert.Equal(2, callCount);
    }

    [Fact]
    public async Task Transition_WhenRetryEntryFailsAgain_ShouldStayInPendingEntry()
    {
        // Arrange
        var onEntry = Substitute.For<Action>();
        onEntry.When(x => x.Invoke()).Do(_ => throw new InvalidOperationException());
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition("To B", "B"))
            .WithState("B", state => state.OnEntry(onEntry).WithTransition("To C", "C"))
            .WithState("C", state => state)
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.Transition("To B", TestContext.Current.CancellationToken)
        );

        // Act
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.Transition("To C", TestContext.Current.CancellationToken)
        );

        // Assert
        Assert.Null(stateMachine.CurrentState);
        Assert.NotNull(stateMachine.NextState);
        Assert.Equal("B", stateMachine.NextState.StateValue);
    }

    [Fact]
    public async Task Deactivate_WhenInPendingEntry_ShouldDeactivateWithoutOnExitOrOnDeactivate()
    {
        // Arrange
        var onExit = Substitute.For<Action>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition("To B", "B"))
            .WithState("B", state => state.OnExit(onExit).OnEntry(() => throw new InvalidOperationException()))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.Transition("To B", TestContext.Current.CancellationToken)
        );

        // Act
        await stateMachine.Deactivate(TestContext.Current.CancellationToken);

        // Assert
        Assert.Null(stateMachine.CurrentState);
        onExit.DidNotReceive().Invoke();
    }

    [Fact]
    public async Task Deactivate_ThenActivate_WhenInPendingEntry_ShouldReenterInitialState()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition("To B", "B"))
            .WithState("B", state => state.OnEntry(() => throw new InvalidOperationException()))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.Transition("To B", TestContext.Current.CancellationToken)
        );
        await stateMachine.Deactivate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("A", stateMachine.CurrentState.StateValue);
    }

    [Fact]
    public async Task CanTransition_WhenInPendingEntry_ShouldRetryEntryThenReturnTrue()
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
            stateMachine.Transition("To B", TestContext.Current.CancellationToken)
        );

        // Act
        var result = await stateMachine.CanTransition("To C", TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result);
        Assert.Equal(2, callCount);
    }

    [Fact]
    public async Task TryTransition_WhenInPendingEntry_ShouldRetryEntryThenTransition()
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
    public async Task Transition_WhenOnExitThrows_ShouldStillRollbackToRecovery()
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
            stateMachine.Transition("To B", TestContext.Current.CancellationToken)
        );

        // Assert
        Assert.Same(exception, thrownException);
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("A", stateMachine.CurrentState.StateValue);
    }

    [Fact]
    public async Task Transition_WhenOnStateChangeThrows_ShouldStillRollbackToRecovery()
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
            stateMachine.Transition("To B", TestContext.Current.CancellationToken)
        );

        // Assert
        Assert.Same(exception, thrownException);
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("A", stateMachine.CurrentState.StateValue);
    }
}
