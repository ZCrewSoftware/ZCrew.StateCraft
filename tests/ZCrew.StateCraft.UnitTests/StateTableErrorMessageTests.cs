using ZCrew.StateCraft.UnitTests.Stubs;

namespace ZCrew.StateCraft.UnitTests;

public class StateTableErrorMessageTests
{
    [Fact(Skip = "BL-F07: LookupState error always says 'parameterless' regardless of arity")]
    public void LookupState_T_WhenNotFound_ShouldIncludeSearchedTypeInErrorMessage()
    {
        // Arrange
        var stateTable = new StateTable<string, string>([]);

        // Act
        var lookupState = () => stateTable.LookupState<int>("A");

        // Assert
        var exception = Assert.Throws<InvalidOperationException>(lookupState);
        Assert.Contains("Int32", exception.Message);
    }

    [Fact(Skip = "BL-F07: LookupState error always says 'parameterless' regardless of arity")]
    public void LookupState_T1T2_WhenNotFound_ShouldIncludeSearchedTypesInErrorMessage()
    {
        // Arrange
        var stateTable = new StateTable<string, string>([]);

        // Act
        var lookupState = () => stateTable.LookupState<int, string>("A");

        // Assert
        var exception = Assert.Throws<InvalidOperationException>(lookupState);
        Assert.Contains("Int32", exception.Message);
        Assert.Contains("String", exception.Message);
    }

    [Fact(Skip = "BL-F07: LookupState error always says 'parameterless' regardless of arity")]
    public void LookupState_T_WhenWrongType_ShouldIncludeRegisteredTypesInErrorMessage()
    {
        // Arrange
        var state = new StubState<string, string>("A", typeof(int));
        var stateTable = new StateTable<string, string>([state]);

        // Act
        var lookupState = () => stateTable.LookupState<string>("A");

        // Assert
        var exception = Assert.Throws<InvalidOperationException>(lookupState);
        Assert.Contains("String", exception.Message);
        Assert.DoesNotContain("parameterless", exception.Message);
    }
}
