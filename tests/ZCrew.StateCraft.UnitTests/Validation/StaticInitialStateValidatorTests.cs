using ZCrew.StateCraft.Info;
using ZCrew.StateCraft.Validation;

namespace ZCrew.StateCraft.UnitTests.Validation;

public class StaticInitialStateValidatorTests
{
    [Fact]
    public void Validate_WhenInitialStateIsNull_ShouldPass()
    {
        // Arrange
        var info = new StateMachineInfo<string, string>(null);
        info.Add(new StateInfo<string, string>(info, "A", []));
        info.Add(new StateInfo<string, string>(info, "B", []));
        var context = new StateMachineValidationContext<string, string> { Info = info };

        // Act
        StaticInitialStateValidator.Validate(context);

        // Assert
        Assert.Empty(context.ValidationErrors);
    }

    [Fact]
    public void Validate_WhenInitialStateIsDynamic_ShouldPass()
    {
        // Arrange
        var initialState = new DynamicInitialStateInfo<string, string>("provider", []);
        var info = new StateMachineInfo<string, string>(initialState);
        info.Add(new StateInfo<string, string>(info, "A", []));
        var context = new StateMachineValidationContext<string, string> { Info = info };

        // Act
        StaticInitialStateValidator.Validate(context);

        // Assert
        Assert.Empty(context.ValidationErrors);
    }

    [Fact]
    public void Validate_WhenParameterlessInitialStateExists_ShouldPass()
    {
        // Arrange
        var initialState = new StaticInitialStateInfo<string, string>("A", [], []);
        var info = new StateMachineInfo<string, string>(initialState);
        info.Add(new StateInfo<string, string>(info, "A", []));
        info.Add(new StateInfo<string, string>(info, "B", []));
        var context = new StateMachineValidationContext<string, string> { Info = info };

        // Act
        StaticInitialStateValidator.Validate(context);

        // Assert
        Assert.Empty(context.ValidationErrors);
    }

    [Fact]
    public void Validate_WhenParameterizedInitialStateExists_ShouldPass()
    {
        // Arrange
        var initialState = new StaticInitialStateInfo<string, string>("A", [42], [typeof(int)]);
        var info = new StateMachineInfo<string, string>(initialState);
        info.Add(new StateInfo<string, string>(info, "A", [typeof(int)]));
        var context = new StateMachineValidationContext<string, string> { Info = info };

        // Act
        StaticInitialStateValidator.Validate(context);

        // Assert
        Assert.Empty(context.ValidationErrors);
    }

    [Fact]
    public void Validate_WhenMultiParameterInitialStateExists_ShouldPass()
    {
        // Arrange
        var initialState = new StaticInitialStateInfo<string, string>(
            "A",
            [42, "value"],
            [typeof(int), typeof(string)]
        );
        var info = new StateMachineInfo<string, string>(initialState);
        info.Add(new StateInfo<string, string>(info, "A", [typeof(int), typeof(string)]));
        var context = new StateMachineValidationContext<string, string> { Info = info };

        // Act
        StaticInitialStateValidator.Validate(context);

        // Assert
        Assert.Empty(context.ValidationErrors);
    }

    [Fact]
    public void Validate_WhenInitialStateExistsAlongsideSameValueDifferentArity_ShouldPass()
    {
        // Arrange
        var initialState = new StaticInitialStateInfo<string, string>("A", [42], [typeof(int)]);
        var info = new StateMachineInfo<string, string>(initialState);
        info.Add(new StateInfo<string, string>(info, "A", [typeof(int)]));
        info.Add(new StateInfo<string, string>(info, "A", [typeof(string)]));
        var context = new StateMachineValidationContext<string, string> { Info = info };

        // Act
        StaticInitialStateValidator.Validate(context);

        // Assert
        Assert.Empty(context.ValidationErrors);
    }

    [Fact]
    public void Validate_WhenInitialStateNotFound_ShouldFail()
    {
        // Arrange
        var initialState = new StaticInitialStateInfo<string, string>("A", [], []);
        var info = new StateMachineInfo<string, string>(initialState);
        info.Add(new StateInfo<string, string>(info, "B", []));
        info.Add(new StateInfo<string, string>(info, "C", []));
        var context = new StateMachineValidationContext<string, string> { Info = info };

        // Act
        StaticInitialStateValidator.Validate(context);

        // Assert
        var error = Assert.Single(context.ValidationErrors);
        Assert.Contains("Initial state: A was not found", error);
        Assert.DoesNotContain("Specify", error);
    }

    [Fact]
    public void Validate_WhenNoStatesRegistered_ShouldFail()
    {
        // Arrange
        var initialState = new StaticInitialStateInfo<string, string>("A", [], []);
        var info = new StateMachineInfo<string, string>(initialState);
        var context = new StateMachineValidationContext<string, string> { Info = info };

        // Act
        StaticInitialStateValidator.Validate(context);

        // Assert
        var error = Assert.Single(context.ValidationErrors);
        Assert.Contains("Initial state: A was not found", error);
    }

    [Fact]
    public void Validate_WhenWrongArityWithParameterizedRegistered_ShouldSuggestCorrection()
    {
        // Arrange
        var initialState = new StaticInitialStateInfo<string, string>("A", [], []);
        var info = new StateMachineInfo<string, string>(initialState);
        info.Add(new StateInfo<string, string>(info, "A", [typeof(int)]));
        var context = new StateMachineValidationContext<string, string> { Info = info };

        // Act
        StaticInitialStateValidator.Validate(context);

        // Assert
        var error = Assert.Single(context.ValidationErrors);
        Assert.Contains("was not found", error);
        Assert.Contains("Specify the correct parameters like:", error);
        Assert.Contains("WithInitialState<int>(String.A, ...)", error);
    }

    [Fact]
    public void Validate_WhenWrongArityWithParameterlessRegistered_ShouldSuggestCorrection()
    {
        // Arrange
        var initialState = new StaticInitialStateInfo<string, string>("A", [42], [typeof(int)]);
        var info = new StateMachineInfo<string, string>(initialState);
        info.Add(new StateInfo<string, string>(info, "A", []));
        var context = new StateMachineValidationContext<string, string> { Info = info };

        // Act
        StaticInitialStateValidator.Validate(context);

        // Assert
        var error = Assert.Single(context.ValidationErrors);
        Assert.Contains("was not found", error);
        Assert.Contains("Specify the correct parameters like:", error);
        Assert.Contains("WithInitialState(String.A)", error);
    }

    [Fact]
    public void Validate_WhenMatchesMultipleStatesByValue_ShouldFail()
    {
        // Arrange
        var initialState = new StaticInitialStateInfo<string, string>("A", [], []);
        var info = new StateMachineInfo<string, string>(initialState);
        info.Add(new StateInfo<string, string>(info, "A", [typeof(int)]));
        info.Add(new StateInfo<string, string>(info, "A", [typeof(string)]));
        var context = new StateMachineValidationContext<string, string> { Info = info };

        // Act
        StaticInitialStateValidator.Validate(context);

        // Assert
        var error = Assert.Single(context.ValidationErrors);
        Assert.Contains("states with the same value were registered", error);
        Assert.Contains("A<int>, A<string>", error);
        Assert.Contains("you'd like to use as the initial state", error);
    }
}
