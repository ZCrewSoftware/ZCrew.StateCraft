using ZCrew.StateCraft.UnitTests.Stubs;

namespace ZCrew.StateCraft.UnitTests;

public class StateTableTests
{
    [Fact]
    public void LookupState_WhenStateExists_ShouldReturnState()
    {
        // Arrange
        var state = new StubState<string, string>("A");
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
        var stateA = new StubState<string, string>("A");
        var stateB = new StubState<string, string>("B");
        var stateTable = new StateTable<string, string>([stateA, stateB]);

        // Act
        var resultA = stateTable.LookupState("A");
        var resultB = stateTable.LookupState("B");

        // Assert
        Assert.Same(stateA, resultA);
        Assert.Same(stateB, resultB);
    }

    [Fact]
    public void LookupState_WhenStateDoesNotExist_ShouldThrowWithStateDisplay()
    {
        // Arrange
        var state = new StubState<string, string>("A");
        var stateTable = new StateTable<string, string>([state]);

        // Act
        var lookupState = () => stateTable.LookupState("B");

        // Assert
        var exception = Assert.Throws<InvalidOperationException>(lookupState);
        Assert.Contains("State=B", exception.Message);
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
        var state = new StubState<string, string>("A", typeof(int));
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
        var stateA = new StubState<string, string>("A", typeof(int));
        var stateB = new StubState<string, string>("B", typeof(string));
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
        var state = new StubState<string, string>("A", typeof(int));
        var stateTable = new StateTable<string, string>([state]);

        // Act
        var lookupState = () => stateTable.LookupState<int>("B");

        // Assert
        Assert.Throws<InvalidOperationException>(lookupState);
    }

    [Fact]
    public void LookupState_T_WhenWrongParameterType_ShouldThrowWithSearchedAndRegisteredTypes()
    {
        // Arrange
        var state = new StubState<string, string>("A", typeof(int));
        var stateTable = new StateTable<string, string>([state]);

        // Act
        var lookupState = () => stateTable.LookupState<string>("A");

        // Assert
        var exception = Assert.Throws<InvalidOperationException>(lookupState);
        Assert.Contains("A<string>", exception.Message);
        Assert.Contains("Registered: A<int>", exception.Message);
    }

    [Fact]
    public void LookupState_T_WhenMultipleRegisteredTypes_ShouldListAllRegisteredStates()
    {
        // Arrange
        var stateA1 = new StubState<string, string>("A");
        var stateA2 = new StubState<string, string>("A", typeof(string));
        var stateA3 = new StubState<string, string>("A", typeof(long));
        var stateTable = new StateTable<string, string>([stateA1, stateA2, stateA3]);

        // Act
        var lookupState = () => stateTable.LookupState<int>("A");

        // Assert
        var exception = Assert.Throws<InvalidOperationException>(lookupState);
        Assert.Contains("State=A<int>", exception.Message);
        Assert.Contains("Registered: A, A<string>, A<long>", exception.Message);
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
