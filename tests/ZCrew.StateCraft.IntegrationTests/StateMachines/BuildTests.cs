using NSubstitute;

namespace ZCrew.StateCraft.IntegrationTests.StateMachines;

public class BuildTests
{
    [Fact]
    public void Build_WhenNoInitialState_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var configuration = StateMachine.Configure<string, string>();

        // Act
        var build = () => configuration.Build();

        // Assert
        Assert.Throws<InvalidOperationException>(build);
    }

    [Fact]
    public void Build_WhenInitialStateValue_ShouldSucceed()
    {
        // Arrange
        var configuration = StateMachine.Configure<string, string>().WithInitialState("A");

        // Act
        var stateMachine = configuration.Build();

        // Assert
        Assert.NotNull(stateMachine);
    }

    [Fact]
    public void Build_WhenInitialStateFunc_ShouldSucceed()
    {
        // Arrange
        var configuration = StateMachine.Configure<string, string>().WithInitialState(() => "A");

        // Act
        var stateMachine = configuration.Build();

        // Assert
        Assert.NotNull(stateMachine);
    }

    [Fact]
    public void Build_WhenInitialStateFuncTask_ShouldSucceed()
    {
        // Arrange
        var configuration = StateMachine.Configure<string, string>().WithInitialState(_ => Task.FromResult("A"));

        // Act
        var stateMachine = configuration.Build();

        // Assert
        Assert.NotNull(stateMachine);
    }

    [Fact]
    public void Build_WhenInitialStateFuncValueTask_ShouldSucceed()
    {
        // Arrange
        var configuration = StateMachine.Configure<string, string>().WithInitialState(_ => ValueTask.FromResult("A"));

        // Act
        var stateMachine = configuration.Build();

        // Assert
        Assert.NotNull(stateMachine);
    }

    [Fact]
    public void Build_WhenNoExceptionBehaviorProvider_ShouldHaveDefaultBehavior()
    {
        // Arrange
        var configuration = StateMachine.Configure<string, string>().WithInitialState("A");

        // Act
        var stateMachine = configuration.Build();

        // Assert
        Assert.IsType<DefaultExceptionBehavior>(stateMachine.ExceptionBehavior);
    }

    [Fact]
    public void Build_WhenExceptionBehaviorProvider_ShouldUseProvidedBehavior()
    {
        // Arrange
        var exceptionBehavior = Substitute.For<IExceptionBehavior>();
        var configuration = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithExceptionBehavior(_ => exceptionBehavior);

        // Act
        var stateMachine = configuration.Build();

        // Assert
        Assert.Same(exceptionBehavior, stateMachine.ExceptionBehavior);
    }

    [Fact]
    public async Task Build_WhenHandlerAddedAfterBuild_ShouldNotAffectPreviouslyBuiltInstance()
    {
        // Arrange
        var handler = Substitute.For<Action<string, string, string>>();

        var configuration = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition("To B", t => t.To("B")))
            .WithState("B", state => state);

        var machine1 = configuration.Build();
        configuration.OnStateChange(handler);

        // Act
        await machine1.Activate(TestContext.Current.CancellationToken);
        await machine1.Transition("To B", TestContext.Current.CancellationToken);

        // Assert
        handler
            .DidNotReceive()
            .Invoke(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>());
    }

    [Fact]
    public async Task Build_WhenCalledTwice_ShouldProduceIndependentOnStateChangeHandlers()
    {
        // Arrange
        var handler1Count = 0;
        var handler2Count = 0;

        var configuration = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition("To B", t => t.To("B")))
            .WithState("B", state => state)
            .OnStateChange((_, _, _) => handler1Count++);

        var machine1 = configuration.Build();

        configuration.OnStateChange((_, _, _) => handler2Count++);

        var machine2 = configuration.Build();

        // Act
        await machine1.Activate(TestContext.Current.CancellationToken);
        await machine1.Transition("To B", TestContext.Current.CancellationToken);

        // Assert
        Assert.True(handler1Count > 0, "Handler1 should have been invoked on machine1");
        Assert.Equal(0, handler2Count);
    }

    [Fact]
    public async Task Build_WhenCalledTwice_ShouldNotShareMutableState()
    {
        // Arrange
        var configuration = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition("To B", t => t.To("B")))
            .WithState("B", state => state.WithTransition("To A", t => t.To("A")));

        var machine1 = configuration.Build();
        var machine2 = configuration.Build();

        // Act
        await machine1.Activate(TestContext.Current.CancellationToken);
        await machine1.Transition("To B", TestContext.Current.CancellationToken);

        await machine2.Activate(TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal("B", machine1.CurrentState?.StateValue);
        Assert.Equal("A", machine2.CurrentState?.StateValue);
    }
}
