using NSubstitute;
using ZCrew.StateCraft.Extensions;

namespace ZCrew.StateCraft.IntegrationTests.ExceptionHandling;

public partial class ActivateExceptionTests
{
    [Fact]
    public async Task Activate_T1_T2_WhenInitialStateProviderThrowsException_ShouldThrowAndHaveExpectedProperties()
    {
        // Arrange
        var exception = new InvalidOperationException("Test exception");
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState<int, string>(() => throw exception)
            .WithState("A", state => state.WithParameters<int, string>())
            .Build();

        // Act
        var thrownException = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.Activate(TestContext.Current.CancellationToken)
        );

        // Assert
        Assert.Same(exception, thrownException);
        Assert.Null(stateMachine.CurrentState);
        Assert.Null(stateMachine.PreviousState);
        Assert.Null(stateMachine.NextState);
        Assert.False(stateMachine.Parameters.IsCurrentSet);
        Assert.False(stateMachine.Parameters.IsPreviousSet);
        Assert.False(stateMachine.Parameters.IsNextSet);
    }

    [Fact]
    public async Task Activate_T1_T2_WhenInitialStateProviderThrowsExceptionOnce_ShouldRetrySuccessfully()
    {
        // Arrange
        var callCount = 0;
        var stateProvider = Substitute.For<Func<(string, int, string)>>();
        stateProvider
            .Invoke()
            .Returns(_ =>
            {
                if (Interlocked.Increment(ref callCount) == 1)
                    throw new InvalidOperationException();
                return ("A", 42, "hello");
            });
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState(stateProvider)
            .WithState("A", state => state.WithParameters<int, string>())
            .Build();

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.Activate(TestContext.Current.CancellationToken)
        );

        // Act
        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("A", stateMachine.CurrentState.StateValue);
    }

    [Fact]
    public async Task Activate_T1_T2_WhenOnActivateThrowsException_ShouldThrowAndHaveExpectedProperties()
    {
        // Arrange
        var exception = new InvalidOperationException("Test exception");
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42, "hello")
            .WithState("A", state => state.WithParameters<int, string>().OnActivate((_, _, _) => throw exception))
            .Build();

        // Act
        var thrownException = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.Activate(TestContext.Current.CancellationToken)
        );

        // Assert
        Assert.Same(exception, thrownException);
        Assert.Null(stateMachine.CurrentState);
        Assert.Null(stateMachine.PreviousState);
        Assert.Null(stateMachine.NextState);
        Assert.False(stateMachine.Parameters.IsCurrentSet);
        Assert.False(stateMachine.Parameters.IsPreviousSet);
        Assert.False(stateMachine.Parameters.IsNextSet);
    }

    [Fact]
    public async Task Activate_T1_T2_WhenOnActivateThrowsExceptionOnce_ShouldRetrySuccessfully()
    {
        // Arrange
        var callCount = 0;
        var onActivate = Substitute.For<Action<string, int, string>>();
        onActivate
            .When(x => x.Invoke(Arg.Any<string>(), Arg.Any<int>(), Arg.Any<string>()))
            .Do(_ =>
            {
                if (Interlocked.Increment(ref callCount) == 1)
                    throw new InvalidOperationException();
            });
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42, "hello")
            .WithState("A", state => state.WithParameters<int, string>().OnActivate(onActivate))
            .Build();

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.Activate(TestContext.Current.CancellationToken)
        );

        // Act
        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("A", stateMachine.CurrentState.StateValue);
    }

    [Fact]
    public async Task Activate_T1_T2_WhenOnEntryThrowsException_ShouldThrowAndHaveExpectedProperties()
    {
        // Arrange
        var exception = new InvalidOperationException("Test exception");
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42, "hello")
            .WithState("A", state => state.WithParameters<int, string>().OnEntry((_, _) => throw exception))
            .Build();

        // Act
        var thrownException = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.Activate(TestContext.Current.CancellationToken)
        );

        // Assert
        Assert.Same(exception, thrownException);
        Assert.Null(stateMachine.CurrentState);
        Assert.Null(stateMachine.PreviousState);
        Assert.Null(stateMachine.NextState);
        Assert.False(stateMachine.Parameters.IsCurrentSet);
        Assert.False(stateMachine.Parameters.IsPreviousSet);
        Assert.False(stateMachine.Parameters.IsNextSet);
    }

    [Fact]
    public async Task Activate_T1_T2_WhenOnEntryThrowsExceptionOnce_ShouldRetrySuccessfully()
    {
        // Arrange
        var callCount = 0;
        var onEntry = Substitute.For<Action<int, string>>();
        onEntry
            .When(x => x.Invoke(Arg.Any<int>(), Arg.Any<string>()))
            .Do(_ =>
            {
                if (Interlocked.Increment(ref callCount) == 1)
                    throw new InvalidOperationException();
            });
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42, "hello")
            .WithState("A", state => state.WithParameters<int, string>().OnEntry(onEntry))
            .Build();

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.Activate(TestContext.Current.CancellationToken)
        );

        // Act
        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("A", stateMachine.CurrentState.StateValue);
    }
}
