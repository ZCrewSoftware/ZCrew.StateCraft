namespace ZCrew.StateCraft.IntegrationTests.Transitions;

public class TransitionShortcutTests
{
    [Fact(
        Skip = "BL-F09: WithTransition shortcut silently drops parameters when used from parameterized to parameterized state"
    )]
    public async Task WithTransition_Shortcut_WhenParameterizedToParameterized_ShouldNotSilentlyDropParameters()
    {
        // Arrange — shortcut from A(int) to B(int) implies WithNoParameters(), discarding the source parameter
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42)
            .WithState("A", state => state.WithParameter<int>().WithTransition("To B", "B"))
            .WithState("B", state => state.WithParameter<int>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Assert — B should receive the parameter, but shortcut drops it
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("B", stateMachine.CurrentState.StateValue);
        Assert.Equal(42, stateMachine.Parameters.GetCurrentParameter<int>());
    }

    [Fact(
        Skip = "BL-F09: Build(Validate) does not warn when shortcut transition is used on parameterized state"
    )]
    public void Build_Validate_WhenShortcutUsedOnParameterizedState_ShouldWarn()
    {
        // Arrange & Act — shortcut on parameterized state passes validation without warning
        var build = () =>
            StateMachine
                .Configure<string, string>()
                .WithInitialState("A", 42)
                .WithState("A", state => state.WithParameter<int>().WithTransition("To B", "B"))
                .WithState("B", state => state)
                .Build(StateMachineBuildOptions.Validate);

        // Assert — validation should detect the parameter drop, but currently passes silently
        Assert.Throws<InvalidOperationException>(build);
    }
}
