using NSubstitute;
using ZCrew.StateCraft.Rendering;
using ZCrew.StateCraft.States.Configuration;
using ZCrew.StateCraft.Transitions;

namespace ZCrew.StateCraft.UnitTests.Rendering;

public class DirectTransitionConfigurationTests
{
    [Fact]
    public void AddToRenderingContext_WhenInvoked_ShouldAppendSingleTransition()
    {
        // Arrange
        var previous = Substitute.For<IPreviousStateConfiguration<string, string>>();
        var next = Substitute.For<INextStateConfiguration<string, string>>();
        var configuration = new DirectTransitionConfiguration<string, string>(previous, next, "T");
        var context = new StateMachineRenderingContext<string, string>();

        // Act
        configuration.AddToRenderingContext(context);

        // Assert
        Assert.Single(context.Transitions);
    }

    [Fact]
    public void AddToRenderingContext_WhenInvoked_ShouldUseTransitionValueAsDescriptor()
    {
        // Arrange
        var previous = Substitute.For<IPreviousStateConfiguration<string, string>>();
        var next = Substitute.For<INextStateConfiguration<string, string>>();
        var configuration = new DirectTransitionConfiguration<string, string>(previous, next, "T");
        var context = new StateMachineRenderingContext<string, string>();

        // Act
        configuration.AddToRenderingContext(context);

        // Assert
        var transition = Assert.Single(context.Transitions);
        Assert.Equal("T", transition.Descriptor);
    }

    [Fact]
    public void AddToRenderingContext_WhenNeitherSideIsConditional_ShouldHaveNoConditions()
    {
        // Arrange
        var previous = Substitute.For<IPreviousStateConfiguration<string, string>>();
        var next = Substitute.For<INextStateConfiguration<string, string>>();
        previous.RenderConditions().Returns([]);
        next.RenderConditions().Returns([]);
        var configuration = new DirectTransitionConfiguration<string, string>(previous, next, "T");
        var context = new StateMachineRenderingContext<string, string>();

        // Act
        configuration.AddToRenderingContext(context);

        // Assert
        var transition = Assert.Single(context.Transitions);
        Assert.Empty(transition.Conditions);
    }

    [Fact]
    public void AddToRenderingContext_WhenOnlyPreviousIsConditional_ShouldIncludePreviousConditions()
    {
        // Arrange
        var previous = Substitute.For<IPreviousStateConfiguration<string, string>>();
        var next = Substitute.For<INextStateConfiguration<string, string>>();
        previous.RenderConditions().Returns(["prev"]);
        next.RenderConditions().Returns([]);
        var configuration = new DirectTransitionConfiguration<string, string>(previous, next, "T");
        var context = new StateMachineRenderingContext<string, string>();

        // Act
        configuration.AddToRenderingContext(context);

        // Assert
        var transition = Assert.Single(context.Transitions);
        Assert.Equal(["prev"], transition.Conditions);
    }

    [Fact]
    public void AddToRenderingContext_WhenOnlyNextIsConditional_ShouldIncludeNextConditions()
    {
        // Arrange
        var previous = Substitute.For<IPreviousStateConfiguration<string, string>>();
        var next = Substitute.For<INextStateConfiguration<string, string>>();
        previous.RenderConditions().Returns([]);
        next.RenderConditions().Returns(["next"]);
        var configuration = new DirectTransitionConfiguration<string, string>(previous, next, "T");
        var context = new StateMachineRenderingContext<string, string>();

        // Act
        configuration.AddToRenderingContext(context);

        // Assert
        var transition = Assert.Single(context.Transitions);
        Assert.Equal(["next"], transition.Conditions);
    }

    [Fact]
    public void AddToRenderingContext_WhenBothSidesAreConditional_ShouldIncludePreviousThenNextConditionsInOrder()
    {
        // Arrange
        var previous = Substitute.For<IPreviousStateConfiguration<string, string>>();
        var next = Substitute.For<INextStateConfiguration<string, string>>();
        previous.RenderConditions().Returns(["prev1", "prev2"]);
        next.RenderConditions().Returns(["next1", "next2"]);
        var configuration = new DirectTransitionConfiguration<string, string>(previous, next, "T");
        var context = new StateMachineRenderingContext<string, string>();

        // Act
        configuration.AddToRenderingContext(context);

        // Assert
        var transition = Assert.Single(context.Transitions);
        Assert.Equal(["prev1", "prev2", "next1", "next2"], transition.Conditions);
    }
}
