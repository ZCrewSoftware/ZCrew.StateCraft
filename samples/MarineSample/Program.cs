using System.Text;
using MarineSample;

var marine = new Marine();
var display = new GameDisplay(marine);
var rng = new Random();

var stateMachine = new MarineStateMachineBuilder(marine, display, rng).CreateStateMachine();
display.SetStateMachine(stateMachine);

// --- Console Loop ---

var running = true;
Console.CancelKeyPress += (_, e) =>
{
    e.Cancel = true;
    running = false;
};

display.InitialRender();
await stateMachine.Activate();
await display.RefreshCommands();
display.Render();

using var renderCts = new CancellationTokenSource();
_ = display.StartRenderLoop(renderCts.Token);

var inputBuffer = new StringBuilder();

while (running)
{
    if (!Console.KeyAvailable)
    {
        await Task.Delay(50);
        continue;
    }

    var key = Console.ReadKey(true);

    if (key.Key == ConsoleKey.Enter)
    {
        var input = inputBuffer.ToString().Trim();
        inputBuffer.Clear();
        display.SetInputBuffer("");

        if (string.IsNullOrEmpty(input))
        {
            continue;
        }

        if (input.Equals("quit", StringComparison.OrdinalIgnoreCase))
        {
            display.LogAction("How do you take a leak in this damn suit?");
            display.LogWarning("(SCREAMS)");
            display.Render();
            break;
        }

        if (!Enum.TryParse<MarineTransition>(input, ignoreCase: true, out var transition))
        {
            display.LogError($"Unknown command: {input}");
            display.Render();
            continue;
        }

        if (!await stateMachine.TryTransition(transition))
        {
            display.LogWarning("Can't do that right now.");
        }

        await display.RefreshCommands();
        display.Render();
    }
    else if (key.Key == ConsoleKey.Backspace)
    {
        if (inputBuffer.Length > 0)
        {
            inputBuffer.Remove(inputBuffer.Length - 1, 1);
            display.SetInputBuffer(inputBuffer.ToString());
            display.Render();
        }
    }
    else if (key.Key == ConsoleKey.Escape)
    {
        inputBuffer.Clear();
        display.SetInputBuffer("");
        display.Render();
    }
    else if (!char.IsControl(key.KeyChar))
    {
        inputBuffer.Append(key.KeyChar);
        display.SetInputBuffer(inputBuffer.ToString());
        display.Render();
    }
}

await renderCts.CancelAsync();
await stateMachine.Deactivate();
display.Cleanup();
