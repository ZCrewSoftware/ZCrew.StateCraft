namespace ZCrew.StateCraft.IntegrationTests.Transitions;

public class MappedTransitionTests
{
    [Fact]
    public async Task Transition_WhenMappedPostConditionFails_ShouldNotAffectSubsequentTransition()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42)
            .WithState(
                "A",
                state =>
                    state
                        .WithParameter<int>()
                        .WithTransition(
                            "Go",
                            t =>
                                t.WithMappedParameter<string>(x => x.ToString())
                                    .If((string _) => false)
                                    .To("B")
                        )
                        .WithTransition("Go", t => t.WithMappedParameter<int>(x => x * 2).To("C"))
            )
            .WithState("B", state => state.WithParameter<string>())
            .WithState("C", state => state.WithParameter<int>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("Go", TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("C", stateMachine.CurrentState.StateValue);
    }
}
