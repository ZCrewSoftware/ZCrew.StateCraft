using ZCrew.StateCraft.Validation;

namespace ZCrew.StateCraft.UnitTests.Validation;

public class TransitionToValidatorTests
{
    [Fact]
    public void Validate_WhenStateMachineHasNoStates_ShouldPass()
    {
        // Arrange
        var configuration = StateMachine.Configure<string, string>();
        var context = new StateMachineValidationContext<string, string> { Configuration = configuration };

        // Act
        TransitionToValidator.Validate(context);

        // Assert
        Assert.Empty(context.ValidationErrors);
    }

    [Fact]
    public void Validate_WhenStateMachineHasNoTransitions_ShouldPass()
    {
        // Arrange
        var configuration = StateMachine.Configure<string, string>().WithState("A", state => state);
        var context = new StateMachineValidationContext<string, string> { Configuration = configuration };

        // Act
        TransitionToValidator.Validate(context);

        // Assert
        Assert.Empty(context.ValidationErrors);
    }

    [Fact]
    public void Validate_WhenTransitionIsTuple_ShouldPass()
    {
        // Arrange
        var configuration = StateMachine
            .Configure<string, string>()
            .WithState("A", state => state.WithTransition<Tuple<int, string>>("To B", "B"))
            .WithState("B", state => state.WithParameter<Tuple<int, string>>());
        var context = new StateMachineValidationContext<string, string> { Configuration = configuration };

        // Act
        TransitionToValidator.Validate(context);

        // Assert
        Assert.Empty(context.ValidationErrors);
    }

    [Fact]
    public void Validate_WhenTransitionIsValueTuple_ShouldPass()
    {
        // Arrange
        var configuration = StateMachine
            .Configure<string, string>()
            .WithState("A", state => state.WithTransition<(int, string)>("To B", "B"))
            .WithState("B", state => state.WithParameter<(int, string)>());
        var context = new StateMachineValidationContext<string, string> { Configuration = configuration };

        // Act
        TransitionToValidator.Validate(context);

        // Assert
        Assert.Empty(context.ValidationErrors);
    }

    [Fact]
    public void Validate_WhenTransitionIsGenericType_ShouldPass()
    {
        // Arrange
        var configuration = StateMachine
            .Configure<string, string>()
            .WithState("A", state => state.WithTransition<List<string>>("To B", "B"))
            .WithState("B", state => state.WithParameter<List<string>>());
        var context = new StateMachineValidationContext<string, string> { Configuration = configuration };

        // Act
        TransitionToValidator.Validate(context);

        // Assert
        Assert.Empty(context.ValidationErrors);
    }

    [Fact]
    public void Validate_WhenTransitionIsNullableValueType_ShouldPass()
    {
        // Arrange
        var configuration = StateMachine
            .Configure<string, string>()
            .WithState("A", state => state.WithTransition<int?>("To B", "B"))
            .WithState("B", state => state.WithParameter<int?>());
        var context = new StateMachineValidationContext<string, string> { Configuration = configuration };

        // Act
        TransitionToValidator.Validate(context);

        // Assert
        Assert.Empty(context.ValidationErrors);
    }

    [Fact]
    public void Validate_WhenTransitionToNonExistentState_ShouldFail()
    {
        // Arrange
        var configuration = StateMachine
            .Configure<string, string>()
            .WithState("A", state => state.WithTransition("To B", "B"));
        var context = new StateMachineValidationContext<string, string> { Configuration = configuration };

        // Act
        TransitionToValidator.Validate(context);

        // Assert
        var error = Assert.Single(context.ValidationErrors);
        Assert.Contains("To B(A) → B", error);
    }

    [Fact]
    public void Validate_WhenParameterizedTransitionToParameterlessState_ShouldFail()
    {
        // Arrange
        var configuration = StateMachine
            .Configure<string, string>()
            .WithState("A", state => state.WithTransition<int>("To B", "B"))
            .WithState("B", state => state.WithNoParameters());
        var context = new StateMachineValidationContext<string, string> { Configuration = configuration };

        // Act
        TransitionToValidator.Validate(context);

        // Assert
        var error = Assert.Single(context.ValidationErrors);
        Assert.Contains("To B(A) → B<int>", error);
    }

    [Fact]
    public void Validate_WhenParameterlessTransitionToParameterizedState_ShouldFail()
    {
        // Arrange
        var configuration = StateMachine
            .Configure<string, string>()
            .WithState("A", state => state.WithTransition("To B", "B"))
            .WithState("B", state => state.WithParameter<int>());
        var context = new StateMachineValidationContext<string, string> { Configuration = configuration };

        // Act
        TransitionToValidator.Validate(context);

        // Assert
        var error = Assert.Single(context.ValidationErrors);
        Assert.Contains("To B(A) → B", error);
    }

    [Fact]
    public void Validate_WhenTransitionToStateWithNonAssignableParameterType_ShouldFail()
    {
        // Arrange
        var configuration = StateMachine
            .Configure<string, string>()
            .WithState("A", state => state.WithTransition<string>("To B", "B"))
            .WithState("B", state => state.WithParameter<int>());
        var context = new StateMachineValidationContext<string, string> { Configuration = configuration };

        // Act
        TransitionToValidator.Validate(context);

        // Assert
        var error = Assert.Single(context.ValidationErrors);
        Assert.Contains("To B(A) → B<string>", error);
    }

    [Fact]
    public void Validate_WhenTransitionToStateWithAssignableParameterType_ShouldFail()
    {
        // Arrange
        var configuration = StateMachine
            .Configure<string, string>()
            .WithState("A", state => state.WithTransition<string>("To B", "B"))
            .WithState("B", state => state.WithParameter<object>());
        var context = new StateMachineValidationContext<string, string> { Configuration = configuration };

        // Act
        TransitionToValidator.Validate(context);

        // Assert
        var error = Assert.Single(context.ValidationErrors);
        Assert.Contains("To B(A) → B<string>", error);
    }

    [Fact]
    public void Validate_WhenMultipleErrors_ShouldReportAllErrors()
    {
        // Arrange
        var configuration = StateMachine
            .Configure<string, string>()
            .WithState("A", state => state.WithTransition("To C", "C"))
            .WithState("B", state => state.WithTransition("To C", "C"));
        var context = new StateMachineValidationContext<string, string> { Configuration = configuration };

        // Act
        TransitionToValidator.Validate(context);

        // Assert
        Assert.Collection(
            context.ValidationErrors,
            error => Assert.Contains("To C(A) → C", error),
            error => Assert.Contains("To C(B) → C", error)
        );
    }
}
