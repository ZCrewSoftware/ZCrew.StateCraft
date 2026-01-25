using NSubstitute;

namespace ZCrew.StateCraft.IntegrationTests.ExceptionHandling;

public class TriggerExceptionTests
{
    [Fact(Timeout = 5000)]
    public async Task Trigger_WhenSignalThrowsException_ShouldCallExceptionHandler()
    {
        // Arrange
        var signal = new TaskCompletionSource();
        var exception = new InvalidOperationException("Signal error");
        var handlerCalled = new TaskCompletionSource();
        var exceptionHandler = Substitute.For<Func<Exception, ExceptionResult>>();
        exceptionHandler
            .Invoke(Arg.Any<Exception>())
            .Returns(_ =>
            {
                handlerCalled.TrySetResult();
                return ExceptionResult.Continue();
            });

        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .OnException(exceptionHandler)
            .WithState("A", state => state)
            .WithTrigger(trigger => trigger.Once().Await(signal.Task.WaitAsync).ThenInvoke(() => { }))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        signal.SetException(exception);
        await handlerCalled.Task;

        // Assert
        exceptionHandler.Received(1).Invoke(exception);
    }

    [Fact(Timeout = 5000)]
    public async Task Trigger_WhenTriggerActionThrowsException_ShouldCallExceptionHandler()
    {
        // Arrange
        var signal = new TaskCompletionSource();
        var exception = new InvalidOperationException("Trigger error");
        var handlerCalled = new TaskCompletionSource();
        var exceptionHandler = Substitute.For<Func<Exception, ExceptionResult>>();
        exceptionHandler
            .Invoke(Arg.Any<Exception>())
            .Returns(_ =>
            {
                handlerCalled.TrySetResult();
                return ExceptionResult.Continue();
            });

        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .OnException(exceptionHandler)
            .WithState("A", state => state)
            .WithTrigger(trigger => trigger.Once().Await(() => { }).ThenInvoke(signal.Task.WaitAsync))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        signal.SetException(exception);
        await handlerCalled.Task;

        // Assert
        exceptionHandler.Received(1).Invoke(exception);
    }

    [Fact(Timeout = 5000)]
    public async Task Trigger_WhenSignalThrowsSynchronously_ShouldCallExceptionHandler()
    {
        // Arrange
        var exception = new InvalidOperationException("Immediate signal error");
        var handlerCalled = new TaskCompletionSource();
        var exceptionHandler = Substitute.For<Func<Exception, ExceptionResult>>();
        exceptionHandler
            .Invoke(Arg.Any<Exception>())
            .Returns(_ =>
            {
                handlerCalled.TrySetResult();
                return ExceptionResult.Continue();
            });

        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .OnException(exceptionHandler)
            .WithState("A", state => state)
            .WithTrigger(trigger => trigger.Once().Await(() => throw exception).ThenInvoke(() => { }))
            .Build();
        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await handlerCalled.Task;

        // Assert
        exceptionHandler.Received(1).Invoke(exception);
    }

    [Fact(Timeout = 5000)]
    public async Task Trigger_WhenCancelled_ShouldNotCallExceptionHandler()
    {
        // Arrange
        var exceptionHandler = Substitute.For<Func<Exception, ExceptionResult>>();
        exceptionHandler.Invoke(Arg.Any<Exception>()).Returns(ExceptionResult.Continue());

        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .OnException(exceptionHandler)
            .WithState("A", state => state)
            .WithTrigger(trigger =>
                trigger.Once().Await(token => Task.Delay(Timeout.Infinite, token)).ThenInvoke(() => { })
            )
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Deactivate(TestContext.Current.CancellationToken);

        // Assert
        exceptionHandler.DidNotReceive().Invoke(Arg.Any<Exception>());
    }

    [Fact(Timeout = 5000)]
    public async Task RepeatingTrigger_WhenExceptionThrown_ShouldStopRepeating()
    {
        // Arrange
        var signalGate = new SemaphoreSlim(0);
        var triggerGate = new SemaphoreSlim(0);
        var triggerCount = 0;
        var exception = new InvalidOperationException("Stop repeating");
        var handlerCalled = new TaskCompletionSource();
        var exceptionHandler = Substitute.For<Func<Exception, ExceptionResult>>();
        exceptionHandler
            .Invoke(Arg.Any<Exception>())
            .Returns(_ =>
            {
                handlerCalled.TrySetResult();
                return ExceptionResult.Continue();
            });

        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .OnException(exceptionHandler)
            .WithState("A", state => state)
            .WithTrigger(trigger =>
                trigger
                    .Repeat()
                    .Await(signalGate.WaitAsync)
                    .ThenInvoke(() =>
                    {
                        triggerCount++;
                        triggerGate.Release();
                        if (triggerCount == 2)
                        {
                            throw exception;
                        }
                    })
            )
            .Build();

        var token = TestContext.Current.CancellationToken;
        await stateMachine.Activate(token);

        // Act
        signalGate.Release();
        await triggerGate.WaitAsync(token);
        signalGate.Release();
        await triggerGate.WaitAsync(token);
        await handlerCalled.Task;

        // Assert
        exceptionHandler.Received(1).Invoke(Arg.Any<Exception>());
    }
}
