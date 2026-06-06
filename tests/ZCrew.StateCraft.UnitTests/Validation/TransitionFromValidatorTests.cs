using ZCrew.StateCraft.Info;
using ZCrew.StateCraft.Validation;

namespace ZCrew.StateCraft.UnitTests.Validation;

public class TransitionFromValidatorTests
{
    [Fact]
    public void Validate_WhenStateMachineHasNoStates_ShouldPass()
    {
        // Arrange
        var info = new StateMachineInfo<string, string>(null);
        var context = new StateMachineValidationContext<string, string> { Info = info };

        // Act
        TransitionFromValidator.Validate(context);

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
        TransitionFromValidator.Validate(context);

        // Assert
        Assert.Empty(context.ValidationErrors);
    }

    [Fact]
    public void Validate_WhenTransitionIsTuple_ShouldPass()
    {
        // Arrange
        var info = new StateMachineInfo<string, string>(null);
        info.Add(new StateInfo<string, string>(info, "A", [typeof(Tuple<int, string>)]));
        info.Add(new StateInfo<string, string>(info, "B", []));
        info.Add(
            new DirectTransitionInfo<string, string>(
                info,
                "To B",
                [typeof(Tuple<int, string>)],
                new ConditionalStateInfo<string, string>(info, "A", [typeof(Tuple<int, string>)], []),
                new ConditionalStateInfo<string, string>(info, "B", [], [])
            )
        );
        var context = new StateMachineValidationContext<string, string> { Info = info };

        // Act
        TransitionFromValidator.Validate(context);

        // Assert
        Assert.Empty(context.ValidationErrors);
    }

    [Fact]
    public void Validate_WhenTransitionIsValueTuple_ShouldPass()
    {
        // Arrange
        var info = new StateMachineInfo<string, string>(null);
        info.Add(new StateInfo<string, string>(info, "A", [typeof((int, string))]));
        info.Add(new StateInfo<string, string>(info, "B", []));
        info.Add(
            new DirectTransitionInfo<string, string>(
                info,
                "To B",
                [typeof((int, string))],
                new ConditionalStateInfo<string, string>(info, "A", [typeof((int, string))], []),
                new ConditionalStateInfo<string, string>(info, "B", [], [])
            )
        );
        var context = new StateMachineValidationContext<string, string> { Info = info };

        // Act
        TransitionFromValidator.Validate(context);

        // Assert
        Assert.Empty(context.ValidationErrors);
    }

    [Fact]
    public void Validate_WhenTransitionIsGenericType_ShouldPass()
    {
        // Arrange
        var info = new StateMachineInfo<string, string>(null);
        info.Add(new StateInfo<string, string>(info, "A", [typeof(List<string>)]));
        info.Add(new StateInfo<string, string>(info, "B", []));
        info.Add(
            new DirectTransitionInfo<string, string>(
                info,
                "To B",
                [typeof(List<string>)],
                new ConditionalStateInfo<string, string>(info, "A", [typeof(List<string>)], []),
                new ConditionalStateInfo<string, string>(info, "B", [], [])
            )
        );
        var context = new StateMachineValidationContext<string, string> { Info = info };

        // Act
        TransitionFromValidator.Validate(context);

        // Assert
        Assert.Empty(context.ValidationErrors);
    }

    [Fact]
    public void Validate_WhenTransitionIsNullableValueType_ShouldPass()
    {
        // Arrange
        var info = new StateMachineInfo<string, string>(null);
        info.Add(new StateInfo<string, string>(info, "A", [typeof(int?)]));
        info.Add(new StateInfo<string, string>(info, "B", []));
        info.Add(
            new DirectTransitionInfo<string, string>(
                info,
                "To B",
                [typeof(int?)],
                new ConditionalStateInfo<string, string>(info, "A", [typeof(int?)], []),
                new ConditionalStateInfo<string, string>(info, "B", [], [])
            )
        );
        var context = new StateMachineValidationContext<string, string> { Info = info };

        // Act
        TransitionFromValidator.Validate(context);

        // Assert
        Assert.Empty(context.ValidationErrors);
    }

    [Fact]
    public void Validate_WhenTransitionFromNonExistentState_ShouldFail()
    {
        // Arrange
        var info = new StateMachineInfo<string, string>(null);
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
        TransitionFromValidator.Validate(context);

        // Assert
        var error = Assert.Single(context.ValidationErrors);
        Assert.Contains("To B(A) → B", error);
    }

    [Fact]
    public void Validate_WhenParameterizedTransitionFromParameterlessState_ShouldFail()
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
                new ConditionalStateInfo<string, string>(info, "A", [typeof(int)], []),
                new ConditionalStateInfo<string, string>(info, "B", [], [])
            )
        );
        var context = new StateMachineValidationContext<string, string> { Info = info };

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
        var info = new StateMachineInfo<string, string>(null);
        info.Add(new StateInfo<string, string>(info, "A", [typeof(int)]));
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
        var info = new StateMachineInfo<string, string>(null);
        info.Add(new StateInfo<string, string>(info, "A", [typeof(int)]));
        info.Add(new StateInfo<string, string>(info, "A", [typeof(string)]));
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
        var info = new StateMachineInfo<string, string>(null);
        info.Add(new StateInfo<string, string>(info, "A", [typeof(int)]));
        info.Add(new StateInfo<string, string>(info, "B", []));
        info.Add(
            new DirectTransitionInfo<string, string>(
                info,
                "To B",
                [typeof(string)],
                new ConditionalStateInfo<string, string>(info, "A", [typeof(string)], []),
                new ConditionalStateInfo<string, string>(info, "B", [], [])
            )
        );
        var context = new StateMachineValidationContext<string, string> { Info = info };

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
        var info = new StateMachineInfo<string, string>(null);
        info.Add(new StateInfo<string, string>(info, "A", [typeof(object)]));
        info.Add(new StateInfo<string, string>(info, "B", []));
        info.Add(
            new DirectTransitionInfo<string, string>(
                info,
                "To B",
                [typeof(string)],
                new ConditionalStateInfo<string, string>(info, "A", [typeof(string)], []),
                new ConditionalStateInfo<string, string>(info, "B", [], [])
            )
        );
        var context = new StateMachineValidationContext<string, string> { Info = info };

        // Act
        TransitionFromValidator.Validate(context);

        // Assert
        Assert.Empty(context.ValidationErrors);
    }

    [Fact]
    public void Validate_WhenMultipleErrors_ShouldReportAllErrors()
    {
        // Arrange
        var info = new StateMachineInfo<string, string>(null);
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
        TransitionFromValidator.Validate(context);

        // Assert
        Assert.Collection(
            context.ValidationErrors,
            error => Assert.Contains("To C(A) → C", error),
            error => Assert.Contains("To C(B) → C", error)
        );
    }
}
