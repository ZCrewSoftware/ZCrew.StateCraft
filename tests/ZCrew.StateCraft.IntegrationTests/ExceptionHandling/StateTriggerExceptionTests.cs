using NSubstitute;

namespace ZCrew.StateCraft.IntegrationTests.ExceptionHandling;

public class StateTriggerExceptionTests
{
    [Fact(Timeout = 5000)]
    public async Task StateScopedTrigger_WhenSignalThrows_ShouldCallExceptionHandler()
    {
        // Arrange
        var signal = new TaskCompletionSource();
        var exception = new InvalidOperationException("Signal error");
        var handlerCalled = new TaskCompletionSource();
        var exceptionHandler = Substitute.For<Action<ExceptionContext>>();
        exceptionHandler.When(h => h.Invoke(Arg.Any<ExceptionContext>())).Do(_ => handlerCalled.TrySetResult());

        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .OnException(exceptionHandler)
            .WithState(
                "A",
                state => state.WithTrigger(t => t.Once().Await(signal.Task.WaitAsync).ThenInvoke(() => { }))
            )
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        signal.SetException(exception);
        await handlerCalled.Task;

        // Assert
        exceptionHandler.Received(1).Invoke(Arg.Is<ExceptionContext>(ctx => ctx.Exception == exception));
    }

    [Fact(Timeout = 5000)]
    public async Task StateScopedTrigger_WhenActionThrows_ShouldCallExceptionHandler()
    {
        // Arrange
        var signal = new TaskCompletionSource();
        var exception = new InvalidOperationException("Trigger error");
        var handlerCalled = new TaskCompletionSource();
        var exceptionHandler = Substitute.For<Action<ExceptionContext>>();
        exceptionHandler.When(h => h.Invoke(Arg.Any<ExceptionContext>())).Do(_ => handlerCalled.TrySetResult());

        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .OnException(exceptionHandler)
            .WithState(
                "A",
                state => state.WithTrigger(t => t.Once().Await(() => { }).ThenInvoke(signal.Task.WaitAsync))
            )
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        signal.SetException(exception);
        await handlerCalled.Task;

        // Assert
        exceptionHandler.Received(1).Invoke(Arg.Is<ExceptionContext>(ctx => ctx.Exception == exception));
    }

    [Fact(Timeout = 5000)]
    public async Task StateScopedTrigger_WhenCancelled_ShouldNotCallExceptionHandler()
    {
        // Arrange
        var exceptionHandler = Substitute.For<Action<ExceptionContext>>();

        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .OnException(exceptionHandler)
            .WithState(
                "A",
                state =>
                    state
                        .WithTrigger(t =>
                            t.Once().Await(token => Task.Delay(Timeout.Infinite, token)).ThenInvoke(() => { })
                        )
                        .WithTransition("ToB", "B")
            )
            .WithState("B", state => state)
            .Build();

        var token = TestContext.Current.CancellationToken;
        await stateMachine.Activate(token);

        // Act
        await stateMachine.Transition("ToB", token);

        // Assert
        exceptionHandler.DidNotReceive().Invoke(Arg.Any<ExceptionContext>());
    }
}
