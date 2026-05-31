using ZCrew.StateCraft.Info.Extensions;

namespace ZCrew.StateCraft.IntegrationTests.Info;

public class StateInfoExtensionsTests
{
    [Fact]
    public void GetTransitions_WhenStateHasOutgoingTransition_ShouldReturnIt()
    {
        // Arrange
        var info = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition("To B", "B"))
            .WithState("B", state => state)
            .GetInfo();

        // Act
        var transitions = info.GetState("A").GetTransitions().ToList();

        // Assert
        var transition = Assert.Single(transitions);
        Assert.Equal("To B", transition.TransitionValue);
    }

    [Fact]
    public void GetTransitions_WhenStateHasNoOutgoingTransitions_ShouldReturnEmpty()
    {
        // Arrange
        var info = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition("To B", "B"))
            .WithState("B", state => state)
            .GetInfo();

        // Act
        var transitions = info.GetState("B").GetTransitions().ToList();

        // Assert
        Assert.Empty(transitions);
    }

    [Fact]
    public void GetTransitions_WhenFromTransitionAndStateIsIncludedSource_ShouldReturnIt()
    {
        // Arrange
        var info = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state)
            .WithState("B", state => state)
            .WithState("D", state => state.WithTransition("To D", t => t.From().AllOtherStates()))
            .GetInfo();

        // Act
        var transitions = info.GetState("A").GetTransitions().ToList();

        // Assert
        var transition = Assert.Single(transitions);
        Assert.Equal("To D", transition.TransitionValue);
    }

    [Fact]
    public void GetTransitions_WhenFromTransitionAndStateIsExcludedDestination_ShouldReturnEmpty()
    {
        // Arrange
        var info = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state)
            .WithState("B", state => state)
            .WithState("D", state => state.WithTransition("To D", t => t.From().AllOtherStates()))
            .GetInfo();

        // Act
        var transitions = info.GetState("D").GetTransitions().ToList();

        // Assert
        Assert.Empty(transitions);
    }

    [Fact]
    public void GetNextStates_WhenLinearChain_ShouldReturnImmediateSuccessorOnly()
    {
        // Arrange
        var info = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition("To B", "B"))
            .WithState("B", state => state.WithTransition("To C", "C"))
            .WithState("C", state => state)
            .GetInfo();

        // Act
        var nextStates = info.GetState("A").GetNextStates().Select(state => state.StateValue).ToList();

        // Assert
        Assert.Equal(["B"], nextStates);
    }

    [Fact]
    public void GetNextStates_WhenMultipleTransitionsToDifferentStates_ShouldReturnAll()
    {
        // Arrange
        var info = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition("To B", "B").WithTransition("To C", "C"))
            .WithState("B", state => state)
            .WithState("C", state => state)
            .GetInfo();

        // Act
        var nextStates = info.GetState("A").GetNextStates().Select(state => state.StateValue).ToList();

        // Assert
        Assert.Equal(2, nextStates.Count);
        Assert.Contains("B", nextStates);
        Assert.Contains("C", nextStates);
    }

    [Fact]
    public void GetNextStates_WhenMultipleTransitionsToSameState_ShouldReturnDuplicates()
    {
        // Arrange
        var info = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition("First To B", "B").WithTransition("Second To B", "B"))
            .WithState("B", state => state)
            .GetInfo();

        // Act
        var nextStates = info.GetState("A").GetNextStates().Select(state => state.StateValue).ToList();

        // Assert
        Assert.Equal(["B", "B"], nextStates);
    }

    [Fact]
    public void IsAssignableFrom_WhenSameReference_ShouldReturnTrue()
    {
        // Arrange
        var info = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state)
            .GetInfo();
        var stateA = info.GetState("A");

        // Act
        var isAssignable = stateA.IsAssignableFrom(stateA);

        // Assert
        Assert.True(isAssignable);
    }

    [Fact]
    public void IsAssignableFrom_WhenOtherIsNull_ShouldReturnFalse()
    {
        // Arrange
        var info = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state)
            .GetInfo();
        var stateA = info.GetState("A");

        // Act
        var isAssignable = stateA.IsAssignableFrom(null);

        // Assert
        Assert.False(isAssignable);
    }

    [Fact]
    public void IsAssignableFrom_WhenDifferentInstanceSameIdentity_ShouldReturnTrue()
    {
        // Arrange
        static IStateMachineInfo<string, string> BuildInfo() =>
            StateMachine.Configure<string, string>().WithInitialState("A").WithState("A", state => state).GetInfo();
        var stateA = BuildInfo().GetState("A");
        var stateAFromOtherMachine = BuildInfo().GetState("A");

        // Act
        var isAssignable = stateA.IsAssignableFrom(stateAFromOtherMachine);

        // Assert
        Assert.True(isAssignable);
    }

    [Fact]
    public void IsAssignableFrom_WhenDifferentStateValue_ShouldReturnFalse()
    {
        // Arrange
        var info = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state)
            .WithState("B", state => state)
            .GetInfo();
        var stateA = info.GetState("A");

        // Act
        var isAssignable = stateA.IsAssignableFrom(info.GetState("B"));

        // Assert
        Assert.False(isAssignable);
    }

    [Fact]
    public void IsAssignableFrom_WhenSuppliedParameterIsMoreDerived_ShouldReturnTrue()
    {
        // Arrange
        var objectInfo = StateMachine
            .Configure<string, string>()
            .WithInitialState("Start")
            .WithState("Start", state => state)
            .WithState("X", state => state.WithParameter<object>())
            .GetInfo();
        var stringInfo = StateMachine
            .Configure<string, string>()
            .WithInitialState("Start")
            .WithState("Start", state => state)
            .WithState("X", state => state.WithParameter<string>())
            .GetInfo();

        // Act
        var isAssignable = objectInfo
            .GetState("X", typeof(object))
            .IsAssignableFrom(stringInfo.GetState("X", typeof(string)));

        // Assert
        Assert.True(isAssignable);
    }

    [Fact]
    public void IsAssignableFrom_WhenSuppliedParameterIsLessDerived_ShouldReturnFalse()
    {
        // Arrange
        var objectInfo = StateMachine
            .Configure<string, string>()
            .WithInitialState("Start")
            .WithState("Start", state => state)
            .WithState("X", state => state.WithParameter<object>())
            .GetInfo();
        var stringInfo = StateMachine
            .Configure<string, string>()
            .WithInitialState("Start")
            .WithState("Start", state => state)
            .WithState("X", state => state.WithParameter<string>())
            .GetInfo();

        // Act
        var isAssignable = stringInfo
            .GetState("X", typeof(string))
            .IsAssignableFrom(objectInfo.GetState("X", typeof(object)));

        // Assert
        Assert.False(isAssignable);
    }

    [Fact]
    public void IsAssignableFrom_WhenValueMismatch_ShouldReturnFalse()
    {
        // Arrange
        var info = StateMachine
            .Configure<string, string>()
            .WithInitialState("Start")
            .WithState("Start", state => state)
            .WithState("X", state => state.WithParameter<object>())
            .GetInfo();

        // Act
        var isAssignable = info.GetState("X", typeof(object)).IsAssignableFrom("Start", [typeof(string)]);

        // Assert
        Assert.False(isAssignable);
    }

    [Fact]
    public void IsAssignableFrom_WhenCovariantParameterType_ShouldReturnTrue()
    {
        // Arrange
        var info = StateMachine
            .Configure<string, string>()
            .WithInitialState("Start")
            .WithState("Start", state => state)
            .WithState("X", state => state.WithParameter<object>())
            .GetInfo();

        // Act
        var isAssignable = info.GetState("X", typeof(object)).IsAssignableFrom("X", [typeof(string)]);

        // Assert
        Assert.True(isAssignable);
    }

    [Fact]
    public void IsAssignableFrom_WhenContravariantParameterType_ShouldReturnFalse()
    {
        // Arrange
        var info = StateMachine
            .Configure<string, string>()
            .WithInitialState("Start")
            .WithState("Start", state => state)
            .WithState("X", state => state.WithParameter<string>())
            .GetInfo();

        // Act
        var isAssignable = info.GetState("X", typeof(string)).IsAssignableFrom("X", [typeof(object)]);

        // Assert
        Assert.False(isAssignable);
    }

    [Fact]
    public void IsAssignableFrom_WhenParameterCountMismatch_ShouldReturnFalse()
    {
        // Arrange
        var info = StateMachine
            .Configure<string, string>()
            .WithInitialState("Start")
            .WithState("Start", state => state)
            .WithState("X", state => state.WithParameter<object>())
            .GetInfo();

        // Act
        var isAssignable = info.GetState("X", typeof(object)).IsAssignableFrom("X", [typeof(string), typeof(int)]);

        // Assert
        Assert.False(isAssignable);
    }

    [Fact]
    public void IsAssignableFrom_WhenParameterlessAndEmptyTypes_ShouldReturnTrue()
    {
        // Arrange
        var info = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state)
            .GetInfo();

        // Act
        var isAssignable = info.GetState("A").IsAssignableFrom("A", []);

        // Assert
        Assert.True(isAssignable);
    }

    [Fact]
    public void Equals_WhenMatchingValueAndExactParameterTypes_ShouldReturnTrue()
    {
        // Arrange
        var info = StateMachine
            .Configure<string, string>()
            .WithInitialState("X", 1)
            .WithState("X", state => state.WithParameter<int>())
            .GetInfo();

        // Act
        var equal = info.GetState("X", typeof(int)).Equals("X", [typeof(int)]);

        // Assert
        Assert.True(equal);
    }

    [Fact]
    public void Equals_WhenValueMismatch_ShouldReturnFalse()
    {
        // Arrange
        var info = StateMachine
            .Configure<string, string>()
            .WithInitialState("X", 1)
            .WithState("X", state => state.WithParameter<int>())
            .GetInfo();

        // Act
        var equal = info.GetState("X", typeof(int)).Equals("Y", [typeof(int)]);

        // Assert
        Assert.False(equal);
    }

    [Fact]
    public void Equals_WhenAssignableButNotIdenticalParameterType_ShouldReturnFalse()
    {
        // Arrange
        var info = StateMachine
            .Configure<string, string>()
            .WithInitialState("Start")
            .WithState("Start", state => state)
            .WithState("X", state => state.WithParameter<object>())
            .GetInfo();

        // Act
        var equal = info.GetState("X", typeof(object)).Equals("X", [typeof(string)]);

        // Assert
        Assert.False(equal);
    }

    [Fact]
    public void Equals_WhenParameterTypeOrderDiffers_ShouldReturnFalse()
    {
        // Arrange
        var info = StateMachine
            .Configure<string, string>()
            .WithInitialState("M", 1, "seed")
            .WithState("M", state => state.WithParameters<int, string>())
            .GetInfo();

        // Act
        var equal = info.GetState("M", typeof(int), typeof(string)).Equals("M", [typeof(string), typeof(int)]);

        // Assert
        Assert.False(equal);
    }

    [Fact]
    public void Equals_WhenParameterlessAndEmptyTypes_ShouldReturnTrue()
    {
        // Arrange
        var info = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state)
            .GetInfo();

        // Act
        var equal = info.GetState("A").Equals("A", []);

        // Assert
        Assert.True(equal);
    }
}
