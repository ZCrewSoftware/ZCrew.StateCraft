using ZCrew.StateCraft.Info;
using ZCrew.StateCraft.Validation;

namespace ZCrew.StateCraft.UnitTests.Validation;

public class DuplicateStateValidatorTests
{
    [Fact]
    public void Validate_WhenStateMachineHasNoStates_ShouldPass()
    {
        // Arrange
        var info = new StateMachineInfo<string, string>(null);
        var context = new StateMachineValidationContext<string, string> { Info = info };

        // Act
        DuplicateStateValidator.Validate(context);

        // Assert
        Assert.Empty(context.ValidationErrors);
    }

    [Fact]
    public void Validate_WhenStateMachineHasSingleState_ShouldPass()
    {
        // Arrange
        var info = new StateMachineInfo<string, string>(null);
        info.Add(new StateInfo<string, string>(info, "A", []));
        var context = new StateMachineValidationContext<string, string> { Info = info };

        // Act
        DuplicateStateValidator.Validate(context);

        // Assert
        Assert.Empty(context.ValidationErrors);
    }

    [Fact]
    public void Validate_WhenStateMachineHasMultipleUniqueStates_ShouldPass()
    {
        // Arrange
        var info = new StateMachineInfo<string, string>(null);
        info.Add(new StateInfo<string, string>(info, "A", []));
        info.Add(new StateInfo<string, string>(info, "B", []));
        info.Add(new StateInfo<string, string>(info, "C", []));
        var context = new StateMachineValidationContext<string, string> { Info = info };

        // Act
        DuplicateStateValidator.Validate(context);

        // Assert
        Assert.Empty(context.ValidationErrors);
    }

    [Fact]
    public void Validate_WhenSameStateNameWithDifferentTypeParameters_ShouldPass()
    {
        // Arrange
        var info = new StateMachineInfo<string, string>(null);
        info.Add(new StateInfo<string, string>(info, "A", []));
        info.Add(new StateInfo<string, string>(info, "A", [typeof(int)]));
        var context = new StateMachineValidationContext<string, string> { Info = info };

        // Act
        DuplicateStateValidator.Validate(context);

        // Assert
        Assert.Empty(context.ValidationErrors);
    }

    [Fact]
    public void Validate_WhenSameStateNameWithDifferentTypeParameterTypes_ShouldPass()
    {
        // Arrange
        var info = new StateMachineInfo<string, string>(null);
        info.Add(new StateInfo<string, string>(info, "A", [typeof(int)]));
        info.Add(new StateInfo<string, string>(info, "A", [typeof(string)]));
        var context = new StateMachineValidationContext<string, string> { Info = info };

        // Act
        DuplicateStateValidator.Validate(context);

        // Assert
        Assert.Empty(context.ValidationErrors);
    }

    [Fact]
    public void Validate_WhenDuplicateParameterlessState_ShouldFail()
    {
        // Arrange
        var info = new StateMachineInfo<string, string>(null);
        info.Add(new StateInfo<string, string>(info, "A", []));
        info.Add(new StateInfo<string, string>(info, "A", []));
        var context = new StateMachineValidationContext<string, string> { Info = info };

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
        var info = new StateMachineInfo<string, string>(null);
        info.Add(new StateInfo<string, string>(info, "A", [typeof(int)]));
        info.Add(new StateInfo<string, string>(info, "A", [typeof(int)]));
        var context = new StateMachineValidationContext<string, string> { Info = info };

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
        var info = new StateMachineInfo<string, string>(null);
        info.Add(new StateInfo<string, string>(info, "A", []));
        info.Add(new StateInfo<string, string>(info, "A", []));
        info.Add(new StateInfo<string, string>(info, "B", []));
        info.Add(new StateInfo<string, string>(info, "B", []));
        var context = new StateMachineValidationContext<string, string> { Info = info };

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
        var info = new StateMachineInfo<string, string>(null);
        info.Add(new StateInfo<string, string>(info, "A", []));
        info.Add(new StateInfo<string, string>(info, "A", []));
        info.Add(new StateInfo<string, string>(info, "A", []));
        var context = new StateMachineValidationContext<string, string> { Info = info };

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
