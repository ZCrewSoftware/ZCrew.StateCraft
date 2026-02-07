using NSubstitute;
using ZCrew.StateCraft.Parameters.Contracts;
using ZCrew.StateCraft.UnitTests.Stubs;

namespace ZCrew.StateCraft.UnitTests;

public class TransitionTableTests
{
    [Fact]
    public async Task LookupParameterlessTransition_WhenTransitionExists_ShouldReturnTransition()
    {
        // Arrange
        var transition = new StubParameterlessTransition<string, string>("A", "GoTo B", "B");
        var transitionTable = new TransitionTable<string, string>([transition]);

        // Act
        var result = await transitionTable.LookupParameterlessTransition(
            "GoTo B",
            TestContext.Current.CancellationToken
        );

        // Assert
        Assert.Same(transition, result);
    }

    [Fact]
    public async Task LookupParameterlessTransition_WhenMultipleTransitionsExist_ShouldReturnCorrectTransition()
    {
        // Arrange
        var transitionA = new StubParameterlessTransition<string, string>("A", "T1", "B");
        var transitionB = new StubParameterlessTransition<string, string>("B", "T2", "C");
        var transitionTable = new TransitionTable<string, string>([transitionA, transitionB]);

        // Act
        var token = TestContext.Current.CancellationToken;
        var resultA = await transitionTable.LookupParameterlessTransition("T1", token);
        var resultB = await transitionTable.LookupParameterlessTransition("T2", token);

        // Assert
        Assert.Same(transitionA, resultA);
        Assert.Same(transitionB, resultB);
    }

    [Fact]
    public async Task LookupParameterlessTransition_WhenTransitionDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var transition = new StubParameterlessTransition<string, string>("A", "T1", "B");
        var transitionTable = new TransitionTable<string, string>([transition]);

