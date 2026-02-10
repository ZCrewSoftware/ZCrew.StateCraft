using NSubstitute;
using ZCrew.StateCraft.Parameters;
using ZCrew.StateCraft.Parameters.Contracts;
using ZCrew.StateCraft.UnitTests.Stubs;

namespace ZCrew.StateCraft.UnitTests;

public class TransitionTableTests
{
    [Fact]
    public async Task LookupTransition_WhenParameterlessTransitionExists_ShouldReturnTransition()
    {
        // Arrange
        var transition = new StubTransition<string, string>("A", "GoTo B", "B");
        var transitionTable = new TransitionTable<string, string>([transition]);
        var parameters = new StateMachineParameters();
        parameters.SetEmptyNextParameters();

        // Act
        var result = await transitionTable.LookupTransition(
            "GoTo B",
            parameters,
            TestContext.Current.CancellationToken
        );

        // Assert
        Assert.Same(transition, result);
    }

    [Fact]
    public async Task LookupTransition_WhenMultipleTransitionsExist_ShouldReturnCorrectTransition()
    {
        // Arrange
        var transitionA = new StubTransition<string, string>("A", "T1", "B");
        var transitionB = new StubTransition<string, string>("B", "T2", "C");
        var transitionTable = new TransitionTable<string, string>([transitionA, transitionB]);
        var parameters = new StateMachineParameters();
        parameters.SetEmptyNextParameters();

        // Act
        var token = TestContext.Current.CancellationToken;
        var resultA = await transitionTable.LookupTransition("T1", parameters, token);
        var resultB = await transitionTable.LookupTransition("T2", parameters, token);

        // Assert
        Assert.Same(transitionA, resultA);
        Assert.Same(transitionB, resultB);
    }

    [Fact]
    public async Task LookupTransition_WhenTransitionDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var transition = new StubTransition<string, string>("A", "T1", "B");
        var transitionTable = new TransitionTable<string, string>([transition]);
        var parameters = new StateMachineParameters();
        parameters.SetEmptyNextParameters();

        // Act
        var result = await transitionTable.LookupTransition(
            "NonExistent",
            parameters,
            TestContext.Current.CancellationToken
        );

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task LookupTransition_WhenConditionFails_ShouldReturnNull()
    {
        // Arrange
        var transition = Substitute.ForPartsOf<StubTransition<string, string>>("A", "T1", "B");
        transition.EvaluateConditions(Arg.Any<IStateMachineParameters>(), Arg.Any<CancellationToken>()).Returns(false);
        var transitionTable = new TransitionTable<string, string>([transition]);
        var parameters = new StateMachineParameters();
        parameters.SetEmptyNextParameters();

        // Act
        var result = await transitionTable.LookupTransition("T1", parameters, TestContext.Current.CancellationToken);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task LookupTransition_WhenTableIsEmpty_ShouldReturnNull()
    {
        // Arrange
        var transitionTable = new TransitionTable<string, string>([]);
        var parameters = new StateMachineParameters();
        parameters.SetEmptyNextParameters();

        // Act
        var result = await transitionTable.LookupTransition("T1", parameters, TestContext.Current.CancellationToken);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task LookupTransition_WhenParameterizedTransitionExists_ShouldReturnTransition()
    {
        // Arrange
        var transition = new StubTransition<string, string>("A", "T1", "B", typeof(int));
        var transitionTable = new TransitionTable<string, string>([transition]);
        var parameters = new StateMachineParameters();
        parameters.SetNextParameter(42);

        // Act
        var result = await transitionTable.LookupTransition("T1", parameters, TestContext.Current.CancellationToken);

        // Assert
        Assert.Same(transition, result);
    }

    [Fact]
    public async Task LookupTransition_WhenParameterizedTransitionDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var transition = new StubTransition<string, string>("A", "T1", "B", typeof(int));
        var transitionTable = new TransitionTable<string, string>([transition]);
        var parameters = new StateMachineParameters();
        parameters.SetNextParameter(42);

        // Act
        var result = await transitionTable.LookupTransition(
            "NonExistent",
            parameters,
            TestContext.Current.CancellationToken
        );

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task LookupTransition_WhenWrongNextParameterType_ShouldReturnNull()
    {
        // Arrange
        var transition = new StubTransition<string, string>("A", "T1", "B", typeof(int));
        var transitionTable = new TransitionTable<string, string>([transition]);
        var parameters = new StateMachineParameters();
        parameters.SetNextParameter("wrong type");

        // Act
        var result = await transitionTable.LookupTransition("T1", parameters, TestContext.Current.CancellationToken);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task LookupTransition_WhenParameterizedConditionFails_ShouldReturnNull()
    {
        // Arrange
        var transition = Substitute.ForPartsOf<StubTransition<string, string>>("A", "T1", "B", new[] { typeof(int) });
        transition.EvaluateConditions(Arg.Any<IStateMachineParameters>(), Arg.Any<CancellationToken>()).Returns(false);
        var transitionTable = new TransitionTable<string, string>([transition]);
        var parameters = new StateMachineParameters();
        parameters.SetNextParameter(42);

        // Act
        var result = await transitionTable.LookupTransition("T1", parameters, TestContext.Current.CancellationToken);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task LookupTransition_WhenNextParameterIsAssignableType_ShouldReturnTransition()
    {
        // Arrange
        var transition = new StubTransition<string, string>("A", "T1", "B", typeof(object));
        var transitionTable = new TransitionTable<string, string>([transition]);
        var parameters = new StateMachineParameters();
        parameters.SetNextParameter("a string value");

        // Act
        var result = await transitionTable.LookupTransition("T1", parameters, TestContext.Current.CancellationToken);

        // Assert
        Assert.Same(transition, result);
    }
}
