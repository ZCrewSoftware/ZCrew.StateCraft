namespace ZCrew.StateCraft.IntegrationTests.Transitions;

public class MappedTransitionStaleParameterTests
{
    [Fact]
    public async Task Transition_WhenMappedPostConditionFails_ShouldNotAffectSubsequentTransition()
    {
        // Arrange — two mapped transitions on the same trigger. The first one's
        // post-condition always fails. Without clearing stale mapped values,
        // the second transition would be skipped by the type filter because the
        // parameter slot still holds the first mapping's types.
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42)
            .WithState(
                "A",
                state =>
                    state
                        .WithParameter<int>()
                        // First: maps int->string, but post-condition always fails
                        .WithTransition(
                            "Go",
                            t =>
                                t.WithMappedParameter<string>(x => x.ToString())
                                    .If((string _) => false)
                                    .To("B")
                        )
                        // Second: maps int->int, should succeed
                        .WithTransition("Go", t => t.WithMappedParameter<int>(x => x * 2).To("C"))
            )
            .WithState("B", state => state.WithParameter<string>())
            .WithState("C", state => state.WithParameter<int>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act — should fall through to the second mapped transition
        await stateMachine.Transition("Go", TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("C", stateMachine.CurrentState.StateValue);
    }
}
