using ZCrew.StateCraft.Parameters;

namespace ZCrew.StateCraft.UnitTests.Parameters;

public class StateMachineParametersTests
{
    [Fact]
    public void GetCurrentParameter_String_WhenParameterExists_ShouldReturnValue()
    {
        // Arrange
        var parameters = new StateMachineParameters();
        parameters.SetNextParameters(["test-value"]);
        parameters.CommitTransition();

        // Act
        var result = parameters.GetCurrentParameter<string>(0);

        // Assert
        Assert.Equal("test-value", result);
    }

    [Fact]
    public void GetCurrentParameter_WhenMultipleParametersExist_ShouldReturnCorrectValueAtEachIndex()
    {
        // Arrange
        var parameters = new StateMachineParameters();
        parameters.SetNextParameters(["first", 42, true]);
        parameters.CommitTransition();

        // Act
        var stringResult = parameters.GetCurrentParameter<string>(0);
        var intResult = parameters.GetCurrentParameter<int>(1);
        var boolResult = parameters.GetCurrentParameter<bool>(2);

        // Assert
        Assert.Equal("first", stringResult);
        Assert.Equal(42, intResult);
        Assert.True(boolResult);
    }

    [Fact]
    public void GetCurrentParameter_String_WhenParameterIsNull_ShouldReturnDefault()
    {
        // Arrange
        var parameters = new StateMachineParameters();
        parameters.SetNextParameters([null]);
        parameters.CommitTransition();

        // Act
        var result = parameters.GetCurrentParameter<string?>(0);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetCurrentParameter_String_WhenIndexIsNegative_ShouldThrowArgumentException()
    {
        // Arrange
        var parameters = new StateMachineParameters();
        parameters.SetNextParameters(["value"]);
        parameters.CommitTransition();

        // Act
        var act = object () => parameters.GetCurrentParameter<string>(-1);

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void GetCurrentParameter_String_WhenIndexExceedsCount_ShouldThrowArgumentException()
    {
        // Arrange
        var parameters = new StateMachineParameters();
        parameters.SetNextParameters(["value"]);
        parameters.CommitTransition();

        // Act
        var act = object () => parameters.GetCurrentParameter<string>(1);

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void GetCurrentParameter_String_WhenNoParametersSet_ShouldThrowArgumentException()
    {
        // Arrange
        var parameters = new StateMachineParameters();

        // Act
        var act = object () => parameters.GetCurrentParameter<string>(0);

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void GetCurrentParameter_Int_WhenTypeMismatch_ShouldThrowArgumentException()
    {
        // Arrange
        var parameters = new StateMachineParameters();
        parameters.SetNextParameters(["string-value"]);
        parameters.CommitTransition();

        // Act
        var act = () => (object)parameters.GetCurrentParameter<int>(0);

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void GetPreviousParameter_String_WhenParameterExists_ShouldReturnValue()
    {
        // Arrange
        var parameters = new StateMachineParameters();
        parameters.SetNextParameters(["previous-value"]);
        parameters.CommitTransition();
        parameters.BeginTransition();

        // Act
        var result = parameters.GetPreviousParameter<string>(0);

        // Assert
        Assert.Equal("previous-value", result);
    }

    [Fact]
    public void GetPreviousParameter_WhenMultipleParametersExist_ShouldReturnCorrectValueAtEachIndex()
    {
        // Arrange
        var parameters = new StateMachineParameters();
        parameters.SetNextParameters(["first", 42, true]);
        parameters.CommitTransition();
        parameters.BeginTransition();

        // Act
        var stringResult = parameters.GetPreviousParameter<string>(0);
        var intResult = parameters.GetPreviousParameter<int>(1);
        var boolResult = parameters.GetPreviousParameter<bool>(2);

        // Assert
        Assert.Equal("first", stringResult);
        Assert.Equal(42, intResult);
        Assert.True(boolResult);
    }

    [Fact]
    public void GetPreviousParameter_String_WhenParameterIsNull_ShouldReturnDefault()
    {
        // Arrange
        var parameters = new StateMachineParameters();
        parameters.SetNextParameters([null]);
        parameters.CommitTransition();
        parameters.BeginTransition();

        // Act
        var result = parameters.GetPreviousParameter<string?>(0);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetPreviousParameter_String_WhenIndexIsNegative_ShouldThrowArgumentOutOfRangeException()
    {
        // Arrange
        var parameters = new StateMachineParameters();
        parameters.SetNextParameters(["value"]);
        parameters.CommitTransition();
        parameters.BeginTransition();

        // Act
        var act = object () => parameters.GetPreviousParameter<string>(-1);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(act);
    }

    [Fact]
    public void GetPreviousParameter_String_WhenIndexExceedsCount_ShouldThrowArgumentOutOfRangeException()
    {
        // Arrange
        var parameters = new StateMachineParameters();
        parameters.SetNextParameters(["value"]);
        parameters.CommitTransition();
        parameters.BeginTransition();

        // Act
        var act = object () => parameters.GetPreviousParameter<string>(1);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(act);
    }

    [Fact]
    public void GetPreviousParameter_String_WhenNoParametersSet_ShouldThrowArgumentOutOfRangeException()
    {
        // Arrange
        var parameters = new StateMachineParameters();

        // Act
        var act = object () => parameters.GetPreviousParameter<string>(0);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(act);
    }

    [Fact]
    public void GetPreviousParameter_Int_WhenTypeMismatch_ShouldThrowArgumentException()
    {
        // Arrange
        var parameters = new StateMachineParameters();
        parameters.SetNextParameters(["string-value"]);
        parameters.CommitTransition();
        parameters.BeginTransition();

        // Act
        var act = () => (object)parameters.GetPreviousParameter<int>(0);

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void GetNextParameter_String_WhenParameterExists_ShouldReturnValue()
    {
        // Arrange
        var parameters = new StateMachineParameters();
        parameters.SetNextParameters(["next-value"]);

        // Act
        var result = parameters.GetNextParameter<string>(0);

        // Assert
        Assert.Equal("next-value", result);
    }

    [Fact]
    public void GetNextParameter_WhenMultipleParametersExist_ShouldReturnCorrectValueAtEachIndex()
    {
        // Arrange
        var parameters = new StateMachineParameters();
        parameters.SetNextParameters(["first", 42, true]);

        // Act
        var stringResult = parameters.GetNextParameter<string>(0);
        var intResult = parameters.GetNextParameter<int>(1);
        var boolResult = parameters.GetNextParameter<bool>(2);

        // Assert
        Assert.Equal("first", stringResult);
        Assert.Equal(42, intResult);
        Assert.True(boolResult);
    }

    [Fact]
    public void GetNextParameter_String_WhenParameterIsNull_ShouldReturnDefault()
    {
        // Arrange
        var parameters = new StateMachineParameters();
        parameters.SetNextParameters([null]);

        // Act
        var result = parameters.GetNextParameter<string?>(0);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetNextParameter_String_WhenIndexIsNegative_ShouldThrowArgumentException()
    {
        // Arrange
        var parameters = new StateMachineParameters();
        parameters.SetNextParameters(["value"]);

        // Act
        var act = object () => parameters.GetNextParameter<string>(-1);

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void GetNextParameter_String_WhenIndexExceedsCount_ShouldThrowArgumentException()
    {
        // Arrange
        var parameters = new StateMachineParameters();
        parameters.SetNextParameters(["value"]);

        // Act
        var act = object () => parameters.GetNextParameter<string>(1);

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void GetNextParameter_String_WhenNoParametersSet_ShouldThrowArgumentException()
    {
        // Arrange
        var parameters = new StateMachineParameters();

        // Act
        var act = object () => parameters.GetNextParameter<string>(0);

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void GetNextParameter_Int_WhenTypeMismatch_ShouldThrowArgumentException()
    {
        // Arrange
        var parameters = new StateMachineParameters();
        parameters.SetNextParameters(["string-value"]);

        // Act
        var act = () => (object)parameters.GetNextParameter<int>(0);

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void SetNextParameters_WhenCalled_ShouldStageParameters()
    {
        // Arrange
        var parameters = new StateMachineParameters();

        // Act
        parameters.SetNextParameters(["staged"]);

        // Assert
        var result = parameters.GetNextParameter<string>(0);
        Assert.Equal("staged", result);
    }

    [Fact]
    public void SetNextParameters_WhenCalledMultipleTimes_ShouldOverwritePrevious()
    {
        // Arrange
        var parameters = new StateMachineParameters();
        parameters.SetNextParameters(["first"]);

        // Act
        parameters.SetNextParameters(["second"]);

        // Assert
        var result = parameters.GetNextParameter<string>(0);
        Assert.Equal("second", result);
    }

    [Fact]
    public void SetNextParameters_WhenCalledWithEmptyArray_ShouldSetZeroParameters()
    {
        // Arrange
        var parameters = new StateMachineParameters();

        // Act
        parameters.SetNextParameters([]);

        // Assert
        var act = object () => parameters.GetNextParameter<string>(0);
        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void SetNextParameters_WhenCalledWithMaxParameters_ShouldSucceed()
    {
        // Arrange
        var parameters = new StateMachineParameters();

        // Act
        parameters.SetNextParameters(["one", "two", "three", "four"]);
        parameters.CommitTransition();

        // Assert
        Assert.Equal("one", parameters.GetCurrentParameter<string>(0));
        Assert.Equal("two", parameters.GetCurrentParameter<string>(1));
        Assert.Equal("three", parameters.GetCurrentParameter<string>(2));
        Assert.Equal("four", parameters.GetCurrentParameter<string>(3));
    }

    [Fact]
    public void SetNextParameters_WhenCalledWithMoreThanMaxParameters_ShouldThrowArgumentException()
    {
        // Arrange
        var parameters = new StateMachineParameters();

        // Act
        var act = () => parameters.SetNextParameters(["one", "two", "three", "four", "five"]);

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void BeginTransition_WhenCalled_ShouldMoveCurrentToPrevious()
    {
        // Arrange
        var parameters = new StateMachineParameters();
        parameters.SetNextParameters(["current-value"]);
        parameters.CommitTransition();

        // Act
        parameters.BeginTransition();

        // Assert
        var result = parameters.GetPreviousParameter<string>(0);
        Assert.Equal("current-value", result);
    }

    [Fact]
    public void BeginTransition_WhenCalled_ShouldClearCurrent()
    {
        // Arrange
        var parameters = new StateMachineParameters();
        parameters.SetNextParameters(["current-value"]);
        parameters.CommitTransition();

        // Act
        parameters.BeginTransition();

        // Assert
        var act = object () => parameters.GetCurrentParameter<string>(0);
        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void BeginTransition_WhenNoCurrentParameters_ShouldNotThrow()
    {
        // Arrange
        var parameters = new StateMachineParameters();

        // Act
        var act = () => parameters.BeginTransition();

        // Assert
        var exception = Record.Exception(act);
        Assert.Null(exception);
    }

    [Fact]
    public void CommitTransition_WhenCalled_ShouldMoveNextToCurrent()
    {
        // Arrange
        var parameters = new StateMachineParameters();
        parameters.SetNextParameters(["next-value"]);

        // Act
        parameters.CommitTransition();

        // Assert
        var result = parameters.GetCurrentParameter<string>(0);
        Assert.Equal("next-value", result);
    }

    [Fact]
    public void CommitTransition_WhenCalled_ShouldClearNext()
    {
        // Arrange
        var parameters = new StateMachineParameters();
        parameters.SetNextParameters(["next-value"]);

        // Act
        parameters.CommitTransition();

        // Assert
        var act = object () => parameters.GetNextParameter<string>(0);
        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void CommitTransition_WhenCalled_ShouldClearPrevious()
    {
        // Arrange
        var parameters = new StateMachineParameters();
        parameters.SetNextParameters(["first"]);
        parameters.CommitTransition();
        parameters.BeginTransition();
        parameters.SetNextParameters(["second"]);

        // Act
        parameters.CommitTransition();

        // Assert
        var act = object () => parameters.GetPreviousParameter<string>(0);
        Assert.Throws<ArgumentOutOfRangeException>(act);
    }

    [Fact]
    public void CommitTransition_WhenCalled_ShouldResetCanCommitTransition()
    {
        // Arrange
        var parameters = new StateMachineParameters();
        parameters.SetNextParameters(["value"]);

        // Act
        parameters.CommitTransition();

        // Assert
        var result = parameters.CanCommitTransition();
        Assert.False(result);
    }

    [Fact]
    public void CommitTransition_WhenFullWorkflowCompletes_ShouldPromoteParametersCorrectly()
    {
        // Arrange
        var parameters = new StateMachineParameters();
        parameters.SetNextParameters(["state-A", 100]);
        parameters.CommitTransition();
        parameters.BeginTransition();
        parameters.SetNextParameters(["state-B", 200]);

        // Act
        parameters.CommitTransition();

        // Assert
        var stringResult = parameters.GetCurrentParameter<string>(0);
        var intResult = parameters.GetCurrentParameter<int>(1);
        var canCommit = parameters.CanCommitTransition();
        Assert.Equal("state-B", stringResult);
        Assert.Equal(200, intResult);
        Assert.False(canCommit);
    }

    [Fact]
    public void RollbackTransition_WhenCalled_ShouldRestorePreviousToCurrent()
    {
        // Arrange
        var parameters = new StateMachineParameters();
        parameters.SetNextParameters(["original"]);
        parameters.CommitTransition();
        parameters.BeginTransition();

        // Act
        parameters.RollbackTransition();

        // Assert
        var result = parameters.GetCurrentParameter<string>(0);
        Assert.Equal("original", result);
    }

    [Fact]
    public void RollbackTransition_WhenCalled_ShouldClearPrevious()
    {
        // Arrange
        var parameters = new StateMachineParameters();
        parameters.SetNextParameters(["original"]);
        parameters.CommitTransition();
        parameters.BeginTransition();

        // Act
        parameters.RollbackTransition();

        // Assert
        var act = object () => parameters.GetPreviousParameter<string>(0);
        Assert.Throws<ArgumentOutOfRangeException>(act);
    }

    [Fact]
    public void RollbackTransition_WhenCalled_ShouldClearNext()
    {
        // Arrange
        var parameters = new StateMachineParameters();
        parameters.SetNextParameters(["original"]);
        parameters.CommitTransition();
        parameters.BeginTransition();
        parameters.SetNextParameters(["staged"]);

        // Act
        parameters.RollbackTransition();

        // Assert
        var act = object () => parameters.GetNextParameter<string>(0);
        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void RollbackTransition_WhenCalled_ShouldResetCanCommitTransition()
    {
        // Arrange
        var parameters = new StateMachineParameters();
        parameters.SetNextParameters(["original"]);
        parameters.CommitTransition();
        parameters.BeginTransition();
        parameters.SetNextParameters(["staged"]);

        // Act
        parameters.RollbackTransition();

        // Assert
        var result = parameters.CanCommitTransition();
        Assert.False(result);
    }

    [Fact]
    public void RollbackTransition_WhenFullWorkflowRollsBack_ShouldRestoreOriginalState()
    {
        // Arrange
        var parameters = new StateMachineParameters();
        parameters.SetNextParameters(["original"]);
        parameters.CommitTransition();
        parameters.BeginTransition();
        parameters.SetNextParameters(["new-value"]);

        // Act
        parameters.RollbackTransition();

        // Assert
        var result = parameters.GetCurrentParameter<string>(0);
        var canCommit = parameters.CanCommitTransition();
        Assert.Equal("original", result);
        Assert.False(canCommit);
    }

    [Fact]
    public void CanCommitTransition_WhenNextParametersNotSet_ShouldReturnFalse()
    {
        // Arrange
        var parameters = new StateMachineParameters();

        // Act
        var result = parameters.CanCommitTransition();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void CanCommitTransition_WhenNextParametersSet_ShouldReturnTrue()
    {
        // Arrange
        var parameters = new StateMachineParameters();
        parameters.SetNextParameters(["value"]);

        // Act
        var result = parameters.CanCommitTransition();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void CanCommitTransition_WhenNextParametersSetWithEmptyArray_ShouldReturnTrue()
    {
        // Arrange
        var parameters = new StateMachineParameters();
        parameters.SetNextParameters([]);

        // Act
        var result = parameters.CanCommitTransition();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void CanCommitTransition_AfterCommit_ShouldReturnFalse()
    {
        // Arrange
        var parameters = new StateMachineParameters();
        parameters.SetNextParameters(["value"]);
        parameters.CommitTransition();

        // Act
        var result = parameters.CanCommitTransition();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void CanCommitTransition_AfterRollback_ShouldReturnFalse()
    {
        // Arrange
        var parameters = new StateMachineParameters();
        parameters.BeginTransition();
        parameters.SetNextParameters(["value"]);

        // Act
        parameters.RollbackTransition();

        // Assert
        var result = parameters.CanCommitTransition();
        Assert.False(result);
    }

    [Fact]
    public void Clear_WhenCalled_ShouldClearPreviousSlot()
    {
        // Arrange
        var parameters = new StateMachineParameters();
        parameters.SetNextParameters(["first"]);
        parameters.CommitTransition();
        parameters.BeginTransition();

        // Act
        parameters.Clear();

        // Assert
        var act = object () => parameters.GetPreviousParameter<string>(0);
        Assert.Throws<ArgumentOutOfRangeException>(act);
    }

    [Fact]
    public void Clear_WhenCalled_ShouldClearCurrentSlot()
    {
        // Arrange
        var parameters = new StateMachineParameters();
        parameters.SetNextParameters(["value"]);
        parameters.CommitTransition();

        // Act
        parameters.Clear();

        // Assert
        var act = object () => parameters.GetCurrentParameter<string>(0);
        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void Clear_WhenCalled_ShouldClearNextSlot()
    {
        // Arrange
        var parameters = new StateMachineParameters();
        parameters.SetNextParameters(["value"]);

        // Act
        parameters.Clear();

        // Assert
        var act = object () => parameters.GetNextParameter<string>(0);
        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void Clear_WhenCalled_ShouldResetCanCommitTransition()
    {
        // Arrange
        var parameters = new StateMachineParameters();
        parameters.SetNextParameters(["value"]);

        // Act
        parameters.Clear();

        // Assert
        var result = parameters.CanCommitTransition();
        Assert.False(result);
    }
}
