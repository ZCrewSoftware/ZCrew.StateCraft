using ZCrew.StateCraft.Info;
using ZCrew.StateCraft.Validation;

namespace ZCrew.StateCraft.UnitTests.Validation;

public class UnreachableTransitionValidatorTests
{
    [Fact]
    public void Validate_WhenStateMachineHasNoStates_ShouldPass()
    {
        // Arrange
        var info = new StateMachineInfo<string, string>(new StaticInitialStateInfo<string, string>("A", [], []));
        var context = new StateMachineValidationContext<string, string> { Info = info };

        // Act
        UnreachableTransitionValidator.Validate(context);

        // Assert
        Assert.Empty(context.ValidationErrors);
    }

    [Fact]
    public void Validate_WhenStateMachineHasNoTransitions_ShouldPass()
    {
        // Arrange
        var info = new StateMachineInfo<string, string>(new StaticInitialStateInfo<string, string>("A", [], []));
        info.Add(new StateInfo<string, string>(info, "A", []));
        var context = new StateMachineValidationContext<string, string> { Info = info };

        // Act
        UnreachableTransitionValidator.Validate(context);

        // Assert
        Assert.Empty(context.ValidationErrors);
    }

    [Fact]
    public void Validate_WhenSingleTransition_ShouldPass()
    {
        // Arrange
        var info = new StateMachineInfo<string, string>(new StaticInitialStateInfo<string, string>("A", [], []));
        info.Add(new StateInfo<string, string>(info, "A", []));
        info.Add(new StateInfo<string, string>(info, "B", []));
        info.Add(
            new DirectTransitionInfo<string, string>(
                info,
                "To B",
                [],
                new ConditionalStateInfo<string, string>(info, "A", [], []),
                new ConditionalStateInfo<string, string>(info, "B", [], [])
            )
        );
        var context = new StateMachineValidationContext<string, string> { Info = info };

        // Act
        UnreachableTransitionValidator.Validate(context);

        // Assert
        Assert.Empty(context.ValidationErrors);
    }

    [Fact]
    public void Validate_WhenMultipleTransitionsToDifferentStates_ShouldPass()
    {
        // Arrange
        var info = new StateMachineInfo<string, string>(new StaticInitialStateInfo<string, string>("A", [], []));
        info.Add(new StateInfo<string, string>(info, "A", []));
        info.Add(new StateInfo<string, string>(info, "B", []));
        info.Add(new StateInfo<string, string>(info, "C", []));
        info.Add(
            new DirectTransitionInfo<string, string>(
                info,
                "To B",
                [],
                new ConditionalStateInfo<string, string>(info, "A", [], []),
                new ConditionalStateInfo<string, string>(info, "B", [], [])
            )
        );
        info.Add(
            new DirectTransitionInfo<string, string>(
                info,
                "To C",
                [],
                new ConditionalStateInfo<string, string>(info, "A", [], []),
                new ConditionalStateInfo<string, string>(info, "C", [], [])
            )
        );
        var context = new StateMachineValidationContext<string, string> { Info = info };

        // Act
        UnreachableTransitionValidator.Validate(context);

        // Assert
        Assert.Empty(context.ValidationErrors);
    }

    [Fact]
    public void Validate_WhenMultipleTransitionsToSameStateWithDifferentParameterCounts_ShouldPass()
    {
        // Arrange
        var info = new StateMachineInfo<string, string>(new StaticInitialStateInfo<string, string>("A", [], []));
        info.Add(new StateInfo<string, string>(info, "A", []));
        info.Add(new StateInfo<string, string>(info, "B", []));
        info.Add(new StateInfo<string, string>(info, "B", [typeof(int)]));
        info.Add(
            new DirectTransitionInfo<string, string>(
                info,
                "To B",
                [],
                new ConditionalStateInfo<string, string>(info, "A", [], []),
                new ConditionalStateInfo<string, string>(info, "B", [], [])
            )
        );
        info.Add(
            new DirectTransitionInfo<string, string>(
                info,
                "To B",
                [typeof(int)],
                new ConditionalStateInfo<string, string>(info, "A", [], []),
                new ConditionalStateInfo<string, string>(info, "B", [typeof(int)], [])
            )
        );
        var context = new StateMachineValidationContext<string, string> { Info = info };

        // Act
        UnreachableTransitionValidator.Validate(context);

        // Assert
        Assert.Empty(context.ValidationErrors);
    }

