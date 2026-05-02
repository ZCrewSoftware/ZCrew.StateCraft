using ZCrew.StateCraft.Rendering;
using ZCrew.StateCraft.States;

namespace ZCrew.StateCraft.UnitTests.Rendering;

public class StateConfigurationTests
{
    [Fact]
    public void AddToRenderingContext_WhenContextIsEmpty_ShouldAppendSingleStateModel()
    {
        // Arrange
        var configuration = new StateConfiguration<string, string>("S");
        var context = new StateMachineRenderingContext<string, string>();

        // Act
        configuration.AddToRenderingContext(context);

        // Assert
        var state = Assert.Single(context.States);
        Assert.Equal("S", state.State);
    }

    [Fact]
    public void AddToRenderingContext_WhenInvoked_ShouldUseStateValueAsName()
    {
        // Arrange
        var configuration = new StateConfiguration<string, string>("S");
        var context = new StateMachineRenderingContext<string, string>();

        // Act
        configuration.AddToRenderingContext(context);

        // Assert
        var state = Assert.Single(context.States);
        Assert.Equal("S", state.Name);
    }

    [Fact]
    public void AddToRenderingContext_WhenInvoked_ShouldUseToStringAsDescriptor()
    {
        // Arrange
        var configuration = new StateConfiguration<string, string>("S");
        var context = new StateMachineRenderingContext<string, string>();

        // Act
        configuration.AddToRenderingContext(context);

        // Assert
        var state = Assert.Single(context.States);
        Assert.Equal(configuration.ToString(), state.Descriptor);
    }

    [Fact]
    public void AddToRenderingContext_WhenInvoked_ShouldExposeEmptyTypeParameters()
    {
        // Arrange
        var configuration = new StateConfiguration<string, string>("S");
        var context = new StateMachineRenderingContext<string, string>();

        // Act
        configuration.AddToRenderingContext(context);

        // Assert
        var state = Assert.Single(context.States);
        Assert.Empty(state.TypeParameters);
    }

    [Fact]
    public void AddToRenderingContext_WhenContextHasExistingStates_ShouldAppendWithoutReplacing()
    {
        // Arrange
        var configuration = new StateConfiguration<string, string>("B");
        var existing = new StateConfiguration<string, string>("A");
        var context = new StateMachineRenderingContext<string, string>();
        existing.AddToRenderingContext(context);

        // Act
        configuration.AddToRenderingContext(context);

        // Assert
        Assert.Collection(
            context.States,
            first => Assert.Equal("A", first.State),
            second => Assert.Equal("B", second.State)
        );
    }
}
