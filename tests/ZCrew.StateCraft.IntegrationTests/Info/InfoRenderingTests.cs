namespace ZCrew.StateCraft.IntegrationTests.Info;

// Smoke tests that each Info type's ToString() override delegates to the identity rendering extensions.
// The full rendering matrix lives in the Identities unit tests.
public class InfoRenderingTests
{
    [Fact]
    public void ToString_WhenStateIsParameterized_ShouldDelegateToIdentityRendering()
    {
        // Arrange
        var configuration = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 1)
            .WithState("A", state => state.WithParameter<int>());

        // Act
        var state = Assert.Single(configuration.GetInfo().States);

        // Assert
        Assert.Equal("A<int>", state.ToString());
    }

    [Fact]
    public void ToString_WhenDirectTransition_ShouldDelegateToIdentityRendering()
    {
        // Arrange
        var configuration = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition("To B", "B"))
            .WithState("B", state => state);

        // Act
        var transition = Assert.Single(configuration.GetInfo().Transitions);

        // Assert
        Assert.Equal("To B(A) → B", transition.ToString());
    }

    [Fact]
    public void ToString_WhenFromTransition_ShouldDelegateToIdentityRendering()
    {
        // Arrange
        var configuration = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state)
            .WithState("D", state => state.WithTransition("To D", t => t.From().AllOtherStates()));

        // Act
        var transition = Assert.Single(configuration.GetInfo().Transitions);

        // Assert
        Assert.Equal("To D(Any State Except: D) → D", transition.ToString());
    }

    [Fact]
    public void ToString_WhenMappedTransition_ShouldDelegateToIdentityRendering()
    {
        // Arrange
        var configuration = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 1)
            .WithState(
                "A",
                state =>
                    state
                        .WithParameter<int>()
                        .WithTransition("To B", t => t.WithMappedParameter<string>(value => value.ToString()).To("B"))
            )
            .WithState("B", state => state.WithParameter<string>());

        // Act
        var transition = Assert.Single(configuration.GetInfo().Transitions);

        // Assert
        Assert.Equal("To B(A<int>) → B<string>", transition.ToString());
    }
}
