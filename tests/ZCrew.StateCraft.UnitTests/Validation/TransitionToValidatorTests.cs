using ZCrew.StateCraft.Validation;
using ZCrew.StateCraft.Validation.Models;

namespace ZCrew.StateCraft.UnitTests.Validation;

public class TransitionToValidatorTests
{
    [Fact]
    public void Validate_WhenStateMachineHasNoStates_ShouldPass()
    {
        // Arrange
        var context = new StateMachineValidationContext<string, string>();

        // Act
        TransitionToValidator.Validate(context);

        // Assert
        Assert.Empty(context.ValidationErrors);
    }

    [Fact]
    public void Validate_WhenStateMachineHasNoTransitions_ShouldPass()
    {
        // Arrange
        var context = new StateMachineValidationContext<string, string>
        {
            States = { new StateValidationModel<string, string>("A", []) },
        };

        // Act
        TransitionToValidator.Validate(context);

        // Assert
        Assert.Empty(context.ValidationErrors);
    }

    [Fact]
    public void Validate_WhenTransitionIsTuple_ShouldPass()
    {
        // Arrange
        var context = new StateMachineValidationContext<string, string>
        {
            States =
            {
                new StateValidationModel<string, string>("A", []),
                new StateValidationModel<string, string>("B", [typeof(Tuple<int, string>)]),
            },
            Transitions =
            {
                new TransitionValidationModel<string, string>(
                    "A",
                    "To B",
                    "B",
                    [],
                    [typeof(Tuple<int, string>)],
                    [typeof(Tuple<int, string>)],
                    false
                ),
            },
        };

        // Act
        TransitionToValidator.Validate(context);

        // Assert
        Assert.Empty(context.ValidationErrors);
    }

    [Fact]
    public void Validate_WhenTransitionIsValueTuple_ShouldPass()
    {
        // Arrange
        var context = new StateMachineValidationContext<string, string>
        {
            States =
            {
                new StateValidationModel<string, string>("A", []),
                new StateValidationModel<string, string>("B", [typeof((int, string))]),
            },
            Transitions =
            {
                new TransitionValidationModel<string, string>(
                    "A",
                    "To B",
                    "B",
                    [],
                    [typeof((int, string))],
                    [typeof((int, string))],
                    false
                ),
            },
        };

        // Act
        TransitionToValidator.Validate(context);

        // Assert
        Assert.Empty(context.ValidationErrors);
    }

    [Fact]
    public void Validate_WhenTransitionIsGenericType_ShouldPass()
    {
        // Arrange
        var context = new StateMachineValidationContext<string, string>
        {
            States =
            {
                new StateValidationModel<string, string>("A", []),
                new StateValidationModel<string, string>("B", [typeof(List<string>)]),
            },
            Transitions =
            {
                new TransitionValidationModel<string, string>(
                    "A",
                    "To B",
                    "B",
                    [],
                    [typeof(List<string>)],
                    [typeof(List<string>)],
                    false
                ),
            },
        };

        // Act
        TransitionToValidator.Validate(context);

        // Assert
        Assert.Empty(context.ValidationErrors);
    }

    [Fact]
    public void Validate_WhenTransitionIsNullableValueType_ShouldPass()
    {
        // Arrange
        var context = new StateMachineValidationContext<string, string>
        {
            States =
            {
                new StateValidationModel<string, string>("A", []),
                new StateValidationModel<string, string>("B", [typeof(int?)]),
            },
            Transitions =
            {
                new TransitionValidationModel<string, string>(
                    "A",
                    "To B",
                    "B",
                    [],
                    [typeof(int?)],
                    [typeof(int?)],
                    false
                ),
            },
        };

        // Act
        TransitionToValidator.Validate(context);

        // Assert
        Assert.Empty(context.ValidationErrors);
    }

