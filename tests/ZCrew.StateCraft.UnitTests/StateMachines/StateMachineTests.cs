using NSubstitute;
using ZCrew.Extensions.Tasks;
using ZCrew.StateCraft.StateMachines;

namespace ZCrew.StateCraft.UnitTests.StateMachines;

public class StateMachineTests
{
    [Fact]
    public async Task RunWithExceptionHandling_WhenActionSucceeds_ShouldComplete()
    {
        // Arrange
        var handler = Substitute.For<Func<Exception, ExceptionResult>>();
        handler.Invoke(Arg.Any<Exception>()).Returns(ExceptionResult.Continue());
        var stateMachine = CreateStateMachine(handler.AsAsyncFunc());

        // Act
        await stateMachine.RunWithExceptionHandling(() => Task.CompletedTask, TestContext.Current.CancellationToken);

        // Assert
        handler.DidNotReceive().Invoke(Arg.Any<Exception>());
    }

    [Fact]
    public async Task RunWithExceptionHandling_T_WhenActionSucceeds_ShouldReturnResult()
    {
        // Arrange
        var handler = Substitute.For<Func<Exception, ExceptionResult>>();
        handler.Invoke(Arg.Any<Exception>()).Returns(ExceptionResult.Continue());
        var stateMachine = CreateStateMachine(handler.AsAsyncFunc());

        // Act
        var result = await stateMachine.RunWithExceptionHandling(
            () => Task.FromResult(42),
            TestContext.Current.CancellationToken
        );

        // Assert
        Assert.Equal(42, result);
        handler.DidNotReceive().Invoke(Arg.Any<Exception>());
    }

    [Fact]
    public async Task RunWithExceptionHandling_WhenCancellationExceptionWithActiveToken_ShouldSkipHandlers()
    {
        // Arrange
        var handler = Substitute.For<Func<Exception, ExceptionResult>>();
        handler.Invoke(Arg.Any<Exception>()).Returns(ExceptionResult.Continue());
        var stateMachine = CreateStateMachine(handler.AsAsyncFunc());

        var cts = CancellationTokenSource.CreateLinkedTokenSource(TestContext.Current.CancellationToken);
        await cts.CancelAsync();

        // Act
        var act = () =>
            stateMachine.RunWithExceptionHandling(() => throw new OperationCanceledException(cts.Token), cts.Token);

        // Assert
        await Assert.ThrowsAsync<OperationCanceledException>(act);
        handler.DidNotReceive().Invoke(Arg.Any<Exception>());
    }

    [Fact]
    public async Task RunWithExceptionHandling_T_WhenCancellationExceptionWithActiveToken_ShouldSkipHandlers()
    {
        // Arrange
        var handler = Substitute.For<Func<Exception, ExceptionResult>>();
        handler.Invoke(Arg.Any<Exception>()).Returns(ExceptionResult.Continue());
        var stateMachine = CreateStateMachine(handler.AsAsyncFunc());

        var cts = CancellationTokenSource.CreateLinkedTokenSource(TestContext.Current.CancellationToken);
        await cts.CancelAsync();

        // Act
        var act = () =>
            stateMachine.RunWithExceptionHandling<int>(
                () => throw new OperationCanceledException(cts.Token),
                cts.Token
            );

        // Assert
        await Assert.ThrowsAsync<OperationCanceledException>(act);
        handler.DidNotReceive().Invoke(Arg.Any<Exception>());
    }

    [Fact]
    public async Task RunWithExceptionHandling_WhenExceptionWithNoHandlers_ShouldRethrow()
    {
        // Arrange
        var exception = new InvalidOperationException("Test exception");
        var stateMachine = CreateStateMachine();

        // Act
        var act = () =>
            stateMachine.RunWithExceptionHandling(() => throw exception, TestContext.Current.CancellationToken);

        // Assert
        var thrownException = await Assert.ThrowsAsync<InvalidOperationException>(act);
        Assert.Same(exception, thrownException);
    }

    [Fact]
    public async Task RunWithExceptionHandling_T_WhenExceptionWithNoHandlers_ShouldRethrow()
    {
        // Arrange
        var exception = new InvalidOperationException("Test exception");
        var stateMachine = CreateStateMachine();

        // Act
        var act = () =>
            stateMachine.RunWithExceptionHandling<int>(() => throw exception, TestContext.Current.CancellationToken);

        // Assert
        var thrownException = await Assert.ThrowsAsync<InvalidOperationException>(act);
        Assert.Same(exception, thrownException);
    }

