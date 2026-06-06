using ZCrew.StateCraft.Info;
using ZCrew.StateCraft.Validation;

namespace ZCrew.StateCraft.UnitTests.Validation;

public class TransitionToValidatorTests
{
    [Fact]
    public void Validate_WhenStateMachineHasNoStates_ShouldPass()
    {
        // Arrange
        var info = new StateMachineInfo<string, string>(null);
        var context = new StateMachineValidationContext<string, string> { Info = info };

        // Act
        TransitionToValidator.Validate(context);

        // Assert
        Assert.Empty(context.ValidationErrors);
    }

    [Fact]
    public void Validate_WhenStateMachineHasNoTransitions_ShouldPass()
    {
        // Arrange
        var info = new StateMachineInfo<string, string>(null);
        info.Add(new StateInfo<string, string>(info, "A", []));
        var context = new StateMachineValidationContext<string, string> { Info = info };

        // Act
        TransitionToValidator.Validate(context);

        // Assert
        Assert.Empty(context.ValidationErrors);
    }

    [Fact]
    public void Validate_WhenTransitionIsTuple_ShouldPass()
    {
        // Arrange
        var info = new StateMachineInfo<string, string>(null);
        info.Add(new StateInfo<string, string>(info, "A", []));
        info.Add(new StateInfo<string, string>(info, "B", [typeof(Tuple<int, string>)]));
        info.Add(
            new DirectTransitionInfo<string, string>(
                info,
                "To B",
                [typeof(Tuple<int, string>)],
                new ConditionalStateInfo<string, string>(info, "A", [], []),
                new ConditionalStateInfo<string, string>(info, "B", [typeof(Tuple<int, string>)], [])
            )
        );
        var context = new StateMachineValidationContext<string, string> { Info = info };

        // Act
        TransitionToValidator.Validate(context);

        // Assert
        Assert.Empty(context.ValidationErrors);
    }

    [Fact]
    public void Validate_WhenTransitionIsValueTuple_ShouldPass()
    {
        // Arrange
        var info = new StateMachineInfo<string, string>(null);
        info.Add(new StateInfo<string, string>(info, "A", []));
        info.Add(new StateInfo<string, string>(info, "B", [typeof((int, string))]));
        info.Add(
            new DirectTransitionInfo<string, string>(
                info,
                "To B",
                [typeof((int, string))],
                new ConditionalStateInfo<string, string>(info, "A", [], []),
                new ConditionalStateInfo<string, string>(info, "B", [typeof((int, string))], [])
            )
        );
        var context = new StateMachineValidationContext<string, string> { Info = info };

        // Act
        TransitionToValidator.Validate(context);

        // Assert
        Assert.Empty(context.ValidationErrors);
    }

    [Fact]
    public void Validate_WhenTransitionIsGenericType_ShouldPass()
    {
        // Arrange
        var info = new StateMachineInfo<string, string>(null);
        info.Add(new StateInfo<string, string>(info, "A", []));
        info.Add(new StateInfo<string, string>(info, "B", [typeof(List<string>)]));
        info.Add(
            new DirectTransitionInfo<string, string>(
                info,
                "To B",
                [typeof(List<string>)],
                new ConditionalStateInfo<string, string>(info, "A", [], []),
                new ConditionalStateInfo<string, string>(info, "B", [typeof(List<string>)], [])
            )
        );
        var context = new StateMachineValidationContext<string, string> { Info = info };

        // Act
        TransitionToValidator.Validate(context);

        // Assert
        Assert.Empty(context.ValidationErrors);
    }

    [Fact]
    public void Validate_WhenTransitionIsNullableValueType_ShouldPass()
    {
        // Arrange
        var info = new StateMachineInfo<string, string>(null);
        info.Add(new StateInfo<string, string>(info, "A", []));
        info.Add(new StateInfo<string, string>(info, "B", [typeof(int?)]));
        info.Add(
            new DirectTransitionInfo<string, string>(
                info,
                "To B",
                [typeof(int?)],
                new ConditionalStateInfo<string, string>(info, "A", [], []),
                new ConditionalStateInfo<string, string>(info, "B", [typeof(int?)], [])
            )
        );
        var context = new StateMachineValidationContext<string, string> { Info = info };

        // Act
        TransitionToValidator.Validate(context);

        // Assert
        Assert.Empty(context.ValidationErrors);
    }

    [Fact]
    public void Validate_WhenTransitionToNonExistentState_ShouldFail()
    {
        // Arrange
        var info = new StateMachineInfo<string, string>(null);
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
        var context = new StateMachineValidationContext<string, string> { Info = info };

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
        var info = new StateMachineInfo<string, string>(null);
        info.Add(new StateInfo<string, string>(info, "A", []));
        info.Add(new StateInfo<string, string>(info, "B", []));
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
        TransitionToValidator.Validate(context);

        // Assert
        var error = Assert.Single(context.ValidationErrors);
        Assert.Contains("To B(A) → B<int>", error);
    }

    [Fact]
    public void Validate_WhenParameterlessTransitionToParameterizedState_ShouldSuggestExplicitForm()
    {
        // Arrange
        var info = new StateMachineInfo<string, string>(null);
        info.Add(new StateInfo<string, string>(info, "A", []));
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
        var context = new StateMachineValidationContext<string, string> { Info = info };

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
        var info = new StateMachineInfo<string, string>(null);
        info.Add(new StateInfo<string, string>(info, "A", []));
        info.Add(new StateInfo<string, string>(info, "B", [typeof(int)]));
        info.Add(new StateInfo<string, string>(info, "B", [typeof(string)]));
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
        var info = new StateMachineInfo<string, string>(null);
        info.Add(new StateInfo<string, string>(info, "A", []));
        info.Add(new StateInfo<string, string>(info, "B", [typeof(int)]));
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
        TransitionToValidator.Validate(context);

        // Assert
        var error = Assert.Single(context.ValidationErrors);
        Assert.Contains("To B(A) → B<string>", error);
    }

    [Fact]
    public void Validate_WhenTransitionToStateWithAssignableParameterType_ShouldPass()
    {
        // Arrange
        var info = new StateMachineInfo<string, string>(null);
        info.Add(new StateInfo<string, string>(info, "A", []));
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
        var context = new StateMachineValidationContext<string, string> { Info = info };

        // Act
        TransitionToValidator.Validate(context);

        // Assert
        Assert.Empty(context.ValidationErrors);
    }

    [Fact]
    public void Validate_WhenMultipleErrors_ShouldReportAllErrors()
    {
        // Arrange
        var info = new StateMachineInfo<string, string>(null);
        info.Add(new StateInfo<string, string>(info, "A", []));
        info.Add(new StateInfo<string, string>(info, "B", []));
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
        TransitionToValidator.Validate(context);

        // Assert
        Assert.Collection(
            context.ValidationErrors,
            error => Assert.Contains("To C(A) → C", error),
            error => Assert.Contains("To C(B) → C", error)
        );
    }
}
