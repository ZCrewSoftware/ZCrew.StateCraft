using ZCrew.StateCraft.Validation;
using ZCrew.StateCraft.Validation.Models;

namespace ZCrew.StateCraft.UnitTests.Validation;

public class TransitionFromValidatorTests
{
    [Fact]
    public void Validate_WhenStateMachineHasNoStates_ShouldPass()
    {
        // Arrange
        var context = new StateMachineValidationContext<string, string>();

        // Act
        TransitionFromValidator.Validate(context);

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
        TransitionFromValidator.Validate(context);

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
                new StateValidationModel<string, string>("A", [typeof(Tuple<int, string>)]),
                new StateValidationModel<string, string>("B", []),
            },
            Transitions =
            {
                new TransitionValidationModel<string, string>(
                    "A",
                    "To B",
                    "B",
                    [typeof(Tuple<int, string>)],
                    [typeof(Tuple<int, string>)],
                    [],
                    false
                ),
            },
        };

        // Act
        TransitionFromValidator.Validate(context);

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
                new StateValidationModel<string, string>("A", [typeof((int, string))]),
                new StateValidationModel<string, string>("B", []),
            },
            Transitions =
            {
                new TransitionValidationModel<string, string>(
                    "A",
                    "To B",
                    "B",
                    [typeof((int, string))],
                    [typeof((int, string))],
                    [],
                    false
                ),
            },
        };

        // Act
        TransitionFromValidator.Validate(context);

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
                new StateValidationModel<string, string>("A", [typeof(List<string>)]),
                new StateValidationModel<string, string>("B", []),
            },
            Transitions =
            {
                new TransitionValidationModel<string, string>(
                    "A",
                    "To B",
                    "B",
                    [typeof(List<string>)],
                    [typeof(List<string>)],
                    [],
                    false
                ),
            },
        };

        // Act
        TransitionFromValidator.Validate(context);

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
                new StateValidationModel<string, string>("A", [typeof(int?)]),
                new StateValidationModel<string, string>("B", []),
            },
            Transitions =
            {
                new TransitionValidationModel<string, string>(
                    "A",
                    "To B",
                    "B",
                    [typeof(int?)],
                    [typeof(int?)],
                    [],
                    false
                ),
            },
        };

        // Act
        TransitionFromValidator.Validate(context);

        // Assert
        Assert.Empty(context.ValidationErrors);
    }

    [Fact]
    public void Validate_WhenTransitionFromNonExistentState_ShouldFail()
    {
        // Arrange
        var context = new StateMachineValidationContext<string, string>
        {
            States = { new StateValidationModel<string, string>("B", []) },
            Transitions = { new TransitionValidationModel<string, string>("A", "To B", "B", [], [], [], false) },
        };

        // Act
        TransitionFromValidator.Validate(context);

        // Assert
        var error = Assert.Single(context.ValidationErrors);
        Assert.Contains("To B(A) → B", error);
    }

    [Fact]
    public void Validate_WhenParameterizedTransitionFromParameterlessState_ShouldFail()
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
                    [typeof(int)],
                    [typeof(int)],
                    [],
                    false
                ),
            },
        };

        // Act
        TransitionFromValidator.Validate(context);

        // Assert
        var error = Assert.Single(context.ValidationErrors);
        Assert.Contains("To B(A<int>) → B", error);
    }

    [Fact]
    public void Validate_WhenParameterlessTransitionFromParameterizedState_ShouldSuggestExplicitForm()
    {
        // Arrange
        var context = new StateMachineValidationContext<string, string>
        {
            States =
            {
                new StateValidationModel<string, string>("A", [typeof(int)]),
                new StateValidationModel<string, string>("B", []),
            },
            Transitions = { new TransitionValidationModel<string, string>("A", "To B", "B", [], [], [], false) },
        };

        // Act
        TransitionFromValidator.Validate(context);

        // Assert
        var error = Assert.Single(context.ValidationErrors);
        Assert.Contains("as parameterless", error);
        Assert.Contains("A<int>", error);
        Assert.Contains("WithTransition(transition, t =>", error);
    }

    [Fact]
    public void Validate_WhenParameterlessTransitionFromMultipleParameterizedStates_ShouldListAllAlternatives()
    {
        // Arrange
        var context = new StateMachineValidationContext<string, string>
        {
            States =
            {
                new StateValidationModel<string, string>("A", [typeof(int)]),
                new StateValidationModel<string, string>("A", [typeof(string)]),
                new StateValidationModel<string, string>("B", []),
            },
            Transitions = { new TransitionValidationModel<string, string>("A", "To B", "B", [], [], [], false) },
        };

        // Act
        TransitionFromValidator.Validate(context);

        // Assert
        var error = Assert.Single(context.ValidationErrors);
        Assert.Contains("as parameterless", error);
        Assert.Contains("A<int>", error);
        Assert.Contains("A<string>", error);
        Assert.Contains("WithTransition(transition, t =>", error);
    }

    [Fact]
    public void Validate_WhenTransitionFromStateWithNonAssignableParameterType_ShouldFail()
    {
        // Arrange
        var context = new StateMachineValidationContext<string, string>
        {
            States =
            {
                new StateValidationModel<string, string>("A", [typeof(int)]),
                new StateValidationModel<string, string>("B", []),
            },
            Transitions =
            {
                new TransitionValidationModel<string, string>(
                    "A",
                    "To B",
                    "B",
                    [typeof(string)],
                    [typeof(string)],
                    [],
                    false
                ),
            },
        };

        // Act
        TransitionFromValidator.Validate(context);

        // Assert
        var error = Assert.Single(context.ValidationErrors);
        Assert.Contains("To B(A<string>) → B", error);
    }

    [Fact]
    public void Validate_WhenTransitionFromStateWithAssignableParameterType_ShouldPass()
    {
        // Arrange
        var context = new StateMachineValidationContext<string, string>
        {
            States =
            {
                new StateValidationModel<string, string>("A", [typeof(object)]),
                new StateValidationModel<string, string>("B", []),
            },
            Transitions =
            {
                new TransitionValidationModel<string, string>(
                    "A",
                    "To B",
                    "B",
                    [typeof(string)],
                    [typeof(string)],
                    [],
                    false
                ),
            },
        };

        // Act
        TransitionFromValidator.Validate(context);

        // Assert
        Assert.Empty(context.ValidationErrors);
    }

    [Fact]
    public void Validate_WhenMultipleErrors_ShouldReportAllErrors()
    {
        // Arrange
        var context = new StateMachineValidationContext<string, string>
        {
            States = { new StateValidationModel<string, string>("C", []) },
            Transitions =
            {
                new TransitionValidationModel<string, string>("A", "To C", "C", [], [], [], false),
                new TransitionValidationModel<string, string>("B", "To C", "C", [], [], [], false),
            },
        };

        // Act
        TransitionFromValidator.Validate(context);

        // Assert
        Assert.Collection(
            context.ValidationErrors,
            error => Assert.Contains("To C(A) → C", error),
            error => Assert.Contains("To C(B) → C", error)
        );
    }
}