    [Fact]
    public async Task RunWithExceptionHandling_WhenHandlerReturnsContinueWithNoMoreHandlers_ShouldRethrow()
    {
        // Arrange
        var exception = new InvalidOperationException("Test exception");
        var handler = Substitute.For<Func<Exception, ExceptionResult>>();
        handler.Invoke(Arg.Any<Exception>()).Returns(ExceptionResult.Continue());
        var stateMachine = CreateStateMachine(handler.AsAsyncFunc());

        // Act
        var act = () =>
            stateMachine.RunWithExceptionHandling(() => throw exception, TestContext.Current.CancellationToken);

        // Assert
        var thrownException = await Assert.ThrowsAsync<InvalidOperationException>(act);
        Assert.Same(exception, thrownException);
        handler.Received(1).Invoke(exception);
    }

    [Fact]
    public async Task RunWithExceptionHandling_T_WhenHandlerReturnsContinueWithNoMoreHandlers_ShouldRethrow()
    {
        // Arrange
        var exception = new InvalidOperationException("Test exception");
        var handler = Substitute.For<Func<Exception, ExceptionResult>>();
        handler.Invoke(Arg.Any<Exception>()).Returns(ExceptionResult.Continue());
        var stateMachine = CreateStateMachine(handler.AsAsyncFunc());

        // Act
        var act = () =>
            stateMachine.RunWithExceptionHandling<int>(() => throw exception, TestContext.Current.CancellationToken);

        // Assert
        var thrownException = await Assert.ThrowsAsync<InvalidOperationException>(act);
        Assert.Same(exception, thrownException);
        handler.Received(1).Invoke(exception);
    }

    [Fact]
    public async Task RunWithExceptionHandling_WhenHandlerReturnsRethrow_ShouldRethrowOriginalException()
    {
        // Arrange
        var exception = new InvalidOperationException("Test exception");
        var handler = Substitute.For<Func<Exception, ExceptionResult>>();
        handler.Invoke(Arg.Any<Exception>()).Returns(ExceptionResult.Rethrow());
        var stateMachine = CreateStateMachine(handler.AsAsyncFunc());

        // Act
        var act = () =>
            stateMachine.RunWithExceptionHandling(() => throw exception, TestContext.Current.CancellationToken);

        // Assert
        var thrownException = await Assert.ThrowsAsync<InvalidOperationException>(act);
        Assert.Same(exception, thrownException);
        handler.Received(1).Invoke(exception);
    }

    [Fact]
    public async Task RunWithExceptionHandling_T_WhenHandlerReturnsRethrow_ShouldRethrowOriginalException()
    {
        // Arrange
        var exception = new InvalidOperationException("Test exception");
        var handler = Substitute.For<Func<Exception, ExceptionResult>>();
        handler.Invoke(Arg.Any<Exception>()).Returns(ExceptionResult.Rethrow());
        var stateMachine = CreateStateMachine(handler.AsAsyncFunc());

        // Act
        var act = () =>
            stateMachine.RunWithExceptionHandling<int>(() => throw exception, TestContext.Current.CancellationToken);

        // Assert
        var thrownException = await Assert.ThrowsAsync<InvalidOperationException>(act);
        Assert.Same(exception, thrownException);
        handler.Received(1).Invoke(exception);
    }

    [Fact]
    public async Task RunWithExceptionHandling_WhenHandlerReturnsThrow_ShouldThrowNewException()
    {
        // Arrange
        var originalException = new InvalidOperationException("Original exception");
        var replacementException = new ArgumentException("Replacement exception");
        var handler = Substitute.For<Func<Exception, ExceptionResult>>();
        handler.Invoke(Arg.Any<Exception>()).Returns(ExceptionResult.Throw(replacementException));
        var stateMachine = CreateStateMachine(handler.AsAsyncFunc());

        // Act
        var act = () =>
            stateMachine.RunWithExceptionHandling(() => throw originalException, TestContext.Current.CancellationToken);

        // Assert
        var thrownException = await Assert.ThrowsAsync<ArgumentException>(act);
        Assert.Same(replacementException, thrownException);
        handler.Received(1).Invoke(originalException);
    }

    [Fact]
    public async Task RunWithExceptionHandling_T_WhenHandlerReturnsThrow_ShouldThrowNewException()
    {
        // Arrange
        var originalException = new InvalidOperationException("Original exception");
        var replacementException = new ArgumentException("Replacement exception");
        var handler = Substitute.For<Func<Exception, ExceptionResult>>();
        handler.Invoke(Arg.Any<Exception>()).Returns(ExceptionResult.Throw(replacementException));
        var stateMachine = CreateStateMachine(handler.AsAsyncFunc());

        // Act
        var act = () =>
            stateMachine.RunWithExceptionHandling<int>(
                () => throw originalException,
                TestContext.Current.CancellationToken
            );

        // Assert
        var thrownException = await Assert.ThrowsAsync<ArgumentException>(act);
        Assert.Same(replacementException, thrownException);
        handler.Received(1).Invoke(originalException);
    }