        // Act
        var result = await transitionTable.LookupParameterlessTransition(
            "NonExistent",
            TestContext.Current.CancellationToken
        );

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task LookupParameterlessTransition_WhenConditionFails_ShouldReturnNull()
    {
        // Arrange
        var transition = Substitute.ForPartsOf<StubParameterlessTransition<string, string>>("A", "T1", "B");
        transition.EvaluateConditions(Arg.Any<IStateMachineParameters>(), Arg.Any<CancellationToken>()).Returns(false);
        var transitionTable = new TransitionTable<string, string>([transition]);

        // Act
        var result = await transitionTable.LookupParameterlessTransition("T1", TestContext.Current.CancellationToken);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task LookupParameterlessTransition_WhenTableIsEmpty_ShouldReturnNull()
    {
        // Arrange
        var transitionTable = new TransitionTable<string, string>([]);

        // Act
        var result = await transitionTable.LookupParameterlessTransition("T1", TestContext.Current.CancellationToken);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task LookupParameterlessTransition_TPrevious_WhenTransitionExists_ShouldReturnTransition()
    {
        // Arrange
        var transition = new StubParameterlessTransition<string, string, int>("A", "T1", "B");
        var transitionTable = new TransitionTable<string, string>([transition]);

        // Act
        var result = await transitionTable.LookupParameterlessTransition(
            "T1",
            42,
            TestContext.Current.CancellationToken
        );

        // Assert
        Assert.Same(transition, result);
    }

    [Fact]
    public async Task LookupParameterlessTransition_TPrevious_WhenTransitionDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var transition = new StubParameterlessTransition<string, string, int>("A", "T1", "B");
        var transitionTable = new TransitionTable<string, string>([transition]);

        // Act
        var result = await transitionTable.LookupParameterlessTransition(
            "NonExistent",
            42,
            TestContext.Current.CancellationToken
        );

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task LookupParameterlessTransition_TPrevious_WhenWrongParameterType_ShouldReturnNull()
    {
        // Arrange
        var transition = new StubParameterlessTransition<string, string, int>("A", "T1", "B");
        var transitionTable = new TransitionTable<string, string>([transition]);

        // Act
        var result = await transitionTable.LookupParameterlessTransition(
            "T1",
            "wrong type",
            TestContext.Current.CancellationToken
        );

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task LookupParameterlessTransition_TPrevious_WhenConditionFails_ShouldReturnNull()
    {
        // Arrange
        var transition = Substitute.ForPartsOf<StubParameterlessTransition<string, string, int>>("A", "T1", "B");
        transition.EvaluateConditions(Arg.Any<IStateMachineParameters>(), Arg.Any<CancellationToken>()).Returns(false);
        var transitionTable = new TransitionTable<string, string>([transition]);

        // Act
        var result = await transitionTable.LookupParameterlessTransition(
            "T1",
            42,
            TestContext.Current.CancellationToken
        );

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task LookupParameterizedTransition_TNext_WhenTransitionExists_ShouldReturnTransition()
    {
        // Arrange
        var transition = new StubParameterizedTransition<string, string, int>("A", "T1", "B");
        var transitionTable = new TransitionTable<string, string>([transition]);

        // Act
        var result = await transitionTable.LookupParameterizedTransition(
            "T1",
            42,
            TestContext.Current.CancellationToken
        );

        // Assert
        Assert.Same(transition, result);
    }

    [Fact]
    public async Task LookupParameterizedTransition_TNext_WhenTransitionDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var transition = new StubParameterizedTransition<string, string, int>("A", "T1", "B");
        var transitionTable = new TransitionTable<string, string>([transition]);

        // Act
        var result = await transitionTable.LookupParameterizedTransition(
            "NonExistent",
            42,
            TestContext.Current.CancellationToken
        );

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task LookupParameterizedTransition_TNext_WhenWrongParameterType_ShouldReturnNull()
    {
        // Arrange
        var transition = new StubParameterizedTransition<string, string, int>("A", "T1", "B");
        var transitionTable = new TransitionTable<string, string>([transition]);

        // Act
        var result = await transitionTable.LookupParameterizedTransition(
            "T1",
            "wrong type",
            TestContext.Current.CancellationToken
        );

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task LookupParameterizedTransition_TNext_WhenConditionFails_ShouldReturnNull()
    {
        // Arrange
        var transition = Substitute.ForPartsOf<StubParameterizedTransition<string, string, int>>("A", "T1", "B");
        transition.EvaluateConditions(Arg.Any<IStateMachineParameters>(), Arg.Any<CancellationToken>()).Returns(false);
        var transitionTable = new TransitionTable<string, string>([transition]);

        // Act
        var result = await transitionTable.LookupParameterizedTransition(
            "T1",
            42,
            TestContext.Current.CancellationToken
        );

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task LookupParameterizedTransition_TPrevious_TNext_WhenTransitionExists_ShouldReturnTransition()
    {
        // Arrange
        var transition = new StubParameterizedTransition<string, string, int, string>("A", "T1", "B");
        var transitionTable = new TransitionTable<string, string>([transition]);

        // Act
        var result = await transitionTable.LookupParameterizedTransition(
            "T1",
            42,
            "next",
            TestContext.Current.CancellationToken
        );

        // Assert
        Assert.Same(transition, result);
    }

    [Fact]
    public async Task LookupParameterizedTransition_TPrevious_TNext_WhenTransitionDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var transition = new StubParameterizedTransition<string, string, int, string>("A", "T1", "B");
        var transitionTable = new TransitionTable<string, string>([transition]);

        // Act
        var result = await transitionTable.LookupParameterizedTransition(
            "NonExistent",
            42,
            "next",
            TestContext.Current.CancellationToken
        );

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task LookupParameterizedTransition_TPrevious_TNext_WhenWrongParameterType_ShouldReturnNull()
    {
        // Arrange
        var transition = new StubParameterizedTransition<string, string, int, string>("A", "T1", "B");
        var transitionTable = new TransitionTable<string, string>([transition]);

        // Act
        var result = await transitionTable.LookupParameterizedTransition(
            "T1",
            "wrong type",
            123,
            TestContext.Current.CancellationToken
        );

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task LookupParameterizedTransition_TPrevious_TNext_WhenConditionFails_ShouldReturnNull()
    {
        // Arrange
        var transition = Substitute.ForPartsOf<StubParameterizedTransition<string, string, int, string>>(
            "A",
            "T1",
            "B"
        );
        transition.EvaluateConditions(Arg.Any<IStateMachineParameters>(), Arg.Any<CancellationToken>()).Returns(false);
        var transitionTable = new TransitionTable<string, string>([transition]);

        // Act
        var result = await transitionTable.LookupParameterizedTransition(
            "T1",
            42,
            "next",
            TestContext.Current.CancellationToken
        );

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task LookupParameterlessTransition_TPrevious_WhenAssignableType_ShouldReturnTransition()
    {
        // Arrange
        var transition = new StubParameterlessTransition<string, string, object>("A", "T1", "B");
        var transitionTable = new TransitionTable<string, string>([transition]);

        // Act
        var result = await transitionTable.LookupParameterlessTransition(
            "T1",
            "a string value",
            TestContext.Current.CancellationToken
        );

        // Assert
        Assert.Same(transition, result);
    }

    [Fact]
    public async Task LookupParameterizedTransition_TNext_WhenAssignableType_ShouldReturnTransition()
    {
        // Arrange
        var transition = new StubParameterizedTransition<string, string, object>("A", "T1", "B");
        var transitionTable = new TransitionTable<string, string>([transition]);

        // Act
        var result = await transitionTable.LookupParameterizedTransition(
            "T1",
            "a string value",
            TestContext.Current.CancellationToken
        );

        // Assert
        Assert.Same(transition, result);
    }

    [Fact]
    public async Task LookupParameterizedTransition_TPrevious_TNext_WhenAssignablePreviousType_ShouldReturnTransition()
    {
        // Arrange
        var transition = new StubParameterizedTransition<string, string, object, int>("A", "T1", "B");
        var transitionTable = new TransitionTable<string, string>([transition]);

        // Act
        var result = await transitionTable.LookupParameterizedTransition(
            "T1",
            "a string value",
            42,
            TestContext.Current.CancellationToken
        );

        // Assert
        Assert.Same(transition, result);
    }

    [Fact]
    public async Task LookupParameterizedTransition_TPrevious_TNext_WhenAssignableNextType_ShouldReturnTransition()
    {
        // Arrange
        var transition = new StubParameterizedTransition<string, string, int, object>("A", "T1", "B");
        var transitionTable = new TransitionTable<string, string>([transition]);

        // Act
        var result = await transitionTable.LookupParameterizedTransition(
            "T1",
            42,
            "a string value",
            TestContext.Current.CancellationToken
        );

        // Assert
        Assert.Same(transition, result);
    }

    [Fact]
    public async Task LookupParameterizedTransition_TPrevious_TNext_WhenBothAssignableTypes_ShouldReturnTransition()
    {
        // Arrange
        var transition = new StubParameterizedTransition<string, string, object, object>("A", "T1", "B");
        var transitionTable = new TransitionTable<string, string>([transition]);

        // Act
        var result = await transitionTable.LookupParameterizedTransition(
            "T1",
            "previous string",
            "next string",
            TestContext.Current.CancellationToken
        );

        // Assert
        Assert.Same(transition, result);
    }
}
