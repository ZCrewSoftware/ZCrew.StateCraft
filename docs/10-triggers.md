# Triggers

Triggers are autonomous transition initiators that run in the background while the state machine is active.
They await an asynchronous signal and then execute an action, typically transitioning the state machine.

Triggers are useful when a component of a system needs to transition the state machine but it is not possible or
desirable to call upon the state machine directly.

Triggers can be configured at two levels:

- **Machine-level** — active for the entire lifetime of the state machine
- **State-scoped** — active only while a specific state is entered, deactivated on exit, and reactivated on reentry

## Configuration

### Machine-Level Triggers

Machine-level triggers are configured using `WithTrigger` on the state machine configuration:

```csharp
var signal = new TaskCompletionSource();

var machine = StateMachine
    .Configure<State, Transition>()
    .WithInitialState(State.Idle)
    .WithState(State.Idle, state => state
        .WithTransition(Transition.Start, State.Running))
    .WithState(State.Running, state => state)
    .WithTrigger(trigger => trigger
        .Once()
        .Await(token => signal.Task.WaitAsync(token))
        .ThenInvoke(async (sm, token) => await sm.Transition(Transition.Start, token)))
    .Build();
```

### State-Scoped Triggers

State-scoped triggers are configured using `WithTrigger` on a state configuration. They activate when the state is
entered (after `OnEntry` handlers) and deactivate when the state is exited (before `OnExit` handlers):

```csharp
var workQueue = Channel.CreateUnbounded<WorkItem>();

var machine = StateMachine
    .Configure<State, Transition>()
    .WithInitialState(State.Idle)
    .WithState(State.Idle, state => state
        .WithTransition(Transition.Start, State.Processing))
    .WithState(State.Processing, state => state
        .WithTrigger(trigger => trigger
            .Repeat()
            .Await(async token => await workQueue.Reader.WaitToReadAsync(token))
            .ThenInvoke(async (sm, token) => await sm.Transition(Transition.ProcessNext, token)))
        .WithTransition(Transition.ProcessNext, State.Processing)
        .WithTransition(Transition.Done, State.Complete))
    .WithState(State.Complete, state => state)
    .Build();
```

When the state machine transitions away from `Processing`, the trigger is deactivated. If the state machine
transitions back to `Processing`, the trigger is reactivated and its `TriggeredCount` resets to zero.

### Handler Signatures

Signals support three signatures, consistent with all StateCraft handlers:

```csharp
// Signal signatures
.Await(() => blockingOperation())                          // Synchronous
.Await(async token => await signal.Task.WaitAsync(token))  // Async Task
.Await(token => signal.WaitAsync(token))                   // Async ValueTask
```

The work that is triggered by the signal supports the three typical handler signatures as well as three handlers that
receive the state machine instance.

```csharp
// ThenInvoke signatures (without state machine access)
.ThenInvoke(() => DoWork())                             // Synchronous
.ThenInvoke(async token => await DoWorkAsync(token))    // Async Task
.ThenInvoke(token => DoWorkAsync(token))                // Async ValueTask

// ThenInvoke signatures (with state machine access)
.ThenInvoke(sm => OnTriggered(sm))                                             // Synchronous
.ThenInvoke(async (sm, token) => await sm.Transition(Transition.Start, token)) // Async Task
.ThenInvoke((sm, token) => OnTriggeredAsync(sm, token))                        // Async ValueTask
```

### Multiple Triggers

A state machine can have multiple triggers at both levels. Machine-level triggers and state-scoped triggers can
coexist:

```csharp
var machine = StateMachine
    .Configure<State, Transition>()
    .WithInitialState(State.Idle)
    .WithState(State.Idle, state => state
        .WithTrigger(trigger => trigger
            .Once()
            .Await(token => startSignal.Task.WaitAsync(token))
            .ThenInvoke(async (sm, token) => await sm.Transition(Transition.Start, token)))
        .WithTransition(Transition.Start, State.Running)
        .WithTransition(Transition.Cancel, State.Canceled))
    .WithState(State.Running, state => state)
    .WithState(State.Canceled, state => state)
    .WithTrigger(trigger => trigger
        .Once()
        .Await(token => cancelSignal.Task.WaitAsync(token))
        .ThenInvoke(async (sm, token) => await sm.Transition(Transition.Cancel, token)))
    .Build();
```

When a state has multiple triggers and one of them transitions the state machine, the remaining triggers on that
state are deactivated as part of the exit sequence.

## Lifecycle

### Activation and Deactivation

Machine-level and state-scoped triggers activate at different points in the state machine lifecycle:

**Activation order:**

