using ZCrew.StateCraft.Rendering;
using ZCrew.StateCraft.States;

namespace ZCrew.StateCraft.UnitTests.Rendering;

public class StateConfigurationTTests
{
    [Fact]
    public void AddToRenderingContext_T_WhenContextIsEmpty_ShouldAppendSingleStateModel()
    {
        // Arrange
        var configuration = new StateConfiguration<string, string, int>("S");
        var context = new StateMachineRenderingContext<string, string>();

        // Act
        configuration.AddToRenderingContext(context);

        // Assert
        var state = Assert.Single(context.States);
        Assert.Equal("S", state.State);
    }

    [Fact]
    public void AddToRenderingContext_T_WhenInvoked_ShouldComposeNameFromStateValueAndTypeRenderingId()
    {
        // Arrange
        var configuration = new StateConfiguration<string, string, int>("S");
        var context = new StateMachineRenderingContext<string, string>();

        // Act
        configuration.AddToRenderingContext(context);

        // Assert
        var state = Assert.Single(context.States);
        Assert.Equal("S_System.Int32", state.Identifier);
    }

    [Fact]
    public void AddToRenderingContext_T_WhenInvoked_ShouldUseToStringAsDescriptor()
    {
        // Arrange
        var configuration = new StateConfiguration<string, string, int>("S");
        var context = new StateMachineRenderingContext<string, string>();

        // Act
        configuration.AddToRenderingContext(context);

        // Assert
        var state = Assert.Single(context.States);
        Assert.Equal(configuration.ToString(), state.Descriptor);
    }

    [Fact]
    public void AddToRenderingContext_T_WhenInvoked_ShouldExposeTypeArgumentInTypeParameters()
    {
        // Arrange
        var configuration = new StateConfiguration<string, string, int>("S");
        var context = new StateMachineRenderingContext<string, string>();

        // Act
        configuration.AddToRenderingContext(context);

        // Assert
        var state = Assert.Single(context.States);
        Assert.Equal([typeof(int)], state.TypeParameters);
    }

    [Fact]
    public void AddToRenderingContext_T_WhenTwoStatesShareValueButDifferTypeArgument_ShouldProduceDistinctNames()
    {
        // Arrange
        var intConfiguration = new StateConfiguration<string, string, int>("S");
        var stringConfiguration = new StateConfiguration<string, string, string>("S");
        var context = new StateMachineRenderingContext<string, string>();

        // Act
        intConfiguration.AddToRenderingContext(context);
        stringConfiguration.AddToRenderingContext(context);

        // Assert
        Assert.Collection(
            context.States,
            first => Assert.Equal("S_System.Int32", first.Identifier),
            second => Assert.Equal("S_System.String", second.Identifier)
        );
    }
}
