namespace ZCrew.StateCraft.Mermaid.IntegrationTests;

public class DescriptorEncodingTests
{
    [Fact]
    public void ToMermaidDiagram_WhenDescriptorContainsAngleBrackets_ShouldEscapeThemInDescriptorButNotInIdentifier()
    {
        // Arrange
        var configuration = StateMachine
            .Configure<string, string>()
            .WithInitialState("Idle")
            .WithState("Idle", state => state)
            .WithState("Working", state => state.WithParameter<int>());

        // Act
        var diagram = configuration.ToMermaidDiagram();

        // Assert
        Assert.Contains("    Working_System.Int32: Working#lt;int#gt;", diagram);
        Assert.DoesNotContain("Working_System.Int32: Working<int>", diagram);
    }

    [Fact]
    public void ToMermaidDiagram_WhenDescriptorContainsConsecutiveSpaces_ShouldReplaceSecondAndLaterSpacesWithNbsp()
    {
        // Arrange
        var configuration = StateMachine
            .Configure<string, string>()
            .WithInitialState("Idle")
            .WithState("Idle", state => state)
            .WithState("Two  Spaces", state => state);

        // Act
        var diagram = configuration.ToMermaidDiagram();

        // Assert
        Assert.Contains("    Two  Spaces: Two #nbsp;Spaces", diagram);
    }

    [Fact]
    public void ToMermaidDiagram_WhenNewlineOptionIsIgnore_ShouldStripNewlinesFromDescriptors()
    {
        // Arrange
        var configuration = StateMachine
            .Configure<string, string>()
            .WithInitialState("Idle")
            .WithState("Idle", state => state)
            .WithState("Multi\nLine", state => state);

        // Act
        var diagram = configuration.ToMermaidDiagram(options => options.Newline = MermaidNewline.Ignore);

        // Assert
        Assert.Contains(": MultiLine", diagram);
    }

    [Fact]
    public void ToMermaidDiagram_WhenNewlineOptionIsSpace_ShouldReplaceNewlinesInDescriptorsWithSpace()
    {
        // Arrange
        var configuration = StateMachine
            .Configure<string, string>()
            .WithInitialState("Idle")
            .WithState("Idle", state => state)
            .WithState("Multi\nLine", state => state);

        // Act
        var diagram = configuration.ToMermaidDiagram(options => options.Newline = MermaidNewline.Space);

        // Assert
        Assert.Contains(": Multi Line", diagram);
    }

    [Fact]
    public void ToMermaidDiagram_WhenNewlineOptionIsHtmlSingleLineBreak_ShouldReplaceNewlinesInDescriptorsWithBrTag()
    {
        // Arrange
        var configuration = StateMachine
            .Configure<string, string>()
            .WithInitialState("Idle")
            .WithState("Idle", state => state)
            .WithState("Multi\nLine", state => state);

        // Act
        var diagram = configuration.ToMermaidDiagram(options => options.Newline = MermaidNewline.HtmlSingleLineBreak);

        // Assert
        Assert.Contains(": Multi<br/>Line", diagram);
    }

    [Fact]
    public void ToMermaidDiagram_WhenStateNameContainsAngleBrackets_ShouldLeaveIdentifierRawAndEncodeOnlyDescriptor()
    {
        // Arrange
        var configuration = StateMachine
            .Configure<string, string>()
            .WithInitialState("Idle")
            .WithState("Idle", state => state)
            .WithState("<weird>", state => state);

        // Act
        var diagram = configuration.ToMermaidDiagram();

        // Assert
        Assert.Contains("    <weird>: #lt;weird#gt;", diagram);
    }
}
