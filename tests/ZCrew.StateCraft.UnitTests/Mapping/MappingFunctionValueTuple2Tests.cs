using NSubstitute;
using NSubstitute.ExceptionExtensions;
using ZCrew.Extensions.Tasks;
using ZCrew.StateCraft.Mapping;
using ZCrew.StateCraft.Parameters.Contracts;

namespace ZCrew.StateCraft.UnitTests.Mapping;

public class MappingFunctionValueTuple2Tests
{
    [Fact]
    public async Task Map_TIn_TOut1_TOut2_WhenCalled_ShouldSetNextParameterFromFunctionResult()
    {
        // Arrange
        var parameters = Substitute.For<IStateMachineParameters>();
        parameters.GetPreviousParameter<int>().Returns(42);

        var function = Substitute.For<IAsyncFunc<int, (string, double)>>();
        function.InvokeAsync(42, Arg.Any<CancellationToken>()).Returns(("result", 3.14));

        var mapper = new MappingFunctionValueTuple2<int, string, double>(function);

        // Act
        await mapper.Map(parameters, TestContext.Current.CancellationToken);

        // Assert
        parameters.Received(1).SetNextParameters("result", 3.14);
    }

    [Fact]
    public async Task Map_TIn_TOut1_TOut2_WhenCalled_ShouldPassParameterFromStateMachineParametersToFunction()
    {
        // Arrange
        var parameters = Substitute.For<IStateMachineParameters>();
        parameters.GetPreviousParameter<int>().Returns(42);

        var function = Substitute.For<IAsyncFunc<int, (string, double)>>();
        function.InvokeAsync(Arg.Any<int>(), Arg.Any<CancellationToken>()).Returns(("result", 3.14));

        var mapper = new MappingFunctionValueTuple2<int, string, double>(function);

        // Act
        await mapper.Map(parameters, TestContext.Current.CancellationToken);

        // Assert
        await function.Received(1).InvokeAsync(42, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Map_TIn_TOut1_TOut2_WhenFunctionThrows_ShouldNotSetNextParameter()
    {
        // Arrange
        var parameters = Substitute.For<IStateMachineParameters>();
        parameters.GetPreviousParameter<int>().Returns(42);

        var function = Substitute.For<IAsyncFunc<int, (string, double)>>();
        function.InvokeAsync(Arg.Any<int>(), Arg.Any<CancellationToken>()).ThrowsAsync(new InvalidOperationException());

        var mapper = new MappingFunctionValueTuple2<int, string, double>(function);

        // Act
        var act = () => mapper.Map(parameters, TestContext.Current.CancellationToken);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(act);
        parameters.DidNotReceive().SetNextParameters(Arg.Any<string>(), Arg.Any<double>());
    }

    [Fact]
    public async Task Map_TIn_TOut1_TOut2_WhenCalledMultipleTimes_ShouldSetNextParameterEachTime()
    {
        // Arrange
        var parameters = Substitute.For<IStateMachineParameters>();
        parameters.GetPreviousParameter<int>().Returns(1, 2);

        var function = Substitute.For<IAsyncFunc<int, (string, double)>>();
        function.InvokeAsync(1, Arg.Any<CancellationToken>()).Returns(("first", 1.0));
        function.InvokeAsync(2, Arg.Any<CancellationToken>()).Returns(("second", 2.0));

        var mapper = new MappingFunctionValueTuple2<int, string, double>(function);

        // Act
        await mapper.Map(parameters, TestContext.Current.CancellationToken);
        await mapper.Map(parameters, TestContext.Current.CancellationToken);

        // Assert
        parameters.Received(1).SetNextParameters("first", 1.0);
        parameters.Received(1).SetNextParameters("second", 2.0);
    }

    [Fact]
    public async Task Map_TIn1_TIn2_TOut1_TOut2_WhenCalled_ShouldSetNextParameterFromFunctionResult()
    {
        // Arrange
        var parameters = Substitute.For<IStateMachineParameters>();
        parameters.GetPreviousParameters<int, string>().Returns((1, "input"));

        var function = Substitute.For<IAsyncFunc<int, string, (double, bool)>>();
        function.InvokeAsync(1, "input", Arg.Any<CancellationToken>()).Returns((3.14, true));

        var mapper = new MappingFunctionValueTuple2<int, string, double, bool>(function);

        // Act
        await mapper.Map(parameters, TestContext.Current.CancellationToken);

        // Assert
        parameters.Received(1).SetNextParameters(3.14, true);
    }

    [Fact]
    public async Task Map_TIn1_TIn2_TOut1_TOut2_WhenCalled_ShouldPassParametersFromStateMachineParametersToFunction()
    {
        // Arrange
        var parameters = Substitute.For<IStateMachineParameters>();
        parameters.GetPreviousParameters<int, string>().Returns((1, "input"));

        var function = Substitute.For<IAsyncFunc<int, string, (double, bool)>>();
        function.InvokeAsync(Arg.Any<int>(), Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns((3.14, true));

        var mapper = new MappingFunctionValueTuple2<int, string, double, bool>(function);

        // Act
        await mapper.Map(parameters, TestContext.Current.CancellationToken);

        // Assert
        await function.Received(1).InvokeAsync(1, "input", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Map_TIn1_TIn2_TOut1_TOut2_WhenFunctionThrows_ShouldNotSetNextParameter()
    {
        // Arrange
        var parameters = Substitute.For<IStateMachineParameters>();
        parameters.GetPreviousParameters<int, string>().Returns((1, "input"));

        var function = Substitute.For<IAsyncFunc<int, string, (double, bool)>>();
        function
            .InvokeAsync(Arg.Any<int>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(new InvalidOperationException());

        var mapper = new MappingFunctionValueTuple2<int, string, double, bool>(function);

        // Act
        var act = () => mapper.Map(parameters, TestContext.Current.CancellationToken);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(act);
        parameters.DidNotReceive().SetNextParameters(Arg.Any<double>(), Arg.Any<bool>());
    }

    [Fact]
    public async Task Map_TIn1_TIn2_TIn3_TOut1_TOut2_WhenCalled_ShouldSetNextParameterFromFunctionResult()
    {
        // Arrange
        var parameters = Substitute.For<IStateMachineParameters>();
        parameters.GetPreviousParameters<int, string, bool>().Returns((1, "input", true));

        var function = Substitute.For<IAsyncFunc<int, string, bool, (double, char)>>();
        function.InvokeAsync(1, "input", true, Arg.Any<CancellationToken>()).Returns((3.14, 'x'));

        var mapper = new MappingFunctionValueTuple2<int, string, bool, double, char>(function);

        // Act
        await mapper.Map(parameters, TestContext.Current.CancellationToken);

        // Assert
        parameters.Received(1).SetNextParameters(3.14, 'x');
    }

    [Fact]
    public async Task Map_TIn1_TIn2_TIn3_TOut1_TOut2_WhenCalled_ShouldPassParametersFromStateMachineParametersToFunction()
    {
        // Arrange
        var parameters = Substitute.For<IStateMachineParameters>();
        parameters.GetPreviousParameters<int, string, bool>().Returns((1, "input", true));

        var function = Substitute.For<IAsyncFunc<int, string, bool, (double, char)>>();
        function
            .InvokeAsync(Arg.Any<int>(), Arg.Any<string>(), Arg.Any<bool>(), Arg.Any<CancellationToken>())
            .Returns((3.14, 'x'));

        var mapper = new MappingFunctionValueTuple2<int, string, bool, double, char>(function);

        // Act
        await mapper.Map(parameters, TestContext.Current.CancellationToken);

        // Assert
        await function.Received(1).InvokeAsync(1, "input", true, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Map_TIn1_TIn2_TIn3_TOut1_TOut2_WhenFunctionThrows_ShouldNotSetNextParameter()
    {
        // Arrange
        var parameters = Substitute.For<IStateMachineParameters>();
        parameters.GetPreviousParameters<int, string, bool>().Returns((1, "input", true));

        var function = Substitute.For<IAsyncFunc<int, string, bool, (double, char)>>();
        function
            .InvokeAsync(Arg.Any<int>(), Arg.Any<string>(), Arg.Any<bool>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(new InvalidOperationException());

        var mapper = new MappingFunctionValueTuple2<int, string, bool, double, char>(function);

        // Act
        var act = () => mapper.Map(parameters, TestContext.Current.CancellationToken);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(act);
        parameters.DidNotReceive().SetNextParameters(Arg.Any<double>(), Arg.Any<char>());
    }

    [Fact]
    public async Task Map_TIn1_TIn2_TIn3_TIn4_TOut1_TOut2_WhenCalled_ShouldSetNextParameterFromFunctionResult()
    {
        // Arrange
        var parameters = Substitute.For<IStateMachineParameters>();
        parameters.GetPreviousParameters<int, string, bool, char>().Returns((1, "input", true, 'y'));

        var function = Substitute.For<IAsyncFunc<int, string, bool, char, (double, long)>>();
        function.InvokeAsync(1, "input", true, 'y', Arg.Any<CancellationToken>()).Returns((3.14, 100L));

        var mapper = new MappingFunctionValueTuple2<int, string, bool, char, double, long>(function);

        // Act
        await mapper.Map(parameters, TestContext.Current.CancellationToken);

        // Assert
        parameters.Received(1).SetNextParameters(3.14, 100L);
    }

    [Fact]
    public async Task Map_TIn1_TIn2_TIn3_TIn4_TOut1_TOut2_WhenCalled_ShouldPassParametersFromStateMachineParametersToFunction()
    {
        // Arrange
        var parameters = Substitute.For<IStateMachineParameters>();
        parameters.GetPreviousParameters<int, string, bool, char>().Returns((1, "input", true, 'y'));

        var function = Substitute.For<IAsyncFunc<int, string, bool, char, (double, long)>>();
        function
            .InvokeAsync(
                Arg.Any<int>(),
                Arg.Any<string>(),
                Arg.Any<bool>(),
                Arg.Any<char>(),
                Arg.Any<CancellationToken>()
            )
            .Returns((3.14, 100L));

        var mapper = new MappingFunctionValueTuple2<int, string, bool, char, double, long>(function);

        // Act
        await mapper.Map(parameters, TestContext.Current.CancellationToken);

        // Assert
        await function.Received(1).InvokeAsync(1, "input", true, 'y', Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Map_TIn1_TIn2_TIn3_TIn4_TOut1_TOut2_WhenFunctionThrows_ShouldNotSetNextParameter()
    {
        // Arrange
        var parameters = Substitute.For<IStateMachineParameters>();
        parameters.GetPreviousParameters<int, string, bool, char>().Returns((1, "input", true, 'y'));

        var function = Substitute.For<IAsyncFunc<int, string, bool, char, (double, long)>>();
        function
            .InvokeAsync(
                Arg.Any<int>(),
                Arg.Any<string>(),
                Arg.Any<bool>(),
                Arg.Any<char>(),
                Arg.Any<CancellationToken>()
            )
            .ThrowsAsync(new InvalidOperationException());

        var mapper = new MappingFunctionValueTuple2<int, string, bool, char, double, long>(function);

        // Act
        var act = () => mapper.Map(parameters, TestContext.Current.CancellationToken);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(act);
        parameters.DidNotReceive().SetNextParameters(Arg.Any<double>(), Arg.Any<long>());
    }
}
