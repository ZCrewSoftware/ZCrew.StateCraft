using NSubstitute;
using ZCrew.Extensions.Tasks;
using ZCrew.StateCraft.UnitTests.Stubs;

namespace ZCrew.StateCraft.UnitTests;

public class StateMachineActivatorTests
{
    [Fact]
    public async Task Activate_WhenValueConstructor_ShouldReturnStateFromTable()
    {
        // Arrange
        var expectedState = Substitute.ForPartsOf<StubState<string, string>>("State");
        var stateMachine = Substitute.ForPartsOf<StubStateMachine<string, string>>(expectedState);
        var activator = new StateMachineActivator<string, string>("State");

        // Act
        await activator.Activate(stateMachine, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(expectedState, stateMachine.NextState);
        Assert.Empty(stateMachine.Parameters.NextParameterTypes);
    }

    [Fact]
    public async Task Activate_WhenFuncConstructor_ShouldInvokeFuncAndReturnStateFromTable()
    {
        // Arrange
        var expectedState = Substitute.ForPartsOf<StubState<string, string>>("State");
        var stateMachine = Substitute.ForPartsOf<StubStateMachine<string, string>>(expectedState);
        var func = Substitute.For<IAsyncFunc<string>>();
        func.InvokeAsync(Arg.Any<CancellationToken>()).Returns("State");

        var activator = new StateMachineActivator<string, string>(func);

        // Act
        await activator.Activate(stateMachine, TestContext.Current.CancellationToken);

        // Assert
        await func.Received(1).InvokeAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Activate_WhenFuncConstructorCalledMultipleTimes_ShouldInvokeFuncEachTime()
    {
        // Arrange
        var expectedState = Substitute.ForPartsOf<StubState<string, string>>("State");
        var stateMachine = Substitute.ForPartsOf<StubStateMachine<string, string>>(expectedState);
        var func = Substitute.For<IAsyncFunc<string>>();
        func.InvokeAsync(Arg.Any<CancellationToken>()).Returns("State");

        var activator = new StateMachineActivator<string, string>(func);

        // Act
        await activator.Activate(stateMachine, TestContext.Current.CancellationToken);
        await activator.Activate(stateMachine, TestContext.Current.CancellationToken);

        // Assert
        await func.Received(2).InvokeAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Activate_T_WhenValueConstructor_ShouldReturnParameterizedStateFromTable()
    {
        // Arrange
        var expectedState = Substitute.ForPartsOf<StubState<string, string>>("State", new[] { typeof(int) });
        var stateMachine = Substitute.ForPartsOf<StubStateMachine<string, string>>(expectedState);
        var activator = new StateMachineActivator<string, string, int>("State", 42);

        // Act
        await activator.Activate(stateMachine, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(expectedState, stateMachine.NextState);
        Assert.Equal(42, stateMachine.Parameters.GetNextParameter<int>());
    }

    [Fact]
    public async Task Activate_T_WhenFuncConstructor_ShouldInvokeFuncAndReturnStateFromTable()
    {
        // Arrange
        var expectedState = Substitute.ForPartsOf<StubState<string, string>>("State", new[] { typeof(int) });
        var stateMachine = Substitute.ForPartsOf<StubStateMachine<string, string>>(expectedState);
        var func = Substitute.For<IAsyncFunc<(string, int)>>();
        func.InvokeAsync(Arg.Any<CancellationToken>()).Returns(("State", 42));

        var activator = new StateMachineActivator<string, string, int>(func);

        // Act
        await activator.Activate(stateMachine, TestContext.Current.CancellationToken);

        // Assert
        await func.Received(1).InvokeAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Activate_T_WhenFuncConstructorCalledMultipleTimes_ShouldInvokeFuncEachTime()
    {
        // Arrange
        var expectedState = Substitute.ForPartsOf<StubState<string, string>>("State", new[] { typeof(int) });
        var stateMachine = Substitute.ForPartsOf<StubStateMachine<string, string>>(expectedState);
        var func = Substitute.For<IAsyncFunc<(string, int)>>();
        func.InvokeAsync(Arg.Any<CancellationToken>()).Returns(("State", 42));

        var activator = new StateMachineActivator<string, string, int>(func);

        // Act
        await activator.Activate(stateMachine, TestContext.Current.CancellationToken);
        await activator.Activate(stateMachine, TestContext.Current.CancellationToken);

        // Assert
        await func.Received(2).InvokeAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Activate_T_WhenFuncConstructor_ShouldPassCancellationToken()
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        var expectedState = Substitute.ForPartsOf<StubState<string, string>>("State", new[] { typeof(int) });
        var stateMachine = Substitute.ForPartsOf<StubStateMachine<string, string>>(expectedState);
        var func = Substitute.For<IAsyncFunc<(string, int)>>();
        func.InvokeAsync(cts.Token).Returns(("State", 42));

        var activator = new StateMachineActivator<string, string, int>(func);

        // Act
        await activator.Activate(stateMachine, cts.Token);

        // Assert
        await func.Received(1).InvokeAsync(cts.Token);
    }
}
