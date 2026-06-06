using ZCrew.StateCraft.Identities.Extensions;

namespace ZCrew.StateCraft.UnitTests.Identities;

public class TransitionIdentityExtensionsTests
{
    [Fact]
    public void ToDisplayString_WhenParameterless_ShouldRenderValue()
    {
        // Arrange
        var transition = Identity.ForTransition("Go");

        // Act
        var display = transition.ToDisplayString();

        // Assert
        Assert.Equal("Go", display);
    }

    [Fact]
    public void ToDisplayString_WhenParameterized_ShouldRenderValueWithType()
    {
        // Arrange
        var transition = Identity.ForTransition("Go", typeof(int));

        // Act
        var display = transition.ToDisplayString();

        // Assert
        Assert.Equal("Go<int>", display);
    }

    [Fact]
    public void RenderFromOneToOne_WhenDistinctStates_ShouldRenderArrow()
    {
        // Arrange
        var transition = Identity.ForTransition("Go");

        // Act
        var rendered = transition.RenderFromOneToOne(Identity.ForState("A"), Identity.ForState("B"));

        // Assert
        Assert.Equal("Go(A) → B", rendered);
    }

    [Fact]
    public void RenderFromOneToOne_WhenSourceMatchesTarget_ShouldRenderReentrantGlyph()
    {
        // Arrange
        var transition = Identity.ForTransition("Loop");

        // Act
        var rendered = transition.RenderFromOneToOne(Identity.ForState("A"), Identity.ForState("A"));

        // Assert
        Assert.Equal("Loop(A) ↩", rendered);
    }

    [Fact]
    public void RenderFromOneToOne_WhenParameterizedStates_ShouldRenderTypeParameters()
    {
        // Arrange
        var transition = Identity.ForTransition("To B");

        // Act
        var rendered = transition.RenderFromOneToOne(
            Identity.ForState("A", typeof(int)),
            Identity.ForState("B", typeof(string))
        );

        // Assert
        Assert.Equal("To B(A<int>) → B<string>", rendered);
    }

    [Fact]
    public void RenderFromAnyToOne_WhenNoExcludedStates_ShouldRenderAnyState()
    {
        // Arrange
        var transition = Identity.ForTransition("To D");

        // Act
        var rendered = transition.RenderFromAnyToOne([], Identity.ForState("D"));

        // Assert
        Assert.Equal("To D(Any State) → D", rendered);
    }

    [Fact]
    public void RenderFromAnyToOne_WhenSingleExcludedState_ShouldRenderAnyStateExcept()
    {
        // Arrange
        var transition = Identity.ForTransition("To D");

        // Act
        var rendered = transition.RenderFromAnyToOne([Identity.ForState("D")], Identity.ForState("D"));

        // Assert
        Assert.Equal("To D(Any State Except: D) → D", rendered);
    }

    [Fact]
    public void RenderFromAnyToOne_WhenMultipleExcludedStates_ShouldListAllInOrder()
    {
        // Arrange
        var transition = Identity.ForTransition("To D");

        // Act
        var rendered = transition.RenderFromAnyToOne(
            [Identity.ForState("A"), Identity.ForState("B")],
            Identity.ForState("D")
        );

        // Assert
        Assert.Equal("To D(Any State Except: A, B) → D", rendered);
    }
}
