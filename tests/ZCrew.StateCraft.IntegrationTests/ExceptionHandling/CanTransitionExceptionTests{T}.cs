using NSubstitute;
using ZCrew.StateCraft.Extensions;

namespace ZCrew.StateCraft.IntegrationTests.ExceptionHandling;

public class CanTransitionExceptionTests_T
{
    [Fact]
    public async Task CanTransition_T_WhenPreviousParameterConditionThrowsException_ShouldThrow()
    {
        // Arrange
        var exception = new InvalidOperationException("Test exception");
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42)
            .WithState(
                "A",
                state =>
                    state
                        .WithParameter<int>()
                        .WithTransition("To B", t => t.If(_ => throw exception).WithParameter<string>().To("B"))
            )
            .WithState("B", state => state.WithParameter<string>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var thrownException = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.CanTransition("To B", "hello", TestContext.Current.CancellationToken)
        );

        // Assert
        Assert.Same(exception, thrownException);
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("A", stateMachine.CurrentState.StateValue);
        Assert.Equal(42, stateMachine.Parameters.GetCurrentParameter<int>());
        Assert.Null(stateMachine.PreviousState);
        Assert.Null(stateMachine.NextState);
        Assert.False(stateMachine.Parameters.IsPreviousSet);
        Assert.False(stateMachine.Parameters.IsNextSet);
    }

    [Fact]
    public async Task CanTransition_T_WhenPreviousParameterConditionThrowsExceptionOnce_ShouldRetrySuccessfully()
    {
        // Arrange
        var exception = new InvalidOperationException("Test exception");
        var condition = Substitute.For<Func<int, bool>>();
        condition.Invoke(Arg.Any<int>()).Returns(_ => throw exception, _ => true);

        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42)
            .WithState(
                "A",
                state =>
                    state
                        .WithParameter<int>()
                        .WithTransition("To B", t => t.If(condition).WithParameter<string>().To("B"))
            )
            .WithState("B", state => state.WithParameter<string>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.CanTransition("To B", "hello", TestContext.Current.CancellationToken)
        );

        // Act
        var result = await stateMachine.CanTransition("To B", "world", TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result);
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("A", stateMachine.CurrentState.StateValue);
        Assert.Equal(42, stateMachine.Parameters.GetCurrentParameter<int>());
    }

    [Fact]
    public async Task CanTransition_T_WhenPreviousParameterConditionThrowsExceptionOnce_ShouldCheckDifferentTransitionSuccessfully()
    {
        // Arrange
        var exception = new InvalidOperationException("Test exception");
        var condition = Substitute.For<Func<int, bool>>();
        condition.Invoke(Arg.Any<int>()).Returns(_ => throw exception, _ => true);

        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42)
            .WithState(
                "A",
                state =>
                    state
                        .WithParameter<int>()
                        .WithTransition("To B", t => t.If(condition).WithParameter<string>().To("B"))
                        .WithTransition<string>("To C", "C")
            )
            .WithState("B", state => state.WithParameter<string>())
            .WithState("C", state => state.WithParameter<string>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.CanTransition("To B", "hello", TestContext.Current.CancellationToken)
        );

        // Act
        var result = await stateMachine.CanTransition("To C", "world", TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result);
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("A", stateMachine.CurrentState.StateValue);
        Assert.Equal(42, stateMachine.Parameters.GetCurrentParameter<int>());
    }

    [Fact]
    public async Task CanTransition_T_WhenNextParameterConditionThrowsException_ShouldThrow()
    {
        // Arrange
        var exception = new InvalidOperationException("Test exception");
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42)
            .WithState(
                "A",
                state =>
                    state
                        .WithParameter<int>()
                        .WithTransition("To B", t => t.WithParameter<string>().If(_ => throw exception).To("B"))
            )
            .WithState("B", state => state.WithParameter<string>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var thrownException = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.CanTransition("To B", "hello", TestContext.Current.CancellationToken)
        );

        // Assert
        Assert.Same(exception, thrownException);
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("A", stateMachine.CurrentState.StateValue);
        Assert.Equal(42, stateMachine.Parameters.GetCurrentParameter<int>());
        Assert.Null(stateMachine.PreviousState);
        Assert.Null(stateMachine.NextState);
        Assert.False(stateMachine.Parameters.IsPreviousSet);
        Assert.False(stateMachine.Parameters.IsNextSet);
    }

    [Fact]
    public async Task CanTransition_T_WhenNextParameterConditionThrowsExceptionOnce_ShouldRetrySuccessfully()
    {
        // Arrange
        var exception = new InvalidOperationException("Test exception");
        var condition = Substitute.For<Func<string, bool>>();
        condition.Invoke(Arg.Any<string>()).Returns(_ => throw exception, _ => true);

        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42)
            .WithState(
                "A",
                state =>
                    state
                        .WithParameter<int>()
                        .WithTransition("To B", t => t.WithParameter<string>().If(condition).To("B"))
            )
            .WithState("B", state => state.WithParameter<string>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.CanTransition("To B", "hello", TestContext.Current.CancellationToken)
        );

        // Act
        var result = await stateMachine.CanTransition("To B", "world", TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result);
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("A", stateMachine.CurrentState.StateValue);
        Assert.Equal(42, stateMachine.Parameters.GetCurrentParameter<int>());
    }

    [Fact]
    public async Task CanTransition_T_WhenNextParameterConditionThrowsExceptionOnce_ShouldCheckDifferentTransitionSuccessfully()
    {
        // Arrange
        var exception = new InvalidOperationException("Test exception");
        var condition = Substitute.For<Func<string, bool>>();
        condition.Invoke(Arg.Any<string>()).Returns(_ => throw exception, _ => true);

        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42)
            .WithState(
                "A",
                state =>
                    state
                        .WithParameter<int>()
                        .WithTransition("To B", t => t.WithParameter<string>().If(condition).To("B"))
                        .WithTransition<string>("To C", "C")
            )
            .WithState("B", state => state.WithParameter<string>())
            .WithState("C", state => state.WithParameter<string>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.CanTransition("To B", "hello", TestContext.Current.CancellationToken)
        );

        // Act
        var result = await stateMachine.CanTransition("To C", "world", TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result);
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("A", stateMachine.CurrentState.StateValue);
        Assert.Equal(42, stateMachine.Parameters.GetCurrentParameter<int>());
    }
}