    [Fact]
    public async Task RunWithExceptionHandling_WhenMultipleHandlersWithContinueThenRethrow_ShouldCallBothAndRethrow()
    {
        // Arrange
        var exception = new InvalidOperationException("Test exception");
        var handler1 = Substitute.For<Func<Exception, ExceptionResult>>();
        var handler2 = Substitute.For<Func<Exception, ExceptionResult>>();
        handler1.Invoke(Arg.Any<Exception>()).Returns(ExceptionResult.Continue());
        handler2.Invoke(Arg.Any<Exception>()).Returns(ExceptionResult.Rethrow());
        var stateMachine = CreateStateMachine(handler1.AsAsyncFunc(), handler2.AsAsyncFunc());

        // Act
        var act = () =>
            stateMachine.RunWithExceptionHandling(() => throw exception, TestContext.Current.CancellationToken);

        // Assert
        var thrownException = await Assert.ThrowsAsync<InvalidOperationException>(act);
        Assert.Same(exception, thrownException);
        handler1.Received(1).Invoke(exception);
        handler2.Received(1).Invoke(exception);
    }

    [Fact]
    public async Task RunWithExceptionHandling_WhenMultipleHandlersWithContinueThenThrow_ShouldCallBothAndThrowNew()
    {
        // Arrange
        var originalException = new InvalidOperationException("Original exception");
        var replacementException = new ArgumentException("Replacement exception");
        var handler1 = Substitute.For<Func<Exception, ExceptionResult>>();
        var handler2 = Substitute.For<Func<Exception, ExceptionResult>>();
        handler1.Invoke(Arg.Any<Exception>()).Returns(ExceptionResult.Continue());
        handler2.Invoke(Arg.Any<Exception>()).Returns(ExceptionResult.Throw(replacementException));
        var stateMachine = CreateStateMachine(handler1.AsAsyncFunc(), handler2.AsAsyncFunc());

        // Act
        var act = () =>
            stateMachine.RunWithExceptionHandling(() => throw originalException, TestContext.Current.CancellationToken);

        // Assert
        var thrownException = await Assert.ThrowsAsync<ArgumentException>(act);
        Assert.Same(replacementException, thrownException);
        handler1.Received(1).Invoke(originalException);
        handler2.Received(1).Invoke(originalException);
    }

    [Fact]
    public async Task RunWithExceptionHandling_WhenFirstHandlerReturnsRethrow_ShouldNotCallSubsequentHandlers()
    {
        // Arrange
        var exception = new InvalidOperationException("Test exception");
        var handler1 = Substitute.For<Func<Exception, ExceptionResult>>();
        var handler2 = Substitute.For<Func<Exception, ExceptionResult>>();
        handler1.Invoke(Arg.Any<Exception>()).Returns(ExceptionResult.Rethrow());
        handler2.Invoke(Arg.Any<Exception>()).Returns(ExceptionResult.Continue());
        var stateMachine = CreateStateMachine(handler1.AsAsyncFunc(), handler2.AsAsyncFunc());

        // Act
        var act = () =>
            stateMachine.RunWithExceptionHandling(() => throw exception, TestContext.Current.CancellationToken);

        // Assert
        var thrownException = await Assert.ThrowsAsync<InvalidOperationException>(act);
        Assert.Same(exception, thrownException);
        handler1.Received(1).Invoke(exception);
        handler2.DidNotReceive().Invoke(Arg.Any<Exception>());
    }

    [Fact]
    public async Task RunWithExceptionHandling_WhenFirstHandlerReturnsThrow_ShouldNotCallSubsequentHandlers()
    {
        // Arrange
        var originalException = new InvalidOperationException("Original exception");
        var replacementException = new ArgumentException("Replacement exception");
        var handler1 = Substitute.For<Func<Exception, ExceptionResult>>();
        var handler2 = Substitute.For<Func<Exception, ExceptionResult>>();
        handler1.Invoke(Arg.Any<Exception>()).Returns(ExceptionResult.Throw(replacementException));
        handler2.Invoke(Arg.Any<Exception>()).Returns(ExceptionResult.Continue());
        var stateMachine = CreateStateMachine(handler1.AsAsyncFunc(), handler2.AsAsyncFunc());

        // Act
        var act = () =>
            stateMachine.RunWithExceptionHandling(() => throw originalException, TestContext.Current.CancellationToken);

        // Assert
        var thrownException = await Assert.ThrowsAsync<ArgumentException>(act);
        Assert.Same(replacementException, thrownException);
        handler1.Received(1).Invoke(originalException);
        handler2.DidNotReceive().Invoke(Arg.Any<Exception>());
    }

