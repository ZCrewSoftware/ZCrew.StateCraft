using NSubstitute;
using ZCrew.StateCraft.Mapping.Contracts;
using ZCrew.StateCraft.Rendering;
using ZCrew.StateCraft.States.Configuration;
using ZCrew.StateCraft.Transitions;

namespace ZCrew.StateCraft.UnitTests.Rendering;

public class MappedTransitionConfigurationTests
{
    [Fact]
    public void AddToRenderingContext_WhenInvoked_ShouldAppendSingleTransition()
    {
        // Arrange
        var configuration = CreateConfiguration("T", out _, out _);
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
        var configuration = CreateConfiguration("T", out _, out _);
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
        var configuration = CreateConfiguration("T", out var previous, out var next);
        previous.RenderConditions().Returns([]);
        next.RenderConditions().Returns([]);
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
        var configuration = CreateConfiguration("T", out var previous, out var next);
        previous.RenderConditions().Returns(["prev"]);
        next.RenderConditions().Returns([]);
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
        var configuration = CreateConfiguration("T", out var previous, out var next);
        previous.RenderConditions().Returns([]);
        next.RenderConditions().Returns(["next"]);
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
        var configuration = CreateConfiguration("T", out var previous, out var next);
        previous.RenderConditions().Returns(["prev1", "prev2"]);
        next.RenderConditions().Returns(["next1", "next2"]);
        var context = new StateMachineRenderingContext<string, string>();

        // Act
        configuration.AddToRenderingContext(context);

        // Assert
        var transition = Assert.Single(context.Transitions);
        Assert.Equal(["prev1", "prev2", "next1", "next2"], transition.Conditions);
    }

    private static MappedTransitionConfiguration<string, string> CreateConfiguration(
        string transition,
        out IPreviousStateConfiguration<string, string> previous,
        out INextStateConfiguration<string, string> next
    )
    {
        previous = Substitute.For<IPreviousStateConfiguration<string, string>>();
        next = Substitute.For<INextStateConfiguration<string, string>>();
        var mapping = Substitute.For<IMappingFunction>();
        return new MappedTransitionConfiguration<string, string>(previous, next, transition, mapping);
    }
}
