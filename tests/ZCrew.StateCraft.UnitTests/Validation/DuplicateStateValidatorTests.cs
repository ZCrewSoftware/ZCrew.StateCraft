using ZCrew.StateCraft.Validation;

namespace ZCrew.StateCraft.UnitTests.Validation;

public class DuplicateStateValidatorTests
{
    [Fact]
    public void Validate_WhenStateMachineHasNoStates_ShouldPass()
    {
        // Arrange
        var configuration = StateMachine.Configure<string, string>();
        var context = new StateMachineValidationContext<string, string> { Configuration = configuration };

        // Act
        DuplicateStateValidator.Validate(context);

        // Assert
        Assert.Empty(context.ValidationErrors);
    }

    [Fact]
    public void Validate_WhenStateMachineHasSingleState_ShouldPass()
    {
        // Arrange
        var configuration = StateMachine.Configure<string, string>().WithState("A", state => state);
        var context = new StateMachineValidationContext<string, string> { Configuration = configuration };

        // Act
        DuplicateStateValidator.Validate(context);

        // Assert
        Assert.Empty(context.ValidationErrors);
    }

    [Fact]
    public void Validate_WhenStateMachineHasMultipleUniqueStates_ShouldPass()
    {
        // Arrange
        var configuration = StateMachine
            .Configure<string, string>()
            .WithState("A", state => state)
            .WithState("B", state => state)
            .WithState("C", state => state);
        var context = new StateMachineValidationContext<string, string> { Configuration = configuration };

        // Act
        DuplicateStateValidator.Validate(context);

        // Assert
        Assert.Empty(context.ValidationErrors);
    }

    [Fact]
    public void Validate_WhenSameStateNameWithDifferentTypeParameters_ShouldPass()
    {
        // Arrange
        var configuration = StateMachine
            .Configure<string, string>()
            .WithState("A", state => state.WithNoParameters())
            .WithState("A", state => state.WithParameter<int>());
        var context = new StateMachineValidationContext<string, string> { Configuration = configuration };

        // Act
        DuplicateStateValidator.Validate(context);

        // Assert
        Assert.Empty(context.ValidationErrors);
    }

    [Fact]
    public void Validate_WhenSameStateNameWithDifferentTypeParameterTypes_ShouldPass()
    {
        // Arrange
        var configuration = StateMachine
            .Configure<string, string>()
            .WithState("A", state => state.WithParameter<int>())
            .WithState("A", state => state.WithParameter<string>());
        var context = new StateMachineValidationContext<string, string> { Configuration = configuration };

        // Act
        DuplicateStateValidator.Validate(context);

        // Assert
        Assert.Empty(context.ValidationErrors);
    }

    [Fact]
    public void Validate_WhenDuplicateParameterlessState_ShouldFail()
    {
        // Arrange
        var configuration = StateMachine
            .Configure<string, string>()
            .WithState("A", state => state)
            .WithState("A", state => state);
        var context = new StateMachineValidationContext<string, string> { Configuration = configuration };

        // Act
        DuplicateStateValidator.Validate(context);

        // Assert
        var error = Assert.Single(context.ValidationErrors);
        Assert.Contains("A", error);
    }

    [Fact]
    public void Validate_WhenDuplicateParameterizedState_ShouldFail()
    {
        // Arrange
        var configuration = StateMachine
            .Configure<string, string>()
            .WithState("A", state => state.WithParameter<int>())
            .WithState("A", state => state.WithParameter<int>());
        var context = new StateMachineValidationContext<string, string> { Configuration = configuration };

        // Act
        DuplicateStateValidator.Validate(context);

        // Assert
        var error = Assert.Single(context.ValidationErrors);
        Assert.Contains("A<int>", error);
    }

    [Fact]
    public void Validate_WhenMultipleDuplicateStates_ShouldReportAllErrors()
    {
        // Arrange
        var configuration = StateMachine
            .Configure<string, string>()
            .WithState("A", state => state)
            .WithState("A", state => state)
            .WithState("B", state => state)
            .WithState("B", state => state);
        var context = new StateMachineValidationContext<string, string> { Configuration = configuration };

        // Act
        DuplicateStateValidator.Validate(context);

        // Assert
        Assert.Collection(
            context.ValidationErrors,
            error => Assert.Contains("A", error),
            error => Assert.Contains("B", error)
        );
    }

    [Fact]
    public void Validate_WhenTripleDuplicateState_ShouldReportTwoErrors()
    {
        // Arrange
        var configuration = StateMachine
            .Configure<string, string>()
            .WithState("A", state => state)
            .WithState("A", state => state)
            .WithState("A", state => state);
        var context = new StateMachineValidationContext<string, string> { Configuration = configuration };

        // Act
        DuplicateStateValidator.Validate(context);

        // Assert
        Assert.Collection(
            context.ValidationErrors,
            error => Assert.Contains("A", error),
            error => Assert.Contains("A", error)
        );
    }
}
