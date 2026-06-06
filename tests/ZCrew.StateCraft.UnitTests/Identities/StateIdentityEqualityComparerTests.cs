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
        var equal = comparer.Equals(Identity.ForState("A", typeof(int)), Identity.ForState("A", typeof(int)));

        // Assert
        Assert.True(equal);
    }

    [Fact]
    public void Equals_WhenDifferentValue_ShouldReturnFalse()
    {
        // Arrange
        var comparer = StateIdentityEqualityComparer<string>.Instance;

        // Act
        var equal = comparer.Equals(Identity.ForState("A", typeof(int)), Identity.ForState("B", typeof(int)));

        // Assert
        Assert.False(equal);
    }

    [Fact]
    public void Equals_WhenDifferentParameterTypes_ShouldReturnFalse()
    {
        // Arrange
        var comparer = StateIdentityEqualityComparer<string>.Instance;

        // Act
        var equal = comparer.Equals(Identity.ForState("A", typeof(int)), Identity.ForState("A", typeof(string)));

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
            Identity.ForState("M", typeof(int), typeof(string)),
            Identity.ForState("M", typeof(string), typeof(int))
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
        var equal = comparer.Equals(Identity.ForState("A"), null);

        // Assert
        Assert.False(equal);
    }

    [Fact]
    public void GetHashCode_WhenSameParameterizedIdentityFromSeparateArrays_ShouldBeEqual()
    {
        // Arrange
        var comparer = StateIdentityEqualityComparer<string>.Instance;
        var first = Identity.ForState("A", typeof(int));
        var second = Identity.ForState("A", typeof(int));

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
        var firstHash = comparer.GetHashCode(Identity.ForState("A"));
        var secondHash = comparer.GetHashCode(Identity.ForState("A"));

        // Assert
        Assert.Equal(firstHash, secondHash);
    }
}
