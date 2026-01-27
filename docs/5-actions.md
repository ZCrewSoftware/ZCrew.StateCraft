# Actions

Actions represent the main work of a state.
Unlike lifecycle handlers, actions are designed for long-running, interruptible tasks that execute while a state is
active.

## Actions vs Lifecycle Handlers

| Aspect                            | Actions                            | Lifecycle Handlers (OnEntry/OnExit) |
|-----------------------------------|------------------------------------|-------------------------------------|
| Execution style                   | Start then unblock state machine   | Block state machine                 |
| Can be interrupted by transitions | Yes                                | No                                  |
| Can trigger transitions           | Yes                                | No                                  |
| When to use                       | Long-running or interruptible work | Quick setup/cleanup operations      |
| Receives cancellation             | Yes, must observe cancellation     | Yes, should observe cancellation    |

## Configuration

Actions are configured on states using the `WithAction` method.

### Parameterless Actions

For states without parameters:

```csharp
.WithState(State.Monitoring, state => state
    .WithAction(action => action
        .Invoke(async token =>
        {
            while (!token.IsCancellationRequested)
            {
                await CheckHealthAsync(token);
                await Task.Delay(TimeSpan.FromSeconds(5), token);
            }
        })))
```

### Parameterized Actions

For states with parameters, the action receives the state parameter:

```csharp
.WithState(State.Processing, state => state
    .WithParameter<JobConfig>()
    .WithAction(action => action
        .Invoke(async (job, token) =>
        {
            foreach (var item in job.Items)
            {
                token.ThrowIfCancellationRequested();
                await ProcessItemAsync(item, token);
            }
        })))
```

### Running Actions Asynchronously

The state machine can be configured to run all actions asynchronously:

```csharp
var machine = StateMachine
    .Configure<State, Transition>()
    .WithInitialState(State.Idle)
    .WithAsynchronousActions()
    .WithState(...)
```

By default, the state machine will wait for the completion of the actions before completing the `Transition` or
`TryTransition` call.
Once the action is started the state machine's lock is released, allowing new transitions.
So, the locking mechanism of the state machine does not change by configuring the state machine to run actions
asynchronously.
Configuring the state machine this way merely allows the caller to wait for a shorter period of time which may be useful
for asynchronous requests.

This applies to **all** actions configured on the state machine.
When the state machine is configured with this option the actions will start and execute all synchronous code until it
awaits an asynchronous task:

```csharp
var actionStartedEvent = new AsyncManualResetEvent();

var machine = StateMachine
    .Configure<State, Transition>()
    .WithInitialState(State.Idle)
    .WithAsynchronousActions()
    .WithState(State.Processing, state => state
        .WithParameter<JobConfig>()
        .WithAction(action => action
            .Invoke(async (job, token) =>
            {
                // It is guaranteed that this code will run before any transition can cancel the action.
                actionStartedEvent.Set();

                // The state machine's lock is still held until this part
                await Task.Yield();

                // Rest of the code is running asynchronously and the state machine can begin transitioning
                foreach (var item in job.Items)
                {
                    token.ThrowIfCancellationRequested();
                    await ProcessItemAsync(item, token);
                }
            })))
```

### Handler Signatures

Actions support three signatures, consistent with all StateCraft handlers:

```csharp
// Synchronous
.WithAction(action => action.Invoke(() => _counter++))

// Asynchronous with Task
.WithAction(action => action.Invoke(async token => await ProcessAsync(token)))

// Asynchronous with ValueTask
.WithAction(action => action.Invoke(token => ProcessAsync(token)))  // Returns ValueTask
```

### Multiple Actions

A state can have multiple actions:

```csharp
.WithState(State.Active, state => state
    .WithAction(action => action
        .Invoke(async token => await MonitorHealthAsync(token)))
    .WithAction(action => action
        .Invoke(async token => await ProcessQueueAsync(token))))
```

All actions for a state run sequentially once the state is entered.
Therefore, it is recommended to usually only have one action.

## Lifecycle

### When Actions Start

Actions start **after** `OnEntry` completes:

1. `OnEntry` handler runs (blocking)
2. Actions start
3. State is now fully active and state machine can transition
4. If `WithAsynchronousActions()` was set then the `Transition` method returns immediately; otherwise, the actions will
   be awaited.

```csharp
.WithState(State.Running, state => state
    .OnEntry(() => Console.WriteLine("1. Entry handler completes first"))
    .WithAction(action => action
        .Invoke(async token =>
        {
            Console.WriteLine("2. Action starts after entry");
            await DoWorkAsync(token);
        })))
```

### During Transitions

When a transition occurs, the state machine:

1. If `WithAsynchronousActions()` was set then the state machine cancels the cancellation token provided to actions
2. Awaits all running actions to complete
3. Proceeds with `OnExit` and the rest of the transition

