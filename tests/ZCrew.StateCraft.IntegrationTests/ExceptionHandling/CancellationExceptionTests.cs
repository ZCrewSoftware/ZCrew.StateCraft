using NSubstitute;

namespace ZCrew.StateCraft.IntegrationTests.ExceptionHandling;

public class CancellationExceptionTests
{
    [Fact]
    public async Task Activate_WhenOnActivateThrowsOperationCanceledException_ShouldNotCallOnExceptionHandler()
    {
        // Arrange
        var onExceptionCalled = false;
        var cts = CancellationTokenSource.CreateLinkedTokenSource(TestContext.Current.CancellationToken);
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .OnException(_ =>
            {
                onExceptionCalled = true;
                return ExceptionResult.Continue();
            })
            .WithState("A", state => state.OnActivate(_ => throw new OperationCanceledException(cts.Token)))
            .Build();

        await cts.CancelAsync();

        // Act
        await Assert.ThrowsAsync<OperationCanceledException>(() => stateMachine.Activate(cts.Token));

        // Assert
        Assert.False(onExceptionCalled);
    }

    [Fact]
    public async Task Activate_WhenOnEntryThrowsOperationCanceledException_ShouldNotCallOnExceptionHandler()
    {
        // Arrange
        var onExceptionCalled = false;
        var cts = CancellationTokenSource.CreateLinkedTokenSource(TestContext.Current.CancellationToken);
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .OnException(_ =>
            {
                onExceptionCalled = true;
                return ExceptionResult.Continue();
            })
            .WithState("A", state => state.OnEntry(() => throw new OperationCanceledException(cts.Token)))
            .Build();

        await cts.CancelAsync();

        // Act
        await Assert.ThrowsAsync<OperationCanceledException>(() => stateMachine.Activate(cts.Token));

        // Assert
        Assert.False(onExceptionCalled);
    }

    [Fact]
    public async Task Deactivate_WhenOnExitThrowsOperationCanceledException_ShouldNotCallOnExceptionHandler()
    {
        // Arrange
        var onExceptionCalled = false;
        var cts = CancellationTokenSource.CreateLinkedTokenSource(TestContext.Current.CancellationToken);
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .OnException(_ =>
            {
                onExceptionCalled = true;
                return ExceptionResult.Continue();
            })
            .WithState("A", state => state.OnExit(() => throw new OperationCanceledException(cts.Token)))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);
        await cts.CancelAsync();

        // Act
        await Assert.ThrowsAsync<OperationCanceledException>(() => stateMachine.Deactivate(cts.Token));