    [Fact]
    public async Task RunWithExceptionHandling_WhenAllHandlersReturnContinue_ShouldRethrowOriginalException()
    {
        // Arrange
        var exception = new InvalidOperationException("Test exception");
        var handler1 = Substitute.For<Func<Exception, ExceptionResult>>();
        var handler2 = Substitute.For<Func<Exception, ExceptionResult>>();
        var handler3 = Substitute.For<Func<Exception, ExceptionResult>>();
        handler1.Invoke(Arg.Any<Exception>()).Returns(ExceptionResult.Continue());
        handler2.Invoke(Arg.Any<Exception>()).Returns(ExceptionResult.Continue());
        handler3.Invoke(Arg.Any<Exception>()).Returns(ExceptionResult.Continue());
        var stateMachine = CreateStateMachine(handler1.AsAsyncFunc(), handler2.AsAsyncFunc(), handler3.AsAsyncFunc());

        // Act
        var act = () =>
            stateMachine.RunWithExceptionHandling(() => throw exception, TestContext.Current.CancellationToken);

        // Assert
        var thrownException = await Assert.ThrowsAsync<InvalidOperationException>(act);
        Assert.Same(exception, thrownException);
        handler1.Received(1).Invoke(exception);
        handler2.Received(1).Invoke(exception);
        handler3.Received(1).Invoke(exception);
    }

    [Fact]
    public async Task RunWithExceptionHandling_WhenHandlerReceivesException_ShouldReceiveOriginalException()
    {
        // Arrange
        var exception = new InvalidOperationException("Test exception");
        var handler = Substitute.For<Func<Exception, ExceptionResult>>();
        handler.Invoke(Arg.Any<Exception>()).Returns(ExceptionResult.Rethrow());
        var stateMachine = CreateStateMachine(handler.AsAsyncFunc());

        // Act
        var act = () =>
            stateMachine.RunWithExceptionHandling(() => throw exception, TestContext.Current.CancellationToken);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(act);
        handler.Received(1).Invoke(exception);
    }

    [Fact]
    public async Task RunWithExceptionHandling_WhenAsyncHandlerReturnsRethrow_ShouldRethrowOriginalException()
    {
        // Arrange
        var exception = new InvalidOperationException("Test exception");
        var handler = Substitute.For<Func<Exception, CancellationToken, Task<ExceptionResult>>>();
        handler
            .Invoke(Arg.Any<Exception>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(ExceptionResult.Rethrow()));
        var stateMachine = CreateStateMachine(handler.AsAsyncFunc());

        // Act
        var act = () =>
            stateMachine.RunWithExceptionHandling(() => throw exception, TestContext.Current.CancellationToken);

        // Assert
        var thrownException = await Assert.ThrowsAsync<InvalidOperationException>(act);
        Assert.Same(exception, thrownException);
        await handler.Received(1).Invoke(exception, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task RunWithExceptionHandling_WhenAsyncHandlerReturnsThrow_ShouldThrowNewException()
    {
        // Arrange
        var originalException = new InvalidOperationException("Original exception");
        var replacementException = new ArgumentException("Replacement exception");
        var handler = Substitute.For<Func<Exception, CancellationToken, Task<ExceptionResult>>>();
        handler
            .Invoke(Arg.Any<Exception>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(ExceptionResult.Throw(replacementException)));
        var stateMachine = CreateStateMachine(handler.AsAsyncFunc());

        // Act
        var act = () =>
            stateMachine.RunWithExceptionHandling(() => throw originalException, TestContext.Current.CancellationToken);

        // Assert
        var thrownException = await Assert.ThrowsAsync<ArgumentException>(act);
        Assert.Same(replacementException, thrownException);
        await handler.Received(1).Invoke(originalException, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task RunWithExceptionHandling_WhenOperationCanceledExceptionWithInactiveToken_ShouldCallHandlers()
    {
        // Arrange
        var exception = new OperationCanceledException("Test cancellation");
        var handler = Substitute.For<Func<Exception, ExceptionResult>>();
        handler.Invoke(Arg.Any<Exception>()).Returns(ExceptionResult.Rethrow());
        var stateMachine = CreateStateMachine(handler.AsAsyncFunc());

        // Act
        var act = () => stateMachine.RunWithExceptionHandling(() => throw exception, CancellationToken.None);

        // Assert
        var thrownException = await Assert.ThrowsAsync<OperationCanceledException>(act);
        Assert.Same(exception, thrownException);
        handler.Received(1).Invoke(exception);
    }

    private static StateMachine<string, string> CreateStateMachine(
        params IAsyncFunc<Exception, ExceptionResult>[] exceptionHandlers
    )
    {
        return new StateMachine<string, string>(
            new StateMachineActivator<string, string>(string.Empty),
            [],
            exceptionHandlers,
            [],
            StateMachineOptions.None
        );
    }
}
