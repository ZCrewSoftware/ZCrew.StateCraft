using ZCrew.StateCraft.Parameters;
using ZCrew.StateCraft.Parameters.Contracts;

namespace ZCrew.StateCraft.UnitTests.Parameters;

public class StateMachineParametersTests
{
    [Fact]
    public void SetNextParameter_T_WhenCalled_ShouldStageParameter()
    {
        // Arrange
        var parameters = new StateMachineParameters();

        // Act
        parameters.SetNextParameter("staged");

        // Assert
        var result = parameters.GetNextParameter<string>();
        Assert.Equal("staged", result);
    }

    [Fact]
    public void SetNextParameter_T_WhenCalled_ShouldStoreType()
    {
        // Arrange
        var parameters = new StateMachineParameters();

        // Act
        parameters.SetNextParameter("value");

        // Assert
        var types = parameters.NextParameterTypes;
        Assert.Single(types);
        Assert.Equal(typeof(string), types[0]);
    }

    [Fact]
    public void SetNextParameter_T_WhenCalledMultipleTimes_ShouldOverwritePrevious()
    {
        // Arrange
        var parameters = new StateMachineParameters();
        parameters.SetNextParameter("first");

        // Act
        parameters.SetNextParameter("second");

        // Assert
        var result = parameters.GetNextParameter<string>();
        Assert.Equal("second", result);
    }

    [Fact]
    public void SetNextParameter_T_WhenCalledWithNull_ShouldSetNull()
    {
        // Arrange
        var parameters = new StateMachineParameters();
        parameters.SetNextParameter("original");

        // Act
        parameters.SetNextParameter<string?>(null);

        // Assert
        var result = parameters.GetNextParameter<string?>();
        Assert.Null(result);
    }

    [Fact]
    public void SetNextParameter_T_WhenCommitted_ShouldBeAccessibleAsCurrent()
    {
        // Arrange
        var parameters = new StateMachineParameters();
        parameters.SetNextParameter("value");

        // Act
        parameters.CommitTransition();

        // Assert
        Assert.Equal("value", parameters.GetCurrentParameter<string>());
    }

    [Fact]
    public void SetNextParameters_T1_T2_WhenCalled_ShouldStageParameters()
    {
        // Arrange
        var parameters = new StateMachineParameters();

        // Act
        parameters.SetNextParameters("first", 42);

        // Assert
        var (s, i) = parameters.GetNextParameters<string, int>();
        Assert.Equal("first", s);
        Assert.Equal(42, i);
    }

    [Fact]
    public void SetNextParameters_T1_T2_WhenCalled_ShouldStoreTypes()
    {
        // Arrange
        var parameters = new StateMachineParameters();

        // Act
        parameters.SetNextParameters("first", 42);

        // Assert
        var types = parameters.NextParameterTypes;
        Assert.Equal(2, types.Count);
        Assert.Equal(typeof(string), types[0]);
        Assert.Equal(typeof(int), types[1]);
    }

    [Fact]
    public void SetNextParameters_T1_T2_WhenCommitted_ShouldBeAccessibleAsCurrent()
    {
        // Arrange
        var parameters = new StateMachineParameters();
        parameters.SetNextParameters("value", 42);

        // Act
        parameters.CommitTransition();

        // Assert
        var (s, i) = parameters.GetCurrentParameters<string, int>();
        Assert.Equal("value", s);
        Assert.Equal(42, i);
    }

    [Fact]
    public void SetNextParameters_T1_T2_T3_WhenCalled_ShouldStageParameters()
    {
        // Arrange
        var parameters = new StateMachineParameters();

        // Act
        parameters.SetNextParameters("first", 42, true);

        // Assert
        var (s, i, b) = parameters.GetNextParameters<string, int, bool>();
        Assert.Equal("first", s);
        Assert.Equal(42, i);
        Assert.True(b);
    }

    [Fact]
    public void SetNextParameters_T1_T2_T3_WhenCalled_ShouldStoreTypes()
    {
        // Arrange
        var parameters = new StateMachineParameters();

        // Act
        parameters.SetNextParameters("first", 42, true);

        // Assert
        var types = parameters.NextParameterTypes;
        Assert.Equal(3, types.Count);
        Assert.Equal(typeof(string), types[0]);
        Assert.Equal(typeof(int), types[1]);
        Assert.Equal(typeof(bool), types[2]);
    }

