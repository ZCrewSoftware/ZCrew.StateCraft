using ZCrew.StateCraft.Validation;
using ZCrew.StateCraft.Validation.Models;

namespace ZCrew.StateCraft.UnitTests.Validation;

public class UnreachableTransitionValidatorTests
{
    [Fact]
    public void Validate_WhenStateMachineHasNoStates_ShouldPass()
    {
        // Arrange
        var context = new StateMachineValidationContext<string, string>();

        // Act
        UnreachableTransitionValidator.Validate(context);

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
        UnreachableTransitionValidator.Validate(context);

        // Assert
        Assert.Empty(context.ValidationErrors);
    }

    [Fact]
    public void Validate_WhenSingleTransition_ShouldPass()
    {
        // Arrange
        var context = new StateMachineValidationContext<string, string>
        {
            States =
            {
                new StateValidationModel<string, string>("A", []),
                new StateValidationModel<string, string>("B", []),
            },
            Transitions = { new TransitionValidationModel<string, string>("A", "To B", "B", [], [], [], false) },
        };

        // Act
        UnreachableTransitionValidator.Validate(context);

        // Assert
        Assert.Empty(context.ValidationErrors);
    }

    [Fact]
    public void Validate_WhenMultipleTransitionsToDifferentStates_ShouldPass()
    {
        // Arrange
        var context = new StateMachineValidationContext<string, string>
        {
            States =
            {
                new StateValidationModel<string, string>("A", []),
                new StateValidationModel<string, string>("B", []),
                new StateValidationModel<string, string>("C", []),
            },
            Transitions =
            {
                new TransitionValidationModel<string, string>("A", "To B", "B", [], [], [], false),
                new TransitionValidationModel<string, string>("A", "To C", "C", [], [], [], false),
            },
        };

        // Act
        UnreachableTransitionValidator.Validate(context);

        // Assert
        Assert.Empty(context.ValidationErrors);
    }

    [Fact]
    public void Validate_WhenMultipleTransitionsToSameStateWithDifferentParameterCounts_ShouldPass()
    {
        // Arrange
        var context = new StateMachineValidationContext<string, string>
        {
            States =
            {
                new StateValidationModel<string, string>("A", []),
                new StateValidationModel<string, string>("B", []),
                new StateValidationModel<string, string>("B", [typeof(int)]),
            },
            Transitions =
            {
                new TransitionValidationModel<string, string>("A", "To B", "B", [], [], [], false),
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
        UnreachableTransitionValidator.Validate(context);

        // Assert
        Assert.Empty(context.ValidationErrors);
    }

    [Fact]
    public void Validate_WhenMultipleTransitionsToSameStateWithNonAssignableTypes_ShouldPass()
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
            Transitions =
            {
                new TransitionValidationModel<string, string>(
                    "A",
                    "To B (int)",
                    "B",
                    [],
                    [typeof(int)],
                    [typeof(int)],
                    false
                ),
                new TransitionValidationModel<string, string>(
                    "A",
                    "To B (string)",
                    "B",
                    [],
                    [typeof(string)],
                    [typeof(string)],
                    false
                ),
            },
        };

        // Act
        UnreachableTransitionValidator.Validate(context);

        // Assert
        Assert.Empty(context.ValidationErrors);
    }

    [Fact]
    public void Validate_WhenDerivedTypeTransitionFollowsBaseTypeTransition_ShouldFail()
    {
        // Arrange
        var context = new StateMachineValidationContext<string, string>
        {
            States =
            {
                new StateValidationModel<string, string>("A", []),
                new StateValidationModel<string, string>("B", [typeof(object)]),
                new StateValidationModel<string, string>("B", [typeof(string)]),
            },
            Transitions =
            {
                new TransitionValidationModel<string, string>(
                    "A",
                    "To B",
                    "B",
                    [],
                    [typeof(object)],
                    [typeof(object)],
                    false
                ),
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
        UnreachableTransitionValidator.Validate(context);

        // Assert
        var error = Assert.Single(context.ValidationErrors);
        Assert.Contains("To B(A) → B<string>", error);
    }

    [Fact]
    public void Validate_WhenBaseTypeTransitionFollowsDerivedTypeTransition_ShouldPass()
    {
        // Arrange
        var context = new StateMachineValidationContext<string, string>
        {
            States =
            {
                new StateValidationModel<string, string>("A", []),
                new StateValidationModel<string, string>("B", [typeof(string)]),
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
                new TransitionValidationModel<string, string>(
                    "A",
                    "To B",
                    "B",
                    [],
                    [typeof(object)],
                    [typeof(object)],
                    false
                ),
            },
        };

        // Act
        UnreachableTransitionValidator.Validate(context);

        // Assert
        Assert.Empty(context.ValidationErrors);
    }

    [Fact]
    public void Validate_WhenDuplicateParameterlessTransitions_ShouldFail()
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
                new TransitionValidationModel<string, string>("A", "To B", "B", [], [], [], false),
                new TransitionValidationModel<string, string>("A", "To B", "B", [], [], [], false),
            },
        };

