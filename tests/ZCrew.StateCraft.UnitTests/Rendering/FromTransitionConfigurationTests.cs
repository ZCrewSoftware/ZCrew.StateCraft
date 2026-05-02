using NSubstitute;
using ZCrew.StateCraft.Rendering;
using ZCrew.StateCraft.Rendering.Models;
using ZCrew.StateCraft.States.Configuration;
using ZCrew.StateCraft.Transitions;

namespace ZCrew.StateCraft.UnitTests.Rendering;

public class FromTransitionConfigurationTests
{
    [Fact]
    public void AddToRenderingContext_WhenContextHasNoStates_ShouldAddNoTransitions()
    {
        // Arrange
        var next = Substitute.For<INextStateConfiguration<string, string>>();
        var configuration = new FromTransitionConfiguration<string, string>("T", next);
        var context = new StateMachineRenderingContext<string, string>();

        // Act
        configuration.AddToRenderingContext(context);

        // Assert
        Assert.Empty(context.Transitions);
    }

    [Fact]
    public void AddToRenderingContext_WhenContextHasMultipleStates_ShouldAddOneTransitionPerState()
    {
        // Arrange
        var next = Substitute.For<INextStateConfiguration<string, string>>();
        var configuration = new FromTransitionConfiguration<string, string>("T", next);
        var context = new StateMachineRenderingContext<string, string>();
        context.States.Add(new StateRenderingModel<string, string>("A", [], "A", "A"));
        context.States.Add(new StateRenderingModel<string, string>("B", [], "B", "B"));
        context.States.Add(new StateRenderingModel<string, string>("C", [], "C", "C"));

        // Act
        configuration.AddToRenderingContext(context);

        // Assert
        Assert.Equal(3, context.Transitions.Count);
    }

    [Fact]
    public void AddToRenderingContext_WhenInvoked_ShouldUseTransitionValueForEveryDescriptor()
    {
        // Arrange
        var next = Substitute.For<INextStateConfiguration<string, string>>();
        var configuration = new FromTransitionConfiguration<string, string>("T", next);
        var context = new StateMachineRenderingContext<string, string>();
        context.States.Add(new StateRenderingModel<string, string>("A", [], "A", "A"));
        context.States.Add(new StateRenderingModel<string, string>("B", [], "B", "B"));

        // Act
        configuration.AddToRenderingContext(context);

        // Assert
        Assert.All(context.Transitions, transition => Assert.Equal("T", transition.Descriptor));
    }

    [Fact]
    public void AddToRenderingContext_WhenNextIsConditional_ShouldUseNextConditionsForEveryTransition()
    {
        // Arrange
        var next = Substitute.For<INextStateConfiguration<string, string>>();
        next.RenderConditions().Returns(["c1", "c2"]);
        var configuration = new FromTransitionConfiguration<string, string>("T", next);
        var context = new StateMachineRenderingContext<string, string>();
        context.States.Add(new StateRenderingModel<string, string>("A", [], "A", "A"));
        context.States.Add(new StateRenderingModel<string, string>("B", [], "B", "B"));

        // Act
        configuration.AddToRenderingContext(context);

        // Assert
        Assert.All(context.Transitions, transition => Assert.Equal(["c1", "c2"], transition.Conditions));
    }

    [Fact]
    public void AddToRenderingContext_WhenStateMatchesParameterlessExclusion_ShouldSkipThatState()
    {
        // Arrange
        var next = Substitute.For<INextStateConfiguration<string, string>>();
        var configuration = new FromTransitionConfiguration<string, string>("T", next);
        configuration.AllStates().Except("B");
        var context = new StateMachineRenderingContext<string, string>();
        context.States.Add(new StateRenderingModel<string, string>("A", [], "A", "A"));
        context.States.Add(new StateRenderingModel<string, string>("B", [], "B", "B"));
        context.States.Add(new StateRenderingModel<string, string>("C", [], "C", "C"));

        // Act
        configuration.AddToRenderingContext(context);

        // Assert
        Assert.Equal(2, context.Transitions.Count);
    }

    [Fact]
    public void AddToRenderingContext_WhenStateMatchesTypedExclusion_ShouldSkipOnlyMatchingArityAndType()
    {
        // Arrange
        var next = Substitute.For<INextStateConfiguration<string, string>>();
        var configuration = new FromTransitionConfiguration<string, string>("T", next);
        configuration.AllStates().Except<int>("A");
        var context = new StateMachineRenderingContext<string, string>();
        context.States.Add(new StateRenderingModel<string, string>("A", [], "A", "A"));
        context.States.Add(new StateRenderingModel<string, string>("A", [typeof(int)], "A_System.Int32", "A<int>"));
        context.States.Add(
            new StateRenderingModel<string, string>("A", [typeof(string)], "A_System.String", "A<string>")
        );

        // Act
        configuration.AddToRenderingContext(context);

        // Assert
        Assert.Equal(2, context.Transitions.Count);
    }
}