    [Fact]
    public void Validate_WhenMultipleTransitionsToSameStateWithNonAssignableTypes_ShouldPass()
    {
        // Arrange
        var info = new StateMachineInfo<string, string>(new StaticInitialStateInfo<string, string>("A", [], []));
        info.Add(new StateInfo<string, string>(info, "A", []));
        info.Add(new StateInfo<string, string>(info, "B", [typeof(int)]));
        info.Add(new StateInfo<string, string>(info, "B", [typeof(string)]));
        info.Add(
            new DirectTransitionInfo<string, string>(
                info,
                "To B (int)",
                [typeof(int)],
                new ConditionalStateInfo<string, string>(info, "A", [], []),
                new ConditionalStateInfo<string, string>(info, "B", [typeof(int)], [])
            )
        );
        info.Add(
            new DirectTransitionInfo<string, string>(
                info,
                "To B (string)",
                [typeof(string)],
                new ConditionalStateInfo<string, string>(info, "A", [], []),
                new ConditionalStateInfo<string, string>(info, "B", [typeof(string)], [])
            )
        );
        var context = new StateMachineValidationContext<string, string> { Info = info };

        // Act
        UnreachableTransitionValidator.Validate(context);

        // Assert
        Assert.Empty(context.ValidationErrors);
    }

    [Fact]
    public void Validate_WhenDerivedTypeTransitionFollowsBaseTypeTransition_ShouldFail()
    {
        // Arrange
        var info = new StateMachineInfo<string, string>(new StaticInitialStateInfo<string, string>("A", [], []));
        info.Add(new StateInfo<string, string>(info, "A", []));
        info.Add(new StateInfo<string, string>(info, "B", [typeof(object)]));
        info.Add(new StateInfo<string, string>(info, "B", [typeof(string)]));
        info.Add(
            new DirectTransitionInfo<string, string>(
                info,
                "To B",
                [typeof(object)],
                new ConditionalStateInfo<string, string>(info, "A", [], []),
                new ConditionalStateInfo<string, string>(info, "B", [typeof(object)], [])
            )
        );
        info.Add(
            new DirectTransitionInfo<string, string>(
                info,
                "To B",
                [typeof(string)],
                new ConditionalStateInfo<string, string>(info, "A", [], []),
                new ConditionalStateInfo<string, string>(info, "B", [typeof(string)], [])
            )
        );
        var context = new StateMachineValidationContext<string, string> { Info = info };

        // Act
        UnreachableTransitionValidator.Validate(context);

        // Assert
        var error = Assert.Single(context.ValidationErrors);
        Assert.Contains("To B<string>(A) → B<string>", error);
    }

    [Fact]
    public void Validate_WhenBaseTypeTransitionFollowsDerivedTypeTransition_ShouldPass()
    {
        // Arrange
        var info = new StateMachineInfo<string, string>(new StaticInitialStateInfo<string, string>("A", [], []));
        info.Add(new StateInfo<string, string>(info, "A", []));
        info.Add(new StateInfo<string, string>(info, "B", [typeof(string)]));
        info.Add(new StateInfo<string, string>(info, "B", [typeof(object)]));
        info.Add(
            new DirectTransitionInfo<string, string>(
                info,
                "To B",
                [typeof(string)],
                new ConditionalStateInfo<string, string>(info, "A", [], []),
                new ConditionalStateInfo<string, string>(info, "B", [typeof(string)], [])
            )
        );
        info.Add(
            new DirectTransitionInfo<string, string>(
                info,
                "To B",
                [typeof(object)],
                new ConditionalStateInfo<string, string>(info, "A", [], []),
                new ConditionalStateInfo<string, string>(info, "B", [typeof(object)], [])
            )
        );
        var context = new StateMachineValidationContext<string, string> { Info = info };

        // Act
        UnreachableTransitionValidator.Validate(context);

        // Assert
        Assert.Empty(context.ValidationErrors);
    }

