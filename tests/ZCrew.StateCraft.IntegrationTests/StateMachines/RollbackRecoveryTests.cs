namespace ZCrew.StateCraft.IntegrationTests.StateMachines;

public class RollbackRecoveryTests
{
    [Fact]
    public async Task CanTransition_WhenInRecovery_ShouldReturnCorrectResult()
    {
        // Arrange
        var callCount = 0;
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState(
                "A",
                state =>
                    state
                        .OnExit(OnExit)
                        .WithTransition("To B", t => t.To("B"))
                        .WithTransition("To C", t => t.To("C"))
            )
            .WithState("B", state => state)
            .WithState("C", state => state)
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => stateMachine.Transition("To B", TestContext.Current.CancellationToken)
        );

        // Act
        var canTransitionToB = await stateMachine.CanTransition("To B", TestContext.Current.CancellationToken);
        var canTransitionToC = await stateMachine.CanTransition("To C", TestContext.Current.CancellationToken);
        var canTransitionInvalid = await stateMachine.CanTransition("To Z", TestContext.Current.CancellationToken);

        // Assert
        Assert.True(canTransitionToB);
        Assert.True(canTransitionToC);
        Assert.False(canTransitionInvalid);

        return;

        Task OnExit(CancellationToken _)
        {
            if (Interlocked.Increment(ref callCount) == 1)
                throw new InvalidOperationException("Exit failure");
            return Task.CompletedTask;
        }
    }

    [Fact]
    public async Task TryTransition_WhenInRecovery_ShouldSucceed()
    {
        // Arrange
        var callCount = 0;
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState(
                "A",
                state =>
                    state
                        .OnExit(OnExit)
                        .WithTransition("To B", t => t.To("B"))
            )
            .WithState("B", state => state)
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => stateMachine.Transition("To B", TestContext.Current.CancellationToken)
        );

        // Act
        var result = await stateMachine.TryTransition("To B", TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result);
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("B", stateMachine.CurrentState.StateValue);

        return;

        Task OnExit(CancellationToken _)
        {
            if (Interlocked.Increment(ref callCount) == 1)
                throw new InvalidOperationException("Exit failure");
            return Task.CompletedTask;
        }
    }

    [Fact]
    public async Task Deactivate_WhenInRecovery_ShouldSucceed()
    {
        // Arrange
        var callCount = 0;
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState(
                "A",
                state =>
                    state
                        .OnExit(OnExit)
                        .WithTransition("To B", t => t.To("B"))
            )
            .WithState("B", state => state)
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => stateMachine.Transition("To B", TestContext.Current.CancellationToken)
        );

        // Act
        await stateMachine.Deactivate(TestContext.Current.CancellationToken);

        // Assert
        Assert.Null(stateMachine.CurrentState);

        return;

        Task OnExit(CancellationToken _)
        {
            if (Interlocked.Increment(ref callCount) == 1)
                throw new InvalidOperationException("Exit failure");
            return Task.CompletedTask;
        }
    }
}