        // Assert
        Assert.False(onExceptionCalled);
    }

    [Fact]
    public async Task Deactivate_WhenOnDeactivateThrowsOperationCanceledException_ShouldNotCallOnExceptionHandler()
    {
        // Arrange
        var onExceptionCalled = false;
        var cts = CancellationTokenSource.CreateLinkedTokenSource(TestContext.Current.CancellationToken);
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .OnException(_ =>
            {
                onExceptionCalled = true;
                return ExceptionResult.Continue();
            })
            .WithState("A", state => state.OnDeactivate(_ => throw new OperationCanceledException(cts.Token)))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);
        await cts.CancelAsync();

        // Act
        await Assert.ThrowsAsync<OperationCanceledException>(() => stateMachine.Deactivate(cts.Token));

        // Assert
        Assert.False(onExceptionCalled);
    }

    [Fact]
    public async Task Transition_WhenOnExitThrowsOperationCanceledException_ShouldNotCallOnExceptionHandler()
    {
        // Arrange
        var onExceptionCalled = false;
        var cts = CancellationTokenSource.CreateLinkedTokenSource(TestContext.Current.CancellationToken);
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .OnException(_ =>
            {
                onExceptionCalled = true;
                return ExceptionResult.Continue();
            })
            .WithState(
                "A",
                state => state.OnExit(() => throw new OperationCanceledException(cts.Token)).WithTransition("To B", "B")
            )
            .WithState("B", state => state)
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);
        await cts.CancelAsync();

        // Act
        await Assert.ThrowsAsync<OperationCanceledException>(() => stateMachine.Transition("To B", cts.Token));

        // Assert
        Assert.False(onExceptionCalled);
    }

    [Fact]
    public async Task Transition_WhenOnEntryThrowsOperationCanceledException_ShouldNotCallOnExceptionHandler()
    {
        // Arrange
        var onExceptionCalled = false;
        var cts = CancellationTokenSource.CreateLinkedTokenSource(TestContext.Current.CancellationToken);
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .OnException(_ =>
            {
                onExceptionCalled = true;
                return ExceptionResult.Continue();
            })
            .WithState("A", state => state.WithTransition("To B", "B"))
            .WithState("B", state => state.OnEntry(() => throw new OperationCanceledException(cts.Token)))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);
        await cts.CancelAsync();

        // Act
        await Assert.ThrowsAsync<OperationCanceledException>(() => stateMachine.Transition("To B", cts.Token));

        // Assert
        Assert.False(onExceptionCalled);
    }

    [Fact]
    public async Task Transition_WhenOnStateChangeThrowsOperationCanceledException_ShouldNotCallOnExceptionHandler()
    {
        // Arrange
        var onExceptionCalled = false;
        var cts = CancellationTokenSource.CreateLinkedTokenSource(TestContext.Current.CancellationToken);
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .OnException(_ =>
            {
                onExceptionCalled = true;
                return ExceptionResult.Continue();
            })
            .OnStateChange((_, _, _) => throw new OperationCanceledException(cts.Token))
            .WithState("A", state => state.WithTransition("To B", "B"))
            .WithState("B", state => state)
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);
        await cts.CancelAsync();

        // Act
        await Assert.ThrowsAsync<OperationCanceledException>(() => stateMachine.Transition("To B", cts.Token));

        // Assert
        Assert.False(onExceptionCalled);
    }

    [Fact]
    public async Task Activate_WhenActionThrowsOperationCanceledException_ShouldNotCallOnExceptionHandlerAndSuppressException()
    {
        // Arrange
        var onException = Substitute.For<Func<Exception, ExceptionResult>>();
        onException.Invoke(Arg.Any<Exception>()).Returns(ExceptionResult.Continue());
        var cts = CancellationTokenSource.CreateLinkedTokenSource(TestContext.Current.CancellationToken);
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .OnException(onException)
            .WithState(
                "A",
                state => state.WithAction(a => a.Invoke(() => throw new OperationCanceledException(cts.Token)))
            )
            .Build();

        await cts.CancelAsync();

        // Act
        await stateMachine.Activate(cts.Token);

        // Assert
        onException.DidNotReceive().Invoke(Arg.Any<Exception>());
    }

    [Fact]
    public async Task Activate_WhenAsyncActionThrowsOperationCanceledException_ShouldNotCallOnExceptionHandler()
    {
        // Arrange
        var onException = Substitute.For<Func<Exception, ExceptionResult>>();
        onException.Invoke(Arg.Any<Exception>()).Returns(ExceptionResult.Continue());
        var cts = CancellationTokenSource.CreateLinkedTokenSource(TestContext.Current.CancellationToken);
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .OnException(onException)
            .WithState("A", state => state.WithAction(a => a.Invoke(Action)))
            .Build();

        await cts.CancelAsync();

        // Act
        await stateMachine.Activate(cts.Token);

        // Assert
        onException.DidNotReceive().Invoke(Arg.Any<Exception>());

        return;

        async Task Action(CancellationToken token)
        {
            await Task.Yield();
            throw new OperationCanceledException(token);
        }
    }

    [Fact]
    public async Task Transition_WhenActionThrowsOperationCanceledException_ShouldNotCallOnExceptionHandler()
    {
        // Arrange
        var onException = Substitute.For<Func<Exception, ExceptionResult>>();
        onException.Invoke(Arg.Any<Exception>()).Returns(ExceptionResult.Continue());
        var cts = CancellationTokenSource.CreateLinkedTokenSource(TestContext.Current.CancellationToken);
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .OnException(onException)
            .WithState("A", state => state.WithTransition("To B", "B"))
            .WithState(
                "B",
                state => state.WithAction(a => a.Invoke(() => throw new OperationCanceledException(cts.Token)))
            )
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);
        await cts.CancelAsync();

        // Act
        await stateMachine.Transition("To B", cts.Token);

        // Assert
        onException.DidNotReceive().Invoke(Arg.Any<Exception>());
    }

    [Fact]
    public async Task Transition_WhenAsyncActionThrowsOperationCanceledException_ShouldNotCallOnExceptionHandler()
    {
        // Arrange
        var onException = Substitute.For<Func<Exception, ExceptionResult>>();
        onException.Invoke(Arg.Any<Exception>()).Returns(ExceptionResult.Continue());
        var cts = CancellationTokenSource.CreateLinkedTokenSource(TestContext.Current.CancellationToken);
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .OnException(onException)
            .WithState("A", state => state.WithTransition("To B", "B"))
            .WithState("B", state => state.WithAction(a => a.Invoke(Action)))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);
        await cts.CancelAsync();

        // Act
        await stateMachine.Transition("To B", cts.Token);

        // Assert
        onException.DidNotReceive().Invoke(Arg.Any<Exception>());

        return;

        async Task Action(CancellationToken token)
        {
            await Task.Yield();
            throw new OperationCanceledException(token);
        }
    }
}