    [Fact]
    public void Validate_WhenDuplicateParameterlessTransitions_ShouldFail()
    {
        // Arrange
        var info = new StateMachineInfo<string, string>(new StaticInitialStateInfo<string, string>("A", [], []));
        info.Add(new StateInfo<string, string>(info, "A", []));
        info.Add(new StateInfo<string, string>(info, "B", []));
        info.Add(
            new DirectTransitionInfo<string, string>(
                info,
                "To B",
                [],
                new ConditionalStateInfo<string, string>(info, "A", [], []),
                new ConditionalStateInfo<string, string>(info, "B", [], [])
            )
        );
        info.Add(
            new DirectTransitionInfo<string, string>(
                info,
                "To B",
                [],
                new ConditionalStateInfo<string, string>(info, "A", [], []),
                new ConditionalStateInfo<string, string>(info, "B", [], [])
            )
        );
        var context = new StateMachineValidationContext<string, string> { Info = info };

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
        var info = new StateMachineInfo<string, string>(new StaticInitialStateInfo<string, string>("A", [], []));
        info.Add(new StateInfo<string, string>(info, "A", []));
        info.Add(new StateInfo<string, string>(info, "B", [typeof(int)]));
        info.Add(
            new DirectTransitionInfo<string, string>(
                info,
                "To B",
                [typeof(int)],
                new ConditionalStateInfo<string, string>(info, "A", [], []),
                new ConditionalStateInfo<string, string>(info, "B", [typeof(int)], [])
            )
        );
        info.Add(
            new DirectTransitionInfo<string, string>(
                info,
                "To B",
                [typeof(int)],
                new ConditionalStateInfo<string, string>(info, "A", [], []),
                new ConditionalStateInfo<string, string>(info, "B", [typeof(int)], [])
            )
        );
        var context = new StateMachineValidationContext<string, string> { Info = info };

        // Act
        UnreachableTransitionValidator.Validate(context);

        // Assert
        var error = Assert.Single(context.ValidationErrors);
        Assert.Contains("To B<int>(A) → B<int>", error);
    }

