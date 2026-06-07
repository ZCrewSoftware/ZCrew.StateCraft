using ZCrew.StateCraft.Identities;

namespace ZCrew.StateCraft.UnitTests.Identities;

public class StateIdentityEqualityComparerTests
{
    [Fact]
    public void Equals_WhenSameValueAndParameterTypes_ShouldReturnTrue()
    {
        // Arrange
        var comparer = StateIdentityEqualityComparer<string>.Instance;

        // Act
        var equal = comparer.Equals(StateIdentity.For("A", typeof(int)), StateIdentity.For("A", typeof(int)));

        // Assert
        Assert.True(equal);
    }

    [Fact]
    public void Equals_WhenDifferentValue_ShouldReturnFalse()
    {
        // Arrange
        var comparer = StateIdentityEqualityComparer<string>.Instance;

        // Act
        var equal = comparer.Equals(StateIdentity.For("A", typeof(int)), StateIdentity.For("B", typeof(int)));

        // Assert
        Assert.False(equal);
    }

    [Fact]
    public void Equals_WhenDifferentParameterTypes_ShouldReturnFalse()
    {
        // Arrange
        var comparer = StateIdentityEqualityComparer<string>.Instance;

        // Act
        var equal = comparer.Equals(StateIdentity.For("A", typeof(int)), StateIdentity.For("A", typeof(string)));

        // Assert
        Assert.False(equal);
    }

    [Fact]
    public void Equals_WhenParameterTypeOrderDiffers_ShouldReturnFalse()
    {
        // Arrange
        var comparer = StateIdentityEqualityComparer<string>.Instance;

        // Act
        var equal = comparer.Equals(
            StateIdentity.For("M", typeof(int), typeof(string)),
            StateIdentity.For("M", typeof(string), typeof(int))
        );

        // Assert
        Assert.False(equal);
    }

    [Fact]
    public void Equals_WhenBothNull_ShouldReturnTrue()
    {
        // Arrange
        var comparer = StateIdentityEqualityComparer<string>.Instance;

        // Act
        var equal = comparer.Equals(null, null);

        // Assert
        Assert.True(equal);
    }

    [Fact]
    public void Equals_WhenOneNull_ShouldReturnFalse()
    {
        // Arrange
        var comparer = StateIdentityEqualityComparer<string>.Instance;

        // Act
        var equal = comparer.Equals(StateIdentity.For("A"), null);

        // Assert
        Assert.False(equal);
    }

    [Fact]
    public void GetHashCode_WhenSameParameterizedIdentityFromSeparateArrays_ShouldBeEqual()
    {
        // Arrange
        var comparer = StateIdentityEqualityComparer<string>.Instance;
        var first = StateIdentity.For("A", typeof(int));
        var second = StateIdentity.For("A", typeof(int));

        // Act
        var firstHash = comparer.GetHashCode(first);
        var secondHash = comparer.GetHashCode(second);

        // Assert
        Assert.Equal(firstHash, secondHash);
    }

    [Fact]
    public void GetHashCode_WhenParameterless_ShouldBeEqualForSameValue()
    {
        // Arrange
        var comparer = StateIdentityEqualityComparer<string>.Instance;

        // Act
        var firstHash = comparer.GetHashCode(StateIdentity.For("A"));
        var secondHash = comparer.GetHashCode(StateIdentity.For("A"));

        // Assert
        Assert.Equal(firstHash, secondHash);
    }
}
