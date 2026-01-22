using NSubstitute;

namespace ZCrew.StateCraft.IntegrationTests.ExceptionHandling;

public class TriggerExceptionTests
{
    [Fact]
    public async Task Trigger_WhenSignalThrowsException_ShouldCallExceptionHandler()
    {
        // Arrange
        var signal = new TaskCompletionSource();
        var exception = new InvalidOperationException("Signal error");
        var exceptionHandler = Substitute.For<Func<Exception, ExceptionResult>>();
        exceptionHandler.Invoke(Arg.Any<Exception>()).Returns(ExceptionResult.Continue());

        Func<CancellationToken, Task> signalFunc = token => signal.Task.WaitAsync(token);

        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .OnException(exceptionHandler)
            .WithState("A", state => state)
            .WithTrigger(trigger => trigger.Once().Await(signalFunc).ThenInvoke(() => { }))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        signal.SetException(exception);
        await Task.Delay(50, TestContext.Current.CancellationToken);
        await stateMachine.Deactivate(TestContext.Current.CancellationToken);

        // Assert
        exceptionHandler.Received(1).Invoke(exception);
    }

    [Fact]
    public async Task Trigger_WhenTriggerActionThrowsException_ShouldCallExceptionHandler()
    {
        // Arrange
        var exception = new InvalidOperationException("Trigger error");
        var exceptionHandler = Substitute.For<Func<Exception, ExceptionResult>>();
        exceptionHandler.Invoke(Arg.Any<Exception>()).Returns(ExceptionResult.Continue());

        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .OnException(exceptionHandler)
            .WithState("A", state => state)
            .WithTrigger(trigger => trigger.Once().Await(() => { }).ThenInvoke(() => throw exception))
            .Build();

        // Act
        await stateMachine.Activate(TestContext.Current.CancellationToken);
        await Task.Delay(50, TestContext.Current.CancellationToken);
        await stateMachine.Deactivate(TestContext.Current.CancellationToken);

        // Assert
        exceptionHandler.Received(1).Invoke(exception);
    }

    [Fact]
    public async Task Trigger_WhenSignalThrowsSynchronously_ShouldCallExceptionHandler()
    {
        // Arrange
        var exception = new InvalidOperationException("Immediate signal error");
        var exceptionHandler = Substitute.For<Func<Exception, ExceptionResult>>();
        exceptionHandler.Invoke(Arg.Any<Exception>()).Returns(ExceptionResult.Continue());

        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .OnException(exceptionHandler)
            .WithState("A", state => state)
            .WithTrigger(trigger => trigger.Once().Await(() => throw exception).ThenInvoke(() => { }))
            .Build();

        // Act
        await stateMachine.Activate(TestContext.Current.CancellationToken);
        await Task.Delay(50, TestContext.Current.CancellationToken);
        await stateMachine.Deactivate(TestContext.Current.CancellationToken);

        // Assert
        exceptionHandler.Received(1).Invoke(exception);
    }

    [Fact]
    public async Task Trigger_WhenCancelled_ShouldNotCallExceptionHandler()
    {
        // Arrange
        var exceptionHandler = Substitute.For<Func<Exception, ExceptionResult>>();
        exceptionHandler.Invoke(Arg.Any<Exception>()).Returns(ExceptionResult.Continue());

        Func<CancellationToken, Task> signalFunc = token => Task.Delay(Timeout.Infinite, token);

        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .OnException(exceptionHandler)
            .WithState("A", state => state)
            .WithTrigger(trigger => trigger.Once().Await(signalFunc).ThenInvoke(() => { }))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Deactivate(TestContext.Current.CancellationToken);

        // Wait a bit to ensure any exception handling would have completed
        await Task.Delay(50, TestContext.Current.CancellationToken);

        // Assert
        exceptionHandler.DidNotReceive().Invoke(Arg.Any<Exception>());
    }

    [Fact]
    public async Task RepeatingTrigger_WhenExceptionThrown_ShouldStopRepeating()
    {
        // Arrange
        var signalGate = new SemaphoreSlim(0);
        var triggerCount = 0;
        var exception = new InvalidOperationException("Stop repeating");
        var exceptionHandler = Substitute.For<Func<Exception, ExceptionResult>>();
        exceptionHandler.Invoke(Arg.Any<Exception>()).Returns(ExceptionResult.Continue());

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
                        if (triggerCount >= 2)
                        {
                            throw exception;
                        }
                    })
            )
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        signalGate.Release();
        await Task.Delay(100, TestContext.Current.CancellationToken);
        signalGate.Release();
        await Task.Delay(100, TestContext.Current.CancellationToken);
        await stateMachine.Deactivate(TestContext.Current.CancellationToken);

        // Assert
        exceptionHandler.Received(1).Invoke(Arg.Any<Exception>());
        Assert.Equal(2, triggerCount);
    }
}