    [Fact]
    public void SetNextParameters_T1_T2_T3_WhenCommitted_ShouldBeAccessibleAsCurrent()
    {
        // Arrange
        var parameters = new StateMachineParameters();
        parameters.SetNextParameters("value", 42, true);

        // Act
        parameters.CommitTransition();

        // Assert
        var (s, i, b) = parameters.GetCurrentParameters<string, int, bool>();
        Assert.Equal("value", s);
        Assert.Equal(42, i);
        Assert.True(b);
    }

    [Fact]
    public void SetNextParameters_T1_T2_T3_T4_WhenCalled_ShouldStageParameters()
    {
        // Arrange
        var parameters = new StateMachineParameters();

        // Act
        parameters.SetNextParameters("first", 42, true, 'x');

        // Assert
        var (s, i, b, c) = parameters.GetNextParameters<string, int, bool, char>();
        Assert.Equal("first", s);
        Assert.Equal(42, i);
        Assert.True(b);
        Assert.Equal('x', c);
    }

    [Fact]
    public void SetNextParameters_T1_T2_T3_T4_WhenCalled_ShouldStoreTypes()
    {
        // Arrange
        var parameters = new StateMachineParameters();

        // Act
        parameters.SetNextParameters("first", 42, true, 'x');

        // Assert
        var types = parameters.NextParameterTypes;
        Assert.Equal(4, types.Count);
        Assert.Equal(typeof(string), types[0]);
        Assert.Equal(typeof(int), types[1]);
        Assert.Equal(typeof(bool), types[2]);
        Assert.Equal(typeof(char), types[3]);
    }

    [Fact]
    public void SetNextParameters_T1_T2_T3_T4_WhenCommitted_ShouldBeAccessibleAsCurrent()
    {
        // Arrange
        var parameters = new StateMachineParameters();
        parameters.SetNextParameters("value", 42, true, 'x');

        // Act
        parameters.CommitTransition();

        // Assert
        var (s, i, b, c) = parameters.GetCurrentParameters<string, int, bool, char>();
        Assert.Equal("value", s);
        Assert.Equal(42, i);
        Assert.True(b);
        Assert.Equal('x', c);
    }

    [Fact]
    public void SetEmptyNextParameters_WhenCalled_ShouldSetNextParametersSetFlag()
    {
        // Arrange
        var parameters = new StateMachineParameters();

        // Act
        parameters.SetEmptyNextParameters();

        // Assert
        Assert.True(parameters.Status.HasFlag(StateMachineParametersFlags.NextParametersSet));
    }

    [Fact]
    public void SetEmptyNextParameters_WhenCalled_ShouldHaveZeroParameterCount()
    {
        // Arrange
        var parameters = new StateMachineParameters();

        // Act
        parameters.SetEmptyNextParameters();

        // Assert
        var types = parameters.NextParameterTypes;
        Assert.Empty(types);
    }

    [Fact]
    public void SetEmptyNextParameters_WhenCalled_ShouldThrowOnGetNextParameter()
    {
        // Arrange
        var parameters = new StateMachineParameters();
        parameters.SetEmptyNextParameters();

        // Act
        var act = () => parameters.GetNextParameter<string>();

        // Assert
        Assert.Throws<InvalidOperationException>(act);
    }

    [Fact]
    public void SetEmptyNextParameters_WhenCommitted_ShouldBeAccessibleAsCurrent()
    {
        // Arrange
        var parameters = new StateMachineParameters();
        parameters.SetEmptyNextParameters();

        // Act
        parameters.CommitTransition();

        // Assert
        Assert.True(parameters.Status.HasFlag(StateMachineParametersFlags.CurrentParametersSet));
        Assert.Empty(parameters.CurrentParameterTypes);
    }

    [Fact]
    public void GetPreviousParameter_T_WhenParameterExists_ShouldReturnValue()
    {
        // Arrange
        var parameters = new StateMachineParameters();
        parameters.SetNextParameter("previous-value");
        parameters.CommitTransition();
        parameters.BeginTransition();

        // Act
        var result = parameters.GetPreviousParameter<string>();

        // Assert
        Assert.Equal("previous-value", result);
    }