1. State machine activator runs
2. Initial state is activated (`OnActivate`)
3. **Machine-level triggers are activated**
4. Initial state's `OnEntry` runs
5. **Initial state's state-scoped triggers are activated**
6. Initial state action starts

**Transition order (State A → State B):**

1. Cancel and await State A's action (if asynchronous)
2. **State A's state-scoped triggers are deactivated**
3. State A's `OnExit` runs
4. `OnStateChange` handlers run
5. State B's `OnEntry` runs
6. **State B's state-scoped triggers are activated**
7. State B's action starts

**Deactivation order:**

1. Cancel and await final state's action (if asynchronous)
2. **Final state's state-scoped triggers are deactivated**
3. Final state's `OnExit` runs
4. **Machine-level triggers are deactivated**
5. Final state is deactivated (`OnDeactivate`)

When a trigger is deactivated, its cancellation token is canceled and the background task is awaited.
The trigger's `TriggeredCount` is reset to zero on deactivation.

### RunOnce

A `Once()` trigger executes a single time per activation cycle. After the signal completes and the action runs, the
trigger remains dormant until it is deactivated and reactivated. For machine-level triggers, this means the state
machine must be deactivated and reactivated. For state-scoped triggers, transitioning away and back to the state
reactivates the trigger:

```csharp
var notification = new TaskCompletionSource();

.WithTrigger(trigger => trigger
    .Once()
    .Await(token => notification.Task.WaitAsync(token))
    .ThenInvoke(async (sm, token) => await sm.Transition(Transition.Process, token)))
```

After deactivation and reactivation, the trigger resets and awaits the signal again.

### Repeat

A `Repeat()` trigger loops continuously. After each signal-and-action cycle completes, it awaits the signal again:

```csharp
var workQueue = Channel.CreateUnbounded<WorkItem>();

.WithTrigger(trigger => trigger
    .Repeat()
    .Await(async token => await workQueue.Reader.WaitToReadAsync(token))
    .ThenInvoke(async (sm, token) => await sm.Transition(Transition.ProcessNext, token)))
```

The loop continues until the trigger is deactivated or an exception is thrown. For machine-level triggers, this is
when the state machine is deactivated. For state-scoped triggers, this is when the state is exited.

### Exception Handling

Exceptions thrown in either the signal or the action are routed through the state machine's exception handling
mechanism (`OnException`). After an exception, the trigger stops and does not reactivate:

```csharp
var machine = StateMachine
    .Configure<State, Transition>()
    .WithInitialState(State.Idle)
    .OnException(ex =>
    {
        Console.WriteLine($"Trigger error: {ex.Message}");
        return ExceptionResult.Continue();
    })
    .WithState(State.Idle, state => state)
    .WithTrigger(trigger => trigger
        .Repeat()
        .Await(token => signal.WaitAsync(token))
        .ThenInvoke(() => ProcessWork()))
    .Build();
```

This applies to both `Once()` and `Repeat()` triggers. A repeating trigger that throws on its third iteration will
not loop again after the exception.

`OperationCanceledException` thrown when the trigger's cancellation token is canceled is suppressed and does not invoke
the exception handler. This is the expected path during deactivation.

## Example

```csharp
enum State { Idle, Running, Canceled, Done }
enum Transition { Start, Cancel, Complete }

var startSignal = new TaskCompletionSource();
var externalCancellation = new CancellationTokenSource();
var workAvailable = new SemaphoreSlim(0);

var machine = StateMachine
    .Configure<State, Transition>()
    .WithInitialState(State.Idle)

    .WithState(State.Idle, state => state
        .WithTransition(Transition.Start, State.Running))

    .WithState(State.Running, state => state
        .WithAction(action => action
            .Invoke(async token =>
            {
                while (!token.IsCancellationRequested)
                {
                    await workAvailable.WaitAsync(token);
                    await ProcessWorkAsync(token);
                }
            }))
        .WithTransition(Transition.Cancel, State.Canceled)
        .WithTransition(Transition.Complete, State.Done))

    .WithState(State.Canceled, state => state)
    .WithState(State.Done, state => state)

    // Trigger: start processing when a signal is received
    .WithTrigger(trigger => trigger
        .Once()
        .Await(token => startSignal.Task.WaitAsync(token))
        .ThenInvoke(async (sm, token) => await sm.Transition(Transition.Start, token)))

    // Trigger: cancel when an external token is canceled
    .WithTrigger(trigger => trigger
        .Once()
        .Await(token =>
        {
            var linked = CancellationTokenSource.CreateLinkedTokenSource(
                externalCancellation.Token, token);
            return Task.Delay(Timeout.Infinite, linked.Token);
        })
        .ThenInvoke(async (sm, token) => await sm.Transition(Transition.Cancel, token)))

    .Build();

await machine.Activate(cancellationToken);

// External code can cancel the state machine without calling Transition directly
externalCancellation.Cancel();
```

