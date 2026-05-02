using ZCrew.StateCraft.Rendering;
using ZCrew.StateCraft.States;

namespace ZCrew.StateCraft.UnitTests.Rendering;

public class StateConfigurationT1T2T3T4Tests
{
    [Fact]
    public void AddToRenderingContext_T1_T2_T3_T4_WhenContextIsEmpty_ShouldAppendSingleStateModel()
    {
        // Arrange
        var configuration = new StateConfiguration<string, string, int, string, bool, double>("S");
        var context = new StateMachineRenderingContext<string, string>();

        // Act
        configuration.AddToRenderingContext(context);

        // Assert
        var state = Assert.Single(context.States);
        Assert.Equal("S", state.State);
    }

    [Fact]
    public void AddToRenderingContext_T1_T2_T3_T4_WhenInvoked_ShouldComposeNameFromStateValueAndTypeRenderingIds()
    {
        // Arrange
        var configuration = new StateConfiguration<string, string, int, string, bool, double>("S");
        var context = new StateMachineRenderingContext<string, string>();

        // Act
        configuration.AddToRenderingContext(context);

        // Assert
        var state = Assert.Single(context.States);
        Assert.Equal("S_System.Int32_System.String_System.Boolean_System.Double", state.Identifier);
    }

    [Fact]
    public void AddToRenderingContext_T1_T2_T3_T4_WhenInvoked_ShouldUseToStringAsDescriptor()
    {
        // Arrange
        var configuration = new StateConfiguration<string, string, int, string, bool, double>("S");
        var context = new StateMachineRenderingContext<string, string>();

        // Act
        configuration.AddToRenderingContext(context);

        // Assert
        var state = Assert.Single(context.States);
        Assert.Equal(configuration.ToString(), state.Descriptor);
    }

    [Fact]
    public void AddToRenderingContext_T1_T2_T3_T4_WhenInvoked_ShouldExposeTypeArgumentsInDeclarationOrder()
    {
        // Arrange
        var configuration = new StateConfiguration<string, string, int, string, bool, double>("S");
        var context = new StateMachineRenderingContext<string, string>();

        // Act
        configuration.AddToRenderingContext(context);

        // Assert
        var state = Assert.Single(context.States);
        Assert.Equal([typeof(int), typeof(string), typeof(bool), typeof(double)], state.TypeParameters);
    }
}