    [Fact]
    public void Validate_WhenTripleDuplicateTransitions_ShouldReportTwoErrors()
    {
        // Arrange
        var info = new StateMachineInfo<string, string>(new StaticInitialStateInfo<string, string>("A", [], []));
        info.Add(new StateInfo<string, string>(info, "A", []));
        info.Add(new StateInfo<string, string>(info, "B", []));
        info.Add(
            new DirectTransitionInfo<string, string>(
                info,
                "To B",
                [],
                new ConditionalStateInfo<string, string>(info, "A", [], []),
                new ConditionalStateInfo<string, string>(info, "B", [], [])
            )
        );
        info.Add(
            new DirectTransitionInfo<string, string>(
                info,
                "To B",
                [],
                new ConditionalStateInfo<string, string>(info, "A", [], []),
                new ConditionalStateInfo<string, string>(info, "B", [], [])
            )
        );
        info.Add(
            new DirectTransitionInfo<string, string>(
                info,
                "To B",
                [],
                new ConditionalStateInfo<string, string>(info, "A", [], []),
                new ConditionalStateInfo<string, string>(info, "B", [], [])
            )
        );
        var context = new StateMachineValidationContext<string, string> { Info = info };

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
        var info = new StateMachineInfo<string, string>(new StaticInitialStateInfo<string, string>("A", [], []));
        info.Add(new StateInfo<string, string>(info, "A", []));
        info.Add(new StateInfo<string, string>(info, "B", []));
        info.Add(new StateInfo<string, string>(info, "C", []));
        info.Add(
            new DirectTransitionInfo<string, string>(
                info,
                "To C",
                [],
                new ConditionalStateInfo<string, string>(info, "A", [], []),
                new ConditionalStateInfo<string, string>(info, "C", [], [])
            )
        );
        info.Add(
            new DirectTransitionInfo<string, string>(
                info,
                "To C",
                [],
                new ConditionalStateInfo<string, string>(info, "A", [], []),
                new ConditionalStateInfo<string, string>(info, "C", [], [])
            )
        );
        info.Add(
            new DirectTransitionInfo<string, string>(
                info,
                "To C",
                [],
                new ConditionalStateInfo<string, string>(info, "B", [], []),
                new ConditionalStateInfo<string, string>(info, "C", [], [])
            )
        );
        info.Add(
            new DirectTransitionInfo<string, string>(
                info,
                "To C",
                [],
                new ConditionalStateInfo<string, string>(info, "B", [], []),
                new ConditionalStateInfo<string, string>(info, "C", [], [])
            )
        );
        var context = new StateMachineValidationContext<string, string> { Info = info };

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
        var info = new StateMachineInfo<string, string>(new StaticInitialStateInfo<string, string>("A", [], []));
        info.Add(new StateInfo<string, string>(info, "A", []));
        info.Add(new StateInfo<string, string>(info, "B", []));
        info.Add(new StateInfo<string, string>(info, "C", []));
        info.Add(
            new DirectTransitionInfo<string, string>(
                info,
                "To C",
                [],
                new ConditionalStateInfo<string, string>(info, "A", [], []),
                new ConditionalStateInfo<string, string>(info, "C", [], [])
            )
        );
        info.Add(
            new DirectTransitionInfo<string, string>(
                info,
                "To C",
                [],
                new ConditionalStateInfo<string, string>(info, "B", [], []),
                new ConditionalStateInfo<string, string>(info, "C", [], [])
            )
        );
        var context = new StateMachineValidationContext<string, string> { Info = info };

        // Act
        UnreachableTransitionValidator.Validate(context);

        // Assert
        Assert.Empty(context.ValidationErrors);
    }

    [Fact]
    public void Validate_WhenSecondParameterlessTransitionIsConditional_ShouldFail()
    {
        // Arrange
        var info = new StateMachineInfo<string, string>(new StaticInitialStateInfo<string, string>("A", [], []));
        info.Add(new StateInfo<string, string>(info, "A", []));
        info.Add(
            new DirectTransitionInfo<string, string>(
                info,
                "To B",
                [],
                new ConditionalStateInfo<string, string>(info, "A", [], []),
                new ConditionalStateInfo<string, string>(info, "B", [], [])
            )
        );
        info.Add(
            new DirectTransitionInfo<string, string>(
                info,
                "To B",
                [],
                new ConditionalStateInfo<string, string>(info, "A", [], [new ConditionInfo(null, [])]),
                new ConditionalStateInfo<string, string>(info, "B", [], [])
            )
        );
        var context = new StateMachineValidationContext<string, string> { Info = info };

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
        var info = new StateMachineInfo<string, string>(new StaticInitialStateInfo<string, string>("A", [], []));
        info.Add(new StateInfo<string, string>(info, "A", []));
        info.Add(
            new DirectTransitionInfo<string, string>(
                info,
                "To B",
                [],
                new ConditionalStateInfo<string, string>(info, "A", [], [new ConditionInfo(null, [])]),
                new ConditionalStateInfo<string, string>(info, "B", [], [])
            )
        );
        info.Add(
            new DirectTransitionInfo<string, string>(
                info,
                "To B",
                [],
                new ConditionalStateInfo<string, string>(info, "A", [], []),
                new ConditionalStateInfo<string, string>(info, "B", [], [])
            )
        );
        var context = new StateMachineValidationContext<string, string> { Info = info };

        // Act
        UnreachableTransitionValidator.Validate(context);

        // Assert
        Assert.Empty(context.ValidationErrors);
    }

