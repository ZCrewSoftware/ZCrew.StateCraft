using ZCrew.StateCraft.Validation;

namespace ZCrew.StateCraft.UnitTests.Validation;

public class UnreachableTransitionValidatorTests
{
    [Fact]
    public void Validate_WhenStateMachineHasNoStates_ShouldPass()
    {
        // Arrange
        var configuration = StateMachine.Configure<string, string>();
        var context = new StateMachineValidationContext<string, string> { Configuration = configuration };

        // Act
        UnreachableTransitionValidator.Validate(context);

        // Assert
        Assert.Empty(context.ValidationErrors);
    }

    [Fact]
    public void Validate_WhenStateMachineHasNoTransitions_ShouldPass()
    {
        // Arrange
        var configuration = StateMachine.Configure<string, string>().WithState("A", state => state);
        var context = new StateMachineValidationContext<string, string> { Configuration = configuration };

        // Act
        UnreachableTransitionValidator.Validate(context);

        // Assert
        Assert.Empty(context.ValidationErrors);
    }

    [Fact]
    public void Validate_WhenSingleTransition_ShouldPass()
    {
        // Arrange
        var configuration = StateMachine
            .Configure<string, string>()
            .WithState("A", state => state.WithTransition("To B", "B"))
            .WithState("B", state => state);
        var context = new StateMachineValidationContext<string, string> { Configuration = configuration };

        // Act
        UnreachableTransitionValidator.Validate(context);

        // Assert
        Assert.Empty(context.ValidationErrors);
    }

    [Fact]
    public void Validate_WhenMultipleTransitionsToDifferentStates_ShouldPass()
    {
        // Arrange
        var configuration = StateMachine
            .Configure<string, string>()
            .WithState("A", state => state.WithTransition("To B", "B").WithTransition("To C", "C"))
            .WithState("B", state => state)
            .WithState("C", state => state);
        var context = new StateMachineValidationContext<string, string> { Configuration = configuration };

        // Act
        UnreachableTransitionValidator.Validate(context);

        // Assert
        Assert.Empty(context.ValidationErrors);
    }

    [Fact]
    public void Validate_WhenMultipleTransitionsToSameStateWithDifferentParameterCounts_ShouldPass()
    {
        // Arrange
        var configuration = StateMachine
            .Configure<string, string>()
            .WithState("A", state => state.WithTransition("To B", "B").WithTransition<int>("To B", "B"))
            .WithState("B", state => state.WithNoParameters())
            .WithState("B", state => state.WithParameter<int>());
        var context = new StateMachineValidationContext<string, string> { Configuration = configuration };

        // Act
        UnreachableTransitionValidator.Validate(context);

        // Assert
        Assert.Empty(context.ValidationErrors);
    }

    [Fact]
    public void Validate_WhenMultipleTransitionsToSameStateWithNonAssignableTypes_ShouldPass()
    {
        // Arrange
        var configuration = StateMachine
            .Configure<string, string>()
            .WithState(
                "A",
                state => state.WithTransition<int>("To B (int)", "B").WithTransition<string>("To B (string)", "B")
            )
            .WithState("B", state => state.WithParameter<int>())
            .WithState("B", state => state.WithParameter<string>());
        var context = new StateMachineValidationContext<string, string> { Configuration = configuration };

        // Act
        UnreachableTransitionValidator.Validate(context);

        // Assert
        Assert.Empty(context.ValidationErrors);
    }

    [Fact]
    public void Validate_WhenDerivedTypeTransitionFollowsBaseTypeTransition_ShouldFail()
    {
        // Arrange
        var configuration = StateMachine
            .Configure<string, string>()
            .WithState("A", state => state.WithTransition<object>("To B", "B").WithTransition<string>("To B", "B"))
            .WithState("B", state => state.WithParameter<object>())
            .WithState("B", state => state.WithParameter<string>());
        var context = new StateMachineValidationContext<string, string> { Configuration = configuration };

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
        var configuration = StateMachine
            .Configure<string, string>()
            .WithState("A", state => state.WithTransition<string>("To B", "B").WithTransition<object>("To B", "B"))
            .WithState("B", state => state.WithParameter<string>())
            .WithState("B", state => state.WithParameter<object>());
        var context = new StateMachineValidationContext<string, string> { Configuration = configuration };

        // Act
        UnreachableTransitionValidator.Validate(context);

        // Assert
        Assert.Empty(context.ValidationErrors);
    }

    [Fact]
    public void Validate_WhenDuplicateParameterlessTransitions_ShouldFail()
    {
        // Arrange
        var configuration = StateMachine
            .Configure<string, string>()
            .WithState("A", state => state.WithTransition("To B", "B").WithTransition("To B", "B"))
            .WithState("B", state => state);
        var context = new StateMachineValidationContext<string, string> { Configuration = configuration };

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
        var configuration = StateMachine
            .Configure<string, string>()
            .WithState("A", state => state.WithTransition<int>("To B", "B").WithTransition<int>("To B", "B"))
            .WithState("B", state => state.WithParameter<int>());
        var context = new StateMachineValidationContext<string, string> { Configuration = configuration };

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
        var configuration = StateMachine
            .Configure<string, string>()
            .WithState(
                "A",
                state => state.WithTransition("To B", "B").WithTransition("To B", "B").WithTransition("To B", "B")
            )
            .WithState("B", state => state);
        var context = new StateMachineValidationContext<string, string> { Configuration = configuration };

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
        var configuration = StateMachine
            .Configure<string, string>()
            .WithState("A", state => state.WithTransition("To C", "C").WithTransition("To C", "C"))
            .WithState("B", state => state.WithTransition("To C", "C").WithTransition("To C", "C"))
            .WithState("C", state => state);
        var context = new StateMachineValidationContext<string, string> { Configuration = configuration };

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
        var configuration = StateMachine
            .Configure<string, string>()
            .WithState("A", state => state.WithTransition("To C", "C"))
            .WithState("B", state => state.WithTransition("To C", "C"))
            .WithState("C", state => state);
        var context = new StateMachineValidationContext<string, string> { Configuration = configuration };

        // Act
        UnreachableTransitionValidator.Validate(context);

        // Assert
        Assert.Empty(context.ValidationErrors);
    }

