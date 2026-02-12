using NSubstitute;
using ZCrew.Extensions.Tasks;
using ZCrew.StateCraft.Exceptions;

namespace ZCrew.StateCraft.UnitTests.Exceptions;

public class DefaultExceptionBehaviorTests
{
    [Fact]
    public async Task CallOnEntry_WhenNoException_ShouldCompleteSuccessfully()
    {
        // Arrange
        var handler = Substitute.For<IAsyncFunc<Exception, ExceptionResult>>();
        var behavior = new DefaultExceptionBehavior([handler]);

        // Act
        await behavior.CallOnEntry(_ => Task.CompletedTask, TestContext.Current.CancellationToken);

        // Assert
        await handler.DidNotReceive().InvokeAsync(Arg.Any<Exception>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CallOnExit_WhenNoException_ShouldCompleteSuccessfully()
    {
        // Arrange
        var handler = Substitute.For<IAsyncFunc<Exception, ExceptionResult>>();
        var behavior = new DefaultExceptionBehavior([handler]);

        // Act
        await behavior.CallOnExit(_ => Task.CompletedTask, TestContext.Current.CancellationToken);

        // Assert
        await handler.DidNotReceive().InvokeAsync(Arg.Any<Exception>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CallOnStateChange_WhenNoException_ShouldCompleteSuccessfully()
    {
        // Arrange
        var handler = Substitute.For<IAsyncFunc<Exception, ExceptionResult>>();
        var behavior = new DefaultExceptionBehavior([handler]);

        // Act
        await behavior.CallOnStateChange(_ => Task.CompletedTask, TestContext.Current.CancellationToken);

        // Assert
        await handler.DidNotReceive().InvokeAsync(Arg.Any<Exception>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CallOnActivate_WhenNoException_ShouldCompleteSuccessfully()
    {
        // Arrange
        var handler = Substitute.For<IAsyncFunc<Exception, ExceptionResult>>();
        var behavior = new DefaultExceptionBehavior([handler]);

        // Act
        await behavior.CallOnActivate(_ => Task.CompletedTask, TestContext.Current.CancellationToken);

        // Assert
        await handler.DidNotReceive().InvokeAsync(Arg.Any<Exception>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CallOnDeactivate_WhenNoException_ShouldCompleteSuccessfully()
    {
        // Arrange
        var handler = Substitute.For<IAsyncFunc<Exception, ExceptionResult>>();
        var behavior = new DefaultExceptionBehavior([handler]);

        // Act
        await behavior.CallOnDeactivate(_ => Task.CompletedTask, TestContext.Current.CancellationToken);

        // Assert
        await handler.DidNotReceive().InvokeAsync(Arg.Any<Exception>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CallCondition_WhenNoException_ShouldReturnHandlerResult()
    {
        // Arrange
        var handler = Substitute.For<IAsyncFunc<Exception, ExceptionResult>>();
        var behavior = new DefaultExceptionBehavior([handler]);

        // Act
        var result = await behavior.CallCondition(_ => Task.FromResult(true), TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result);
        await handler.DidNotReceive().InvokeAsync(Arg.Any<Exception>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CallMap_WhenNoException_ShouldCompleteSuccessfully()
    {
        // Arrange
        var handler = Substitute.For<IAsyncFunc<Exception, ExceptionResult>>();
        var behavior = new DefaultExceptionBehavior([handler]);

        // Act
        await behavior.CallMap(_ => Task.CompletedTask, TestContext.Current.CancellationToken);

        // Assert
        await handler.DidNotReceive().InvokeAsync(Arg.Any<Exception>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CallAction_WhenNoException_ShouldCompleteSuccessfully()
    {
        // Arrange
        var handler = Substitute.For<IAsyncFunc<Exception, ExceptionResult>>();
        var behavior = new DefaultExceptionBehavior([handler]);

        // Act
        await behavior.CallAction(_ => Task.CompletedTask, TestContext.Current.CancellationToken);

        // Assert
        await handler.DidNotReceive().InvokeAsync(Arg.Any<Exception>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CallTrigger_WhenNoException_ShouldCompleteSuccessfully()
    {
        // Arrange
        var handler = Substitute.For<IAsyncFunc<Exception, ExceptionResult>>();
        var behavior = new DefaultExceptionBehavior([handler]);

        // Act
        await behavior.CallTrigger(_ => Task.CompletedTask, TestContext.Current.CancellationToken);

        // Assert
        await handler.DidNotReceive().InvokeAsync(Arg.Any<Exception>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CallOnEntry_WhenException_ShouldInvokeExceptionHandler()
    {
        // Arrange
        var exception = new InvalidOperationException("test");
        var handler = Substitute.For<IAsyncFunc<Exception, ExceptionResult>>();
        handler.InvokeAsync(exception, Arg.Any<CancellationToken>()).Returns(ExceptionResult.Continue());
        var behavior = new DefaultExceptionBehavior([handler]);

        // Act
        var callOnEntry = () => behavior.CallOnEntry(_ => throw exception, TestContext.Current.CancellationToken);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(callOnEntry);
        await handler.Received(1).InvokeAsync(exception, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CallOnExit_WhenException_ShouldInvokeExceptionHandler()
    {
        // Arrange
        var exception = new InvalidOperationException("test");
        var handler = Substitute.For<IAsyncFunc<Exception, ExceptionResult>>();
        handler.InvokeAsync(exception, Arg.Any<CancellationToken>()).Returns(ExceptionResult.Continue());
        var behavior = new DefaultExceptionBehavior([handler]);

        // Act
        var callOnExit = () => behavior.CallOnExit(_ => throw exception, TestContext.Current.CancellationToken);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(callOnExit);
        await handler.Received(1).InvokeAsync(exception, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CallOnStateChange_WhenException_ShouldInvokeExceptionHandler()
    {
        // Arrange
        var exception = new InvalidOperationException("test");
        var handler = Substitute.For<IAsyncFunc<Exception, ExceptionResult>>();
        handler.InvokeAsync(exception, Arg.Any<CancellationToken>()).Returns(ExceptionResult.Continue());
        var behavior = new DefaultExceptionBehavior([handler]);

        // Act
        var callOnStateChange = () =>
            behavior.CallOnStateChange(_ => throw exception, TestContext.Current.CancellationToken);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(callOnStateChange);
        await handler.Received(1).InvokeAsync(exception, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CallOnActivate_WhenException_ShouldInvokeExceptionHandler()
    {
        // Arrange
        var exception = new InvalidOperationException("test");
        var handler = Substitute.For<IAsyncFunc<Exception, ExceptionResult>>();
        handler.InvokeAsync(exception, Arg.Any<CancellationToken>()).Returns(ExceptionResult.Continue());
        var behavior = new DefaultExceptionBehavior([handler]);

        // Act
        var callOnActivate = () => behavior.CallOnActivate(_ => throw exception, TestContext.Current.CancellationToken);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(callOnActivate);
        await handler.Received(1).InvokeAsync(exception, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CallOnDeactivate_WhenException_ShouldInvokeExceptionHandler()
    {
        // Arrange
        var exception = new InvalidOperationException("test");
        var handler = Substitute.For<IAsyncFunc<Exception, ExceptionResult>>();
        handler.InvokeAsync(exception, Arg.Any<CancellationToken>()).Returns(ExceptionResult.Continue());
        var behavior = new DefaultExceptionBehavior([handler]);

        // Act
        var callOnDeactivate = () =>
            behavior.CallOnDeactivate(_ => throw exception, TestContext.Current.CancellationToken);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(callOnDeactivate);
        await handler.Received(1).InvokeAsync(exception, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CallCondition_WhenException_ShouldInvokeExceptionHandler()
    {
        // Arrange
        var exception = new InvalidOperationException("test");
        var handler = Substitute.For<IAsyncFunc<Exception, ExceptionResult>>();
        handler.InvokeAsync(exception, Arg.Any<CancellationToken>()).Returns(ExceptionResult.Continue());
        var behavior = new DefaultExceptionBehavior([handler]);

        // Act
        var callCondition = () => behavior.CallCondition(_ => throw exception, TestContext.Current.CancellationToken);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(callCondition);
        await handler.Received(1).InvokeAsync(exception, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CallMap_WhenException_ShouldInvokeExceptionHandler()
    {
        // Arrange
        var exception = new InvalidOperationException("test");
        var handler = Substitute.For<IAsyncFunc<Exception, ExceptionResult>>();
        handler.InvokeAsync(exception, Arg.Any<CancellationToken>()).Returns(ExceptionResult.Continue());
        var behavior = new DefaultExceptionBehavior([handler]);

        // Act
        var callMap = () => behavior.CallMap(_ => throw exception, TestContext.Current.CancellationToken);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(callMap);
        await handler.Received(1).InvokeAsync(exception, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CallAction_WhenException_ShouldInvokeExceptionHandler()
    {
        // Arrange
        var exception = new InvalidOperationException("test");
        var handler = Substitute.For<IAsyncFunc<Exception, ExceptionResult>>();
        handler.InvokeAsync(exception, Arg.Any<CancellationToken>()).Returns(ExceptionResult.Continue());
        var behavior = new DefaultExceptionBehavior([handler]);

        // Act
        var callAction = () => behavior.CallAction(_ => throw exception, TestContext.Current.CancellationToken);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(callAction);
        await handler.Received(1).InvokeAsync(exception, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CallTrigger_WhenException_ShouldInvokeExceptionHandler()
    {
        // Arrange
        var exception = new InvalidOperationException("test");
        var handler = Substitute.For<IAsyncFunc<Exception, ExceptionResult>>();
        handler.InvokeAsync(exception, Arg.Any<CancellationToken>()).Returns(ExceptionResult.Continue());
        var behavior = new DefaultExceptionBehavior([handler]);

        // Act
        var callTrigger = () => behavior.CallTrigger(_ => throw exception, TestContext.Current.CancellationToken);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(callTrigger);
        await handler.Received(1).InvokeAsync(exception, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CallOnEntry_WhenCanceled_ShouldRethrowWithoutInvokingHandlers()
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();
        var handler = Substitute.For<IAsyncFunc<Exception, ExceptionResult>>();
        var behavior = new DefaultExceptionBehavior([handler]);

        // Act
        var callOnEntry = () => behavior.CallOnEntry(_ => throw new OperationCanceledException(), cts.Token);

        // Assert
        await Assert.ThrowsAsync<OperationCanceledException>(callOnEntry);
        await handler.DidNotReceive().InvokeAsync(Arg.Any<Exception>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CallOnExit_WhenCanceled_ShouldRethrowWithoutInvokingHandlers()
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();
        var handler = Substitute.For<IAsyncFunc<Exception, ExceptionResult>>();
        var behavior = new DefaultExceptionBehavior([handler]);

        // Act
        var callOnExit = () => behavior.CallOnExit(_ => throw new OperationCanceledException(), cts.Token);

        // Assert
        await Assert.ThrowsAsync<OperationCanceledException>(callOnExit);
        await handler.DidNotReceive().InvokeAsync(Arg.Any<Exception>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CallOnStateChange_WhenCanceled_ShouldRethrowWithoutInvokingHandlers()
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();
        var handler = Substitute.For<IAsyncFunc<Exception, ExceptionResult>>();
        var behavior = new DefaultExceptionBehavior([handler]);

        // Act
        var callOnStateChange = () =>
            behavior.CallOnStateChange(_ => throw new OperationCanceledException(), cts.Token);

        // Assert
        await Assert.ThrowsAsync<OperationCanceledException>(callOnStateChange);
        await handler.DidNotReceive().InvokeAsync(Arg.Any<Exception>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CallOnActivate_WhenCanceled_ShouldRethrowWithoutInvokingHandlers()
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();
        var handler = Substitute.For<IAsyncFunc<Exception, ExceptionResult>>();
        var behavior = new DefaultExceptionBehavior([handler]);

        // Act
        var callOnActivate = () => behavior.CallOnActivate(_ => throw new OperationCanceledException(), cts.Token);

        // Assert
        await Assert.ThrowsAsync<OperationCanceledException>(callOnActivate);
        await handler.DidNotReceive().InvokeAsync(Arg.Any<Exception>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CallOnDeactivate_WhenCanceled_ShouldRethrowWithoutInvokingHandlers()
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();
        var handler = Substitute.For<IAsyncFunc<Exception, ExceptionResult>>();
        var behavior = new DefaultExceptionBehavior([handler]);

        // Act
        var callOnDeactivate = () => behavior.CallOnDeactivate(_ => throw new OperationCanceledException(), cts.Token);

        // Assert
        await Assert.ThrowsAsync<OperationCanceledException>(callOnDeactivate);
        await handler.DidNotReceive().InvokeAsync(Arg.Any<Exception>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CallCondition_WhenCanceled_ShouldRethrowWithoutInvokingHandlers()
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();
        var handler = Substitute.For<IAsyncFunc<Exception, ExceptionResult>>();
        var behavior = new DefaultExceptionBehavior([handler]);

        // Act
        var callCondition = () => behavior.CallCondition(_ => throw new OperationCanceledException(), cts.Token);

        // Assert
        await Assert.ThrowsAsync<OperationCanceledException>(callCondition);
        await handler.DidNotReceive().InvokeAsync(Arg.Any<Exception>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CallMap_WhenCanceled_ShouldRethrowWithoutInvokingHandlers()
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();
        var handler = Substitute.For<IAsyncFunc<Exception, ExceptionResult>>();
        var behavior = new DefaultExceptionBehavior([handler]);

        // Act
        var callMap = () => behavior.CallMap(_ => throw new OperationCanceledException(), cts.Token);

        // Assert
        await Assert.ThrowsAsync<OperationCanceledException>(callMap);
        await handler.DidNotReceive().InvokeAsync(Arg.Any<Exception>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CallAction_WhenCanceled_ShouldSuppressWithoutInvokingHandlers()
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();
        var handler = Substitute.For<IAsyncFunc<Exception, ExceptionResult>>();
        var behavior = new DefaultExceptionBehavior([handler]);

        // Act
        await behavior.CallAction(_ => throw new OperationCanceledException(), cts.Token);

        // Assert
        await handler.DidNotReceive().InvokeAsync(Arg.Any<Exception>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CallTrigger_WhenCanceled_ShouldSuppressWithoutInvokingHandlers()
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();
        var handler = Substitute.For<IAsyncFunc<Exception, ExceptionResult>>();
        var behavior = new DefaultExceptionBehavior([handler]);

        // Act
        await behavior.CallTrigger(_ => throw new OperationCanceledException(), cts.Token);

        // Assert
        await handler.DidNotReceive().InvokeAsync(Arg.Any<Exception>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CallOnEntry_WhenOperationCanceledButTokenNotCanceled_ShouldInvokeHandlers()
    {
        // Arrange
        var exception = new OperationCanceledException();
        var handler = Substitute.For<IAsyncFunc<Exception, ExceptionResult>>();
        handler.InvokeAsync(exception, Arg.Any<CancellationToken>()).Returns(ExceptionResult.Continue());
        var behavior = new DefaultExceptionBehavior([handler]);

        // Act
        var callOnEntry = () => behavior.CallOnEntry(_ => throw exception, TestContext.Current.CancellationToken);

        // Assert
        await Assert.ThrowsAsync<OperationCanceledException>(callOnEntry);
        await handler.Received(1).InvokeAsync(exception, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CallAction_WhenOperationCanceledButTokenNotCanceled_ShouldInvokeHandlers()
    {
        // Arrange
        var exception = new OperationCanceledException();
        var handler = Substitute.For<IAsyncFunc<Exception, ExceptionResult>>();
        handler.InvokeAsync(exception, Arg.Any<CancellationToken>()).Returns(ExceptionResult.Continue());
        var behavior = new DefaultExceptionBehavior([handler]);

        // Act
        var callAction = () => behavior.CallAction(_ => throw exception, TestContext.Current.CancellationToken);

        // Assert
        await Assert.ThrowsAsync<OperationCanceledException>(callAction);
        await handler.Received(1).InvokeAsync(exception, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task OnException_WhenHandlerReturnsContinue_ShouldInvokeAllHandlersThenRethrow()
    {
        // Arrange
        var exception = new InvalidOperationException("test");
        var handler1 = Substitute.For<IAsyncFunc<Exception, ExceptionResult>>();
        var handler2 = Substitute.For<IAsyncFunc<Exception, ExceptionResult>>();
        handler1.InvokeAsync(exception, Arg.Any<CancellationToken>()).Returns(ExceptionResult.Continue());
        handler2.InvokeAsync(exception, Arg.Any<CancellationToken>()).Returns(ExceptionResult.Continue());
        var behavior = new DefaultExceptionBehavior([handler1, handler2]);

        // Act
        var callOnEntry = () => behavior.CallOnEntry(_ => throw exception, TestContext.Current.CancellationToken);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(callOnEntry);
        await handler1.Received(1).InvokeAsync(exception, Arg.Any<CancellationToken>());
        await handler2.Received(1).InvokeAsync(exception, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task OnException_WhenHandlerReturnsRethrow_ShouldRethrowOriginalException()
    {
        // Arrange
        var exception = new InvalidOperationException("test");
        var handler = Substitute.For<IAsyncFunc<Exception, ExceptionResult>>();
        handler.InvokeAsync(exception, Arg.Any<CancellationToken>()).Returns(ExceptionResult.Rethrow());
        var behavior = new DefaultExceptionBehavior([handler]);

        // Act
        var callOnEntry = () => behavior.CallOnEntry(_ => throw exception, TestContext.Current.CancellationToken);

        // Assert
        var thrown = await Assert.ThrowsAsync<InvalidOperationException>(callOnEntry);
        Assert.Same(exception, thrown);
    }

    [Fact]
    public async Task OnException_WhenHandlerReturnsRethrow_ShouldNotInvokeSubsequentHandlers()
    {
        // Arrange
        var exception = new InvalidOperationException("test");
        var handler1 = Substitute.For<IAsyncFunc<Exception, ExceptionResult>>();
        var handler2 = Substitute.For<IAsyncFunc<Exception, ExceptionResult>>();
        handler1.InvokeAsync(exception, Arg.Any<CancellationToken>()).Returns(ExceptionResult.Rethrow());
        var behavior = new DefaultExceptionBehavior([handler1, handler2]);

        // Act
        var callOnEntry = () => behavior.CallOnEntry(_ => throw exception, TestContext.Current.CancellationToken);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(callOnEntry);
        await handler1.Received(1).InvokeAsync(exception, Arg.Any<CancellationToken>());
        await handler2.DidNotReceive().InvokeAsync(Arg.Any<Exception>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task OnException_WhenHandlerReturnsThrow_ShouldThrowReplacementException()
    {
        // Arrange
        var originalException = new InvalidOperationException("original");
        var replacementException = new ArgumentException("replacement");
        var handler = Substitute.For<IAsyncFunc<Exception, ExceptionResult>>();
        handler
            .InvokeAsync(originalException, Arg.Any<CancellationToken>())
            .Returns(ExceptionResult.Throw(replacementException));
        var behavior = new DefaultExceptionBehavior([handler]);

        // Act
        var callOnEntry = () =>
            behavior.CallOnEntry(_ => throw originalException, TestContext.Current.CancellationToken);

        // Assert
        var thrown = await Assert.ThrowsAsync<ArgumentException>(callOnEntry);
        Assert.Same(replacementException, thrown);
    }

    [Fact]
    public async Task OnException_WhenHandlerReturnsThrow_ShouldNotInvokeSubsequentHandlers()
    {
        // Arrange
        var originalException = new InvalidOperationException("original");
        var replacementException = new ArgumentException("replacement");
        var handler1 = Substitute.For<IAsyncFunc<Exception, ExceptionResult>>();
        var handler2 = Substitute.For<IAsyncFunc<Exception, ExceptionResult>>();
        handler1
            .InvokeAsync(originalException, Arg.Any<CancellationToken>())
            .Returns(ExceptionResult.Throw(replacementException));
        var behavior = new DefaultExceptionBehavior([handler1, handler2]);

        // Act
        var callOnEntry = () =>
            behavior.CallOnEntry(_ => throw originalException, TestContext.Current.CancellationToken);

        // Assert
        await Assert.ThrowsAsync<ArgumentException>(callOnEntry);
        await handler1.Received(1).InvokeAsync(originalException, Arg.Any<CancellationToken>());
        await handler2.DidNotReceive().InvokeAsync(Arg.Any<Exception>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task OnException_WhenNoHandlers_ShouldRethrowOriginalException()
    {
        // Arrange
        var exception = new InvalidOperationException("test");
        var behavior = new DefaultExceptionBehavior([]);

        // Act
        var callOnEntry = () => behavior.CallOnEntry(_ => throw exception, TestContext.Current.CancellationToken);

        // Assert
        var thrown = await Assert.ThrowsAsync<InvalidOperationException>(callOnEntry);
        Assert.Same(exception, thrown);
    }

    [Fact]
    public async Task OnException_WhenMultipleHandlers_ShouldInvokeInOrder()
    {
        // Arrange
        var exception = new InvalidOperationException("test");
        var callOrder = new List<int>();
        var handler1 = Substitute.For<IAsyncFunc<Exception, ExceptionResult>>();
        var handler2 = Substitute.For<IAsyncFunc<Exception, ExceptionResult>>();
        var handler3 = Substitute.For<IAsyncFunc<Exception, ExceptionResult>>();
        handler1
            .InvokeAsync(exception, Arg.Any<CancellationToken>())
            .Returns(ExceptionResult.Continue())
            .AndDoes(_ => callOrder.Add(1));
        handler2
            .InvokeAsync(exception, Arg.Any<CancellationToken>())
            .Returns(ExceptionResult.Continue())
            .AndDoes(_ => callOrder.Add(2));
        handler3
            .InvokeAsync(exception, Arg.Any<CancellationToken>())
            .Returns(ExceptionResult.Continue())
            .AndDoes(_ => callOrder.Add(3));
        var behavior = new DefaultExceptionBehavior([handler1, handler2, handler3]);

        // Act
        var callOnEntry = () => behavior.CallOnEntry(_ => throw exception, TestContext.Current.CancellationToken);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(callOnEntry);
        Assert.Equal([1, 2, 3], callOrder);
    }

    [Fact]
    public async Task OnException_WhenContinueThenRethrow_ShouldStopAtRethrow()
    {
        // Arrange
        var exception = new InvalidOperationException("test");
        var handler1 = Substitute.For<IAsyncFunc<Exception, ExceptionResult>>();
        var handler2 = Substitute.For<IAsyncFunc<Exception, ExceptionResult>>();
        var handler3 = Substitute.For<IAsyncFunc<Exception, ExceptionResult>>();
        handler1.InvokeAsync(exception, Arg.Any<CancellationToken>()).Returns(ExceptionResult.Continue());
        handler2.InvokeAsync(exception, Arg.Any<CancellationToken>()).Returns(ExceptionResult.Rethrow());
        var behavior = new DefaultExceptionBehavior([handler1, handler2, handler3]);

        // Act
        var callOnEntry = () => behavior.CallOnEntry(_ => throw exception, TestContext.Current.CancellationToken);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(callOnEntry);
        await handler1.Received(1).InvokeAsync(exception, Arg.Any<CancellationToken>());
        await handler2.Received(1).InvokeAsync(exception, Arg.Any<CancellationToken>());
        await handler3.DidNotReceive().InvokeAsync(Arg.Any<Exception>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task OnException_ShouldPassCancellationTokenNoneToHandlers()
    {
        // Arrange
        var exception = new InvalidOperationException("test");
        var handler = Substitute.For<IAsyncFunc<Exception, ExceptionResult>>();
        handler.InvokeAsync(exception, CancellationToken.None).Returns(ExceptionResult.Continue());
        var behavior = new DefaultExceptionBehavior([handler]);

        // Act
        var callOnEntry = () => behavior.CallOnEntry(_ => throw exception, TestContext.Current.CancellationToken);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(callOnEntry);
        await handler.Received(1).InvokeAsync(exception, CancellationToken.None);
    }
}