    [Fact]
    public void Validate_WhenSecondParameterizedTransitionIsConditional_ShouldFail()
    {
        // Arrange
        var info = new StateMachineInfo<string, string>(new StaticInitialStateInfo<string, string>("A", [], []));
        info.Add(new StateInfo<string, string>(info, "A", []));
        info.Add(new StateInfo<string, string>(info, "B", [typeof(int)]));
        info.Add(
            new DirectTransitionInfo<string, string>(
                info,
                "To B",
                [typeof(int)],
                new ConditionalStateInfo<string, string>(info, "A", [], []),
                new ConditionalStateInfo<string, string>(info, "B", [typeof(int)], [])
            )
        );
        info.Add(
            new DirectTransitionInfo<string, string>(
                info,
                "To B",
                [typeof(int)],
                new ConditionalStateInfo<string, string>(info, "A", [], [new ConditionInfo(null, [])]),
                new ConditionalStateInfo<string, string>(info, "B", [typeof(int)], [])
            )
        );
        var context = new StateMachineValidationContext<string, string> { Info = info };

        // Act
        UnreachableTransitionValidator.Validate(context);

        // Assert
        var error = Assert.Single(context.ValidationErrors);
        Assert.Contains("To B<int>(A) → B<int>", error);
    }

    [Fact]
    public void Validate_WhenFirstParameterizedTransitionIsConditional_ShouldPass()
    {
        // Arrange
        var info = new StateMachineInfo<string, string>(new StaticInitialStateInfo<string, string>("A", [], []));
        info.Add(new StateInfo<string, string>(info, "A", []));
        info.Add(new StateInfo<string, string>(info, "B", [typeof(int)]));
        info.Add(
            new DirectTransitionInfo<string, string>(
                info,
                "To B",
                [typeof(int)],
                new ConditionalStateInfo<string, string>(info, "A", [], [new ConditionInfo(null, [])]),
                new ConditionalStateInfo<string, string>(info, "B", [typeof(int)], [])
            )
        );
        info.Add(
            new DirectTransitionInfo<string, string>(
                info,
                "To B",
                [typeof(int)],
                new ConditionalStateInfo<string, string>(info, "A", [], []),
                new ConditionalStateInfo<string, string>(info, "B", [typeof(int)], [])
            )
        );
        var context = new StateMachineValidationContext<string, string> { Info = info };

        // Act
        UnreachableTransitionValidator.Validate(context);

        // Assert
        Assert.Empty(context.ValidationErrors);
    }

    [Fact]
    public void Validate_WhenSingleReentrantParameterlessTransition_ShouldPass()
    {
        // Arrange
        var info = new StateMachineInfo<string, string>(new StaticInitialStateInfo<string, string>("A", [], []));
        info.Add(new StateInfo<string, string>(info, "A", []));
        info.Add(
            new DirectTransitionInfo<string, string>(
                info,
                "Loop",
                [],
                new ConditionalStateInfo<string, string>(info, "A", [], []),
                new ConditionalStateInfo<string, string>(info, "A", [], [])
            )
        );
        var context = new StateMachineValidationContext<string, string> { Info = info };

        // Act
        UnreachableTransitionValidator.Validate(context);

        // Assert
        Assert.Empty(context.ValidationErrors);
    }

    [Fact]
    public void Validate_WhenDuplicateReentrantParameterlessTransitions_ShouldFail()
    {
        // Arrange
        var info = new StateMachineInfo<string, string>(new StaticInitialStateInfo<string, string>("A", [], []));
        info.Add(new StateInfo<string, string>(info, "A", []));
        info.Add(
            new DirectTransitionInfo<string, string>(
                info,
                "Loop",
                [],
                new ConditionalStateInfo<string, string>(info, "A", [], []),
                new ConditionalStateInfo<string, string>(info, "A", [], [])
            )
        );
        info.Add(
            new DirectTransitionInfo<string, string>(
                info,
                "Loop",
                [],
                new ConditionalStateInfo<string, string>(info, "A", [], []),
                new ConditionalStateInfo<string, string>(info, "A", [], [])
            )
        );
        var context = new StateMachineValidationContext<string, string> { Info = info };

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
        var info = new StateMachineInfo<string, string>(new StaticInitialStateInfo<string, string>("A", [], []));
        info.Add(new StateInfo<string, string>(info, "A", [typeof(int)]));
        info.Add(
            new DirectTransitionInfo<string, string>(
                info,
                "Loop",
                [typeof(int)],
                new ConditionalStateInfo<string, string>(info, "A", [typeof(int)], []),
                new ConditionalStateInfo<string, string>(info, "A", [typeof(int)], [])
            )
        );
        var context = new StateMachineValidationContext<string, string> { Info = info };

        // Act
        UnreachableTransitionValidator.Validate(context);

        // Assert
        Assert.Empty(context.ValidationErrors);
    }

