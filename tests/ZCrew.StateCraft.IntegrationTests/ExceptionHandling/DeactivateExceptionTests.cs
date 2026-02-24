using NSubstitute;
using ZCrew.StateCraft.Extensions;

namespace ZCrew.StateCraft.IntegrationTests.ExceptionHandling;

public class DeactivateExceptionTests
{
    [Fact]
    public async Task Deactivate_WhenOnExitThrowsException_ShouldThrowAndHaveExpectedProperties()
    {
        // Arrange
        var exception = new InvalidOperationException("Test exception");
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.OnExit(() => throw exception))
            .Build();
        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var thrownException = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.Deactivate(TestContext.Current.CancellationToken)
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
    }

    [Fact]
    public async Task Deactivate_WhenOnExitThrowsExceptionOnce_ShouldRetrySuccessfully()
    {
        // Arrange
        var callCount = 0;
        var onExit = Substitute.For<Action>();
        onExit
            .When(x => x.Invoke())
            .Do(_ =>
            {
                if (Interlocked.Increment(ref callCount) == 1)
                    throw new InvalidOperationException();
            });
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.OnExit(onExit))
            .Build();
        await stateMachine.Activate(TestContext.Current.CancellationToken);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.Deactivate(TestContext.Current.CancellationToken)
        );

        // Act
        await stateMachine.Deactivate(TestContext.Current.CancellationToken);

        // Assert
        Assert.Null(stateMachine.CurrentState);
        Assert.Null(stateMachine.PreviousState);
        Assert.Null(stateMachine.NextState);
    }

    [Fact]
    public async Task Deactivate_T_WhenOnExitThrowsException_ShouldThrowAndHaveExpectedProperties()
    {
        // Arrange
        var exception = new InvalidOperationException("Test exception");
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42)
            .WithState("A", state => state.WithParameter<int>().OnExit(_ => throw exception))
            .Build();
        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var thrownException = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.Deactivate(TestContext.Current.CancellationToken)
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
    }

    [Fact]
    public async Task Deactivate_T_WhenOnExitThrowsExceptionOnce_ShouldRetrySuccessfully()
    {
        // Arrange
        var callCount = 0;
        var onExit = Substitute.For<Action<int>>();
        onExit
            .When(x => x.Invoke(Arg.Any<int>()))
            .Do(_ =>
            {
                if (Interlocked.Increment(ref callCount) == 1)
                    throw new InvalidOperationException();
            });
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42)
            .WithState("A", state => state.WithParameter<int>().OnExit(onExit))
            .Build();
        await stateMachine.Activate(TestContext.Current.CancellationToken);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.Deactivate(TestContext.Current.CancellationToken)
        );

        // Act
        await stateMachine.Deactivate(TestContext.Current.CancellationToken);

        // Assert
        Assert.Null(stateMachine.CurrentState);
        Assert.Null(stateMachine.PreviousState);
        Assert.Null(stateMachine.NextState);
    }

    [Fact]
    public async Task Deactivate_WhenOnDeactivateThrowsException_ShouldThrowAndHaveExpectedProperties()
    {
        // Arrange
        var exception = new InvalidOperationException("Test exception");
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.OnDeactivate(_ => throw exception))
            .Build();
        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var thrownException = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.Deactivate(TestContext.Current.CancellationToken)
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
    }

    [Fact]
    public async Task Deactivate_WhenOnDeactivateThrowsExceptionOnce_ShouldRetrySuccessfully()
    {
        // Arrange
        var callCount = 0;
        var onDeactivate = Substitute.For<Action<string>>();
        onDeactivate
            .When(x => x.Invoke(Arg.Any<string>()))
            .Do(_ =>
            {
                if (Interlocked.Increment(ref callCount) == 1)
                    throw new InvalidOperationException();
            });
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.OnDeactivate(onDeactivate))
            .Build();
        await stateMachine.Activate(TestContext.Current.CancellationToken);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.Deactivate(TestContext.Current.CancellationToken)
        );

        // Act
        await stateMachine.Deactivate(TestContext.Current.CancellationToken);

        // Assert
        Assert.Null(stateMachine.CurrentState);
        Assert.Null(stateMachine.PreviousState);
        Assert.Null(stateMachine.NextState);
    }

    [Fact]
    public async Task Deactivate_T_WhenOnDeactivateThrowsException_ShouldThrowAndHaveExpectedProperties()
    {
        // Arrange
        var exception = new InvalidOperationException("Test exception");
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42)
            .WithState("A", state => state.WithParameter<int>().OnDeactivate((_, _) => throw exception))
            .Build();
        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var thrownException = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.Deactivate(TestContext.Current.CancellationToken)
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
    }

    [Fact]
    public async Task Deactivate_T_WhenOnDeactivateThrowsExceptionOnce_ShouldRetrySuccessfully()
    {
        // Arrange
        var callCount = 0;
        var onDeactivate = Substitute.For<Action<string, int>>();
        onDeactivate
            .When(x => x.Invoke(Arg.Any<string>(), Arg.Any<int>()))
            .Do(_ =>
            {
                if (Interlocked.Increment(ref callCount) == 1)
                    throw new InvalidOperationException();
            });
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42)
            .WithState("A", state => state.WithParameter<int>().OnDeactivate(onDeactivate))
            .Build();
        await stateMachine.Activate(TestContext.Current.CancellationToken);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.Deactivate(TestContext.Current.CancellationToken)
        );

        // Act
        await stateMachine.Deactivate(TestContext.Current.CancellationToken);

        // Assert
        Assert.Null(stateMachine.CurrentState);
        Assert.Null(stateMachine.PreviousState);
        Assert.Null(stateMachine.NextState);
    }
}
