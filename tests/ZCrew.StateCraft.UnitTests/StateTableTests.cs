using ZCrew.StateCraft.UnitTests.Stubs;

namespace ZCrew.StateCraft.UnitTests;

public class StateTableTests
{
    [Fact]
    public void LookupState_WhenStateExists_ShouldReturnState()
    {
        // Arrange
        var state = new StubParameterlessState<string, string>("A");
        var stateTable = new StateTable<string, string>([state]);

        // Act
        var result = stateTable.LookupState("A");

        // Assert
        Assert.Same(state, result);
    }

    [Fact]
    public void LookupState_WhenMultipleStatesExist_ShouldReturnCorrectState()
    {
        // Arrange
        var stateA = new StubParameterlessState<string, string>("A");
        var stateB = new StubParameterlessState<string, string>("B");
        var stateTable = new StateTable<string, string>([stateA, stateB]);

        // Act
        var resultA = stateTable.LookupState("A");
        var resultB = stateTable.LookupState("B");

        // Assert
        Assert.Same(stateA, resultA);
        Assert.Same(stateB, resultB);
    }

    [Fact]
    public void LookupState_WhenStateDoesNotExist_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var state = new StubParameterlessState<string, string>("A");
        var stateTable = new StateTable<string, string>([state]);

        // Act
        var lookupState = () => stateTable.LookupState("B");

        // Assert
        Assert.Throws<InvalidOperationException>(lookupState);
    }

    [Fact]
    public void LookupState_WhenTableIsEmpty_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var stateTable = new StateTable<string, string>([]);

        // Act
        var lookupState = () => stateTable.LookupState("A");

        // Assert
        Assert.Throws<InvalidOperationException>(lookupState);
    }

    [Fact]
    public void LookupState_T_WhenStateExists_ShouldReturnState()
    {
        // Arrange
        var state = new StubParameterizedState<string, string, int>("A");
        var stateTable = new StateTable<string, string>([state]);

        // Act
        var result = stateTable.LookupState<int>("A");

        // Assert
        Assert.Same(state, result);
    }

    [Fact]
    public void LookupState_T_WhenMultipleStatesExist_ShouldReturnCorrectState()
    {
        // Arrange
        var stateA = new StubParameterizedState<string, string, int>("A");
        var stateB = new StubParameterizedState<string, string, string>("B");
        var stateTable = new StateTable<string, string>([stateA, stateB]);

        // Act
        var resultA = stateTable.LookupState<int>("A");
        var resultB = stateTable.LookupState<string>("B");

        // Assert
        Assert.Same(stateA, resultA);
        Assert.Same(stateB, resultB);
    }

    [Fact]
    public void LookupState_T_WhenStateDoesNotExist_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var state = new StubParameterizedState<string, string, int>("A");
        var stateTable = new StateTable<string, string>([state]);

        // Act
        var lookupState = () => stateTable.LookupState<int>("B");

        // Assert
        Assert.Throws<InvalidOperationException>(lookupState);
    }

    [Fact]
    public void LookupState_T_WhenWrongParameterType_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var state = new StubParameterizedState<string, string, int>("A");
        var stateTable = new StateTable<string, string>([state]);

        // Act
        var lookupState = () => stateTable.LookupState<string>("A");

        // Assert
        Assert.Throws<InvalidOperationException>(lookupState);
    }

    [Fact]
    public void LookupState_T_WhenTableIsEmpty_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var stateTable = new StateTable<string, string>([]);

        // Act
        var lookupState = () => stateTable.LookupState<int>("A");

        // Assert
        Assert.Throws<InvalidOperationException>(lookupState);
    }
}
