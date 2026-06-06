using ZCrew.StateCraft.Identities.Extensions;

namespace ZCrew.StateCraft.UnitTests.Identities;

public class StateIdentityExtensionsTests
{
    [Fact]
    public void ToDisplayString_WhenParameterless_ShouldRenderValue()
    {
        // Arrange
        var state = Identity.ForState("A");

        // Act
        var display = state.ToDisplayString();

        // Assert
        Assert.Equal("A", display);
    }

    [Fact]
    public void ToDisplayString_WhenSingleParameter_ShouldRenderValueWithType()
    {
        // Arrange
        var state = Identity.ForState("A", typeof(int));

        // Act
        var display = state.ToDisplayString();

        // Assert
        Assert.Equal("A<int>", display);
    }

    [Fact]
    public void ToDisplayString_WhenMultipleParameters_ShouldRenderValueWithTypesInOrder()
    {
        // Arrange
        var state = Identity.ForState("A", typeof(int), typeof(string));

        // Act
        var display = state.ToDisplayString();

        // Assert
        Assert.Equal("A<int, string>", display);
    }

    [Fact]
    public void Matches_WhenMatchingValueAndExactParameterTypes_ShouldReturnTrue()
    {
        // Arrange
        var state = Identity.ForState("X", typeof(int));

        // Act
        var matches = state.Matches(Identity.ForState("X", typeof(int)));

        // Assert
        Assert.True(matches);
    }

    [Fact]
    public void Matches_WhenValueMismatch_ShouldReturnFalse()
    {
        // Arrange
        var state = Identity.ForState("X", typeof(int));

        // Act
        var matches = state.Matches("Y", [typeof(int)]);

        // Assert
        Assert.False(matches);
    }

    [Fact]
    public void Matches_WhenAssignableButNotIdenticalParameterType_ShouldReturnFalse()
    {
        // Arrange
        var state = Identity.ForState("X", typeof(object));

        // Act
        var matches = state.Matches("X", [typeof(string)]);

        // Assert
        Assert.False(matches);
    }

    [Fact]
    public void Matches_WhenParameterTypeOrderDiffers_ShouldReturnFalse()
    {
        // Arrange
        var state = Identity.ForState("M", typeof(int), typeof(string));

        // Act
        var matches = state.Matches("M", [typeof(string), typeof(int)]);

        // Assert
        Assert.False(matches);
    }

    [Fact]
    public void Matches_WhenParameterlessAndEmptyTypes_ShouldReturnTrue()
    {
        // Arrange
        var state = Identity.ForState("A");

        // Act
        var matches = state.Matches("A", []);

        // Assert
        Assert.True(matches);
    }

    [Fact]
    public void IsAssignableFrom_WhenSameReference_ShouldReturnTrue()
    {
        // Arrange
        var state = Identity.ForState("A");

        // Act
        var isAssignable = state.IsAssignableFrom(state);

        // Assert
        Assert.True(isAssignable);
    }

    [Fact]
    public void IsAssignableFrom_WhenOtherIsNull_ShouldReturnFalse()
    {
        // Arrange
        var state = Identity.ForState("A");

        // Act
        var isAssignable = state.IsAssignableFrom(null);

        // Assert
        Assert.False(isAssignable);
    }

    [Fact]
    public void IsAssignableFrom_WhenDifferentInstanceSameIdentity_ShouldReturnTrue()
    {
        // Arrange
        var state = Identity.ForState("A");

        // Act
        var isAssignable = state.IsAssignableFrom(Identity.ForState("A"));

        // Assert
        Assert.True(isAssignable);
    }

    [Fact]
    public void IsAssignableFrom_WhenDifferentStateValue_ShouldReturnFalse()
    {
        // Arrange
        var state = Identity.ForState("A");

        // Act
        var isAssignable = state.IsAssignableFrom(Identity.ForState("B"));

        // Assert
        Assert.False(isAssignable);
    }

    [Fact]
    public void IsAssignableFrom_WhenSuppliedParameterIsMoreDerived_ShouldReturnTrue()
    {
        // Arrange
        var state = Identity.ForState("X", typeof(object));

        // Act
        var isAssignable = state.IsAssignableFrom(Identity.ForState("X", typeof(string)));

        // Assert
        Assert.True(isAssignable);
    }

    [Fact]
    public void IsAssignableFrom_WhenSuppliedParameterIsLessDerived_ShouldReturnFalse()
    {
        // Arrange
        var state = Identity.ForState("X", typeof(string));

        // Act
        var isAssignable = state.IsAssignableFrom(Identity.ForState("X", typeof(object)));

        // Assert
        Assert.False(isAssignable);
    }

    [Fact]
    public void IsAssignableFrom_WhenValueMismatch_ShouldReturnFalse()
    {
        // Arrange
        var state = Identity.ForState("X", typeof(object));

        // Act
        var isAssignable = state.IsAssignableFrom("Start", [typeof(string)]);

        // Assert
        Assert.False(isAssignable);
    }

    [Fact]
    public void IsAssignableFrom_WhenCovariantParameterType_ShouldReturnTrue()
    {
        // Arrange
        var state = Identity.ForState("X", typeof(object));

        // Act
        var isAssignable = state.IsAssignableFrom("X", [typeof(string)]);

        // Assert
        Assert.True(isAssignable);
    }

    [Fact]
    public void IsAssignableFrom_WhenContravariantParameterType_ShouldReturnFalse()
    {
        // Arrange
        var state = Identity.ForState("X", typeof(string));

        // Act
        var isAssignable = state.IsAssignableFrom("X", [typeof(object)]);

        // Assert
        Assert.False(isAssignable);
    }

    [Fact]
    public void IsAssignableFrom_WhenParameterCountMismatch_ShouldReturnFalse()
    {
        // Arrange
        var state = Identity.ForState("X", typeof(object));

        // Act
        var isAssignable = state.IsAssignableFrom("X", [typeof(string), typeof(int)]);

        // Assert
        Assert.False(isAssignable);
    }

    [Fact]
    public void IsAssignableFrom_WhenParameterlessAndEmptyTypes_ShouldReturnTrue()
    {
        // Arrange
        var state = Identity.ForState("A");

        // Act
        var isAssignable = state.IsAssignableFrom("A", []);

        // Assert
        Assert.True(isAssignable);
    }
}