        // Act
        UnreachableTransitionValidator.Validate(context);

        // Assert
        var error = Assert.Single(context.ValidationErrors);
        Assert.Contains("To B(A) → B", error);
    }

    [Fact]
    public void Validate_WhenDuplicateParameterizedTransitions_ShouldFail()
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
                    [typeof(int)],
                    [typeof(int)],
                    false
                ),
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
        UnreachableTransitionValidator.Validate(context);

        // Assert
        var error = Assert.Single(context.ValidationErrors);
        Assert.Contains("To B(A) → B<int>", error);
    }

    [Fact]
    public void Validate_WhenTripleDuplicateTransitions_ShouldReportTwoErrors()
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
                new TransitionValidationModel<string, string>("A", "To B", "B", [], [], [], false),
                new TransitionValidationModel<string, string>("A", "To B", "B", [], [], [], false),
                new TransitionValidationModel<string, string>("A", "To B", "B", [], [], [], false),
            },
        };

        // Act
        UnreachableTransitionValidator.Validate(context);

        // Assert
        Assert.Collection(
            context.ValidationErrors,
            error => Assert.Contains("To B(A) → B", error),
            error => Assert.Contains("To B(A) → B", error)
        );
    }

    [Fact]
    public void Validate_WhenMultipleStatesWithUnreachableTransitions_ShouldReportAllErrors()
    {
        // Arrange
        var context = new StateMachineValidationContext<string, string>
        {
            States =
            {
                new StateValidationModel<string, string>("A", []),
                new StateValidationModel<string, string>("B", []),
                new StateValidationModel<string, string>("C", []),
            },
            Transitions =
            {
                new TransitionValidationModel<string, string>("A", "To C", "C", [], [], [], false),
                new TransitionValidationModel<string, string>("A", "To C", "C", [], [], [], false),
                new TransitionValidationModel<string, string>("B", "To C", "C", [], [], [], false),
                new TransitionValidationModel<string, string>("B", "To C", "C", [], [], [], false),
            },
        };

        // Act
        UnreachableTransitionValidator.Validate(context);

        // Assert
        Assert.Collection(
            context.ValidationErrors,
            error => Assert.Contains("To C(A) → C", error),
            error => Assert.Contains("To C(B) → C", error)
        );
    }

    [Fact]
    public void Validate_WhenTransitionsFromDifferentStatesAreDuplicates_ShouldPass()
    {
        // Arrange
        var context = new StateMachineValidationContext<string, string>
        {
            States =
            {
                new StateValidationModel<string, string>("A", []),
                new StateValidationModel<string, string>("B", []),
                new StateValidationModel<string, string>("C", []),
            },
            Transitions =
            {
                new TransitionValidationModel<string, string>("A", "To C", "C", [], [], [], false),
                new TransitionValidationModel<string, string>("B", "To C", "C", [], [], [], false),
            },
        };

        // Act
        UnreachableTransitionValidator.Validate(context);

        // Assert
        Assert.Empty(context.ValidationErrors);
    }

    [Fact]
    public void Validate_WhenSecondParameterlessTransitionIsConditional_ShouldFail()
    {
        // Arrange
        var context = new StateMachineValidationContext<string, string>
        {
            States = { new StateValidationModel<string, string>("A", []) },
            Transitions =
            {
                new TransitionValidationModel<string, string>("A", "To B", "B", [], [], [], false),
                new TransitionValidationModel<string, string>("A", "To B", "B", [], [], [], true),
            },
        };

        // Act
        UnreachableTransitionValidator.Validate(context);

        // Assert
        var error = Assert.Single(context.ValidationErrors);
        Assert.Contains("To B(A) → B", error);
    }

    [Fact]
    public void Validate_WhenFirstParameterlessTransitionIsConditional_ShouldPass()
    {
        // Arrange
        var context = new StateMachineValidationContext<string, string>
        {
            States = { new StateValidationModel<string, string>("A", []) },
            Transitions =
            {
                new TransitionValidationModel<string, string>("A", "To B", "B", [], [], [], true),
                new TransitionValidationModel<string, string>("A", "To B", "B", [], [], [], false),
            },
        };

        // Act
        UnreachableTransitionValidator.Validate(context);

        // Assert
        Assert.Empty(context.ValidationErrors);
    }

    [Fact]
    public void Validate_WhenSecondParameterizedTransitionIsConditional_ShouldFail()
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
                    [typeof(int)],
                    [typeof(int)],
                    false
                ),
                new TransitionValidationModel<string, string>("A", "To B", "B", [], [typeof(int)], [typeof(int)], true),
            },
        };

        // Act
        UnreachableTransitionValidator.Validate(context);

        // Assert
        var error = Assert.Single(context.ValidationErrors);
        Assert.Contains("To B(A) → B<int>", error);
    }

    [Fact]
    public void Validate_WhenFirstParameterizedTransitionIsConditional_ShouldPass()
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
                new TransitionValidationModel<string, string>("A", "To B", "B", [], [typeof(int)], [typeof(int)], true),
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
        UnreachableTransitionValidator.Validate(context);

        // Assert
        Assert.Empty(context.ValidationErrors);
    }

    [Fact]
    public void Validate_WhenSingleReentrantParameterlessTransition_ShouldPass()
    {
        // Arrange
        var context = new StateMachineValidationContext<string, string>
        {
            States = { new StateValidationModel<string, string>("A", []) },
            Transitions = { new TransitionValidationModel<string, string>("A", "Loop", "A", [], [], [], false) },
        };

        // Act
        UnreachableTransitionValidator.Validate(context);

        // Assert
        Assert.Empty(context.ValidationErrors);
    }

    [Fact]
    public void Validate_WhenDuplicateReentrantParameterlessTransitions_ShouldFail()
    {
        // Arrange
        var context = new StateMachineValidationContext<string, string>
        {
            States = { new StateValidationModel<string, string>("A", []) },
            Transitions =
            {
                new TransitionValidationModel<string, string>("A", "Loop", "A", [], [], [], false),
                new TransitionValidationModel<string, string>("A", "Loop", "A", [], [], [], false),
            },
        };

        // Act
        UnreachableTransitionValidator.Validate(context);

        // Assert
        var error = Assert.Single(context.ValidationErrors);
        Assert.Contains("Loop(A) ↩", error);
    }

    [Fact]
    public void Validate_WhenSingleReentrantWithSameParameterTransition_ShouldPass()
    {
        // Arrange
        var context = new StateMachineValidationContext<string, string>
        {
            States = { new StateValidationModel<string, string>("A", [typeof(int)]) },
            Transitions =
            {
                new TransitionValidationModel<string, string>(
                    "A",
                    "Loop",
                    "A",
                    [typeof(int)],
                    [typeof(int)],
                    [typeof(int)],
                    false
                ),
            },
        };

        // Act
        UnreachableTransitionValidator.Validate(context);

        // Assert
        Assert.Empty(context.ValidationErrors);
    }

    [Fact]
    public void Validate_WhenDuplicateReentrantWithSameParameterTransitions_ShouldFail()
    {
        // Arrange
        var context = new StateMachineValidationContext<string, string>
        {
            States = { new StateValidationModel<string, string>("A", [typeof(int)]) },
            Transitions =
            {
                new TransitionValidationModel<string, string>(
                    "A",
                    "Loop",
                    "A",
                    [typeof(int)],
                    [typeof(int)],
                    [typeof(int)],
                    false
                ),
                new TransitionValidationModel<string, string>(
                    "A",
                    "Loop",
                    "A",
                    [typeof(int)],
                    [typeof(int)],
                    [typeof(int)],
                    false
                ),
            },
        };

        // Act
        UnreachableTransitionValidator.Validate(context);

        // Assert
        var error = Assert.Single(context.ValidationErrors);
        Assert.Contains("Loop(A<int>) ↩", error);
    }

    [Fact]
    public void Validate_WhenSingleReentrantParameterizedTransition_ShouldPass()
    {
        // Arrange
        var context = new StateMachineValidationContext<string, string>
        {
            States = { new StateValidationModel<string, string>("A", [typeof(int)]) },
            Transitions =
            {
                new TransitionValidationModel<string, string>(
                    "A",
                    "Loop",
                    "A",
                    [typeof(int)],
                    [typeof(int)],
                    [typeof(int)],
                    false
                ),
            },
        };

        // Act
        UnreachableTransitionValidator.Validate(context);

        // Assert
        Assert.Empty(context.ValidationErrors);
    }

    [Fact]
    public void Validate_WhenDuplicateReentrantParameterizedTransitions_ShouldFail()
    {
        // Arrange
        var context = new StateMachineValidationContext<string, string>
        {
            States = { new StateValidationModel<string, string>("A", [typeof(int)]) },
            Transitions =
            {
                new TransitionValidationModel<string, string>(
                    "A",
                    "Loop",
                    "A",
                    [typeof(int)],
                    [typeof(int)],
                    [typeof(int)],
                    false
                ),
                new TransitionValidationModel<string, string>(
                    "A",
                    "Loop",
                    "A",
                    [typeof(int)],
                    [typeof(int)],
                    [typeof(int)],
                    false
                ),
            },
        };

        // Act
        UnreachableTransitionValidator.Validate(context);

        // Assert
        var error = Assert.Single(context.ValidationErrors);
        Assert.Contains("Loop(A<int>) ↩", error);
    }
}
