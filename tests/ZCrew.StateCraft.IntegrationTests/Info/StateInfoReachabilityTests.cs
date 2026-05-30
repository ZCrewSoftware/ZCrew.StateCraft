using ZCrew.StateCraft.Info.Extensions;

namespace ZCrew.StateCraft.IntegrationTests.Info;

public class StateInfoReachabilityTests
{
    [Fact]
    public void ReachableStates_WhenLinearChain_ShouldReturnAllDownstreamStates()
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
        var reachable = info.GetState("A").ReachableStates().ToList();

        // Assert
        Assert.Equal(["B", "C"], reachable.Select(state => state.StateValue));
    }

    [Fact]
    public void ReachableStates_WhenNoReturnPath_ShouldNotContainSelf()
    {
        // Arrange
        var info = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition("To B", "B"))
            .WithState("B", state => state)
            .GetInfo();

        // Act
        var reachable = info.GetState("A").ReachableStates().ToList();

        // Assert
        Assert.DoesNotContain(reachable, state => state.StateValue == "A");
    }

    [Fact]
    public void ReachableStates_WhenMultiStateCycle_ShouldContainSelf()
    {
        // Arrange
        var info = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition("To B", "B"))
            .WithState("B", state => state.WithTransition("To A", "A"))
            .GetInfo();

        // Act
        var reachable = info.GetState("A").ReachableStates().ToList();

        // Assert
        Assert.Contains(reachable, state => state.StateValue == "A");
        Assert.Contains(reachable, state => state.StateValue == "B");
    }

    [Fact]
    public void CanReach_WhenTargetDownstream_ShouldBeTrue()
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
        var canReach = info.GetState("A").CanReach(info.GetState("C"));

        // Assert
        Assert.True(canReach);
    }

    [Fact]
    public void CanReach_WhenTargetUpstream_ShouldBeFalse()
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
        var canReach = info.GetState("C").CanReach(info.GetState("A"));

        // Assert
        Assert.False(canReach);
    }

    [Fact]
    public void CanReach_WhenSelfWithoutReturnPath_ShouldBeFalse()
    {
        // Arrange
        var info = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition("To B", "B"))
            .WithState("B", state => state)
            .GetInfo();

        // Act
        var canReach = info.GetState("A").CanReach(info.GetState("A"));

        // Assert
        Assert.False(canReach);
    }

    [Fact]
    public void FindPath_WhenLinearChain_ShouldReturnOrderedSteps()
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
        var path = info.GetState("A").FindPathTo(info.GetState("C"));

        // Assert
        Assert.NotNull(path);
        Assert.Equal(["To B", "To C"], path.Steps.Select(step => step.Transition.TransitionValue));
        Assert.Equal(["B", "C"], path.Steps.Select(step => step.NextState.StateValue));
    }

    [Fact]
    public void FindPath_WhenMultipleRoutes_ShouldReturnShortest()
    {
        // Arrange
        var info = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition("To B", "B").WithTransition("To D", "D"))
            .WithState("B", state => state.WithTransition("B to D", "D"))
            .WithState("D", state => state)
            .GetInfo();

        // Act
        var path = info.GetState("A").FindPathTo(info.GetState("D"));

        // Assert
        Assert.NotNull(path);
        var step = Assert.Single(path.Steps);
        Assert.Equal("To D", step.Transition.TransitionValue);
        Assert.Equal("D", step.NextState.StateValue);
    }

    [Fact]
    public void FindPath_WhenUnreachable_ShouldThrowUnreachableStateException()
    {
        // Arrange
        var info = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition("To B", "B"))
            .WithState("B", state => state)
            .GetInfo();

        // Act
        var findPath = () => info.GetState("B").FindPathTo(info.GetState("A"));

        // Assert
        Assert.Throws<UnreachableStateException>(findPath);
    }

    [Fact]
    public void FindPath_WhenMultiStateCycleBackToSelf_ShouldReturnCyclePath()
    {
        // Arrange
        var info = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition("To B", "B"))
            .WithState("B", state => state.WithTransition("To A", "A"))
            .GetInfo();

        // Act
        var path = info.GetState("A").FindPathTo(info.GetState("A"));

        // Assert
        Assert.NotNull(path);
        Assert.Equal(["To B", "To A"], path.Steps.Select(step => step.Transition.TransitionValue));
        Assert.Equal(["B", "A"], path.Steps.Select(step => step.NextState.StateValue));
    }

    [Fact]
    public void FindPath_WhenDirectSelfLoop_ShouldReturnSingleStep()
    {
        // Arrange
        var info = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition("Loop", "A"))
            .GetInfo();

        // Act
        var path = info.GetState("A").FindPathTo(info.GetState("A"));

        // Assert
        Assert.NotNull(path);
        var step = Assert.Single(path.Steps);
        Assert.Equal("Loop", step.Transition.TransitionValue);
        Assert.Equal("A", step.NextState.StateValue);
    }

    [Fact]
    public void FindPath_T_WhenReentrantSelfLoop_ShouldReturnSingleStep()
    {
        // Arrange
        var info = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42)
            .WithState(
                "A",
                state => state.WithParameter<int>().WithTransition("Loop", t => t.WithSameParameter().To("A"))
            )
            .GetInfo();

        // Act
        var path = info.GetState("A", typeof(int)).FindPathTo(info.GetState("A", typeof(int)));

        // Assert
        Assert.NotNull(path);
        var step = Assert.Single(path.Steps);
        Assert.Equal("Loop", step.Transition.TransitionValue);
        Assert.Equal("A", step.NextState.StateValue);
    }

    [Fact]
    public void TryFindPath_WhenReachable_ShouldReturnTrueWithPath()
    {
        // Arrange
        var info = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition("To B", "B"))
            .WithState("B", state => state)
            .GetInfo();

        // Act
        var found = info.GetState("A").TryFindPathTo(info.GetState("B"), out var path);

        // Assert
        Assert.True(found);
        Assert.NotNull(path);
        var step = Assert.Single(path.Steps);
        Assert.Equal("B", step.NextState.StateValue);
    }

    [Fact]
    public void TryFindPath_WhenUnreachable_ShouldReturnFalseWithNullPath()
    {
        // Arrange
        var info = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition("To B", "B"))
            .WithState("B", state => state)
            .GetInfo();

        // Act
        var found = info.GetState("B").TryFindPathTo(info.GetState("A"), out var path);

        // Assert
        Assert.False(found);
        Assert.Null(path);
    }

    [Fact]
    public void FindPath_WhenTargetNull_ShouldThrow()
    {
        // Arrange
        var info = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state)
            .GetInfo();
        var stateInfo = info.GetState("A");

        // Act
        var findPath = () => stateInfo.FindPathTo(null!);

        // Assert
        Assert.Throws<ArgumentNullException>(findPath);
    }
}
