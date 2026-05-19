using NSubstitute;

namespace ZCrew.StateCraft.IntegrationTests.ExceptionHandling;

public class OnStateChangeExceptionTests
{
    [Fact]
    public async Task Transition_WhenOnStateChangeThrows_ShouldRouteExceptionThroughExceptionHandler()
    {
        // Arrange
        var exception = new InvalidOperationException("StateChange error");
        var exceptionHandler = Substitute.For<Func<Exception, ExceptionResult>>();
        exceptionHandler.Invoke(Arg.Any<Exception>()).Returns(ExceptionResult.Rethrow());

        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .OnException(exceptionHandler)
            .OnStateChange((_, _, _) => throw exception)
            .WithState("A", state => state.WithTransition("To B", "B"))
            .WithState("B", state => state)
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => stateMachine.Transition("To B", TestContext.Current.CancellationToken)
        );

        // Assert
        exceptionHandler.Received(1).Invoke(exception);
    }

    [Fact]
    public async Task Transition_WhenOnStateChangeThrowsOnce_ShouldRemainOperationalAfterRecovery()
    {
        // Arrange
        var callCount = 0;
        var onStateChange = Substitute.For<Action<string, string, string>>();
        onStateChange
            .When(x => x.Invoke(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>()))
            .Do(_ =>
            {
                if (Interlocked.Increment(ref callCount) == 1)
                    throw new InvalidOperationException("First transition fails");
            });

        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .OnStateChange(onStateChange)
            .WithState("A", state => state.WithTransition("To B", "B"))
            .WithState("B", state => state)
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => stateMachine.Transition("To B", TestContext.Current.CancellationToken)
        );

        // Act — machine should still accept transitions after recovery
        await stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("B", stateMachine.CurrentState.StateValue);
    }
}
