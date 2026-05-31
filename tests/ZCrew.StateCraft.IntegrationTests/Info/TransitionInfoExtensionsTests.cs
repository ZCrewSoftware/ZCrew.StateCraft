using ZCrew.StateCraft.Info.Extensions;

namespace ZCrew.StateCraft.IntegrationTests.Info;

public class TransitionInfoExtensionsTests
{
    [Fact]
    public void GetPreviousStates_WhenDirectTransition_ShouldReturnSingleSourceState()
    {
        // Arrange
        var info = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition("To B", "B"))
            .WithState("B", state => state)
            .GetInfo();
        var transition = Assert.Single(info.Transitions);

        // Act
        var previousStates = transition.GetPreviousStates().ToList();

        // Assert
        var previous = Assert.Single(previousStates);
        Assert.Equal("A", previous.StateValue);
    }

    [Fact]
    public void GetPreviousStates_WhenMappedTransition_ShouldReturnSingleSourceState()
    {
        // Arrange
        var info = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 1)
            .WithState(
                "A",
                state =>
                    state
                        .WithParameter<int>()
                        .WithTransition("To B", t => t.WithMappedParameter<string>(value => value.ToString()).To("B"))
            )
            .WithState("B", state => state.WithParameter<string>())
            .GetInfo();
        var transition = Assert.Single(info.Transitions);

        // Act
        var previousStates = transition.GetPreviousStates().ToList();

        // Assert
        var previous = Assert.Single(previousStates);
        Assert.Equal("A", previous.StateValue);
    }

    [Fact]
    public void GetPreviousStates_WhenFromAllOtherStates_ShouldReturnEveryStateExceptDestination()
    {
        // Arrange
        var info = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state)
            .WithState("B", state => state)
            .WithState("C", state => state)
            .WithState("D", state => state.WithTransition("To D", t => t.From().AllOtherStates()))
            .GetInfo();
        var transition = Assert.Single(info.Transitions);

        // Act
        var previousStates = transition.GetPreviousStates().Select(state => state.StateValue).ToList();

        // Assert
        Assert.Equal(3, previousStates.Count);
        Assert.Contains("A", previousStates);
        Assert.Contains("B", previousStates);
        Assert.Contains("C", previousStates);
        Assert.DoesNotContain("D", previousStates);
    }

    [Fact]
    public void GetPreviousStates_WhenFromAllStates_ShouldReturnEveryState()
    {
        // Arrange
        var info = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state)
            .WithState("B", state => state)
            .WithState("D", state => state.WithTransition("To D", t => t.From().AllStates()))
            .GetInfo();
        var transition = Assert.Single(info.Transitions);

        // Act
        var previousStates = transition.GetPreviousStates().Select(state => state.StateValue).ToList();

        // Assert
        Assert.Equal(3, previousStates.Count);
        Assert.Contains("A", previousStates);
        Assert.Contains("B", previousStates);
        Assert.Contains("D", previousStates);
    }

    [Fact]
    public void GetPreviousStates_WhenFromAllOtherStatesWithExcept_ShouldOmitDestinationAndExcepted()
    {
        // Arrange
        var info = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state)
            .WithState("B", state => state)
            .WithState("C", state => state)
            .WithState("D", state => state.WithTransition("To D", t => t.From().AllOtherStates().Except("B")))
            .GetInfo();
        var transition = Assert.Single(info.Transitions);

        // Act
        var previousStates = transition.GetPreviousStates().Select(state => state.StateValue).ToList();

        // Assert
        Assert.Equal(2, previousStates.Count);
        Assert.Contains("A", previousStates);
        Assert.Contains("C", previousStates);
        Assert.DoesNotContain("B", previousStates);
        Assert.DoesNotContain("D", previousStates);
    }

    [Fact]
    public void GetNextStates_WhenDirectTransition_ShouldReturnSingleDestinationState()
    {
        // Arrange
        var info = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition("To B", "B"))
            .WithState("B", state => state)
            .GetInfo();
        var transition = Assert.Single(info.Transitions);

        // Act
        var nextStates = transition.GetNextStates().ToList();

        // Assert
        var next = Assert.Single(nextStates);
        Assert.Equal("B", next.StateValue);
    }

    [Fact]
    public void GetNextStates_WhenMappedTransition_ShouldReturnSingleDestinationState()
    {
        // Arrange
        var info = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 1)
            .WithState(
                "A",
                state =>
                    state
                        .WithParameter<int>()
                        .WithTransition("To B", t => t.WithMappedParameter<string>(value => value.ToString()).To("B"))
            )
            .WithState("B", state => state.WithParameter<string>())
            .GetInfo();
        var transition = Assert.Single(info.Transitions);

        // Act
        var nextStates = transition.GetNextStates().ToList();

        // Assert
        var next = Assert.Single(nextStates);
        Assert.Equal("B", next.StateValue);
    }

    [Fact]
    public void GetNextStates_WhenFromTransition_ShouldReturnOnlyDestinationWithoutExpanding()
    {
        // Arrange
        var info = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state)
            .WithState("B", state => state)
            .WithState("C", state => state)
            .WithState("D", state => state.WithTransition("To D", t => t.From().AllOtherStates()))
            .GetInfo();
        var transition = Assert.Single(info.Transitions);

        // Act
        var nextStates = transition.GetNextStates().ToList();

        // Assert
        var next = Assert.Single(nextStates);
        Assert.Equal("D", next.StateValue);
    }

    [Fact]
    public void IsTransitionFrom_WhenDirectSourceState_ShouldReturnTrue()
    {
        // Arrange
        var info = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition("To B", "B"))
            .WithState("B", state => state)
            .GetInfo();
        var transition = Assert.Single(info.Transitions);

        // Act
        var isFromSource = transition.IsTransitionFrom(info.GetState("A"));

        // Assert
        Assert.True(isFromSource);
    }

    [Fact]
    public void IsTransitionFrom_WhenUnrelatedState_ShouldReturnFalse()
    {
        // Arrange
        var info = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition("To B", "B"))
            .WithState("B", state => state)
            .GetInfo();
        var transition = Assert.Single(info.Transitions);

        // Act
        var isFromDestination = transition.IsTransitionFrom(info.GetState("B"));

        // Assert
        Assert.False(isFromDestination);
    }

    [Fact]
    public void IsTransitionFrom_WhenFromIncludedSource_ShouldReturnTrue()
    {
        // Arrange
        var info = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state)
            .WithState("B", state => state)
            .WithState("D", state => state.WithTransition("To D", t => t.From().AllOtherStates()))
            .GetInfo();
        var transition = Assert.Single(info.Transitions);

        // Act
        var isFromIncluded = transition.IsTransitionFrom(info.GetState("A"));

        // Assert
        Assert.True(isFromIncluded);
    }

    [Fact]
    public void IsTransitionFrom_WhenFromExcludedSource_ShouldReturnFalse()
    {
        // Arrange
        var info = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state)
            .WithState("B", state => state)
            .WithState("D", state => state.WithTransition("To D", t => t.From().AllOtherStates()))
            .GetInfo();
        var transition = Assert.Single(info.Transitions);

        // Act
        var isFromExcluded = transition.IsTransitionFrom(info.GetState("D"));

        // Assert
        Assert.False(isFromExcluded);
    }

    [Fact]
    public void IsTransitionFrom_WhenStateFromSeparateEqualMachine_ShouldMatchByIdentity()
    {
        // Arrange
        static IStateMachineInfo<string, string> BuildInfo() =>
            StateMachine
                .Configure<string, string>()
                .WithInitialState("A")
                .WithState("A", state => state.WithTransition("To B", "B"))
                .WithState("B", state => state)
                .GetInfo();
        var transition = Assert.Single(BuildInfo().Transitions);
        var sourceFromOtherMachine = BuildInfo().GetState("A");

        // Act
        var isFromSource = transition.IsTransitionFrom(sourceFromOtherMachine);

        // Assert
        Assert.True(isFromSource);
    }

    [Fact]
    public void IsTransitionFrom_WhenMatchingValueAndParameterTypes_ShouldReturnTrue()
    {
        // Arrange
        var info = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 1)
            .WithState("A", state => state.WithParameter<int>().WithTransition("To B", "B"))
            .WithState("B", state => state)
            .GetInfo();
        var transition = Assert.Single(info.Transitions);

        // Act
        var isFrom = transition.IsTransitionFrom("A", [typeof(int)]);

        // Assert
        Assert.True(isFrom);
    }

    [Fact]
    public void IsTransitionFrom_WhenMatchingValueButWrongParameterTypes_ShouldReturnFalse()
    {
        // Arrange
        var info = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 1)
            .WithState("A", state => state.WithParameter<int>().WithTransition("To B", "B"))
            .WithState("B", state => state)
            .GetInfo();
        var transition = Assert.Single(info.Transitions);

        // Act
        var isFrom = transition.IsTransitionFrom("A", [typeof(string)]);

        // Assert
        Assert.False(isFrom);
    }

    [Fact]
    public void IsTransitionFrom_WhenWrongValue_ShouldReturnFalse()
    {
        // Arrange
        var info = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition("To B", "B"))
            .WithState("B", state => state)
            .GetInfo();
        var transition = Assert.Single(info.Transitions);

        // Act
        var isFrom = transition.IsTransitionFrom("B", []);

        // Assert
        Assert.False(isFrom);
    }

    [Fact]
    public void IsTransitionFrom_WhenParameterlessSourceAndEmptyTypes_ShouldReturnTrue()
    {
        // Arrange
        var info = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition("To B", "B"))
            .WithState("B", state => state)
            .GetInfo();
        var transition = Assert.Single(info.Transitions);

        // Act
        var isFrom = transition.IsTransitionFrom("A", []);

        // Assert
        Assert.True(isFrom);
    }

    [Fact]
    public void IsTransitionTo_WhenDirectDestinationState_ShouldReturnTrue()
    {
        // Arrange
        var info = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition("To B", "B"))
            .WithState("B", state => state)
            .GetInfo();
        var transition = Assert.Single(info.Transitions);

        // Act
        var isToDestination = transition.IsTransitionTo(info.GetState("B"));

        // Assert
        Assert.True(isToDestination);
    }

    [Fact]
    public void IsTransitionTo_WhenUnrelatedState_ShouldReturnFalse()
    {
        // Arrange
        var info = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition("To B", "B"))
            .WithState("B", state => state)
            .GetInfo();
        var transition = Assert.Single(info.Transitions);

        // Act
        var isToSource = transition.IsTransitionTo(info.GetState("A"));

        // Assert
        Assert.False(isToSource);
    }

    [Fact]
    public void IsTransitionTo_WhenFromDestinationState_ShouldReturnTrue()
    {
        // Arrange
        var info = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state)
            .WithState("B", state => state)
            .WithState("D", state => state.WithTransition("To D", t => t.From().AllOtherStates()))
            .GetInfo();
        var transition = Assert.Single(info.Transitions);

        // Act
        var isToDestination = transition.IsTransitionTo(info.GetState("D"));

        // Assert
        Assert.True(isToDestination);
    }

    [Fact]
    public void IsTransitionTo_WhenMatchingDestinationValueAndParameterTypes_ShouldReturnTrue()
    {
        // Arrange
        var info = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 1)
            .WithState(
                "A",
                state =>
                    state
                        .WithParameter<int>()
                        .WithTransition("To B", t => t.WithMappedParameter<string>(value => value.ToString()).To("B"))
            )
            .WithState("B", state => state.WithParameter<string>())
            .GetInfo();
        var transition = Assert.Single(info.Transitions);

        // Act
        var isTo = transition.IsTransitionTo("B", [typeof(string)]);

        // Assert
        Assert.True(isTo);
    }

    [Fact]
    public void IsTransitionTo_WhenWrongDestinationValue_ShouldReturnFalse()
    {
        // Arrange
        var info = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition("To B", "B"))
            .WithState("B", state => state)
            .GetInfo();
        var transition = Assert.Single(info.Transitions);

        // Act
        var isTo = transition.IsTransitionTo("A", []);

        // Assert
        Assert.False(isTo);
    }

    [Fact]
    public void GetPreviousStates_WhenTransitionIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        ITransitionInfo<string, string> transition = null!;

        // Act
        var getPreviousStates = () => transition.GetPreviousStates().ToList();

        // Assert
        Assert.Throws<ArgumentNullException>(getPreviousStates);
    }

    [Fact]
    public void GetNextStates_WhenTransitionIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        ITransitionInfo<string, string> transition = null!;

        // Act
        var getNextStates = () => transition.GetNextStates().ToList();

        // Assert
        Assert.Throws<ArgumentNullException>(getNextStates);
    }
}
