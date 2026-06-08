using NSubstitute;
using ZCrew.StateCraft.InitialState;
using ZCrew.StateCraft.UnitTests.Stubs;

namespace ZCrew.StateCraft.UnitTests.InitialState;

public class StaticInitialStateProviderTests
{
    [Fact]
    public async Task Activate_WhenValueConstructor_ShouldReturnStateFromTable()
    {
        // Arrange
        var expectedState = Substitute.ForPartsOf<StubState<string, string>>("State");
        var stateMachine = Substitute.ForPartsOf<StubStateMachine<string, string>>(expectedState);
        var activator = new StaticInitialStateProvider<string, string>("State");

        // Act
        await activator.Activate(stateMachine, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(expectedState, stateMachine.NextState);
        Assert.True(stateMachine.Parameters.IsNextSet);
    }

    [Fact]
    public async Task Activate_T_WhenValueConstructor_ShouldReturnParameterizedStateFromTable()
    {
        // Arrange
        var expectedState = Substitute.ForPartsOf<StubState<string, string>>("State", new[] { typeof(int) });
        var stateMachine = Substitute.ForPartsOf<StubStateMachine<string, string>>(expectedState);
        var activator = new StaticInitialStateProvider<string, string, int>("State", 42);

        // Act
        await activator.Activate(stateMachine, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(expectedState, stateMachine.NextState);
        Assert.Equal(42, stateMachine.Parameters.GetNextParameter<int>());
    }

    [Fact]
    public async Task Activate_T1_T2_WhenValueConstructor_ShouldReturnParameterizedStateFromTable()
    {
        // Arrange
        var expectedState = Substitute.ForPartsOf<StubState<string, string, int, string>>("State");
        var stateMachine = Substitute.ForPartsOf<StubStateMachine<string, string>>(expectedState);
        var activator = new StaticInitialStateProvider<string, string, int, string>("State", 42, "hello");

        // Act
        await activator.Activate(stateMachine, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(expectedState, stateMachine.NextState);
        var (p1, p2) = stateMachine.Parameters.GetNextParameters<int, string>();
        Assert.Equal(42, p1);
        Assert.Equal("hello", p2);
    }

    [Fact]
    public async Task Activate_T1_T2_T3_WhenValueConstructor_ShouldReturnParameterizedStateFromTable()
    {
        // Arrange
        var expectedState = Substitute.ForPartsOf<StubState<string, string, int, string, double>>("State");
        var stateMachine = Substitute.ForPartsOf<StubStateMachine<string, string>>(expectedState);
        var activator = new StaticInitialStateProvider<string, string, int, string, double>("State", 42, "hello", 3.14);

        // Act
        await activator.Activate(stateMachine, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(expectedState, stateMachine.NextState);
        var (p1, p2, p3) = stateMachine.Parameters.GetNextParameters<int, string, double>();
        Assert.Equal(42, p1);
        Assert.Equal("hello", p2);
        Assert.Equal(3.14, p3);
    }

    [Fact]
    public async Task Activate_T1_T2_T3_T4_WhenValueConstructor_ShouldReturnParameterizedStateFromTable()
    {
        // Arrange
        var expectedState = Substitute.ForPartsOf<StubState<string, string, int, string, double, bool>>("State");
        var stateMachine = Substitute.ForPartsOf<StubStateMachine<string, string>>(expectedState);
        var activator = new StaticInitialStateProvider<string, string, int, string, double, bool>(
            "State",
            42,
            "hello",
            3.14,
            true
        );

        // Act
        await activator.Activate(stateMachine, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(expectedState, stateMachine.NextState);
        var (p1, p2, p3, p4) = stateMachine.Parameters.GetNextParameters<int, string, double, bool>();
        Assert.Equal(42, p1);
        Assert.Equal("hello", p2);
        Assert.Equal(3.14, p3);
        Assert.True(p4);
    }
}
