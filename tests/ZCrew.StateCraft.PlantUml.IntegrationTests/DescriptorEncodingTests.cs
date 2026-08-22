namespace ZCrew.StateCraft.PlantUml.IntegrationTests;

public class DescriptorEncodingTests
{
    private static bool IsAuthorized() => true;

    [Fact]
    public void ToPlantUmlDiagram_WhenDescriptorContainsAngleBrackets_ShouldReplaceThemWithUnicodeEscapes()
    {
        // Arrange
        var configuration = StateMachine
            .Configure<string, string>()
            .WithInitialState("Idle")
            .WithState("Idle", state => state.WithTransition("Go", t => t.WithParameter<int>().To("Working")))
            .WithState("Working", state => state.WithParameter<int>());

        // Act
        var diagram = configuration.ToPlantUmlDiagram();

        // Assert
        Assert.Contains("Go<U+003C>int<U+003E>", diagram);
        Assert.DoesNotContain("Go<int>", diagram);
    }

    [Fact]
    public void ToPlantUmlDiagram_WhenDescriptorContainsConsecutiveSpaces_ShouldPreserveThemWithNonBreakingSpaces()
    {
        // Arrange
        var configuration = StateMachine
            .Configure<string, string>()
            .WithInitialState("Idle")
            .WithState("Idle", state => state.WithTransition("Go", t => t.If(IsAuthorized, "a  b").To("Working")))
            .WithState("Working", state => state);

        // Act
        var diagram = configuration.ToPlantUmlDiagram();

        // Assert
        Assert.Contains("If: a <U+00A0>b", diagram);
    }

    [Fact]
    public void ToPlantUmlDiagram_WhenNewlineIsIgnore_ShouldStripNewlinesFromDescriptors()
    {
        // Arrange
        var configuration = NewConfigurationWithDescriptor("first\nsecond");

        // Act
        var diagram = configuration.ToPlantUmlDiagram(options => options.Newline = PlantUmlNewline.Ignore);

        // Assert
        Assert.Contains("If: firstsecond", diagram);
    }

    [Fact]
    public void ToPlantUmlDiagram_WhenNewlineIsSpace_ShouldReplaceNewlinesWithASingleSpace()
    {
        // Arrange
        var configuration = NewConfigurationWithDescriptor("first\nsecond");

        // Act
        var diagram = configuration.ToPlantUmlDiagram(options => options.Newline = PlantUmlNewline.Space);

        // Assert
        Assert.Contains("If: first second", diagram);
    }

    [Fact]
    public void ToPlantUmlDiagram_WhenNewlineIsLineBreak_ShouldReplaceNewlinesWithThePlantUmlEscape()
    {
        // Arrange
        var configuration = NewConfigurationWithDescriptor("first\nsecond");

        // Act
        var diagram = configuration.ToPlantUmlDiagram(options => options.Newline = PlantUmlNewline.LineBreak);

        // Assert
        Assert.Contains(@"If: first\nsecond", diagram);
    }

    [Fact]
    public void ToPlantUmlDiagram_WhenDescriptorContainsCarriageReturnLineFeed_ShouldTreatThePairAsOneNewline()
    {
        // Arrange
        var configuration = NewConfigurationWithDescriptor("first\r\nsecond");

        // Act
        var diagram = configuration.ToPlantUmlDiagram(options => options.Newline = PlantUmlNewline.Space);

        // Assert
        Assert.Contains("If: first second", diagram);
    }

    [Fact]
    public void ToPlantUmlDiagram_WhenDescriptorNeedsNoEncoding_ShouldEmitItUnchanged()
    {
        // Arrange
        var configuration = NewConfigurationWithDescriptor("plain descriptor");

        // Act
        var diagram = configuration.ToPlantUmlDiagram();

        // Assert
        Assert.Contains("If: plain descriptor", diagram);
    }

    private static IStateMachineConfiguration<string, string> NewConfigurationWithDescriptor(string descriptor)
    {
        return StateMachine
            .Configure<string, string>()
            .WithInitialState("Idle")
            .WithState("Idle", state => state.WithTransition("Go", t => t.If(IsAuthorized, descriptor).To("Working")))
            .WithState("Working", state => state);
    }
}
