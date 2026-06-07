using ZCrew.StateCraft.Info;
using ZCrew.StateCraft.Validation;

namespace ZCrew.StateCraft.UnitTests.Validation;

public class MissingExcludedStateValidatorTests
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
        MissingExcludedStateValidator.Validate(context);

        // Assert
        Assert.Empty(context.ValidationErrors);
    }

    [Fact]
    public void Validate_WhenExcludedStateExists_ShouldPass()
    {
        // Arrange
        var info = new StateMachineInfo<string, string>(null);
        info.Add(new StateInfo<string, string>(info, "A", []));
        info.Add(new StateInfo<string, string>(info, "D", []));
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
        MissingExcludedStateValidator.Validate(context);

        // Assert
        Assert.Empty(context.ValidationErrors);
    }

    [Fact]
    public void Validate_WhenExcludedParameterizedStateExists_ShouldPass()
    {
        // Arrange
        var info = new StateMachineInfo<string, string>(null);
        info.Add(new StateInfo<string, string>(info, "B", [typeof(int)]));
        info.Add(new StateInfo<string, string>(info, "D", []));
        info.Add(
            new FromTransitionInfo<string, string>(
                info,
                "To D",
                [],
                new ConditionalStateInfo<string, string>(info, "D", [], []),
                [new StateInfo<string, string>(info, "B", [typeof(int)])]
            )
        );
        var context = new StateMachineValidationContext<string, string> { Info = info };

        // Act
        MissingExcludedStateValidator.Validate(context);

        // Assert
        Assert.Empty(context.ValidationErrors);
    }

    [Fact]
    public void Validate_WhenExcludedStateDoesNotExist_ShouldFail()
    {
        // Arrange
        var info = new StateMachineInfo<string, string>(null);
        info.Add(new StateInfo<string, string>(info, "A", []));
        info.Add(new StateInfo<string, string>(info, "B", []));
        info.Add(
            new FromTransitionInfo<string, string>(
                info,
                "To D",
                [],
                new ConditionalStateInfo<string, string>(info, "D", [], []),
                [new StateInfo<string, string>(info, "Z", [])]
            )
        );
        var context = new StateMachineValidationContext<string, string> { Info = info };

        // Act
        MissingExcludedStateValidator.Validate(context);

        // Assert
        var error = Assert.Single(context.ValidationErrors);
        Assert.Contains("was not found", error);
    }

    [Fact]
    public void Validate_WhenExcludedStateHasWrongArityWithParameterizedRegistered_ShouldSuggestExcept()
    {
        // Arrange
        var info = new StateMachineInfo<string, string>(null);
        info.Add(new StateInfo<string, string>(info, "B", [typeof(int)]));
        info.Add(
            new FromTransitionInfo<string, string>(
                info,
                "To D",
                [],
                new ConditionalStateInfo<string, string>(info, "D", [], []),
                [new StateInfo<string, string>(info, "B", [])]
            )
        );
        var context = new StateMachineValidationContext<string, string> { Info = info };

        // Act
        MissingExcludedStateValidator.Validate(context);

        // Assert
        var error = Assert.Single(context.ValidationErrors);
        Assert.Contains("was not found", error);
        Assert.Contains("Specify the correct parameters like:", error);
        Assert.Contains("Except<int>(String.B)", error);
    }

    [Fact]
    public void Validate_WhenExcludedStateHasWrongArityWithParameterlessRegistered_ShouldSuggestExcept()
    {
        // Arrange
        var info = new StateMachineInfo<string, string>(null);
        info.Add(new StateInfo<string, string>(info, "B", []));
        info.Add(
            new FromTransitionInfo<string, string>(
                info,
                "To D",
                [],
                new ConditionalStateInfo<string, string>(info, "D", [], []),
                [new StateInfo<string, string>(info, "B", [typeof(int)])]
            )
        );
        var context = new StateMachineValidationContext<string, string> { Info = info };

        // Act
        MissingExcludedStateValidator.Validate(context);

        // Assert
        var error = Assert.Single(context.ValidationErrors);
        Assert.Contains("was not found", error);
        Assert.Contains("Specify the correct parameters like:", error);
        Assert.Contains("Except(String.B)", error);
    }

    [Fact]
    public void Validate_WhenExcludedStateMatchesMultipleStatesByValue_ShouldFail()
    {
        // Arrange
        var info = new StateMachineInfo<string, string>(null);
        info.Add(new StateInfo<string, string>(info, "B", [typeof(int)]));
        info.Add(new StateInfo<string, string>(info, "B", [typeof(string)]));
        info.Add(
            new FromTransitionInfo<string, string>(
                info,
                "To D",
                [],
                new ConditionalStateInfo<string, string>(info, "D", [], []),
                [new StateInfo<string, string>(info, "B", [])]
            )
        );
        var context = new StateMachineValidationContext<string, string> { Info = info };

        // Act
        MissingExcludedStateValidator.Validate(context);

        // Assert
        var error = Assert.Single(context.ValidationErrors);
        Assert.Contains("states with the same value were registered", error);
        Assert.Contains("Specify the correct parameters", error);
    }

    [Fact]
    public void Validate_WhenMultipleExcludedStatesMissing_ShouldReportAllErrors()
    {
        // Arrange
        var info = new StateMachineInfo<string, string>(null);
        info.Add(new StateInfo<string, string>(info, "A", []));
        info.Add(
            new FromTransitionInfo<string, string>(
                info,
                "To D",
                [],
                new ConditionalStateInfo<string, string>(info, "D", [], []),
                [new StateInfo<string, string>(info, "Y", []), new StateInfo<string, string>(info, "Z", [])]
            )
        );
        var context = new StateMachineValidationContext<string, string> { Info = info };

        // Act
        MissingExcludedStateValidator.Validate(context);

        // Assert
        Assert.Collection(
            context.ValidationErrors,
            error => Assert.Contains("was not found", error),
            error => Assert.Contains("was not found", error)
        );
    }

    [Fact]
    public void Validate_WhenSomeFromTransitionsValid_ShouldReportOnlyInvalid()
    {
        // Arrange
        var info = new StateMachineInfo<string, string>(null);
        info.Add(new StateInfo<string, string>(info, "A", []));
        info.Add(new StateInfo<string, string>(info, "B", []));
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
                [new StateInfo<string, string>(info, "Z", [])]
            )
        );
        var context = new StateMachineValidationContext<string, string> { Info = info };

        // Act
        MissingExcludedStateValidator.Validate(context);

        // Assert
        var error = Assert.Single(context.ValidationErrors);
        Assert.Contains("was not found", error);
    }
}