    [Fact]
    public void Validate_WhenTransitionToNonExistentState_ShouldFail()
    {
        // Arrange
        var context = new StateMachineValidationContext<string, string>
        {
            States = { new StateValidationModel<string, string>("A", []) },
            Transitions = { new TransitionValidationModel<string, string>("A", "To B", "B", [], [], [], false) },
        };

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
        var context = new StateMachineValidationContext<string, string>
        {
            States =
            {
                new StateValidationModel<string, string>("A", []),
                new StateValidationModel<string, string>("B", []),
            },
            Transitions =
            {
                new TransitionValidationModel<string, string>(
                    "A",
                    "To B",
                    "B",
                    [],
                    [typeof(int)],
                    [typeof(int)],
                    false
                ),
            },
        };

        // Act
        TransitionToValidator.Validate(context);

        // Assert
        var error = Assert.Single(context.ValidationErrors);
        Assert.Contains("To B(A) → B<int>", error);
    }

    [Fact]
    public void Validate_WhenParameterlessTransitionToParameterizedState_ShouldSuggestExplicitForm()
    {
        // Arrange
        var context = new StateMachineValidationContext<string, string>
        {
            States =
            {
                new StateValidationModel<string, string>("A", []),
                new StateValidationModel<string, string>("B", [typeof(int)]),
            },
            Transitions = { new TransitionValidationModel<string, string>("A", "To B", "B", [], [], [], false) },
        };

        // Act
        TransitionToValidator.Validate(context);

        // Assert
        var error = Assert.Single(context.ValidationErrors);
        Assert.Contains("as parameterless", error);
        Assert.Contains("B<int>", error);
        Assert.Contains("WithTransition(transition, t =>", error);
    }

    [Fact]
    public void Validate_WhenParameterlessTransitionToMultipleParameterizedStates_ShouldListAllAlternatives()
    {
        // Arrange
        var context = new StateMachineValidationContext<string, string>
        {
            States =
            {
                new StateValidationModel<string, string>("A", []),
                new StateValidationModel<string, string>("B", [typeof(int)]),
                new StateValidationModel<string, string>("B", [typeof(string)]),
            },
            Transitions = { new TransitionValidationModel<string, string>("A", "To B", "B", [], [], [], false) },
        };

        // Act
        TransitionToValidator.Validate(context);

        // Assert
        var error = Assert.Single(context.ValidationErrors);
        Assert.Contains("as parameterless", error);
        Assert.Contains("B<int>", error);
        Assert.Contains("B<string>", error);
        Assert.Contains("WithTransition(transition, t =>", error);
    }

    [Fact]
    public void Validate_WhenTransitionToStateWithNonAssignableParameterType_ShouldFail()
    {
        // Arrange
        var context = new StateMachineValidationContext<string, string>
        {
            States =
            {
                new StateValidationModel<string, string>("A", []),
                new StateValidationModel<string, string>("B", [typeof(int)]),
            },
            Transitions =
            {
                new TransitionValidationModel<string, string>(
                    "A",
                    "To B",
                    "B",
                    [],
                    [typeof(string)],
                    [typeof(string)],
                    false
                ),
            },
        };

        // Act
        TransitionToValidator.Validate(context);

        // Assert
        var error = Assert.Single(context.ValidationErrors);
        Assert.Contains("To B(A) → B<string>", error);
    }

    [Fact]
    public void Validate_WhenTransitionToStateWithAssignableParameterType_ShouldPass()
    {
        // Arrange
        var context = new StateMachineValidationContext<string, string>
        {
            States =
            {
                new StateValidationModel<string, string>("A", []),
                new StateValidationModel<string, string>("B", [typeof(object)]),
            },
            Transitions =
            {
                new TransitionValidationModel<string, string>(
                    "A",
                    "To B",
                    "B",
                    [],
                    [typeof(string)],
                    [typeof(string)],
                    false
                ),
            },
        };

        // Act
        TransitionToValidator.Validate(context);

        // Assert
        Assert.Empty(context.ValidationErrors);
    }

    [Fact]
    public void Validate_WhenMultipleErrors_ShouldReportAllErrors()
    {
        // Arrange
        var context = new StateMachineValidationContext<string, string>
        {
            States =
            {
                new StateValidationModel<string, string>("A", []),
                new StateValidationModel<string, string>("B", []),
            },
            Transitions =
            {
                new TransitionValidationModel<string, string>("A", "To C", "C", [], [], [], false),
                new TransitionValidationModel<string, string>("B", "To C", "C", [], [], [], false),
            },
        };

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
