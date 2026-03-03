namespace ZCrew.StateCraft.IntegrationTests.StateMachines;

public class BuildInstanceIndependenceTests
{
    [Fact(Skip = "BL-F01: onStateChanges list is shared by reference between Build() instances")]
    public async Task Build_WhenCalledTwice_ShouldProduceIndependentOnStateChangeHandlers()
    {
        // Arrange
        var machine1Changes = new List<(string From, string To)>();
        var machine2Changes = new List<(string From, string To)>();

        var configuration = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition("To B", t => t.To("B")))
            .WithState("B", state => state)
            .OnStateChange((from, _, to) => machine1Changes.Add((from, to)));

        var machine1 = configuration.Build();

        configuration.OnStateChange((from, _, to) => machine2Changes.Add((from, to)));

        var machine2 = configuration.Build();

        // Act
        await machine1.Activate(TestContext.Current.CancellationToken);
        await machine1.Transition("To B", TestContext.Current.CancellationToken);

        await machine2.Activate(TestContext.Current.CancellationToken);
        await machine2.Transition("To B", TestContext.Current.CancellationToken);

        // Assert
        Assert.Single(machine1Changes);
        Assert.Equal(2, machine2Changes.Count);
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
