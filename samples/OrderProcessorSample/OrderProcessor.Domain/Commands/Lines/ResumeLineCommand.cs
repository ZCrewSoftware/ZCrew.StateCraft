namespace OrderProcessor.Domain.Commands.Lines;

public record ResumeLineCommand(Guid OrderId, Guid LineId);