    [Fact]
    public void Validate_WhenSecondParameterlessTransitionIsConditional_ShouldFail()
    {
        // Arrange
        var configuration = StateMachine
            .Configure<string, string>()
            .WithState(
                "A",
                state => state.WithTransition("To B", "B").WithTransition("To B", t => t.If(() => true).To("B"))
            )
            .WithState("B", state => state.WithParameter<int>());
        var context = new StateMachineValidationContext<string, string> { Configuration = configuration };

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
        var configuration = StateMachine
            .Configure<string, string>()
            .WithState(
                "A",
                state => state.WithTransition("To B", t => t.If(() => true).To("B")).WithTransition("To B", "B")
            )
            .WithState("B", state => state.WithParameter<int>());
        var context = new StateMachineValidationContext<string, string> { Configuration = configuration };

        // Act
        UnreachableTransitionValidator.Validate(context);

        // Assert
        Assert.Empty(context.ValidationErrors);
    }

    [Fact]
    public void Validate_WhenSecondParameterizedTransitionIsConditional_ShouldFail()
    {
        // Arrange
        var configuration = StateMachine
            .Configure<string, string>()
            .WithState(
                "A",
                state =>
                    state
                        .WithTransition<int>("To B", "B")
                        .WithTransition("To B", t => t.WithParameter<int>().If(_ => true).To("B"))
            )
            .WithState("B", state => state.WithParameter<int>());
        var context = new StateMachineValidationContext<string, string> { Configuration = configuration };

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
        var configuration = StateMachine
            .Configure<string, string>()
            .WithState(
                "A",
                state =>
                    state
                        .WithTransition("To B", t => t.WithParameter<int>().If(_ => true).To("B"))
                        .WithTransition<int>("To B", "B")
            )
            .WithState("B", state => state.WithParameter<int>());
        var context = new StateMachineValidationContext<string, string> { Configuration = configuration };

        // Act
        UnreachableTransitionValidator.Validate(context);

        // Assert
        Assert.Empty(context.ValidationErrors);
    }

    [Fact]
    public void Validate_WhenSingleReentrantParameterlessTransition_ShouldPass()
    {
        // Arrange
        var configuration = StateMachine
            .Configure<string, string>()
            .WithState("A", state => state.WithTransition("Loop", t => t.WithNoParameters().ToSameState()));
        var context = new StateMachineValidationContext<string, string> { Configuration = configuration };

        // Act
        UnreachableTransitionValidator.Validate(context);

        // Assert
        Assert.Empty(context.ValidationErrors);
    }

    [Fact]
    public void Validate_WhenDuplicateReentrantParameterlessTransitions_ShouldFail()
    {
        // Arrange
        var configuration = StateMachine
            .Configure<string, string>()
            .WithState(
                "A",
                state =>
                    state
                        .WithTransition("Loop", t => t.WithNoParameters().ToSameState())
                        .WithTransition("Loop", t => t.WithNoParameters().ToSameState())
            );
        var context = new StateMachineValidationContext<string, string> { Configuration = configuration };

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
        var configuration = StateMachine
            .Configure<string, string>()
            .WithState(
                "A",
                state => state.WithParameter<int>().WithTransition("Loop", t => t.WithSameParameter().ToSameState())
            );
        var context = new StateMachineValidationContext<string, string> { Configuration = configuration };

        // Act
        UnreachableTransitionValidator.Validate(context);

        // Assert
        Assert.Empty(context.ValidationErrors);
    }

    [Fact]
    public void Validate_WhenDuplicateReentrantWithSameParameterTransitions_ShouldFail()
    {
        // Arrange
        var configuration = StateMachine
            .Configure<string, string>()
            .WithState(
                "A",
                state =>
                    state
                        .WithParameter<int>()
                        .WithTransition("Loop", t => t.WithSameParameter().ToSameState())
                        .WithTransition("Loop", t => t.WithSameParameter().ToSameState())
            );
        var context = new StateMachineValidationContext<string, string> { Configuration = configuration };

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
        var configuration = StateMachine
            .Configure<string, string>()
            .WithState(
                "A",
                state => state.WithParameter<int>().WithTransition("Loop", t => t.WithParameter<int>().ToSameState())
            );
        var context = new StateMachineValidationContext<string, string> { Configuration = configuration };

        // Act
        UnreachableTransitionValidator.Validate(context);

        // Assert
        Assert.Empty(context.ValidationErrors);
    }

    [Fact]
    public void Validate_WhenDuplicateReentrantParameterizedTransitions_ShouldFail()
    {
        // Arrange
        var configuration = StateMachine
            .Configure<string, string>()
            .WithState(
                "A",
                state =>
                    state
                        .WithParameter<int>()
                        .WithTransition("Loop", t => t.WithParameter<int>().ToSameState())
                        .WithTransition("Loop", t => t.WithParameter<int>().ToSameState())
            );
        var context = new StateMachineValidationContext<string, string> { Configuration = configuration };

        // Act
        UnreachableTransitionValidator.Validate(context);

        // Assert
        var error = Assert.Single(context.ValidationErrors);
        Assert.Contains("Loop(A<int>) ↩", error);
    }
}
