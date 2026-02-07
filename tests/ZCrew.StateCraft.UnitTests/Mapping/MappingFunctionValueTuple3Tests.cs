using NSubstitute;
using NSubstitute.ExceptionExtensions;
using ZCrew.Extensions.Tasks;
using ZCrew.StateCraft.Mapping;
using ZCrew.StateCraft.Parameters.Contracts;

namespace ZCrew.StateCraft.UnitTests.Mapping;

public class MappingFunctionValueTuple3Tests
{
    [Fact]
    public async Task Map_TIn_TOut1_TOut2_TOut3_WhenCalled_ShouldSetNextParameterFromFunctionResult()
    {
        // Arrange
        var parameters = Substitute.For<IStateMachineParameters>();
        parameters.GetPreviousParameter<int>().Returns(42);

        var function = Substitute.For<IAsyncFunc<int, (string, double, bool)>>();
        function.InvokeAsync(42, Arg.Any<CancellationToken>()).Returns(("result", 3.14, true));

        var mapper = new MappingFunctionValueTuple3<int, string, double, bool>(function);

        // Act
        await mapper.Map(parameters, TestContext.Current.CancellationToken);

        // Assert
        parameters.Received(1).SetNextParameters("result", 3.14, true);
    }

    [Fact]
    public async Task Map_TIn_TOut1_TOut2_TOut3_WhenCalled_ShouldPassParameterFromStateMachineParametersToFunction()
    {
        // Arrange
        var parameters = Substitute.For<IStateMachineParameters>();
        parameters.GetPreviousParameter<int>().Returns(42);

        var function = Substitute.For<IAsyncFunc<int, (string, double, bool)>>();
        function.InvokeAsync(Arg.Any<int>(), Arg.Any<CancellationToken>()).Returns(("result", 3.14, true));

        var mapper = new MappingFunctionValueTuple3<int, string, double, bool>(function);

        // Act
        await mapper.Map(parameters, TestContext.Current.CancellationToken);

        // Assert
        await function.Received(1).InvokeAsync(42, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Map_TIn_TOut1_TOut2_TOut3_WhenFunctionThrows_ShouldNotSetNextParameter()
    {
        // Arrange
        var parameters = Substitute.For<IStateMachineParameters>();
        parameters.GetPreviousParameter<int>().Returns(42);

        var function = Substitute.For<IAsyncFunc<int, (string, double, bool)>>();
        function.InvokeAsync(Arg.Any<int>(), Arg.Any<CancellationToken>()).ThrowsAsync(new InvalidOperationException());

        var mapper = new MappingFunctionValueTuple3<int, string, double, bool>(function);

        // Act
        var act = () => mapper.Map(parameters, TestContext.Current.CancellationToken);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(act);
        parameters.DidNotReceive().SetNextParameters(Arg.Any<string>(), Arg.Any<double>(), Arg.Any<bool>());
    }

    [Fact]
    public async Task Map_TIn_TOut1_TOut2_TOut3_WhenCalledMultipleTimes_ShouldSetNextParameterEachTime()
    {
        // Arrange
        var parameters = Substitute.For<IStateMachineParameters>();
        parameters.GetPreviousParameter<int>().Returns(1, 2);

        var function = Substitute.For<IAsyncFunc<int, (string, double, bool)>>();
        function.InvokeAsync(1, Arg.Any<CancellationToken>()).Returns(("first", 1.0, true));
        function.InvokeAsync(2, Arg.Any<CancellationToken>()).Returns(("second", 2.0, false));

        var mapper = new MappingFunctionValueTuple3<int, string, double, bool>(function);

        // Act
        await mapper.Map(parameters, TestContext.Current.CancellationToken);
        await mapper.Map(parameters, TestContext.Current.CancellationToken);

        // Assert
        parameters.Received(1).SetNextParameters("first", 1.0, true);
        parameters.Received(1).SetNextParameters("second", 2.0, false);
    }

    [Fact]
    public async Task Map_TIn1_TIn2_TOut1_TOut2_TOut3_WhenCalled_ShouldSetNextParameterFromFunctionResult()
    {
        // Arrange
        var parameters = Substitute.For<IStateMachineParameters>();
        parameters.GetPreviousParameters<int, string>().Returns((1, "input"));

        var function = Substitute.For<IAsyncFunc<int, string, (double, bool, char)>>();
        function.InvokeAsync(1, "input", Arg.Any<CancellationToken>()).Returns((3.14, true, 'x'));

        var mapper = new MappingFunctionValueTuple3<int, string, double, bool, char>(function);

        // Act
        await mapper.Map(parameters, TestContext.Current.CancellationToken);

        // Assert
        parameters.Received(1).SetNextParameters(3.14, true, 'x');
    }

    [Fact]
    public async Task Map_TIn1_TIn2_TOut1_TOut2_TOut3_WhenFunctionThrows_ShouldNotSetNextParameter()
    {
        // Arrange
        var parameters = Substitute.For<IStateMachineParameters>();
        parameters.GetPreviousParameters<int, string>().Returns((1, "input"));

        var function = Substitute.For<IAsyncFunc<int, string, (double, bool, char)>>();
        function
            .InvokeAsync(Arg.Any<int>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(new InvalidOperationException());

        var mapper = new MappingFunctionValueTuple3<int, string, double, bool, char>(function);

        // Act
        var act = () => mapper.Map(parameters, TestContext.Current.CancellationToken);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(act);
        parameters.DidNotReceive().SetNextParameters(Arg.Any<double>(), Arg.Any<bool>(), Arg.Any<char>());
    }

    [Fact]
    public async Task Map_TIn1_TIn2_TIn3_TOut1_TOut2_TOut3_WhenCalled_ShouldSetNextParameterFromFunctionResult()
    {
        // Arrange
        var parameters = Substitute.For<IStateMachineParameters>();
        parameters.GetPreviousParameters<int, string, bool>().Returns((1, "input", true));

        var function = Substitute.For<IAsyncFunc<int, string, bool, (double, char, long)>>();
        function.InvokeAsync(1, "input", true, Arg.Any<CancellationToken>()).Returns((3.14, 'x', 100L));

        var mapper = new MappingFunctionValueTuple3<int, string, bool, double, char, long>(function);

        // Act
        await mapper.Map(parameters, TestContext.Current.CancellationToken);

        // Assert
        parameters.Received(1).SetNextParameters(3.14, 'x', 100L);
    }

    [Fact]
    public async Task Map_TIn1_TIn2_TIn3_TOut1_TOut2_TOut3_WhenFunctionThrows_ShouldNotSetNextParameter()
    {
        // Arrange
        var parameters = Substitute.For<IStateMachineParameters>();
        parameters.GetPreviousParameters<int, string, bool>().Returns((1, "input", true));

        var function = Substitute.For<IAsyncFunc<int, string, bool, (double, char, long)>>();
        function
            .InvokeAsync(Arg.Any<int>(), Arg.Any<string>(), Arg.Any<bool>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(new InvalidOperationException());

        var mapper = new MappingFunctionValueTuple3<int, string, bool, double, char, long>(function);

        // Act
        var act = () => mapper.Map(parameters, TestContext.Current.CancellationToken);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(act);
        parameters.DidNotReceive().SetNextParameters(Arg.Any<double>(), Arg.Any<char>(), Arg.Any<long>());
    }

    [Fact]
    public async Task Map_TIn1_TIn2_TIn3_TIn4_TOut1_TOut2_TOut3_WhenCalled_ShouldSetNextParameterFromFunctionResult()
    {
        // Arrange
        var parameters = Substitute.For<IStateMachineParameters>();
        parameters.GetPreviousParameters<int, string, bool, char>().Returns((1, "input", true, 'y'));

        var function = Substitute.For<IAsyncFunc<int, string, bool, char, (double, long, short)>>();
        function.InvokeAsync(1, "input", true, 'y', Arg.Any<CancellationToken>()).Returns((3.14, 100L, (short)50));

        var mapper = new MappingFunctionValueTuple3<int, string, bool, char, double, long, short>(function);

        // Act
        await mapper.Map(parameters, TestContext.Current.CancellationToken);

        // Assert
        parameters.Received(1).SetNextParameters(3.14, 100L, (short)50);
    }

    [Fact]
    public async Task Map_TIn1_TIn2_TIn3_TIn4_TOut1_TOut2_TOut3_WhenFunctionThrows_ShouldNotSetNextParameter()
    {
        // Arrange
        var parameters = Substitute.For<IStateMachineParameters>();
        parameters.GetPreviousParameters<int, string, bool, char>().Returns((1, "input", true, 'y'));

        var function = Substitute.For<IAsyncFunc<int, string, bool, char, (double, long, short)>>();
        function
            .InvokeAsync(
                Arg.Any<int>(),
                Arg.Any<string>(),
                Arg.Any<bool>(),
                Arg.Any<char>(),
                Arg.Any<CancellationToken>()
            )
            .ThrowsAsync(new InvalidOperationException());

        var mapper = new MappingFunctionValueTuple3<int, string, bool, char, double, long, short>(function);

        // Act
        var act = () => mapper.Map(parameters, TestContext.Current.CancellationToken);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(act);
        parameters.DidNotReceive().SetNextParameters(Arg.Any<double>(), Arg.Any<long>(), Arg.Any<short>());
    }
}
