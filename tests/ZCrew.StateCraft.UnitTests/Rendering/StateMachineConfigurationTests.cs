using ZCrew.StateCraft.Rendering;
using ZCrew.StateCraft.StateMachines;

namespace ZCrew.StateCraft.UnitTests.Rendering;

public class StateMachineConfigurationTests
{
    [Fact]
    public void AddToRenderingContext_WhenInvoked_ShouldSetStateMachineWithFixedDescriptor()
    {
        // Arrange
        var configuration = new StateMachineConfiguration<string, string>();
        var context = new StateMachineRenderingContext<string, string>();

        // Act
        configuration.AddToRenderingContext(context);

        // Assert
        Assert.NotNull(context.StateMachine);
        Assert.Equal("State Machine", context.StateMachine.Descriptor);
    }

    [Fact]
    public void AddToRenderingContext_WhenInvoked_ShouldNotPopulateStatesOrTransitions()
    {
        // Arrange
        var configuration = new StateMachineConfiguration<string, string>();
        var context = new StateMachineRenderingContext<string, string>();

        // Act
        configuration.AddToRenderingContext(context);

        // Assert
        Assert.Empty(context.States);
        Assert.Empty(context.Transitions);
    }
}
