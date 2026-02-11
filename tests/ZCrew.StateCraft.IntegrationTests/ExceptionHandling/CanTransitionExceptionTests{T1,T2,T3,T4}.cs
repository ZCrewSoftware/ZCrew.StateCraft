using NSubstitute;
using ZCrew.StateCraft.Extensions;

namespace ZCrew.StateCraft.IntegrationTests.ExceptionHandling;

public class CanTransitionExceptionTests_T1_T2_T3_T4
{
    [Fact]
    public async Task CanTransition_T1_T2_T3_T4_WhenNextParameterConditionThrowsException_ShouldThrow()
    {
        // Arrange
        var exception = new InvalidOperationException("Test exception");
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState(
                "A",
                state =>
                    state.WithTransition(
                        "To B",
                        t => t.WithParameters<int, string, double, bool>().If((_, _, _, _) => throw exception).To("B")
                    )
            )
            .WithState("B", state => state.WithParameters<int, string, double, bool>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var thrownException = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.CanTransition("To B", 42, "hello", 3.14, true, TestContext.Current.CancellationToken)
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
    public async Task CanTransition_T1_T2_T3_T4_WhenNextParameterConditionThrowsExceptionOnce_ShouldRetrySuccessfully()
    {
        // Arrange
        var callCount = 0;
        var condition = Substitute.For<Func<int, string, double, bool, bool>>();
        condition
            .Invoke(Arg.Any<int>(), Arg.Any<string>(), Arg.Any<double>(), Arg.Any<bool>())
            .Returns(_ =>
            {
                if (Interlocked.Increment(ref callCount) == 1)
                    throw new InvalidOperationException();
                return true;
            });

        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState(
                "A",
                state =>
                    state.WithTransition(
                        "To B",
                        t => t.WithParameters<int, string, double, bool>().If(condition).To("B")
                    )
            )
            .WithState("B", state => state.WithParameters<int, string, double, bool>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.CanTransition("To B", 42, "hello", 3.14, true, TestContext.Current.CancellationToken)
        );

        // Act
        var result = await stateMachine.CanTransition(
            "To B",
            99,
            "world",
            2.72,
            false,
            TestContext.Current.CancellationToken
        );

        // Assert
        Assert.True(result);
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("A", stateMachine.CurrentState.StateValue);
    }

    [Fact]
    public async Task CanTransition_T1_T2_T3_T4_WhenNextParameterConditionThrowsExceptionOnce_ShouldCheckDifferentTransitionSuccessfully()
    {
        // Arrange
        var callCount = 0;
        var condition = Substitute.For<Func<int, string, double, bool, bool>>();
        condition
            .Invoke(Arg.Any<int>(), Arg.Any<string>(), Arg.Any<double>(), Arg.Any<bool>())
            .Returns(_ =>
            {
                if (Interlocked.Increment(ref callCount) == 1)
                    throw new InvalidOperationException();
                return true;
            });

        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState(
                "A",
                state =>
                    state
                        .WithTransition(
                            "To B",
                            t => t.WithParameters<int, string, double, bool>().If(condition).To("B")
                        )
                        .WithTransition<int, string, double, bool>("To C", "C")
            )
            .WithState("B", state => state.WithParameters<int, string, double, bool>())
            .WithState("C", state => state.WithParameters<int, string, double, bool>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.CanTransition("To B", 42, "hello", 3.14, true, TestContext.Current.CancellationToken)
        );

        // Act
        var result = await stateMachine.CanTransition(
            "To C",
            99,
            "world",
            2.72,
            false,
            TestContext.Current.CancellationToken
        );

        // Assert
        Assert.True(result);
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("A", stateMachine.CurrentState.StateValue);
    }
}
