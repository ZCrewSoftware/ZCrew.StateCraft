using Spectre.Console;
using Spectre.Console.Rendering;
using ZCrew.StateCraft.StateMachines.Contracts;

namespace MarineSample;

public class GameDisplay
{
    private readonly Lock renderLock = new();
    private readonly List<string> logEntries = [];
    private readonly Marine marine;

    private IStateMachine<MarineState, MarineTransition>? stateMachine;
    private string inputBuffer = "";
    private MarineState currentState = MarineState.Idle;
    private string[] availableCommands = [];
    private bool dirty = true;

    private const int LogLines = 8;

    public GameDisplay(Marine marine)
    {
        this.marine = marine;
    }

    public void SetStateMachine(IStateMachine<MarineState, MarineTransition> sm)
    {
        this.stateMachine = sm;
    }

    public void LogAction(string message)
    {
        AddLog($"[green]{Markup.Escape(message)}[/]");
    }

    public void LogWarning(string message)
    {
        AddLog($"[yellow]{Markup.Escape(message)}[/]");
    }

    public void LogError(string message)
    {
        AddLog($"[red]{Markup.Escape(message)}[/]");
    }

    public void LogStateChange(MarineState from, MarineTransition transition, MarineState to)
    {
        AddLog($"[magenta][[{from}]] --{transition}--> [[{to}]][/]");
    }

    public void UpdateState(MarineState state)
    {
        lock (this.renderLock)
        {
            this.currentState = state;
            this.dirty = true;
        }
    }

    public void SetInputBuffer(string value)
    {
        lock (this.renderLock)
        {
            this.inputBuffer = value;
            this.dirty = true;
        }
    }

    public async Task RefreshCommands(CancellationToken token = default)
    {
        if (this.stateMachine is null)
        {
            return;
        }

        var commands = new List<string>();
        foreach (var t in Enum.GetValues<MarineTransition>())
        {
            if (await this.stateMachine.CanTransition(t, token))
            {
                commands.Add(t.ToString());
            }
        }

        lock (this.renderLock)
        {
            this.availableCommands = [.. commands];
            this.dirty = true;
        }
    }

    public async Task StartRenderLoop(CancellationToken token)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(500));
        try
        {
            while (await timer.WaitForNextTickAsync(token))
            {
                await RefreshCommands(token);
                Render();
            }
        }
        catch (OperationCanceledException) { }
    }

    public void Render()
    {
        lock (this.renderLock)
        {
            if (!this.dirty)
            {
                return;
            }
            this.dirty = false;

            Console.CursorVisible = false;
            Console.Write("\x1b[H"); // cursor home

            // --- Row 1: Header panel ---
            var hpColor = GetHpColor();
            var statsLine = new Markup(
                $"[white bold]{Markup.Escape(this.marine.Name)}[/]"
                    + $" [grey]|[/] HP: [{hpColor} bold]{this.marine.Health}/{this.marine.MaxHealth}[/]"
                    + $" [grey]|[/] Kills: [white]{this.marine.Kills}[/]"
                    + $" [grey]|[/] Pos: [white]({this.marine.Position.X},{this.marine.Position.Y})[/]"
                    + $" [grey]|[/] State: [cyan bold]{this.currentState}[/]"
            );

            var commandsLine = GetCommandsLine();
            AnsiConsole.Write(
                new Panel(new Rows(statsLine, commandsLine))
                    .Header("[bold cyan] TERRAN MARINE COMMAND CONSOLE [/]")
                    .HeaderAlignment(Justify.Center)
                    .Border(BoxBorder.Rounded)
                    .BorderStyle(Style.Parse("cyan"))
                    .Padding(2, 0)
                    .Expand()
            );

            // --- Row 2: Log panel ---
            var entries = this.logEntries.ToArray();
            var logRows = new List<IRenderable>();
            for (var i = 0; i < LogLines; i++)
            {
                logRows.Add(i < entries.Length ? new Markup(entries[i]) : new Text(""));
            }

            AnsiConsole.Write(
                new Panel(new Rows(logRows))
                    .Header("[grey] Combat Log [/]")
                    .Border(BoxBorder.Rounded)
                    .BorderStyle(Style.Parse("grey"))
                    .Padding(2, 0)
                    .Expand()
            );

            // --- Row 3: Input panel ---
            AnsiConsole.Write(
                new Panel(new Markup($"[white bold]>[/] {Markup.Escape(this.inputBuffer)}"))
                    .Border(BoxBorder.Rounded)
                    .BorderStyle(Style.Parse("white"))
                    .Padding(2, 0)
                    .Expand()
            );

            Console.Write("\x1b[J"); // clear below panels
            // Position cursor inside input panel: up 2 (over bottom border + blank),
            // then to the column after typed text
            Console.Write("\x1b[2A");
            Console.Write($"\x1b[{this.inputBuffer.Length + 6}G");
            Console.CursorVisible = true;
        }
    }

    public void InitialRender()
    {
        AnsiConsole.Clear();
        this.dirty = true;
        Render();
    }

    public void Cleanup()
    {
        Console.CursorVisible = true;
        AnsiConsole.Reset();
    }

    private string GetHpColor()
    {
        return this.marine.Health switch
        {
            > 50 => "green",
            > 25 => "yellow",
            _ => "red",
        };
    }

    private IRenderable GetCommandsLine()
    {
        if (this.availableCommands.Length <= 0)
        {
            return new Markup("Commands: [grey]quit to exit[/]");
        }

        var commands = string.Join("[grey],[/] ", this.availableCommands.Select(c => $"[yellow]{c}[/]"));
        return new Markup($"Commands: {commands} [grey]| quit to exit[/]");
    }

    private void AddLog(string markup)
    {
        lock (this.renderLock)
        {
            this.logEntries.Add(markup);
            while (this.logEntries.Count > LogLines)
            {
                this.logEntries.RemoveAt(0);
            }
            this.dirty = true;
        }
    }
}