    [Fact]
    public void GetPreviousParameter_T_WhenParameterIsNull_ShouldReturnDefault()
    {
        // Arrange
        var parameters = new StateMachineParameters();
        parameters.SetNextParameter<string?>(null);
        parameters.CommitTransition();
        parameters.BeginTransition();

        // Act
        var result = parameters.GetPreviousParameter<string?>();

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetPreviousParameter_T_WhenNoParametersSet_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var parameters = new StateMachineParameters();

        // Act
        var act = () => parameters.GetPreviousParameter<string>();

        // Assert
        Assert.Throws<InvalidOperationException>(act);
    }

    [Fact]
    public void GetPreviousParameter_Int_WhenTypeMismatch_ShouldThrowInvalidCastException()
    {
        // Arrange
        var parameters = new StateMachineParameters();
        parameters.SetNextParameter("string-value");
        parameters.CommitTransition();
        parameters.BeginTransition();

        // Act
        var act = () => (object)parameters.GetPreviousParameter<int>();

        // Assert
        Assert.Throws<InvalidCastException>(act);
    }

    [Fact]
    public void GetPreviousParameters_T1_T2_WhenParametersExist_ShouldReturnValues()
    {
        // Arrange
        var parameters = new StateMachineParameters();
        parameters.SetNextParameters("value", 42);
        parameters.CommitTransition();
        parameters.BeginTransition();

        // Act
        var (s, i) = parameters.GetPreviousParameters<string, int>();

        // Assert
        Assert.Equal("value", s);
        Assert.Equal(42, i);
    }

    [Fact]
    public void GetPreviousParameters_T1_T2_WhenCountTooLow_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var parameters = new StateMachineParameters();
        parameters.SetNextParameter("value");
        parameters.CommitTransition();
        parameters.BeginTransition();

        // Act
        Action act = () => parameters.GetPreviousParameters<string, int>();

