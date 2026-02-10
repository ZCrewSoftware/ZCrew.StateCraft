using NSubstitute;
using ZCrew.StateCraft.Extensions;

namespace ZCrew.StateCraft.IntegrationTests.ExceptionHandling;

public partial class ActivateExceptionTests
{
    [Fact]
    public async Task Activate_T_WhenInitialStateProviderThrowsException_ShouldThrowAndHaveExpectedProperties()
    {
        // Arrange
        var exception = new InvalidOperationException("Test exception");
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState<int>(() => throw exception)
            .WithState("A", state => state.WithParameter<int>())
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
        Assert.Null(stateMachine.CurrentTransition);
    }

    [Fact]
    public async Task Activate_T_WhenInitialStateProviderThrowsExceptionOnce_ShouldRetrySuccessfully()
    {
        // Arrange
        var callCount = 0;
        var stateProvider = Substitute.For<Func<(string, int)>>();
        stateProvider
            .Invoke()
            .Returns(_ =>
            {
                if (Interlocked.Increment(ref callCount) == 1)
                    throw new InvalidOperationException();
                return ("A", 42);
            });
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState(stateProvider)
            .WithState("A", state => state.WithParameter<int>())
            .Build();

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.Activate(TestContext.Current.CancellationToken)
        );

        // Act
        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("A", stateMachine.CurrentState.StateValue);
        Assert.Equal(42, stateMachine.Parameters.GetCurrentParameter<int>());
    }

    [Fact]
    public async Task Activate_T_WhenOnActivateThrowsException_ShouldThrowAndHaveExpectedProperties()
    {
        // Arrange
        var exception = new InvalidOperationException("Test exception");
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42)
            .WithState("A", state => state.WithParameter<int>().OnActivate((_, _) => throw exception))
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
        Assert.Null(stateMachine.CurrentTransition);
    }

    [Fact]
    public async Task Activate_T_WhenOnActivateThrowsExceptionOnce_ShouldRetrySuccessfully()
    {
        // Arrange
        var callCount = 0;
        var onActivate = Substitute.For<Action<string, int>>();
        onActivate
            .When(x => x.Invoke(Arg.Any<string>(), Arg.Any<int>()))
            .Do(_ =>
            {
                if (Interlocked.Increment(ref callCount) == 1)
                    throw new InvalidOperationException();
            });
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42)
            .WithState("A", state => state.WithParameter<int>().OnActivate(onActivate))
            .Build();

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.Activate(TestContext.Current.CancellationToken)
        );

        // Act
        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("A", stateMachine.CurrentState.StateValue);
        Assert.Equal(42, stateMachine.Parameters.GetCurrentParameter<int>());
    }

    [Fact]
    public async Task Activate_T_WhenOnEntryThrowsException_ShouldThrowAndHaveExpectedProperties()
    {
        // Arrange
        var exception = new InvalidOperationException("Test exception");
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42)
            .WithState("A", state => state.WithParameter<int>().OnEntry(_ => throw exception))
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
        Assert.Null(stateMachine.CurrentTransition);
    }

    [Fact]
    public async Task Activate_T_WhenOnEntryThrowsExceptionOnce_ShouldRetrySuccessfully()
    {
        // Arrange
        var callCount = 0;
        var onEntry = Substitute.For<Action<int>>();
        onEntry
            .When(x => x.Invoke(Arg.Any<int>()))
            .Do(_ =>
            {
                if (Interlocked.Increment(ref callCount) == 1)
                    throw new InvalidOperationException();
            });
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42)
            .WithState("A", state => state.WithParameter<int>().OnEntry(onEntry))
            .Build();

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.Activate(TestContext.Current.CancellationToken)
        );

        // Act
        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("A", stateMachine.CurrentState.StateValue);
        Assert.Equal(42, stateMachine.Parameters.GetCurrentParameter<int>());
    }
}
