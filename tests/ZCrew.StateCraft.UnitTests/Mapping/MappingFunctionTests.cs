using NSubstitute;
using NSubstitute.ExceptionExtensions;
using ZCrew.Extensions.Tasks;
using ZCrew.StateCraft.Mapping;
using ZCrew.StateCraft.Parameters.Contracts;

namespace ZCrew.StateCraft.UnitTests.Mapping;

public class MappingFunctionTests
{
    [Fact]
    public async Task Map_TIn_TOut_WhenCalled_ShouldSetNextParameterFromFunctionResult()
    {
        // Arrange
        var parameters = Substitute.For<IStateMachineParameters>();
        parameters.GetPreviousParameter<int>().Returns(42);

        var function = Substitute.For<IAsyncFunc<int, string>>();
        function.InvokeAsync(42, Arg.Any<CancellationToken>()).Returns("result");

        var mapper = new MappingFunction<int, string>(function);

        // Act
        await mapper.Map(parameters, TestContext.Current.CancellationToken);

        // Assert
        parameters.Received(1).SetNextParameter("result");
    }

    [Fact]
    public async Task Map_TIn_TOut_WhenCalled_ShouldPassParameterFromStateMachineParametersToFunction()
    {
        // Arrange
        var parameters = Substitute.For<IStateMachineParameters>();
        parameters.GetPreviousParameter<int>().Returns(42);

        var function = Substitute.For<IAsyncFunc<int, string>>();
        function.InvokeAsync(Arg.Any<int>(), Arg.Any<CancellationToken>()).Returns("result");

        var mapper = new MappingFunction<int, string>(function);

        // Act
        await mapper.Map(parameters, TestContext.Current.CancellationToken);

        // Assert
        await function.Received(1).InvokeAsync(42, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Map_TIn_TOut_WhenFunctionThrows_ShouldNotSetNextParameter()
    {
        // Arrange
        var parameters = Substitute.For<IStateMachineParameters>();
        parameters.GetPreviousParameter<int>().Returns(42);

        var function = Substitute.For<IAsyncFunc<int, string>>();
        function.InvokeAsync(Arg.Any<int>(), Arg.Any<CancellationToken>()).ThrowsAsync(new InvalidOperationException());

        var mapper = new MappingFunction<int, string>(function);

        // Act
        var act = () => mapper.Map(parameters, TestContext.Current.CancellationToken);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(act);
        parameters.DidNotReceive().SetNextParameter(Arg.Any<string>());
    }

    [Fact]
    public async Task Map_TIn_TOut_WhenCalledMultipleTimes_ShouldSetNextParameterEachTime()
    {
        // Arrange
        var parameters = Substitute.For<IStateMachineParameters>();
        parameters.GetPreviousParameter<int>().Returns(1, 2);

        var function = Substitute.For<IAsyncFunc<int, string>>();
        function.InvokeAsync(1, Arg.Any<CancellationToken>()).Returns("first");
        function.InvokeAsync(2, Arg.Any<CancellationToken>()).Returns("second");

        var mapper = new MappingFunction<int, string>(function);

        // Act
        await mapper.Map(parameters, TestContext.Current.CancellationToken);
        await mapper.Map(parameters, TestContext.Current.CancellationToken);

        // Assert
        parameters.Received(1).SetNextParameter("first");
        parameters.Received(1).SetNextParameter("second");
    }

    [Fact]
    public async Task Map_TIn1_TIn2_TOut_WhenCalled_ShouldSetNextParameterFromFunctionResult()
    {
        // Arrange
        var parameters = Substitute.For<IStateMachineParameters>();
        parameters.GetPreviousParameters<int, string>().Returns((1, "input"));

        var function = Substitute.For<IAsyncFunc<int, string, double>>();
        function.InvokeAsync(1, "input", Arg.Any<CancellationToken>()).Returns(3.14);

        var mapper = new MappingFunction<int, string, double>(function);

        // Act
        await mapper.Map(parameters, TestContext.Current.CancellationToken);

        // Assert
        parameters.Received(1).SetNextParameter(3.14);
    }

    [Fact]
    public async Task Map_TIn1_TIn2_TOut_WhenCalled_ShouldPassParametersFromStateMachineParametersToFunction()
    {
        // Arrange
        var parameters = Substitute.For<IStateMachineParameters>();
        parameters.GetPreviousParameters<int, string>().Returns((1, "input"));

        var function = Substitute.For<IAsyncFunc<int, string, double>>();
        function.InvokeAsync(Arg.Any<int>(), Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(3.14);

        var mapper = new MappingFunction<int, string, double>(function);

        // Act
        await mapper.Map(parameters, TestContext.Current.CancellationToken);

        // Assert
        await function.Received(1).InvokeAsync(1, "input", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Map_TIn1_TIn2_TOut_WhenFunctionThrows_ShouldNotSetNextParameter()
    {
        // Arrange
        var parameters = Substitute.For<IStateMachineParameters>();
        parameters.GetPreviousParameters<int, string>().Returns((1, "input"));

        var function = Substitute.For<IAsyncFunc<int, string, double>>();
        function
            .InvokeAsync(Arg.Any<int>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(new InvalidOperationException());

        var mapper = new MappingFunction<int, string, double>(function);

        // Act
        var act = () => mapper.Map(parameters, TestContext.Current.CancellationToken);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(act);
        parameters.DidNotReceive().SetNextParameter(Arg.Any<double>());
    }

    [Fact]
    public async Task Map_TIn1_TIn2_TIn3_TOut_WhenCalled_ShouldSetNextParameterFromFunctionResult()
    {
        // Arrange
        var parameters = Substitute.For<IStateMachineParameters>();
        parameters.GetPreviousParameters<int, string, bool>().Returns((1, "input", true));

        var function = Substitute.For<IAsyncFunc<int, string, bool, double>>();
        function.InvokeAsync(1, "input", true, Arg.Any<CancellationToken>()).Returns(3.14);

        var mapper = new MappingFunction<int, string, bool, double>(function);

        // Act
        await mapper.Map(parameters, TestContext.Current.CancellationToken);

        // Assert
        parameters.Received(1).SetNextParameter(3.14);
    }

    [Fact]
    public async Task Map_TIn1_TIn2_TIn3_TOut_WhenCalled_ShouldPassParametersFromStateMachineParametersToFunction()
    {
        // Arrange
        var parameters = Substitute.For<IStateMachineParameters>();
        parameters.GetPreviousParameters<int, string, bool>().Returns((1, "input", true));

        var function = Substitute.For<IAsyncFunc<int, string, bool, double>>();
        function
            .InvokeAsync(Arg.Any<int>(), Arg.Any<string>(), Arg.Any<bool>(), Arg.Any<CancellationToken>())
            .Returns(3.14);

        var mapper = new MappingFunction<int, string, bool, double>(function);

        // Act
        await mapper.Map(parameters, TestContext.Current.CancellationToken);

        // Assert
        await function.Received(1).InvokeAsync(1, "input", true, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Map_TIn1_TIn2_TIn3_TOut_WhenFunctionThrows_ShouldNotSetNextParameter()
    {
        // Arrange
        var parameters = Substitute.For<IStateMachineParameters>();
        parameters.GetPreviousParameters<int, string, bool>().Returns((1, "input", true));

        var function = Substitute.For<IAsyncFunc<int, string, bool, double>>();
        function
            .InvokeAsync(Arg.Any<int>(), Arg.Any<string>(), Arg.Any<bool>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(new InvalidOperationException());

        var mapper = new MappingFunction<int, string, bool, double>(function);

        // Act
        var act = () => mapper.Map(parameters, TestContext.Current.CancellationToken);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(act);
        parameters.DidNotReceive().SetNextParameter(Arg.Any<double>());
    }

    [Fact]
    public async Task Map_TIn1_TIn2_TIn3_TIn4_TOut_WhenCalled_ShouldSetNextParameterFromFunctionResult()
    {
        // Arrange
        var parameters = Substitute.For<IStateMachineParameters>();
        parameters.GetPreviousParameters<int, string, bool, char>().Returns((1, "input", true, 'x'));

        var function = Substitute.For<IAsyncFunc<int, string, bool, char, double>>();
        function.InvokeAsync(1, "input", true, 'x', Arg.Any<CancellationToken>()).Returns(3.14);

        var mapper = new MappingFunction<int, string, bool, char, double>(function);

        // Act
        await mapper.Map(parameters, TestContext.Current.CancellationToken);

        // Assert
        parameters.Received(1).SetNextParameter(3.14);
    }

    [Fact]
    public async Task Map_TIn1_TIn2_TIn3_TIn4_TOut_WhenCalled_ShouldPassParametersFromStateMachineParametersToFunction()
    {
        // Arrange
        var parameters = Substitute.For<IStateMachineParameters>();
        parameters.GetPreviousParameters<int, string, bool, char>().Returns((1, "input", true, 'x'));

        var function = Substitute.For<IAsyncFunc<int, string, bool, char, double>>();
        function
            .InvokeAsync(
                Arg.Any<int>(),
                Arg.Any<string>(),
                Arg.Any<bool>(),
                Arg.Any<char>(),
                Arg.Any<CancellationToken>()
            )
            .Returns(3.14);

        var mapper = new MappingFunction<int, string, bool, char, double>(function);

        // Act
        await mapper.Map(parameters, TestContext.Current.CancellationToken);

        // Assert
        await function.Received(1).InvokeAsync(1, "input", true, 'x', Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Map_TIn1_TIn2_TIn3_TIn4_TOut_WhenFunctionThrows_ShouldNotSetNextParameter()
    {
        // Arrange
        var parameters = Substitute.For<IStateMachineParameters>();
        parameters.GetPreviousParameters<int, string, bool, char>().Returns((1, "input", true, 'x'));

        var function = Substitute.For<IAsyncFunc<int, string, bool, char, double>>();
        function
            .InvokeAsync(
                Arg.Any<int>(),
                Arg.Any<string>(),
                Arg.Any<bool>(),
                Arg.Any<char>(),
                Arg.Any<CancellationToken>()
            )
            .ThrowsAsync(new InvalidOperationException());

        var mapper = new MappingFunction<int, string, bool, char, double>(function);

        // Act
        var act = () => mapper.Map(parameters, TestContext.Current.CancellationToken);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(act);
        parameters.DidNotReceive().SetNextParameter(Arg.Any<double>());
    }
}
