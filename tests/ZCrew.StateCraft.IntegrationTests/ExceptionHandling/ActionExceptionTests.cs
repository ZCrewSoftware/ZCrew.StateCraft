using NSubstitute;
using ZCrew.StateCraft.Extensions;

namespace ZCrew.StateCraft.IntegrationTests.ExceptionHandling;

public class ActionExceptionTests
{
    [Fact]
    public async Task Activate_WhenActionThrowsException_ShouldCallOnExceptionHandler()
    {
        // Arrange
        var exception = new InvalidOperationException("Test exception");
        var onException = Substitute.For<Func<Exception, ExceptionResult>>();
        onException.Invoke(Arg.Any<Exception>()).Returns(ExceptionResult.Rethrow());
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .OnException(onException)
            .WithState("A", state => state.WithAction(a => a.Invoke(() => throw exception)))
            .Build();

        // Act
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.Activate(TestContext.Current.CancellationToken)
        );

        // Assert
        onException.Received(1).Invoke(exception);
    }

    [Fact]
    public async Task Activate_WhenActionThrowsException_ShouldThrowAndHaveExpectedProperties()
    {
        // Arrange
        var exception = new InvalidOperationException("Test exception");
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithAction(a => a.Invoke(() => throw exception)))
            .Build();

        // Act
        var thrownException = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.Activate(TestContext.Current.CancellationToken)
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
    public async Task Activate_WhenActionThrowsExceptionOnce_ShouldRetrySuccessfully()
    {
        // Arrange
        var callCount = 0;
        var action = Substitute.For<Action>();
        action
            .When(x => x.Invoke())
            .Do(_ =>
            {
                if (Interlocked.Increment(ref callCount) == 1)
                    throw new InvalidOperationException();
            });
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithAction(a => a.Invoke(action)).WithTransition("To B", "B"))
            .WithState("B", state => state)
            .Build();

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.Activate(TestContext.Current.CancellationToken)
        );

        // Act
        await stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("B", stateMachine.CurrentState.StateValue);
    }

    [Fact]
    public async Task Activate_WhenAsyncActionThrowsException_ShouldCallOnExceptionHandler()
    {
        // Arrange
        var exception = new InvalidOperationException("Test exception");
        var onException = Substitute.For<Func<Exception, ExceptionResult>>();
        onException.Invoke(Arg.Any<Exception>()).Returns(ExceptionResult.Rethrow());
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .OnException(onException)
            .WithState("A", state => state.WithAction(a => a.Invoke(Action)))
            .Build();

        // Act
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.Activate(TestContext.Current.CancellationToken)
        );

        // Assert
        onException.Received(1).Invoke(exception);

        return;

        async Task Action(CancellationToken _)
        {
            await Task.Yield();
            throw exception;
        }
    }

    [Fact]
    public async Task Activate_T_WhenActionThrowsException_ShouldCallOnExceptionHandler()
    {
        // Arrange
        var exception = new InvalidOperationException("Test exception");
        var onException = Substitute.For<Func<Exception, ExceptionResult>>();
        onException.Invoke(Arg.Any<Exception>()).Returns(ExceptionResult.Rethrow());
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42)
            .OnException(onException)
            .WithState("A", state => state.WithParameter<int>().WithAction(a => a.Invoke(_ => throw exception)))
            .Build();

        // Act
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.Activate(TestContext.Current.CancellationToken)
        );

        // Assert
        onException.Received(1).Invoke(exception);
    }

    [Fact]
    public async Task Activate_T_WhenActionThrowsException_ShouldThrowAndHaveExpectedProperties()
    {
        // Arrange
        var exception = new InvalidOperationException("Test exception");
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42)
            .WithState("A", state => state.WithParameter<int>().WithAction(a => a.Invoke(_ => throw exception)))
            .Build();

        // Act
        var thrownException = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.Activate(TestContext.Current.CancellationToken)
        );

        // Assert
        Assert.Same(exception, thrownException);
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("A", stateMachine.CurrentState.StateValue);
        Assert.Null(stateMachine.PreviousState);
        Assert.Null(stateMachine.NextState);
        Assert.Equal(42, stateMachine.Parameters.GetCurrentParameter<int>());
        Assert.False(stateMachine.Parameters.IsPreviousSet);
        Assert.False(stateMachine.Parameters.IsNextSet);
        Assert.Null(stateMachine.CurrentTransition);
    }

    [Fact]
    public async Task Activate_T_WhenActionThrowsExceptionOnce_ShouldRetrySuccessfully()
    {
        // Arrange
        var callCount = 0;
        var action = Substitute.For<Action<int>>();
        action
            .When(x => x.Invoke(Arg.Any<int>()))
            .Do(_ =>
            {
                if (Interlocked.Increment(ref callCount) == 1)
                    throw new InvalidOperationException();
            });
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42)
            .WithState(
                "A",
                state =>
                    state.WithParameter<int>().WithAction(a => a.Invoke(action)).WithTransition<string>("To B", "B")
            )
            .WithState("B", state => state.WithParameter<string>())
            .Build();

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.Activate(TestContext.Current.CancellationToken)
        );

        // Act
        await stateMachine.Transition("To B", "hello", TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("B", stateMachine.CurrentState.StateValue);
        Assert.Equal("hello", stateMachine.Parameters.GetCurrentParameter<string>());
    }

    [Fact]
    public async Task Transition_WhenActionThrowsException_ShouldCallOnExceptionHandler()
    {
        // Arrange
        var exception = new InvalidOperationException("Test exception");
        var onException = Substitute.For<Func<Exception, ExceptionResult>>();
        onException.Invoke(Arg.Any<Exception>()).Returns(ExceptionResult.Rethrow());
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .OnException(onException)
            .WithState("A", state => state.WithTransition("To B", "B"))
            .WithState("B", state => state.WithAction(a => a.Invoke(() => throw exception)))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.Transition("To B", TestContext.Current.CancellationToken)
        );

        // Assert
        onException.Received(1).Invoke(exception);
    }

    [Fact]
    public async Task Transition_WhenActionThrowsException_ShouldThrowAndHaveExpectedProperties()
    {
        // Arrange
        var exception = new InvalidOperationException("Test exception");
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition("To B", "B"))
            .WithState("B", state => state.WithAction(a => a.Invoke(() => throw exception)))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var thrownException = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.Transition("To B", TestContext.Current.CancellationToken)
        );

        // Assert
        Assert.Same(exception, thrownException);
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("B", stateMachine.CurrentState.StateValue);
        Assert.Null(stateMachine.PreviousState);
        Assert.Null(stateMachine.NextState);
        Assert.Empty(stateMachine.Parameters.CurrentParameterTypes);
        Assert.False(stateMachine.Parameters.IsPreviousSet);
        Assert.False(stateMachine.Parameters.IsNextSet);
        Assert.Null(stateMachine.CurrentTransition);
    }

    [Fact]
    public async Task Transition_WhenActionThrowsExceptionOnce_ShouldRetrySuccessfully()
    {
        // Arrange
        var callCount = 0;
        var action = Substitute.For<Action>();
        action
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
            .WithState("B", state => state.WithAction(a => a.Invoke(action)).WithTransition("To C", "C"))
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
    }

    [Fact]
    public async Task Transition_WhenAsyncActionThrowsException_ShouldCallOnExceptionHandler()
    {
        // Arrange
        var exception = new InvalidOperationException("Test exception");
        var onException = Substitute.For<Func<Exception, ExceptionResult>>();
        onException.Invoke(Arg.Any<Exception>()).Returns(ExceptionResult.Rethrow());
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .OnException(onException)
            .WithState("A", state => state.WithTransition("To B", "B"))
            .WithState("B", state => state.WithAction(a => a.Invoke(Action)))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.Transition("To B", TestContext.Current.CancellationToken)
        );

        // Assert
        onException.Received(1).Invoke(exception);

        return;

        async Task Action(CancellationToken _)
        {
            await Task.Yield();
            throw exception;
        }
    }

    [Fact]
    public async Task Transition_T_WhenActionThrowsException_ShouldCallOnExceptionHandler()
    {
        // Arrange
        var exception = new InvalidOperationException("Test exception");
        var onException = Substitute.For<Func<Exception, ExceptionResult>>();
        onException.Invoke(Arg.Any<Exception>()).Returns(ExceptionResult.Rethrow());
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42)
            .OnException(onException)
            .WithState("A", state => state.WithParameter<int>().WithTransition<string>("To B", "B"))
            .WithState("B", state => state.WithParameter<string>().WithAction(a => a.Invoke(_ => throw exception)))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.Transition("To B", "hello", TestContext.Current.CancellationToken)
        );

        // Assert
        onException.Received(1).Invoke(exception);
    }

    [Fact]
    public async Task Transition_T_WhenActionThrowsException_ShouldThrowAndHaveExpectedProperties()
    {
        // Arrange
        var exception = new InvalidOperationException("Test exception");
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42)
            .WithState("A", state => state.WithParameter<int>().WithTransition<string>("To B", "B"))
            .WithState("B", state => state.WithParameter<string>().WithAction(a => a.Invoke(_ => throw exception)))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var thrownException = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.Transition("To B", "hello", TestContext.Current.CancellationToken)
        );

        // Assert
        Assert.Same(exception, thrownException);
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("B", stateMachine.CurrentState.StateValue);
        Assert.Null(stateMachine.PreviousState);
        Assert.Null(stateMachine.NextState);
        Assert.Equal("hello", stateMachine.Parameters.GetCurrentParameter<string>());
        Assert.False(stateMachine.Parameters.IsPreviousSet);
        Assert.False(stateMachine.Parameters.IsNextSet);
        Assert.Null(stateMachine.CurrentTransition);
    }

    [Fact]
    public async Task Transition_T_WhenActionThrowsExceptionOnce_ShouldRetrySuccessfully()
    {
        // Arrange
        var callCount = 0;
        var action = Substitute.For<Action<string>>();
        action
            .When(x => x.Invoke(Arg.Any<string>()))
            .Do(_ =>
            {
                if (Interlocked.Increment(ref callCount) == 1)
                    throw new InvalidOperationException();
            });
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42)
            .WithState("A", state => state.WithParameter<int>().WithTransition<string>("To B", "B"))
            .WithState(
                "B",
                state =>
                    state.WithParameter<string>().WithAction(a => a.Invoke(action)).WithTransition<int>("To C", "C")
            )
            .WithState("C", state => state.WithParameter<int>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.Transition("To B", "hello", TestContext.Current.CancellationToken)
        );

        // Act
        await stateMachine.Transition("To C", 99, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("C", stateMachine.CurrentState.StateValue);
        Assert.Equal(99, stateMachine.Parameters.GetCurrentParameter<int>());
    }

    [Fact]
    public async Task TryTransition_WhenActionThrowsException_ShouldCallOnExceptionHandler()
    {
        // Arrange
        var exception = new InvalidOperationException("Test exception");
        var onException = Substitute.For<Func<Exception, ExceptionResult>>();
        onException.Invoke(Arg.Any<Exception>()).Returns(ExceptionResult.Rethrow());
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .OnException(onException)
            .WithState("A", state => state.WithTransition("To B", "B"))
            .WithState("B", state => state.WithAction(a => a.Invoke(() => throw exception)))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.TryTransition("To B", TestContext.Current.CancellationToken)
        );

        // Assert
        onException.Received(1).Invoke(exception);
    }

    [Fact]
    public async Task TryTransition_WhenActionThrowsException_ShouldThrowAndHaveExpectedProperties()
    {
        // Arrange
        var exception = new InvalidOperationException("Test exception");
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition("To B", "B"))
            .WithState("B", state => state.WithAction(a => a.Invoke(() => throw exception)))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var thrownException = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.TryTransition("To B", TestContext.Current.CancellationToken)
        );

        // Assert
        Assert.Same(exception, thrownException);
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("B", stateMachine.CurrentState.StateValue);
        Assert.Null(stateMachine.PreviousState);
        Assert.Null(stateMachine.NextState);
        Assert.Empty(stateMachine.Parameters.CurrentParameterTypes);
        Assert.False(stateMachine.Parameters.IsPreviousSet);
        Assert.False(stateMachine.Parameters.IsNextSet);
        Assert.Null(stateMachine.CurrentTransition);
    }

    [Fact]
    public async Task TryTransition_T_WhenActionThrowsException_ShouldCallOnExceptionHandler()
    {
        // Arrange
        var exception = new InvalidOperationException("Test exception");
        var onException = Substitute.For<Func<Exception, ExceptionResult>>();
        onException.Invoke(Arg.Any<Exception>()).Returns(ExceptionResult.Rethrow());
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42)
            .OnException(onException)
            .WithState("A", state => state.WithParameter<int>().WithTransition<string>("To B", "B"))
            .WithState("B", state => state.WithParameter<string>().WithAction(a => a.Invoke(_ => throw exception)))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.TryTransition("To B", "hello", TestContext.Current.CancellationToken)
        );

        // Assert
        onException.Received(1).Invoke(exception);
    }

    [Fact]
    public async Task TryTransition_T_WhenActionThrowsException_ShouldThrowAndHaveExpectedProperties()
    {
        // Arrange
        var exception = new InvalidOperationException("Test exception");
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42)
            .WithState("A", state => state.WithParameter<int>().WithTransition<string>("To B", "B"))
            .WithState("B", state => state.WithParameter<string>().WithAction(a => a.Invoke(_ => throw exception)))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var thrownException = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.TryTransition("To B", "hello", TestContext.Current.CancellationToken)
        );

        // Assert
        Assert.Same(exception, thrownException);
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("B", stateMachine.CurrentState.StateValue);
        Assert.Null(stateMachine.PreviousState);
        Assert.Null(stateMachine.NextState);
        Assert.Equal("hello", stateMachine.Parameters.GetCurrentParameter<string>());
        Assert.False(stateMachine.Parameters.IsPreviousSet);
        Assert.False(stateMachine.Parameters.IsNextSet);
        Assert.Null(stateMachine.CurrentTransition);
    }

    [Fact]
    public async Task Activate_WhenOnExceptionReturnsThrow_ShouldThrowNewException()
    {
        // Arrange
        var originalException = new InvalidOperationException("Original exception");
        var newException = new ArgumentException("New exception");
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .OnException(_ => ExceptionResult.Throw(newException))
            .WithState("A", state => state.WithAction(a => a.Invoke(() => throw originalException)))
            .Build();

        // Act
        var thrownException = await Assert.ThrowsAsync<ArgumentException>(() =>
            stateMachine.Activate(TestContext.Current.CancellationToken)
        );

        // Assert
        Assert.Same(newException, thrownException);
    }

    [Fact]
    public async Task Activate_WhenOnExceptionReturnsContinue_ShouldCallNextHandler()
    {
        // Arrange
        var exception = new InvalidOperationException("Test exception");
        var firstHandler = Substitute.For<Func<Exception, ExceptionResult>>();
        firstHandler.Invoke(Arg.Any<Exception>()).Returns(ExceptionResult.Continue());
        var secondHandler = Substitute.For<Func<Exception, ExceptionResult>>();
        secondHandler.Invoke(Arg.Any<Exception>()).Returns(ExceptionResult.Rethrow());
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .OnException(firstHandler)
            .OnException(secondHandler)
            .WithState("A", state => state.WithAction(a => a.Invoke(() => throw exception)))
            .Build();

        // Act
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.Activate(TestContext.Current.CancellationToken)
        );

        // Assert
        firstHandler.Received(1).Invoke(exception);
        secondHandler.Received(1).Invoke(exception);
    }
}