```csharp
.WithAction(action => action
    .Invoke(async token =>
    {
        try
        {
            await LongRunningWorkAsync(token);
        }
        catch (OperationCanceledException)
        {
            // Expected when transitioning out of State.A
            Console.WriteLine("Action canceled during transition");

            // State machine expects this exception and this does not need to be suprressed
            throw;
        }
    }))
```

### During Deactivation

Deactivation follows the same pattern as transitions:

1. If `WithAsynchronousActions()` was set then the state machine cancels the cancellation token provided to actions
2. Awaits all running actions to complete
3. `OnExit` runs
4. `OnDeactivate` runs

## Triggering Transitions

Unlike lifecycle handlers, actions **can** trigger transitions:

```csharp
var machine = StateMachine
    .Configure<State, Transition>()
    .WithInitialState(State.Polling)
    .WithState(State.Polling, state => state
        .WithAction(action => action
            .Invoke(async token =>
            {
                while (!token.IsCancellationRequested)
                {
                    var result = await PollAsync(token);
                    if (result.IsComplete)
                    {
                        // Action triggering a transition
                        await machine.Transition(Transition.Complete, token);
                        return;
                    }
                    await Task.Delay(TimeSpan.FromSeconds(1), token);
                }
            }))
        .WithTransition(Transition.Complete, State.Done))
    .WithState(State.Done, state => state)
    .Build();
```

## Best Practices

### Always Observe Cancellation Tokens

This is the most important practice for actions. The cancellation token signals when to stop work:

```csharp
// Good: Respects cancellation throughout
.WithAction(action => action
    .Invoke(async token =>
    {
        while (!token.IsCancellationRequested)
        {
            await ProcessNextItemAsync(token);
        }
    }))

// Good: Pass token to all async operations
.WithAction(action => action
    .Invoke(async token =>
    {
        await httpClient.GetAsync(url, token);
        await Task.Delay(1000, token);
        await database.SaveAsync(data, token);
    }))

// Avoid: Ignoring cancellation
.WithAction(action => action
    .Invoke(async _ =>
    {
        while (true)  // Never terminates!
        {
            await ProcessNextItemAsync();
        }
    }))
```

### Throw OperationCanceledException to Exit

Throwing `OperationCanceledException` is the correct way to exit an action when canceled:

```csharp
// Good: Let cancellation propagate naturally
.WithAction(action => action
    .Invoke(async token =>
    {
        // Task.Delay throws OperationCanceledException when canceled
        await Task.Delay(TimeSpan.FromHours(1), token);
    }))

// Good: Explicit cancellation check
.WithAction(action => action
    .Invoke(async token =>
    {
        foreach (var item in largeCollection)
        {
            token.ThrowIfCancellationRequested();
            await ProcessAsync(item);
        }
    }))
```

### Avoid Blocking at Action Start

Actions should yield quickly to allow the state machine to proceed:

```csharp
// Good: Yields quickly, then does work
.WithAction(action => action
    .Invoke(async token =>
    {
        await Task.Yield();  // Or any async operation
        // Now do long-running work...
    }))

// Avoid: Long synchronous work before first await
.WithAction(action => action
    .Invoke(async token =>
    {
        Thread.Sleep(10000);  // Blocks state machine!
        await Task.CompletedTask;
    }))
```

### Use Actions for Interruptible Work

Reserve actions for work that should be cancellable. Use `OnEntry` for quick, required setup:

```csharp
// Good: Quick setup in handler, interruptible work in action
.WithState(State.Processing, state => state
    .OnEntry(() =>
    {
        _startTime = DateTime.UtcNow;
        _itemsProcessed = 0;
    })
    .WithAction(action => action
        .Invoke(async token =>
        {
            await ProcessAllItemsAsync(token);  // Can be canceled
        })))

// Avoid: All work in OnEntry (blocks transitions)
.WithState(State.Processing, state => state
    .OnEntry(async token =>
    {
        _startTime = DateTime.UtcNow;
        await ProcessAllItemsAsync(token);  // Cannot be interrupted!
    }))
```

## Next Steps

- [Parameterless Transitions](./6-parameterless-transitions.md) - Simple state-to-state transitions
- [Parameterized Transitions](./7-parameterized-transitions.md) - Transitions that carry typed data
- [Mapped Transitions](./8-mapped-transitions.md) - Automatic parameter conversion
- [Reentrant Transitions](./9-reentrant-transitions.md) - Same-parameter transitions
- [Triggers](./10-triggers.md) - Autonomous transitions based on signals
- [Exception Handling](./11-exception-handling.md) - Error handling strategies
- [State Lifecycle](./4-state-lifecycle.md) - Lifecycle handler documentation
