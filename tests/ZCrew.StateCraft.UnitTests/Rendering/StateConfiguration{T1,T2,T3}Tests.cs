using ZCrew.StateCraft.Rendering;
using ZCrew.StateCraft.States;

namespace ZCrew.StateCraft.UnitTests.Rendering;

public class StateConfigurationT1T2T3Tests
{
    [Fact]
    public void AddToRenderingContext_T1_T2_T3_WhenContextIsEmpty_ShouldAppendSingleStateModel()
    {
        // Arrange
        var configuration = new StateConfiguration<string, string, int, string, bool>("S");
        var context = new StateMachineRenderingContext<string, string>();

        // Act
        configuration.AddToRenderingContext(context);

        // Assert
        var state = Assert.Single(context.States);
        Assert.Equal("S", state.State);
    }

    [Fact]
    public void AddToRenderingContext_T1_T2_T3_WhenInvoked_ShouldComposeNameFromStateValueAndTypeRenderingIds()
    {
        // Arrange
        var configuration = new StateConfiguration<string, string, int, string, bool>("S");
        var context = new StateMachineRenderingContext<string, string>();

        // Act
        configuration.AddToRenderingContext(context);

        // Assert
        var state = Assert.Single(context.States);
        Assert.Equal("S_System.Int32_System.String_System.Boolean", state.Name);
    }

    [Fact]
    public void AddToRenderingContext_T1_T2_T3_WhenInvoked_ShouldUseToStringAsDescriptor()
    {
        // Arrange
        var configuration = new StateConfiguration<string, string, int, string, bool>("S");
        var context = new StateMachineRenderingContext<string, string>();

        // Act
        configuration.AddToRenderingContext(context);

        // Assert
        var state = Assert.Single(context.States);
        Assert.Equal(configuration.ToString(), state.Descriptor);
    }

    [Fact]
    public void AddToRenderingContext_T1_T2_T3_WhenInvoked_ShouldExposeTypeArgumentsInDeclarationOrder()
    {
        // Arrange
        var configuration = new StateConfiguration<string, string, int, string, bool>("S");
        var context = new StateMachineRenderingContext<string, string>();

        // Act
        configuration.AddToRenderingContext(context);

        // Assert
        var state = Assert.Single(context.States);
        Assert.Equal([typeof(int), typeof(string), typeof(bool)], state.TypeParameters);
    }
}