## Best Practices

### Use Appropriate Signal Primitives

Choose the signal primitive that matches the trigger's schedule:

- **`TaskCompletionSource`** — Use with `Once()`. Can only be set once, matching the single-fire behavior.
- **`Channel<T>` / `SemaphoreSlim`** — Use with `Repeat()`. Queues events so the trigger fires for every event.
- **`AsyncManualResetEvent` / `AsyncAutoResetEvent`** — Use with `Repeat()`. Ignores events that arrive while the
  trigger action is running (only the first event signals; subsequent events are lost until the trigger re-awaits).

```csharp
// Good: TaskCompletionSource with Once()
var notification = new TaskCompletionSource();
.WithTrigger(trigger => trigger
    .Once()
    .Await(token => notification.Task.WaitAsync(token))
    .ThenInvoke(async (sm, token) => await sm.Transition(Transition.Process, token)))

// Good: Channel with Repeat()
var commands = Channel.CreateUnbounded<Command>();
.WithTrigger(trigger => trigger
    .Repeat()
    .Await(async token => await commands.Reader.WaitToReadAsync(token))
    .ThenInvoke(async (sm, token) => await sm.Transition(Transition.Execute, token)))

// Good: SemaphoreSlim with Repeat()
var workAvailable = new SemaphoreSlim(0);
.WithTrigger(trigger => trigger
    .Repeat()
    .Await(workAvailable.WaitAsync)
    .ThenInvoke(async (sm, token) => await sm.Transition(Transition.Process, token)))
```

### Always Observe Cancellation Tokens

Signal functions receive a cancellation token that is canceled when the state machine is deactivated.
Pass this token to all async operations so the trigger can be interrupted cleanly.

> **Warning:** Cancellation tokens **must** be observed. The token is canceled when the trigger is no longer needed
> — either because the state machine has been deactivated or, for state-scoped triggers, because the state has
> changed. Failing to observe the token can lead to memory leaks, deadlocks, or invalid operations where a stale
> trigger continues to interact with the state machine after it is no longer appropriate to do so.

```csharp
// Good: Passes cancellation token to async operations
.WithTrigger(trigger => trigger
    .Once()
    .Await(async token => await signal.Task.WaitAsync(token))
    .ThenInvoke(async (sm, token) => await sm.Transition(Transition.Start, token)))

// Avoid: Ignoring the cancellation token
.WithTrigger(trigger => trigger
    .Once()
    .Await(async _ => await signal.Task)
    .ThenInvoke(async (sm, _) => await sm.Transition(Transition.Start)))
```

### Throw OperationCanceledException to Exit

Throwing `OperationCanceledException` is the correct way to exit a trigger when canceled.
The state machine suppresses this exception during deactivation and does not invoke the exception handler:

```csharp
// Good: Let cancellation propagate naturally
.WithTrigger(trigger => trigger
    .Once()
    .Await(token => Task.Delay(Timeout.Infinite, token))
    .ThenInvoke(async (sm, token) => await sm.Transition(Transition.Timeout, token)))

// Good: Explicit cancellation check
.WithTrigger(trigger => trigger
    .Repeat()
    .Await(async token =>
    {
        token.ThrowIfCancellationRequested();
        await signal.WaitAsync(token);
    })
    .ThenInvoke(async (sm, token) => await sm.Transition(Transition.Process, token)))
```

### Avoid Long-Running Synchronous Work in Signals

Signals should await an asynchronous condition, not perform blocking work:

```csharp
// Avoid: Blocking synchronous work in signal
.WithTrigger(trigger => trigger
    .Repeat()
    .Await(() => Thread.Sleep(5000))
    .ThenInvoke(async (sm, token) => await sm.Transition(Transition.Poll, token)))

// Good: Use async delay
.WithTrigger(trigger => trigger
    .Repeat()
    .Await(async token => await Task.Delay(TimeSpan.FromSeconds(5), token))
    .ThenInvoke(async (sm, token) => await sm.Transition(Transition.Poll, token)))
```

## Next Steps

- [Exception Handling](./11-exception-handling.md) - Error handling strategies
- [Actions](./5-actions.md) - Long-running work within states
- [Parameterless Transitions](./6-parameterless-transitions.md) - Simple state-to-state transitions
- [Parameterized Transitions](./7-parameterized-transitions.md) - Transitions that carry typed data
