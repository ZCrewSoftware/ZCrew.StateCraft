using ZCrew.StateCraft.Identities.Extensions;

namespace ZCrew.StateCraft.UnitTests.Identities;

public class TransitionIdentityExtensionsTests
{
    [Fact]
    public void ToDisplayString_WhenParameterless_ShouldRenderValue()
    {
        // Arrange
        var transition = TransitionIdentity.For("Go");

        // Act
        var display = transition.ToDisplayString();

        // Assert
        Assert.Equal("Go", display);
    }

    [Fact]
    public void ToDisplayString_WhenParameterized_ShouldRenderValueWithType()
    {
        // Arrange
        var transition = TransitionIdentity.For("Go", typeof(int));

        // Act
        var display = transition.ToDisplayString();

        // Assert
        Assert.Equal("Go<int>", display);
    }

    [Fact]
    public void RenderFromOneToOne_WhenDistinctStates_ShouldRenderArrow()
    {
        // Arrange
        var transition = TransitionIdentity.For("Go");

        // Act
        var rendered = transition.RenderFromOneToOne(StateIdentity.For("A"), StateIdentity.For("B"));

        // Assert
        Assert.Equal("Go(A) → B", rendered);
    }

    [Fact]
    public void RenderFromOneToOne_WhenSourceMatchesTarget_ShouldRenderReentrantGlyph()
    {
        // Arrange
        var transition = TransitionIdentity.For("Loop");

        // Act
        var rendered = transition.RenderFromOneToOne(StateIdentity.For("A"), StateIdentity.For("A"));

        // Assert
        Assert.Equal("Loop(A) ↩", rendered);
    }

    [Fact]
    public void RenderFromOneToOne_WhenParameterizedStates_ShouldRenderTypeParameters()
    {
        // Arrange
        var transition = TransitionIdentity.For("To B");

        // Act
        var rendered = transition.RenderFromOneToOne(
            StateIdentity.For("A", typeof(int)),
            StateIdentity.For("B", typeof(string))
        );

        // Assert
        Assert.Equal("To B(A<int>) → B<string>", rendered);
    }

    [Fact]
    public void RenderFromAnyToOne_WhenNoExcludedStates_ShouldRenderAnyState()
    {
        // Arrange
        var transition = TransitionIdentity.For("To D");

        // Act
        var rendered = transition.RenderFromAnyToOne([], StateIdentity.For("D"));

        // Assert
        Assert.Equal("To D(Any State) → D", rendered);
    }

    [Fact]
    public void RenderFromAnyToOne_WhenSingleExcludedState_ShouldRenderAnyStateExcept()
    {
        // Arrange
        var transition = TransitionIdentity.For("To D");

        // Act
        var rendered = transition.RenderFromAnyToOne([StateIdentity.For("D")], StateIdentity.For("D"));

        // Assert
        Assert.Equal("To D(Any State Except: D) → D", rendered);
    }

    [Fact]
    public void RenderFromAnyToOne_WhenMultipleExcludedStates_ShouldListAllInOrder()
    {
        // Arrange
        var transition = TransitionIdentity.For("To D");

        // Act
        var rendered = transition.RenderFromAnyToOne(
            [StateIdentity.For("A"), StateIdentity.For("B")],
            StateIdentity.For("D")
        );

        // Assert
        Assert.Equal("To D(Any State Except: A, B) → D", rendered);
    }
}
