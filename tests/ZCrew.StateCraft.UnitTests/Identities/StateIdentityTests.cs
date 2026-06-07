namespace ZCrew.StateCraft.UnitTests.Identities;

public class StateIdentityTests
{
    [Fact]
    public void For_WhenNoTypeParameters_ShouldCaptureValueWithEmptyParameterTypes()
    {
        // Act
        var state = StateIdentity.For("A");

        // Assert
        Assert.Equal("A", state.StateValue);
        Assert.Empty(state.StateParameterTypes);
    }

    [Fact]
    public void For_int_WhenOneTypeParameter_ShouldCaptureType()
    {
        // Act
        var state = StateIdentity.For<string, int>("A");

        // Assert
        Assert.Equal("A", state.StateValue);
        Assert.Equal([typeof(int)], state.StateParameterTypes);
    }

    [Fact]
    public void For_int_string_WhenTwoTypeParameters_ShouldCaptureTypesInOrder()
    {
        // Act
        var state = StateIdentity.For<string, int, string>("A");

        // Assert
        Assert.Equal("A", state.StateValue);
        Assert.Equal([typeof(int), typeof(string)], state.StateParameterTypes);
    }

    [Fact]
    public void For_int_string_bool_WhenThreeTypeParameters_ShouldCaptureTypesInOrder()
    {
        // Act
        var state = StateIdentity.For<string, int, string, bool>("A");

        // Assert
        Assert.Equal("A", state.StateValue);
        Assert.Equal([typeof(int), typeof(string), typeof(bool)], state.StateParameterTypes);
    }

    [Fact]
    public void For_int_string_bool_long_WhenFourTypeParameters_ShouldCaptureTypesInOrder()
    {
        // Act
        var state = StateIdentity.For<string, int, string, bool, long>("A");

        // Assert
        Assert.Equal("A", state.StateValue);
        Assert.Equal([typeof(int), typeof(string), typeof(bool), typeof(long)], state.StateParameterTypes);
    }

    [Fact]
    public void For_WhenGenericOverload_ShouldMatchTypeListOverload()
    {
        // Act
        var generic = StateIdentity.For<string, int, string>("A");
        var typeList = StateIdentity.For("A", typeof(int), typeof(string));

        // Assert
        Assert.Equal(typeList.StateValue, generic.StateValue);
        Assert.Equal(typeList.StateParameterTypes, generic.StateParameterTypes);
    }
}
