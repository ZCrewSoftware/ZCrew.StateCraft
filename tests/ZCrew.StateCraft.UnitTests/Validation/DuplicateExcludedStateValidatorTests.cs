using ZCrew.StateCraft.Info;
using ZCrew.StateCraft.Validation;

namespace ZCrew.StateCraft.UnitTests.Validation;

public class DuplicateExcludedStateValidatorTests
{
    [Fact]
    public void Validate_WhenNoFromTransitions_ShouldPass()
    {
        // Arrange
        var info = new StateMachineInfo<string, string>(null);
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
        DuplicateExcludedStateValidator.Validate(context);

        // Assert
        Assert.Empty(context.ValidationErrors);
    }

    [Fact]
    public void Validate_WhenFromTransitionHasNoExcludedStates_ShouldPass()
    {
        // Arrange
        var info = new StateMachineInfo<string, string>(null);
        info.Add(
            new FromTransitionInfo<string, string>(
                info,
                "To D",
                [],
                new ConditionalStateInfo<string, string>(info, "D", [], []),
                []
            )
        );
        var context = new StateMachineValidationContext<string, string> { Info = info };

        // Act
        DuplicateExcludedStateValidator.Validate(context);

        // Assert
        Assert.Empty(context.ValidationErrors);
    }

    [Fact]
    public void Validate_WhenAllExcludedStatesUnique_ShouldPass()
    {
        // Arrange
        var info = new StateMachineInfo<string, string>(null);
        info.Add(
            new FromTransitionInfo<string, string>(
                info,
                "To D",
                [],
                new ConditionalStateInfo<string, string>(info, "D", [], []),
                [
                    new StateInfo<string, string>(info, "A", []),
                    new StateInfo<string, string>(info, "B", []),
                    new StateInfo<string, string>(info, "C", []),
                ]
            )
        );
        var context = new StateMachineValidationContext<string, string> { Info = info };

        // Act
        DuplicateExcludedStateValidator.Validate(context);

        // Assert
        Assert.Empty(context.ValidationErrors);
    }

    [Fact]
    public void Validate_WhenStateExcludedTwice_ShouldFail()
    {
        // Arrange
        var info = new StateMachineInfo<string, string>(null);
        info.Add(
            new FromTransitionInfo<string, string>(
                info,
                "To D",
                [],
                new ConditionalStateInfo<string, string>(info, "D", [], []),
                [new StateInfo<string, string>(info, "A", []), new StateInfo<string, string>(info, "A", [])]
            )
        );
        var context = new StateMachineValidationContext<string, string> { Info = info };

        // Act
        DuplicateExcludedStateValidator.Validate(context);

        // Assert
        var error = Assert.Single(context.ValidationErrors);
        Assert.Contains("A", error);
        Assert.Contains("has already been excluded", error);
    }

    [Fact]
    public void Validate_WhenStateExcludedThreeTimes_ShouldReportTwoErrors()
    {
        // Arrange
        var info = new StateMachineInfo<string, string>(null);
        info.Add(
            new FromTransitionInfo<string, string>(
                info,
                "To D",
                [],
                new ConditionalStateInfo<string, string>(info, "D", [], []),
                [
                    new StateInfo<string, string>(info, "A", []),
                    new StateInfo<string, string>(info, "A", []),
                    new StateInfo<string, string>(info, "A", []),
                ]
            )
        );
        var context = new StateMachineValidationContext<string, string> { Info = info };

        // Act
        DuplicateExcludedStateValidator.Validate(context);

        // Assert
        Assert.Collection(
            context.ValidationErrors,
            error => Assert.Contains("has already been excluded", error),
            error => Assert.Contains("has already been excluded", error)
        );
    }

    [Fact]
    public void Validate_WhenDuplicatesAcrossDifferentTransitions_ShouldPass()
    {
        // Arrange
        var info = new StateMachineInfo<string, string>(null);
        info.Add(
            new FromTransitionInfo<string, string>(
                info,
                "To C",
                [],
                new ConditionalStateInfo<string, string>(info, "C", [], []),
                [new StateInfo<string, string>(info, "A", [])]
            )
        );
        info.Add(
            new FromTransitionInfo<string, string>(
                info,
                "To D",
                [],
                new ConditionalStateInfo<string, string>(info, "D", [], []),
                [new StateInfo<string, string>(info, "A", [])]
            )
        );
        var context = new StateMachineValidationContext<string, string> { Info = info };

        // Act
        DuplicateExcludedStateValidator.Validate(context);

        // Assert
        Assert.Empty(context.ValidationErrors);
    }

    [Fact]
    public void Validate_WhenSameStateValueWithDifferentParameterTypes_ShouldPass()
    {
        // Arrange
        var info = new StateMachineInfo<string, string>(null);
        info.Add(
            new FromTransitionInfo<string, string>(
                info,
                "To D",
                [],
                new ConditionalStateInfo<string, string>(info, "D", [], []),
                [new StateInfo<string, string>(info, "B", []), new StateInfo<string, string>(info, "B", [typeof(int)])]
            )
        );
        var context = new StateMachineValidationContext<string, string> { Info = info };

        // Act
        DuplicateExcludedStateValidator.Validate(context);

        // Assert
        Assert.Empty(context.ValidationErrors);
    }

    [Fact]
    public void Validate_WhenSameParameterizedStateExcludedTwice_ShouldFail()
    {
        // Arrange
        var info = new StateMachineInfo<string, string>(null);
        info.Add(
            new FromTransitionInfo<string, string>(
                info,
                "To D",
                [],
                new ConditionalStateInfo<string, string>(info, "D", [], []),
                [
                    new StateInfo<string, string>(info, "B", [typeof(int)]),
                    new StateInfo<string, string>(info, "B", [typeof(int)]),
                ]
            )
        );
        var context = new StateMachineValidationContext<string, string> { Info = info };

        // Act
        DuplicateExcludedStateValidator.Validate(context);

        // Assert
        var error = Assert.Single(context.ValidationErrors);
        Assert.Contains("B<int>", error);
        Assert.Contains("has already been excluded", error);
    }

    [Fact]
    public void Validate_WhenMultipleTransitionsEachHaveDuplicate_ShouldReportAllErrors()
    {
        // Arrange
        var info = new StateMachineInfo<string, string>(null);
        info.Add(
            new FromTransitionInfo<string, string>(
                info,
                "To C",
                [],
                new ConditionalStateInfo<string, string>(info, "C", [], []),
                [new StateInfo<string, string>(info, "A", []), new StateInfo<string, string>(info, "A", [])]
            )
        );
        info.Add(
            new FromTransitionInfo<string, string>(
                info,
                "To D",
                [],
                new ConditionalStateInfo<string, string>(info, "D", [], []),
                [new StateInfo<string, string>(info, "B", []), new StateInfo<string, string>(info, "B", [])]
            )
        );
        var context = new StateMachineValidationContext<string, string> { Info = info };

        // Act
        DuplicateExcludedStateValidator.Validate(context);

        // Assert
        Assert.Collection(
            context.ValidationErrors,
            error => Assert.Contains("A", error),
            error => Assert.Contains("B", error)
        );
    }
}
