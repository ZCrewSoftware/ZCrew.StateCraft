using ZCrew.StateCraft.Validation;
using ZCrew.StateCraft.Validation.Models;

namespace ZCrew.StateCraft.UnitTests.Validation;

public class DuplicateStateValidatorTests
{
    [Fact]
    public void Validate_WhenStateMachineHasNoStates_ShouldPass()
    {
        // Arrange
        var context = new StateMachineValidationContext<string, string>();

        // Act
        DuplicateStateValidator.Validate(context);

        // Assert
        Assert.Empty(context.ValidationErrors);
    }

    [Fact]
    public void Validate_WhenStateMachineHasSingleState_ShouldPass()
    {
        // Arrange
        var context = new StateMachineValidationContext<string, string>
        {
            States = { new StateValidationModel<string, string>("A", []) },
        };

        // Act
        DuplicateStateValidator.Validate(context);

        // Assert
        Assert.Empty(context.ValidationErrors);
    }

    [Fact]
    public void Validate_WhenStateMachineHasMultipleUniqueStates_ShouldPass()
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
        };

        // Act
        DuplicateStateValidator.Validate(context);

        // Assert
        Assert.Empty(context.ValidationErrors);
    }

    [Fact]
    public void Validate_WhenSameStateNameWithDifferentTypeParameters_ShouldPass()
    {
        // Arrange
        var context = new StateMachineValidationContext<string, string>
        {
            States =
            {
                new StateValidationModel<string, string>("A", []),
                new StateValidationModel<string, string>("A", [typeof(int)]),
            },
        };

        // Act
        DuplicateStateValidator.Validate(context);

        // Assert
        Assert.Empty(context.ValidationErrors);
    }

    [Fact]
    public void Validate_WhenSameStateNameWithDifferentTypeParameterTypes_ShouldPass()
    {
        // Arrange
        var context = new StateMachineValidationContext<string, string>
        {
            States =
            {
                new StateValidationModel<string, string>("A", [typeof(int)]),
                new StateValidationModel<string, string>("A", [typeof(string)]),
            },
        };

        // Act
        DuplicateStateValidator.Validate(context);

        // Assert
        Assert.Empty(context.ValidationErrors);
    }

    [Fact]
    public void Validate_WhenDuplicateParameterlessState_ShouldFail()
    {
        // Arrange
        var context = new StateMachineValidationContext<string, string>
        {
            States =
            {
                new StateValidationModel<string, string>("A", []),
                new StateValidationModel<string, string>("A", []),
            },
        };

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
        var context = new StateMachineValidationContext<string, string>
        {
            States =
            {
                new StateValidationModel<string, string>("A", [typeof(int)]),
                new StateValidationModel<string, string>("A", [typeof(int)]),
            },
        };

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
        var context = new StateMachineValidationContext<string, string>
        {
            States =
            {
                new StateValidationModel<string, string>("A", []),
                new StateValidationModel<string, string>("A", []),
                new StateValidationModel<string, string>("B", []),
                new StateValidationModel<string, string>("B", []),
            },
        };

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
        var context = new StateMachineValidationContext<string, string>
        {
            States =
            {
                new StateValidationModel<string, string>("A", []),
                new StateValidationModel<string, string>("A", []),
                new StateValidationModel<string, string>("A", []),
            },
        };

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
