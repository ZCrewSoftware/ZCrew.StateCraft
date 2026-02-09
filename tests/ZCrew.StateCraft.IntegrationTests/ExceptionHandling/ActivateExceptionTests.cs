using NSubstitute;
using ZCrew.StateCraft.Extensions;

namespace ZCrew.StateCraft.IntegrationTests.ExceptionHandling;

public class ActivateExceptionTests
{
    [Fact]
    public async Task Activate_WhenInitialStateProviderThrowsException_ShouldThrowAndHaveExpectedProperties()
    {
        // Arrange
        var exception = new InvalidOperationException("Test exception");
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState(() => throw exception)
            .WithState("A", state => state)
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
    public async Task Activate_WhenInitialStateProviderThrowsExceptionOnce_ShouldRetrySuccessfully()
    {
        // Arrange
        var callCount = 0;
        var stateProvider = Substitute.For<Func<string>>();
        stateProvider
            .Invoke()
            .Returns(_ =>
            {
                if (Interlocked.Increment(ref callCount) == 1)
                    throw new InvalidOperationException();
                return "A";
            });
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState(stateProvider)
            .WithState("A", state => state)
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
    public async Task Activate_WhenOnActivateThrowsException_ShouldThrowAndHaveExpectedProperties()
    {
        // Arrange
        var exception = new InvalidOperationException("Test exception");
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.OnActivate(_ => throw exception))
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
    public async Task Activate_WhenOnActivateThrowsExceptionOnce_ShouldRetrySuccessfully()
    {
        // Arrange
        var callCount = 0;
        var onActivate = Substitute.For<Action<string>>();
        onActivate
            .When(x => x.Invoke(Arg.Any<string>()))
            .Do(_ =>
            {
                if (Interlocked.Increment(ref callCount) == 1)
                    throw new InvalidOperationException();
            });
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.OnActivate(onActivate))
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
    public async Task Activate_WhenOnEntryThrowsException_ShouldThrowAndHaveExpectedProperties()
    {
        // Arrange
        var exception = new InvalidOperationException("Test exception");
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.OnEntry(() => throw exception))
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
    public async Task Activate_WhenOnEntryThrowsExceptionOnce_ShouldRetrySuccessfully()
    {
        // Arrange
        var callCount = 0;
        var onEntry = Substitute.For<Action>();
        onEntry
            .When(x => x.Invoke())
            .Do(_ =>
            {
                if (Interlocked.Increment(ref callCount) == 1)
                    throw new InvalidOperationException();
            });
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.OnEntry(onEntry))
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

    // [Fact]
    // public async Task Activate_T1_T2_WhenInitialStateProviderThrowsException_ShouldThrowAndHaveExpectedProperties()
    // {
    //     // Arrange
    //     var exception = new InvalidOperationException("Test exception");
    //     var stateMachine = StateMachine
    //         .Configure<string, string>()
    //         .WithInitialState<int, string>(() => throw exception)
    //         .WithState("A", state => state.WithParameter<int, string>())
    //         .Build();
    //
    //     // Act
    //     var thrownException = await Assert.ThrowsAsync<InvalidOperationException>(() =>
    //         stateMachine.Activate(TestContext.Current.CancellationToken)
    //     );
    //
    //     // Assert
    //     Assert.Same(exception, thrownException);
    //     Assert.Null(stateMachine.CurrentState);
    //     Assert.Null(stateMachine.PreviousState);
    //     Assert.Null(stateMachine.NextState);
    //     Assert.False(stateMachine.Parameters.IsCurrentSet);
    //     Assert.False(stateMachine.Parameters.IsPreviousSet);
    //     Assert.False(stateMachine.Parameters.IsNextSet);
    //     Assert.Null(stateMachine.CurrentTransition);
    // }

    // [Fact]
    // public async Task Activate_T1_T2_WhenInitialStateProviderThrowsExceptionOnce_ShouldRetrySuccessfully()
    // {
    //     // Arrange
    //     var callCount = 0;
    //     var stateProvider = Substitute.For<Func<(string, int, string)>>();
    //     stateProvider
    //         .Invoke()
    //         .Returns(_ =>
    //         {
    //             if (Interlocked.Increment(ref callCount) == 1)
    //                 throw new InvalidOperationException();
    //             return ("A", 42, "hello");
    //         });
    //     var stateMachine = StateMachine
    //         .Configure<string, string>()
    //         .WithInitialState(stateProvider)
    //         .WithState("A", state => state.WithParameter<int, string>())
    //         .Build();
    //
    //     await Assert.ThrowsAsync<InvalidOperationException>(() =>
    //         stateMachine.Activate(TestContext.Current.CancellationToken)
    //     );
    //
    //     // Act
    //     await stateMachine.Activate(TestContext.Current.CancellationToken);
    //
    //     // Assert
    //     Assert.NotNull(stateMachine.CurrentState);
    //     Assert.Equal("A", stateMachine.CurrentState.StateValue);
    // }

    // [Fact]
    // public async Task Activate_T1_T2_WhenOnActivateThrowsException_ShouldThrowAndHaveExpectedProperties()
    // {
    //     // Arrange
    //     var exception = new InvalidOperationException("Test exception");
    //     var stateMachine = StateMachine
    //         .Configure<string, string>()
    //         .WithInitialState("A", 42, "hello")
    //         .WithState("A", state => state.WithParameter<int, string>()
    //             .OnActivate((_, _, _) => throw exception))
    //         .Build();
    //
    //     // Act
    //     var thrownException = await Assert.ThrowsAsync<InvalidOperationException>(() =>
    //         stateMachine.Activate(TestContext.Current.CancellationToken)
    //     );
    //
    //     // Assert
    //     Assert.Same(exception, thrownException);
    //     Assert.Null(stateMachine.CurrentState);
    //     Assert.Null(stateMachine.PreviousState);
    //     Assert.Null(stateMachine.NextState);
    //     Assert.False(stateMachine.Parameters.IsCurrentSet);
    //     Assert.False(stateMachine.Parameters.IsPreviousSet);
    //     Assert.False(stateMachine.Parameters.IsNextSet);
    //     Assert.Null(stateMachine.CurrentTransition);
    // }

    // [Fact]
    // public async Task Activate_T1_T2_WhenOnActivateThrowsExceptionOnce_ShouldRetrySuccessfully()
    // {
    //     // Arrange
    //     var callCount = 0;
    //     var onActivate = Substitute.For<Action<string, int, string>>();
    //     onActivate
    //         .When(x => x.Invoke(Arg.Any<string>(), Arg.Any<int>(), Arg.Any<string>()))
    //         .Do(_ =>
    //         {
    //             if (Interlocked.Increment(ref callCount) == 1)
    //                 throw new InvalidOperationException();
    //         });
    //     var stateMachine = StateMachine
    //         .Configure<string, string>()
    //         .WithInitialState("A", 42, "hello")
    //         .WithState("A", state => state.WithParameter<int, string>().OnActivate(onActivate))
    //         .Build();
    //
    //     await Assert.ThrowsAsync<InvalidOperationException>(() =>
    //         stateMachine.Activate(TestContext.Current.CancellationToken)
    //     );
    //
    //     // Act
    //     await stateMachine.Activate(TestContext.Current.CancellationToken);
    //
    //     // Assert
    //     Assert.NotNull(stateMachine.CurrentState);
    //     Assert.Equal("A", stateMachine.CurrentState.StateValue);
    // }

    // [Fact]
    // public async Task Activate_T1_T2_WhenOnEntryThrowsException_ShouldThrowAndHaveExpectedProperties()
    // {
    //     // Arrange
    //     var exception = new InvalidOperationException("Test exception");
    //     var stateMachine = StateMachine
    //         .Configure<string, string>()
    //         .WithInitialState("A", 42, "hello")
    //         .WithState("A", state => state.WithParameter<int, string>()
    //             .OnEntry((_, _) => throw exception))
    //         .Build();
    //
    //     // Act
    //     var thrownException = await Assert.ThrowsAsync<InvalidOperationException>(() =>
    //         stateMachine.Activate(TestContext.Current.CancellationToken)
    //     );
    //
    //     // Assert
    //     Assert.Same(exception, thrownException);
    //     Assert.Null(stateMachine.CurrentState);
    //     Assert.Null(stateMachine.PreviousState);
    //     Assert.Null(stateMachine.NextState);
    //     Assert.False(stateMachine.Parameters.IsCurrentSet);
    //     Assert.False(stateMachine.Parameters.IsPreviousSet);
    //     Assert.False(stateMachine.Parameters.IsNextSet);
    //     Assert.Null(stateMachine.CurrentTransition);
    // }

    // [Fact]
    // public async Task Activate_T1_T2_WhenOnEntryThrowsExceptionOnce_ShouldRetrySuccessfully()
    // {
    //     // Arrange
    //     var callCount = 0;
    //     var onEntry = Substitute.For<Action<int, string>>();
    //     onEntry
    //         .When(x => x.Invoke(Arg.Any<int>(), Arg.Any<string>()))
    //         .Do(_ =>
    //         {
    //             if (Interlocked.Increment(ref callCount) == 1)
    //                 throw new InvalidOperationException();
    //         });
    //     var stateMachine = StateMachine
    //         .Configure<string, string>()
    //         .WithInitialState("A", 42, "hello")
    //         .WithState("A", state => state.WithParameter<int, string>().OnEntry(onEntry))
    //         .Build();
    //
    //     await Assert.ThrowsAsync<InvalidOperationException>(() =>
    //         stateMachine.Activate(TestContext.Current.CancellationToken)
    //     );
    //
    //     // Act
    //     await stateMachine.Activate(TestContext.Current.CancellationToken);
    //
    //     // Assert
    //     Assert.NotNull(stateMachine.CurrentState);
    //     Assert.Equal("A", stateMachine.CurrentState.StateValue);
    // }

    // [Fact]
    // public async Task Activate_T1_T2_T3_WhenInitialStateProviderThrowsException_ShouldThrowAndHaveExpectedProperties()
    // {
    //     // Arrange
    //     var exception = new InvalidOperationException("Test exception");
    //     var stateMachine = StateMachine
    //         .Configure<string, string>()
    //         .WithInitialState<int, string, double>(() => throw exception)
    //         .WithState("A", state => state.WithParameter<int, string, double>())
    //         .Build();
    //
    //     // Act
    //     var thrownException = await Assert.ThrowsAsync<InvalidOperationException>(() =>
    //         stateMachine.Activate(TestContext.Current.CancellationToken)
    //     );
    //
    //     // Assert
    //     Assert.Same(exception, thrownException);
    //     Assert.Null(stateMachine.CurrentState);
    //     Assert.Null(stateMachine.PreviousState);
    //     Assert.Null(stateMachine.NextState);
    //     Assert.False(stateMachine.Parameters.IsCurrentSet);
    //     Assert.False(stateMachine.Parameters.IsPreviousSet);
    //     Assert.False(stateMachine.Parameters.IsNextSet);
    //     Assert.Null(stateMachine.CurrentTransition);
    // }

    // [Fact]
    // public async Task Activate_T1_T2_T3_WhenInitialStateProviderThrowsExceptionOnce_ShouldRetrySuccessfully()
    // {
    //     // Arrange
    //     var callCount = 0;
    //     var stateProvider = Substitute.For<Func<(string, int, string, double)>>();
    //     stateProvider
    //         .Invoke()
    //         .Returns(_ =>
    //         {
    //             if (Interlocked.Increment(ref callCount) == 1)
    //                 throw new InvalidOperationException();
    //             return ("A", 42, "hello", 3.14);
    //         });
    //     var stateMachine = StateMachine
    //         .Configure<string, string>()
    //         .WithInitialState(stateProvider)
    //         .WithState("A", state => state.WithParameter<int, string, double>())
    //         .Build();
    //
    //     await Assert.ThrowsAsync<InvalidOperationException>(() =>
    //         stateMachine.Activate(TestContext.Current.CancellationToken)
    //     );
    //
    //     // Act
    //     await stateMachine.Activate(TestContext.Current.CancellationToken);
    //
    //     // Assert
    //     Assert.NotNull(stateMachine.CurrentState);
    //     Assert.Equal("A", stateMachine.CurrentState.StateValue);
    // }

    // [Fact]
    // public async Task Activate_T1_T2_T3_WhenOnActivateThrowsException_ShouldThrowAndHaveExpectedProperties()
    // {
    //     // Arrange
    //     var exception = new InvalidOperationException("Test exception");
    //     var stateMachine = StateMachine
    //         .Configure<string, string>()
    //         .WithInitialState("A", 42, "hello", 3.14)
    //         .WithState("A", state => state.WithParameter<int, string, double>()
    //             .OnActivate((_, _, _, _) => throw exception))
    //         .Build();
    //
    //     // Act
    //     var thrownException = await Assert.ThrowsAsync<InvalidOperationException>(() =>
    //         stateMachine.Activate(TestContext.Current.CancellationToken)
    //     );
    //
    //     // Assert
    //     Assert.Same(exception, thrownException);
    //     Assert.Null(stateMachine.CurrentState);
    //     Assert.Null(stateMachine.PreviousState);
    //     Assert.Null(stateMachine.NextState);
    //     Assert.False(stateMachine.Parameters.IsCurrentSet);
    //     Assert.False(stateMachine.Parameters.IsPreviousSet);
    //     Assert.False(stateMachine.Parameters.IsNextSet);
    //     Assert.Null(stateMachine.CurrentTransition);
    // }

    // [Fact]
    // public async Task Activate_T1_T2_T3_WhenOnActivateThrowsExceptionOnce_ShouldRetrySuccessfully()
    // {
    //     // Arrange
    //     var callCount = 0;
    //     var onActivate = Substitute.For<Action<string, int, string, double>>();
    //     onActivate
    //         .When(x => x.Invoke(Arg.Any<string>(), Arg.Any<int>(), Arg.Any<string>(), Arg.Any<double>()))
    //         .Do(_ =>
    //         {
    //             if (Interlocked.Increment(ref callCount) == 1)
    //                 throw new InvalidOperationException();
    //         });
    //     var stateMachine = StateMachine
    //         .Configure<string, string>()
    //         .WithInitialState("A", 42, "hello", 3.14)
    //         .WithState("A", state => state.WithParameter<int, string, double>().OnActivate(onActivate))
    //         .Build();
    //
    //     await Assert.ThrowsAsync<InvalidOperationException>(() =>
    //         stateMachine.Activate(TestContext.Current.CancellationToken)
    //     );
    //
    //     // Act
    //     await stateMachine.Activate(TestContext.Current.CancellationToken);
    //
    //     // Assert
    //     Assert.NotNull(stateMachine.CurrentState);
    //     Assert.Equal("A", stateMachine.CurrentState.StateValue);
    // }

    // [Fact]
    // public async Task Activate_T1_T2_T3_WhenOnEntryThrowsException_ShouldThrowAndHaveExpectedProperties()
    // {
    //     // Arrange
    //     var exception = new InvalidOperationException("Test exception");
    //     var stateMachine = StateMachine
    //         .Configure<string, string>()
    //         .WithInitialState("A", 42, "hello", 3.14)
    //         .WithState("A", state => state.WithParameter<int, string, double>()
    //             .OnEntry((_, _, _) => throw exception))
    //         .Build();
    //
    //     // Act
    //     var thrownException = await Assert.ThrowsAsync<InvalidOperationException>(() =>
    //         stateMachine.Activate(TestContext.Current.CancellationToken)
    //     );
    //
    //     // Assert
    //     Assert.Same(exception, thrownException);
    //     Assert.Null(stateMachine.CurrentState);
    //     Assert.Null(stateMachine.PreviousState);
    //     Assert.Null(stateMachine.NextState);
    //     Assert.False(stateMachine.Parameters.IsCurrentSet);
    //     Assert.False(stateMachine.Parameters.IsPreviousSet);
    //     Assert.False(stateMachine.Parameters.IsNextSet);
    //     Assert.Null(stateMachine.CurrentTransition);
    // }

    // [Fact]
    // public async Task Activate_T1_T2_T3_WhenOnEntryThrowsExceptionOnce_ShouldRetrySuccessfully()
    // {
    //     // Arrange
    //     var callCount = 0;
    //     var onEntry = Substitute.For<Action<int, string, double>>();
    //     onEntry
    //         .When(x => x.Invoke(Arg.Any<int>(), Arg.Any<string>(), Arg.Any<double>()))
    //         .Do(_ =>
    //         {
    //             if (Interlocked.Increment(ref callCount) == 1)
    //                 throw new InvalidOperationException();
    //         });
    //     var stateMachine = StateMachine
    //         .Configure<string, string>()
    //         .WithInitialState("A", 42, "hello", 3.14)
    //         .WithState("A", state => state.WithParameter<int, string, double>().OnEntry(onEntry))
    //         .Build();
    //
    //     await Assert.ThrowsAsync<InvalidOperationException>(() =>
    //         stateMachine.Activate(TestContext.Current.CancellationToken)
    //     );
    //
    //     // Act
    //     await stateMachine.Activate(TestContext.Current.CancellationToken);
    //
    //     // Assert
    //     Assert.NotNull(stateMachine.CurrentState);
    //     Assert.Equal("A", stateMachine.CurrentState.StateValue);
    // }

    // [Fact]
    // public async Task Activate_T1_T2_T3_T4_WhenInitialStateProviderThrowsException_ShouldThrowAndHaveExpectedProperties()
    // {
    //     // Arrange
    //     var exception = new InvalidOperationException("Test exception");
    //     var stateMachine = StateMachine
    //         .Configure<string, string>()
    //         .WithInitialState<int, string, double, bool>(() => throw exception)
    //         .WithState("A", state => state.WithParameter<int, string, double, bool>())
    //         .Build();
    //
    //     // Act
    //     var thrownException = await Assert.ThrowsAsync<InvalidOperationException>(() =>
    //         stateMachine.Activate(TestContext.Current.CancellationToken)
    //     );
    //
    //     // Assert
    //     Assert.Same(exception, thrownException);
    //     Assert.Null(stateMachine.CurrentState);
    //     Assert.Null(stateMachine.PreviousState);
    //     Assert.Null(stateMachine.NextState);
    //     Assert.False(stateMachine.Parameters.IsCurrentSet);
    //     Assert.False(stateMachine.Parameters.IsPreviousSet);
    //     Assert.False(stateMachine.Parameters.IsNextSet);
    //     Assert.Null(stateMachine.CurrentTransition);
    // }

    // [Fact]
    // public async Task Activate_T1_T2_T3_T4_WhenInitialStateProviderThrowsExceptionOnce_ShouldRetrySuccessfully()
    // {
    //     // Arrange
    //     var callCount = 0;
    //     var stateProvider = Substitute.For<Func<(string, int, string, double, bool)>>();
    //     stateProvider
    //         .Invoke()
    //         .Returns(_ =>
    //         {
    //             if (Interlocked.Increment(ref callCount) == 1)
    //                 throw new InvalidOperationException();
    //             return ("A", 42, "hello", 3.14, true);
    //         });
    //     var stateMachine = StateMachine
    //         .Configure<string, string>()
    //         .WithInitialState(stateProvider)
    //         .WithState("A", state => state.WithParameter<int, string, double, bool>())
    //         .Build();
    //
    //     await Assert.ThrowsAsync<InvalidOperationException>(() =>
    //         stateMachine.Activate(TestContext.Current.CancellationToken)
    //     );
    //
    //     // Act
    //     await stateMachine.Activate(TestContext.Current.CancellationToken);
    //
    //     // Assert
    //     Assert.NotNull(stateMachine.CurrentState);
    //     Assert.Equal("A", stateMachine.CurrentState.StateValue);
    // }

    // [Fact]
    // public async Task Activate_T1_T2_T3_T4_WhenOnActivateThrowsException_ShouldThrowAndHaveExpectedProperties()
    // {
    //     // Arrange
    //     var exception = new InvalidOperationException("Test exception");
    //     var stateMachine = StateMachine
    //         .Configure<string, string>()
    //         .WithInitialState("A", 42, "hello", 3.14, true)
    //         .WithState("A", state => state.WithParameter<int, string, double, bool>()
    //             .OnActivate((_, _, _, _, _) => throw exception))
    //         .Build();
    //
    //     // Act
    //     var thrownException = await Assert.ThrowsAsync<InvalidOperationException>(() =>
    //         stateMachine.Activate(TestContext.Current.CancellationToken)
    //     );
    //
    //     // Assert
    //     Assert.Same(exception, thrownException);
    //     Assert.Null(stateMachine.CurrentState);
    //     Assert.Null(stateMachine.PreviousState);
    //     Assert.Null(stateMachine.NextState);
    //     Assert.False(stateMachine.Parameters.IsCurrentSet);
    //     Assert.False(stateMachine.Parameters.IsPreviousSet);
    //     Assert.False(stateMachine.Parameters.IsNextSet);
    //     Assert.Null(stateMachine.CurrentTransition);
    // }

    // [Fact]
    // public async Task Activate_T1_T2_T3_T4_WhenOnActivateThrowsExceptionOnce_ShouldRetrySuccessfully()
    // {
    //     // Arrange
    //     var callCount = 0;
    //     var onActivate = Substitute.For<Action<string, int, string, double, bool>>();
    //     onActivate
    //         .When(x => x.Invoke(
    //             Arg.Any<string>(), Arg.Any<int>(), Arg.Any<string>(),
    //             Arg.Any<double>(), Arg.Any<bool>()))
    //         .Do(_ =>
    //         {
    //             if (Interlocked.Increment(ref callCount) == 1)
    //                 throw new InvalidOperationException();
    //         });
    //     var stateMachine = StateMachine
    //         .Configure<string, string>()
    //         .WithInitialState("A", 42, "hello", 3.14, true)
    //         .WithState("A", state => state.WithParameter<int, string, double, bool>()
    //             .OnActivate(onActivate))
    //         .Build();
    //
    //     await Assert.ThrowsAsync<InvalidOperationException>(() =>
    //         stateMachine.Activate(TestContext.Current.CancellationToken)
    //     );
    //
    //     // Act
    //     await stateMachine.Activate(TestContext.Current.CancellationToken);
    //
    //     // Assert
    //     Assert.NotNull(stateMachine.CurrentState);
    //     Assert.Equal("A", stateMachine.CurrentState.StateValue);
    // }

    // [Fact]
    // public async Task Activate_T1_T2_T3_T4_WhenOnEntryThrowsException_ShouldThrowAndHaveExpectedProperties()
    // {
    //     // Arrange
    //     var exception = new InvalidOperationException("Test exception");
    //     var stateMachine = StateMachine
    //         .Configure<string, string>()
    //         .WithInitialState("A", 42, "hello", 3.14, true)
    //         .WithState("A", state => state.WithParameter<int, string, double, bool>()
    //             .OnEntry((_, _, _, _) => throw exception))
    //         .Build();
    //
    //     // Act
    //     var thrownException = await Assert.ThrowsAsync<InvalidOperationException>(() =>
    //         stateMachine.Activate(TestContext.Current.CancellationToken)
    //     );
    //
    //     // Assert
    //     Assert.Same(exception, thrownException);
    //     Assert.Null(stateMachine.CurrentState);
    //     Assert.Null(stateMachine.PreviousState);
    //     Assert.Null(stateMachine.NextState);
    //     Assert.False(stateMachine.Parameters.IsCurrentSet);
    //     Assert.False(stateMachine.Parameters.IsPreviousSet);
    //     Assert.False(stateMachine.Parameters.IsNextSet);
    //     Assert.Null(stateMachine.CurrentTransition);
    // }

    // [Fact]
    // public async Task Activate_T1_T2_T3_T4_WhenOnEntryThrowsExceptionOnce_ShouldRetrySuccessfully()
    // {
    //     // Arrange
    //     var callCount = 0;
    //     var onEntry = Substitute.For<Action<int, string, double, bool>>();
    //     onEntry
    //         .When(x => x.Invoke(Arg.Any<int>(), Arg.Any<string>(), Arg.Any<double>(), Arg.Any<bool>()))
    //         .Do(_ =>
    //         {
    //             if (Interlocked.Increment(ref callCount) == 1)
    //                 throw new InvalidOperationException();
    //         });
    //     var stateMachine = StateMachine
    //         .Configure<string, string>()
    //         .WithInitialState("A", 42, "hello", 3.14, true)
    //         .WithState("A", state => state.WithParameter<int, string, double, bool>().OnEntry(onEntry))
    //         .Build();
    //
    //     await Assert.ThrowsAsync<InvalidOperationException>(() =>
    //         stateMachine.Activate(TestContext.Current.CancellationToken)
    //     );
    //
    //     // Act
    //     await stateMachine.Activate(TestContext.Current.CancellationToken);
    //
    //     // Assert
    //     Assert.NotNull(stateMachine.CurrentState);
    //     Assert.Equal("A", stateMachine.CurrentState.StateValue);
    // }
}