        // Assert
        Assert.Throws<InvalidOperationException>(act);
    }

    [Fact]
    public void GetPreviousParameters_T1_T2_T3_WhenParametersExist_ShouldReturnValues()
    {
        // Arrange
        var parameters = new StateMachineParameters();
        parameters.SetNextParameters("value", 42, true);
        parameters.CommitTransition();
        parameters.BeginTransition();

        // Act
        var (s, i, b) = parameters.GetPreviousParameters<string, int, bool>();

        // Assert
        Assert.Equal("value", s);
        Assert.Equal(42, i);
        Assert.True(b);
    }

    [Fact]
    public void GetPreviousParameters_T1_T2_T3_WhenCountTooLow_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var parameters = new StateMachineParameters();
        parameters.SetNextParameters("value", 42);
        parameters.CommitTransition();
        parameters.BeginTransition();

        // Act
        Action act = () => parameters.GetPreviousParameters<string, int, bool>();

        // Assert
        Assert.Throws<InvalidOperationException>(act);
    }

    [Fact]
    public void GetPreviousParameters_T1_T2_T3_T4_WhenParametersExist_ShouldReturnValues()
    {
        // Arrange
        var parameters = new StateMachineParameters();
        parameters.SetNextParameters("value", 42, true, 'x');
        parameters.CommitTransition();
        parameters.BeginTransition();

        // Act
        var (s, i, b, c) = parameters.GetPreviousParameters<string, int, bool, char>();

        // Assert
        Assert.Equal("value", s);
        Assert.Equal(42, i);
        Assert.True(b);
        Assert.Equal('x', c);
    }

    [Fact]
    public void GetPreviousParameters_T1_T2_T3_T4_WhenCountTooLow_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var parameters = new StateMachineParameters();
        parameters.SetNextParameters("value", 42, true);
        parameters.CommitTransition();
        parameters.BeginTransition();

        // Act
        Action act = () => parameters.GetPreviousParameters<string, int, bool, char>();

        // Assert
        Assert.Throws<InvalidOperationException>(act);
    }

    [Fact]
    public void GetCurrentParameter_T_WhenParameterExists_ShouldReturnValue()
    {
        // Arrange
        var parameters = new StateMachineParameters();
        parameters.SetNextParameter("test-value");
        parameters.CommitTransition();

        // Act
        var result = parameters.GetCurrentParameter<string>();

        // Assert
        Assert.Equal("test-value", result);
    }

    [Fact]
    public void GetCurrentParameter_T_WhenParameterIsNull_ShouldReturnDefault()
    {
        // Arrange
        var parameters = new StateMachineParameters();
        parameters.SetNextParameter<string?>(null);
        parameters.CommitTransition();

        // Act
        var result = parameters.GetCurrentParameter<string?>();

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetCurrentParameter_T_WhenNoParametersSet_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var parameters = new StateMachineParameters();

        // Act
        var act = () => parameters.GetCurrentParameter<string>();

        // Assert
        Assert.Throws<InvalidOperationException>(act);
    }

    [Fact]
    public void GetCurrentParameter_Int_WhenTypeMismatch_ShouldThrowInvalidCastException()
    {
        // Arrange
        var parameters = new StateMachineParameters();
        parameters.SetNextParameter("string-value");
        parameters.CommitTransition();

        // Act
        var act = () => (object)parameters.GetCurrentParameter<int>();

        // Assert
        Assert.Throws<InvalidCastException>(act);
    }

    [Fact]
    public void GetCurrentParameters_T1_T2_WhenParametersExist_ShouldReturnValues()
    {
        // Arrange
        var parameters = new StateMachineParameters();
        parameters.SetNextParameters("value", 42);
        parameters.CommitTransition();

        // Act
        var (s, i) = parameters.GetCurrentParameters<string, int>();

        // Assert
        Assert.Equal("value", s);
        Assert.Equal(42, i);
    }

    [Fact]
    public void GetCurrentParameters_T1_T2_WhenCountTooLow_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var parameters = new StateMachineParameters();
        parameters.SetNextParameter("value");
        parameters.CommitTransition();

        // Act
        Action act = () => parameters.GetCurrentParameters<string, int>();

        // Assert
        Assert.Throws<InvalidOperationException>(act);
    }

    [Fact]
    public void GetCurrentParameters_T1_T2_T3_WhenParametersExist_ShouldReturnValues()
    {
        // Arrange
        var parameters = new StateMachineParameters();
        parameters.SetNextParameters("value", 42, true);
        parameters.CommitTransition();

        // Act
        var (s, i, b) = parameters.GetCurrentParameters<string, int, bool>();

        // Assert
        Assert.Equal("value", s);
        Assert.Equal(42, i);
        Assert.True(b);
    }

    [Fact]
    public void GetCurrentParameters_T1_T2_T3_WhenCountTooLow_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var parameters = new StateMachineParameters();
        parameters.SetNextParameters("value", 42);
        parameters.CommitTransition();

        // Act
        Action act = () => parameters.GetCurrentParameters<string, int, bool>();

        // Assert
        Assert.Throws<InvalidOperationException>(act);
    }

    [Fact]
    public void GetCurrentParameters_T1_T2_T3_T4_WhenParametersExist_ShouldReturnValues()
    {
        // Arrange
        var parameters = new StateMachineParameters();
        parameters.SetNextParameters("value", 42, true, 'x');
        parameters.CommitTransition();

        // Act
        var (s, i, b, c) = parameters.GetCurrentParameters<string, int, bool, char>();

        // Assert
        Assert.Equal("value", s);
        Assert.Equal(42, i);
        Assert.True(b);
        Assert.Equal('x', c);
    }

    [Fact]
    public void GetCurrentParameters_T1_T2_T3_T4_WhenCountTooLow_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var parameters = new StateMachineParameters();
        parameters.SetNextParameters("value", 42, true);
        parameters.CommitTransition();

        // Act
        Action act = () => parameters.GetCurrentParameters<string, int, bool, char>();

        // Assert
        Assert.Throws<InvalidOperationException>(act);
    }

    [Fact]
    public void GetNextParameter_T_WhenParameterExists_ShouldReturnValue()
    {
        // Arrange
        var parameters = new StateMachineParameters();
        parameters.SetNextParameter("next-value");

        // Act
        var result = parameters.GetNextParameter<string>();

        // Assert
        Assert.Equal("next-value", result);
    }

    [Fact]
    public void GetNextParameter_T_WhenParameterIsNull_ShouldReturnDefault()
    {
        // Arrange
        var parameters = new StateMachineParameters();
        parameters.SetNextParameter<string?>(null);

        // Act
        var result = parameters.GetNextParameter<string?>();

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetNextParameter_T_WhenNoParametersSet_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var parameters = new StateMachineParameters();

        // Act
        var act = () => parameters.GetNextParameter<string>();

        // Assert
        Assert.Throws<InvalidOperationException>(act);
    }

    [Fact]
    public void GetNextParameter_T_WhenEmptyParametersSet_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var parameters = new StateMachineParameters();
        parameters.SetEmptyNextParameters();

        // Act
        var act = () => parameters.GetNextParameter<string>();

        // Assert
        Assert.Throws<InvalidOperationException>(act);
    }

    [Fact]
    public void GetNextParameter_Int_WhenTypeMismatch_ShouldThrowInvalidCastException()
    {
        // Arrange
        var parameters = new StateMachineParameters();
        parameters.SetNextParameter("string-value");

        // Act
        var act = () => (object)parameters.GetNextParameter<int>();

        // Assert
        Assert.Throws<InvalidCastException>(act);
    }

    [Fact]
    public void GetNextParameters_T1_T2_WhenParametersExist_ShouldReturnValues()
    {
        // Arrange
        var parameters = new StateMachineParameters();
        parameters.SetNextParameters("value", 42);

        // Act
        var (s, i) = parameters.GetNextParameters<string, int>();

        // Assert
        Assert.Equal("value", s);
        Assert.Equal(42, i);
    }

    [Fact]
    public void GetNextParameters_T1_T2_WhenCountTooLow_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var parameters = new StateMachineParameters();
        parameters.SetNextParameter("value");

        // Act
        Action act = () => parameters.GetNextParameters<string, int>();

        // Assert
        Assert.Throws<InvalidOperationException>(act);
    }

    [Fact]
    public void GetNextParameters_T1_T2_T3_WhenParametersExist_ShouldReturnValues()
    {
        // Arrange
        var parameters = new StateMachineParameters();
        parameters.SetNextParameters("value", 42, true);

        // Act
        var (s, i, b) = parameters.GetNextParameters<string, int, bool>();

        // Assert
        Assert.Equal("value", s);
        Assert.Equal(42, i);
        Assert.True(b);
    }

    [Fact]
    public void GetNextParameters_T1_T2_T3_WhenCountTooLow_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var parameters = new StateMachineParameters();
        parameters.SetNextParameters("value", 42);

        // Act
        Action act = () => parameters.GetNextParameters<string, int, bool>();

        // Assert
        Assert.Throws<InvalidOperationException>(act);
    }

    [Fact]
    public void GetNextParameters_T1_T2_T3_T4_WhenParametersExist_ShouldReturnValues()
    {
        // Arrange
        var parameters = new StateMachineParameters();
        parameters.SetNextParameters("value", 42, true, 'x');

        // Act
        var (s, i, b, c) = parameters.GetNextParameters<string, int, bool, char>();

        // Assert
        Assert.Equal("value", s);
        Assert.Equal(42, i);
        Assert.True(b);
        Assert.Equal('x', c);
    }

    [Fact]
    public void GetNextParameters_T1_T2_T3_T4_WhenCountTooLow_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var parameters = new StateMachineParameters();
        parameters.SetNextParameters("value", 42, true);

        // Act
        Action act = () => parameters.GetNextParameters<string, int, bool, char>();

        // Assert
        Assert.Throws<InvalidOperationException>(act);
    }

    [Fact]
    public void BeginTransition_WhenCalled_ShouldMoveCurrentToPrevious()
    {
        // Arrange
        var parameters = new StateMachineParameters();
        parameters.SetNextParameter("current-value");
        parameters.CommitTransition();

        // Act
        parameters.BeginTransition();

        // Assert
        var result = parameters.GetPreviousParameter<string>();
        Assert.Equal("current-value", result);
    }

    [Fact]
    public void BeginTransition_WhenCalled_ShouldClearCurrent()
    {
        // Arrange
        var parameters = new StateMachineParameters();
        parameters.SetNextParameter("current-value");
        parameters.CommitTransition();

        // Act
        parameters.BeginTransition();

        // Assert
        var act = () => parameters.GetCurrentParameter<string>();
        Assert.Throws<InvalidOperationException>(act);
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
    public void BeginTransition_WithMultipleParameters_ShouldMoveAllToPrevious()
    {
        // Arrange
        var parameters = new StateMachineParameters();
        parameters.SetNextParameters("state-A", 100);
        parameters.CommitTransition();

        // Act
        parameters.BeginTransition();

        // Assert
        var (s, i) = parameters.GetPreviousParameters<string, int>();
        Assert.Equal("state-A", s);
        Assert.Equal(100, i);
    }

    [Fact]
    public void CommitTransition_WhenCalled_ShouldMoveNextToCurrent()
    {
        // Arrange
        var parameters = new StateMachineParameters();
        parameters.SetNextParameter("next-value");

        // Act
        parameters.CommitTransition();

        // Assert
        var result = parameters.GetCurrentParameter<string>();
        Assert.Equal("next-value", result);
    }

    [Fact]
    public void CommitTransition_WhenCalled_ShouldClearNext()
    {
        // Arrange
        var parameters = new StateMachineParameters();
        parameters.SetNextParameter("next-value");

        // Act
        parameters.CommitTransition();

        // Assert
        var act = () => parameters.GetNextParameter<string>();
        Assert.Throws<InvalidOperationException>(act);
    }

    [Fact]
    public void CommitTransition_WhenCalled_ShouldClearPrevious()
    {
        // Arrange
        var parameters = new StateMachineParameters();
        parameters.SetNextParameter("first");
        parameters.CommitTransition();
        parameters.BeginTransition();
        parameters.SetNextParameter("second");

        // Act
        parameters.CommitTransition();

        // Assert
        var act = () => parameters.GetPreviousParameter<string>();
        Assert.Throws<InvalidOperationException>(act);
    }

    [Fact]
    public void CommitTransition_WhenFullWorkflowCompletes_ShouldPromoteParametersCorrectly()
    {
        // Arrange
        var parameters = new StateMachineParameters();
        parameters.SetNextParameters("state-A", 100);
        parameters.CommitTransition();
        parameters.BeginTransition();
        parameters.SetNextParameters("state-B", 200);

        // Act
        parameters.CommitTransition();

        // Assert
        var (s, i) = parameters.GetCurrentParameters<string, int>();
        Assert.Equal("state-B", s);
        Assert.Equal(200, i);
        Assert.True(parameters.Status.HasFlag(StateMachineParametersFlags.CurrentParametersSet));
        Assert.False(parameters.Status.HasFlag(StateMachineParametersFlags.NextParametersSet));
        Assert.False(parameters.Status.HasFlag(StateMachineParametersFlags.PreviousParametersSet));
    }

    [Fact]
    public void CommitTransition_WhenTypesStored_ShouldPreserveTypesInCurrent()
    {
        // Arrange
        var parameters = new StateMachineParameters();
        parameters.SetNextParameters("value", 42, true);

        // Act
        parameters.CommitTransition();

        // Assert
        var types = parameters.CurrentParameterTypes;
        Assert.Equal(3, types.Count);
        Assert.Equal(typeof(string), types[0]);
        Assert.Equal(typeof(int), types[1]);
        Assert.Equal(typeof(bool), types[2]);
    }

    [Fact]
    public void RollbackTransition_WhenCalled_ShouldRestorePreviousToCurrent()
    {
        // Arrange
        var parameters = new StateMachineParameters();
        parameters.SetNextParameter("original");
        parameters.CommitTransition();
        parameters.BeginTransition();

        // Act
        parameters.RollbackTransition();

        // Assert
        var result = parameters.GetCurrentParameter<string>();
        Assert.Equal("original", result);
    }

    [Fact]
    public void RollbackTransition_WhenCalled_ShouldClearPrevious()
    {
        // Arrange
        var parameters = new StateMachineParameters();
        parameters.SetNextParameter("original");
        parameters.CommitTransition();
        parameters.BeginTransition();

        // Act
        parameters.RollbackTransition();

        // Assert
        var act = () => parameters.GetPreviousParameter<string>();
        Assert.Throws<InvalidOperationException>(act);
    }

    [Fact]
    public void RollbackTransition_WhenCalled_ShouldClearNext()
    {
        // Arrange
        var parameters = new StateMachineParameters();
        parameters.SetNextParameter("original");
        parameters.CommitTransition();
        parameters.BeginTransition();
        parameters.SetNextParameter("staged");

        // Act
        parameters.RollbackTransition();

        // Assert
        var act = () => parameters.GetNextParameter<string>();
        Assert.Throws<InvalidOperationException>(act);
    }

    [Fact]
    public void RollbackTransition_WhenCalled_ShouldClearNextParametersSetFlag()
    {
        // Arrange
        var parameters = new StateMachineParameters();
        parameters.SetNextParameter("original");
        parameters.CommitTransition();
        parameters.BeginTransition();
        parameters.SetNextParameter("staged");

        // Act
        parameters.RollbackTransition();

        // Assert
        Assert.False(parameters.Status.HasFlag(StateMachineParametersFlags.NextParametersSet));
    }

    [Fact]
    public void RollbackTransition_WhenFullWorkflowRollsBack_ShouldRestoreOriginalState()
    {
        // Arrange
        var parameters = new StateMachineParameters();
        parameters.SetNextParameter("original");
        parameters.CommitTransition();
        parameters.BeginTransition();
        parameters.SetNextParameter("new-value");

        // Act
        parameters.RollbackTransition();

        // Assert
        Assert.Equal("original", parameters.GetCurrentParameter<string>());
        Assert.True(parameters.Status.HasFlag(StateMachineParametersFlags.CurrentParametersSet));
        Assert.False(parameters.Status.HasFlag(StateMachineParametersFlags.PreviousParametersSet));
        Assert.False(parameters.Status.HasFlag(StateMachineParametersFlags.NextParametersSet));
    }

    [Fact]
    public void Status_WhenInitialized_ShouldBeNone()
    {
        // Arrange
        var parameters = new StateMachineParameters();

        // Act
        var result = parameters.Status;

        // Assert
        Assert.Equal(StateMachineParametersFlags.None, result);
    }

    [Fact]
    public void Status_AfterSetNextParameter_ShouldHaveNextParametersSet()
    {
        // Arrange
        var parameters = new StateMachineParameters();

        // Act
        parameters.SetNextParameter("value");

        // Assert
        Assert.True(parameters.Status.HasFlag(StateMachineParametersFlags.NextParametersSet));
    }

    [Fact]
    public void Status_AfterBeginTransition_ShouldHavePreviousParametersSet()
    {
        // Arrange
        var parameters = new StateMachineParameters();
        parameters.SetNextParameter("value");
        parameters.CommitTransition();

        // Act
        parameters.BeginTransition();

        // Assert
        Assert.True(parameters.Status.HasFlag(StateMachineParametersFlags.PreviousParametersSet));
    }

    [Fact]
    public void Status_AfterBeginTransition_ShouldNotHaveCurrentParametersSet()
    {
        // Arrange
        var parameters = new StateMachineParameters();
        parameters.SetNextParameter("value");
        parameters.CommitTransition();

        // Act
        parameters.BeginTransition();

        // Assert
        Assert.False(parameters.Status.HasFlag(StateMachineParametersFlags.CurrentParametersSet));
    }

    [Fact]
    public void Status_AfterCommitTransition_ShouldHaveCurrentParametersSet()
    {
        // Arrange
        var parameters = new StateMachineParameters();
        parameters.SetNextParameter("value");

        // Act
        parameters.CommitTransition();

        // Assert
        Assert.True(parameters.Status.HasFlag(StateMachineParametersFlags.CurrentParametersSet));
    }

    [Fact]
    public void Status_AfterCommitTransition_ShouldNotHaveNextParametersSet()
    {
        // Arrange
        var parameters = new StateMachineParameters();
        parameters.SetNextParameter("value");

        // Act
        parameters.CommitTransition();

        // Assert
        Assert.False(parameters.Status.HasFlag(StateMachineParametersFlags.NextParametersSet));
    }

    [Fact]
    public void Status_AfterRollbackTransition_ShouldHaveCurrentParametersSet()
    {
        // Arrange
        var parameters = new StateMachineParameters();
        parameters.SetNextParameter("value");
        parameters.CommitTransition();
        parameters.BeginTransition();

        // Act
        parameters.RollbackTransition();

        // Assert
        Assert.True(parameters.Status.HasFlag(StateMachineParametersFlags.CurrentParametersSet));
    }

    [Fact]
    public void Status_AfterRollbackTransition_ShouldNotHavePreviousParametersSet()
    {
        // Arrange
        var parameters = new StateMachineParameters();
        parameters.SetNextParameter("value");
        parameters.CommitTransition();
        parameters.BeginTransition();

        // Act
        parameters.RollbackTransition();

        // Assert
        Assert.False(parameters.Status.HasFlag(StateMachineParametersFlags.PreviousParametersSet));
    }

    [Fact]
    public void Status_AfterClear_ShouldBeNone()
    {
        // Arrange
        var parameters = new StateMachineParameters();
        parameters.SetNextParameter("value");
        parameters.CommitTransition();
        parameters.BeginTransition();

        // Act
        parameters.Clear();

        // Assert
        Assert.Equal(StateMachineParametersFlags.None, parameters.Status);
    }

    [Fact]
    public void Clear_WhenCalled_ShouldClearPreviousSlot()
    {
        // Arrange
        var parameters = new StateMachineParameters();
        parameters.SetNextParameter("first");
        parameters.CommitTransition();
        parameters.BeginTransition();

        // Act
        parameters.Clear();

        // Assert
        var act = () => parameters.GetPreviousParameter<string>();
        Assert.Throws<InvalidOperationException>(act);
    }

    [Fact]
    public void Clear_WhenCalled_ShouldClearCurrentSlot()
    {
        // Arrange
        var parameters = new StateMachineParameters();
        parameters.SetNextParameter("value");
        parameters.CommitTransition();

        // Act
        parameters.Clear();

        // Assert
        var act = () => parameters.GetCurrentParameter<string>();
        Assert.Throws<InvalidOperationException>(act);
    }

    [Fact]
    public void Clear_WhenCalled_ShouldClearNextSlot()
    {
        // Arrange
        var parameters = new StateMachineParameters();
        parameters.SetNextParameter("value");

        // Act
        parameters.Clear();

        // Assert
        var act = () => parameters.GetNextParameter<string>();
        Assert.Throws<InvalidOperationException>(act);
    }

    [Fact]
    public void PreviousParameterTypes_WhenNoParametersSet_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var parameters = new StateMachineParameters();

        // Act
        var act = () => parameters.PreviousParameterTypes;

        // Assert
        Assert.Throws<InvalidOperationException>(act);
    }

    [Fact]
    public void PreviousParameterTypes_WhenParametersSet_ShouldBeAccessible()
    {
        // Arrange
        var parameters = new StateMachineParameters();
        parameters.SetNextParameter("value");
        parameters.CommitTransition();
        parameters.BeginTransition();

        // Act
        var result = parameters.PreviousParameterTypes;

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public void CurrentParameterTypes_WhenNoParametersSet_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var parameters = new StateMachineParameters();

        // Act
        var act = () => parameters.CurrentParameterTypes;

        // Assert
        Assert.Throws<InvalidOperationException>(act);
    }

    [Fact]
    public void CurrentParameterTypes_WhenParametersSet_ShouldBeAccessible()
    {
        // Arrange
        var parameters = new StateMachineParameters();
        parameters.SetNextParameter("value");
        parameters.CommitTransition();

        // Act
        var result = parameters.CurrentParameterTypes;

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public void NextParameterTypes_WhenNoParametersSet_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var parameters = new StateMachineParameters();

        // Act
        var act = () => parameters.NextParameterTypes;

        // Assert
        Assert.Throws<InvalidOperationException>(act);
    }

    [Fact]
    public void NextParameterTypes_WhenParametersSet_ShouldBeAccessible()
    {
        // Arrange
        var parameters = new StateMachineParameters();
        parameters.SetNextParameter("value");

        // Act
        var result = parameters.NextParameterTypes;

        // Assert
        Assert.NotNull(result);
    }
}
