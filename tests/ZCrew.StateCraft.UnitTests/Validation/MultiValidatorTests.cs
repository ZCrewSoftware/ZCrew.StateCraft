namespace ZCrew.StateCraft.UnitTests.Validation;

public class MultiValidatorTests
{
    [Fact]
    public void Validate_WhenAllThreeValidatorsFail_ShouldReportAllErrors()
    {
        // Arrange
        var configuration = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState(
                "A",
                state =>
                    state
                        .WithTransition("To Z", t => t.To("Z"))
                        .WithTransition("To Z", t => t.To("Z"))
            )
            .WithState("A", state => state);

        // Act
        var build = () => configuration.Build(StateMachineBuildOptions.Validate);

        // Assert
        var exception = Assert.Throws<InvalidOperationException>(build);
        Assert.Contains("duplicated", exception.Message);
        Assert.Contains("no matching next state", exception.Message);
        Assert.Contains("unreachable", exception.Message);
    }

    [Fact]
    public void Validate_WhenTwoValidatorsFail_ShouldReportBothErrors()
    {
        // Arrange
        var configuration = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition("To Z", t => t.To("Z")))
            .WithState("A", state => state.WithTransition("To B", t => t.To("B")))
            .WithState("B", state => state);

        // Act
        var build = () => configuration.Build(StateMachineBuildOptions.Validate);

        // Assert
        var exception = Assert.Throws<InvalidOperationException>(build);
        Assert.Contains("duplicated", exception.Message);
        Assert.Contains("no matching next state", exception.Message);
    }

    [Fact]
    public void Validate_WhenOneValidatorFails_ShouldReportSingleError()
    {
        // Arrange
        var configuration = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state)
            .WithState("A", state => state);

        // Act
        var build = () => configuration.Build(StateMachineBuildOptions.Validate);

        // Assert
        var exception = Assert.Throws<InvalidOperationException>(build);
        Assert.Contains("duplicated", exception.Message);
        Assert.DoesNotContain("no matching next state", exception.Message);
        Assert.DoesNotContain("unreachable", exception.Message);
    }
}
