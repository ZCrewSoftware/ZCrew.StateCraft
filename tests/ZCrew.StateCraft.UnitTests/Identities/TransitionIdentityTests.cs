namespace ZCrew.StateCraft.UnitTests.Identities;

public class TransitionIdentityTests
{
    [Fact]
    public void For_WhenNoTypeParameters_ShouldCaptureValueWithEmptyParameterTypes()
    {
        // Act
        var transition = TransitionIdentity.For("Go");

        // Assert
        Assert.Equal("Go", transition.TransitionValue);
        Assert.Empty(transition.TransitionParameterTypes);
    }

    [Fact]
    public void For_int_WhenOneTypeParameter_ShouldCaptureType()
    {
        // Act
        var transition = TransitionIdentity.For<string, int>("Go");

        // Assert
        Assert.Equal("Go", transition.TransitionValue);
        Assert.Equal([typeof(int)], transition.TransitionParameterTypes);
    }

    [Fact]
    public void For_int_string_WhenTwoTypeParameters_ShouldCaptureTypesInOrder()
    {
        // Act
        var transition = TransitionIdentity.For<string, int, string>("Go");

        // Assert
        Assert.Equal("Go", transition.TransitionValue);
        Assert.Equal([typeof(int), typeof(string)], transition.TransitionParameterTypes);
    }

    [Fact]
    public void For_int_string_bool_WhenThreeTypeParameters_ShouldCaptureTypesInOrder()
    {
        // Act
        var transition = TransitionIdentity.For<string, int, string, bool>("Go");

        // Assert
        Assert.Equal("Go", transition.TransitionValue);
        Assert.Equal([typeof(int), typeof(string), typeof(bool)], transition.TransitionParameterTypes);
    }

    [Fact]
    public void For_int_string_bool_long_WhenFourTypeParameters_ShouldCaptureTypesInOrder()
    {
        // Act
        var transition = TransitionIdentity.For<string, int, string, bool, long>("Go");

        // Assert
        Assert.Equal("Go", transition.TransitionValue);
        Assert.Equal([typeof(int), typeof(string), typeof(bool), typeof(long)], transition.TransitionParameterTypes);
    }

    [Fact]
    public void For_WhenGenericOverload_ShouldMatchTypeListOverload()
    {
        // Act
        var generic = TransitionIdentity.For<string, int, string>("Go");
        var typeList = TransitionIdentity.For("Go", typeof(int), typeof(string));

        // Assert
        Assert.Equal(typeList.TransitionValue, generic.TransitionValue);
        Assert.Equal(typeList.TransitionParameterTypes, generic.TransitionParameterTypes);
    }
}