    [Fact]
    public void Validate_WhenDuplicateReentrantWithSameParameterTransitions_ShouldFail()
    {
        // Arrange
        var info = new StateMachineInfo<string, string>(new StaticInitialStateInfo<string, string>("A", [], []));
        info.Add(new StateInfo<string, string>(info, "A", [typeof(int)]));
        info.Add(
            new DirectTransitionInfo<string, string>(
                info,
                "Loop",
                [typeof(int)],
                new ConditionalStateInfo<string, string>(info, "A", [typeof(int)], []),
                new ConditionalStateInfo<string, string>(info, "A", [typeof(int)], [])
            )
        );
        info.Add(
            new DirectTransitionInfo<string, string>(
                info,
                "Loop",
                [typeof(int)],
                new ConditionalStateInfo<string, string>(info, "A", [typeof(int)], []),
                new ConditionalStateInfo<string, string>(info, "A", [typeof(int)], [])
            )
        );
        var context = new StateMachineValidationContext<string, string> { Info = info };

        // Act
        UnreachableTransitionValidator.Validate(context);

        // Assert
        var error = Assert.Single(context.ValidationErrors);
        Assert.Contains("Loop<int>(A<int>) ↩", error);
    }

    [Fact]
    public void Validate_WhenSingleReentrantParameterizedTransition_ShouldPass()
    {
        // Arrange
        var info = new StateMachineInfo<string, string>(new StaticInitialStateInfo<string, string>("A", [], []));
        info.Add(new StateInfo<string, string>(info, "A", [typeof(int)]));
        info.Add(
            new DirectTransitionInfo<string, string>(
                info,
                "Loop",
                [typeof(int)],
                new ConditionalStateInfo<string, string>(info, "A", [typeof(int)], []),
                new ConditionalStateInfo<string, string>(info, "A", [typeof(int)], [])
            )
        );
        var context = new StateMachineValidationContext<string, string> { Info = info };

        // Act
        UnreachableTransitionValidator.Validate(context);

        // Assert
        Assert.Empty(context.ValidationErrors);
    }

    [Fact]
    public void Validate_WhenDuplicateReentrantParameterizedTransitions_ShouldFail()
    {
        // Arrange
        var info = new StateMachineInfo<string, string>(new StaticInitialStateInfo<string, string>("A", [], []));
        info.Add(new StateInfo<string, string>(info, "A", [typeof(int)]));
        info.Add(
            new DirectTransitionInfo<string, string>(
                info,
                "Loop",
                [typeof(int)],
                new ConditionalStateInfo<string, string>(info, "A", [typeof(int)], []),
                new ConditionalStateInfo<string, string>(info, "A", [typeof(int)], [])
            )
        );
        info.Add(
            new DirectTransitionInfo<string, string>(
                info,
                "Loop",
                [typeof(int)],
                new ConditionalStateInfo<string, string>(info, "A", [typeof(int)], []),
                new ConditionalStateInfo<string, string>(info, "A", [typeof(int)], [])
            )
        );
        var context = new StateMachineValidationContext<string, string> { Info = info };

        // Act
        UnreachableTransitionValidator.Validate(context);

        // Assert
        var error = Assert.Single(context.ValidationErrors);
        Assert.Contains("Loop<int>(A<int>) ↩", error);
    }
}
