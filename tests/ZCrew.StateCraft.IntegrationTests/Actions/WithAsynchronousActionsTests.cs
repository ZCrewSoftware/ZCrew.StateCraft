namespace ZCrew.StateCraft.IntegrationTests.Actions;

public class WithAsynchronousActionsTests
{
    [Fact]
    public async Task Activate_WithAsyncAction_ShouldNotAwaitAction()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithAsynchronousActions()
            .WithInitialState("A")
            .WithState(
                "A",
                state =>
                    state.WithAction(a =>
                        a.Invoke(_ => Task.Delay(Timeout.Infinite, TestContext.Current.CancellationToken))
                    )
            )
            .Build();

        // Act
        var activate = stateMachine.Activate(TestContext.Current.CancellationToken);
        var timeout = Task.Delay(TimeSpan.FromSeconds(1), TestContext.Current.CancellationToken);
        var completedTask = await Task.WhenAny(activate, timeout);

        // Assert
        Assert.Same(activate, completedTask);
        Assert.True(activate.IsCompletedSuccessfully);
    }

    [Fact]
    public async Task Activate_WithSyncAction_ShouldNotAwaitAction()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithAsynchronousActions()
            .WithInitialState("A")
            .WithState("A", state => state.WithAction(a => a.Invoke(() => Thread.Sleep(TimeSpan.FromSeconds(5)))))
            .Build();

        // Act
        var activate = stateMachine.Activate(TestContext.Current.CancellationToken);
        var timeout = Task.Delay(TimeSpan.FromSeconds(1), TestContext.Current.CancellationToken);
        var completedTask = await Task.WhenAny(activate, timeout);

        // Assert
        Assert.Same(activate, completedTask);
        Assert.True(activate.IsCompletedSuccessfully);
    }

    [Fact]
    public async Task Activate_WithCompletedAction_ShouldNotAwaitAction()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithAsynchronousActions()
            .WithInitialState("A")
            .WithState("A", state => state.WithAction(a => a.Invoke(_ => Task.CompletedTask)))
            .Build();

        // Act
        var activate = stateMachine.Activate(TestContext.Current.CancellationToken);
        var timeout = Task.Delay(TimeSpan.FromSeconds(1), TestContext.Current.CancellationToken);
        var completedTask = await Task.WhenAny(activate, timeout);

        // Assert
        Assert.Same(activate, completedTask);
        Assert.True(activate.IsCompletedSuccessfully);
    }

    [Fact]
    public async Task Activate_WithAsyncAction_ShouldBeCanceledDuringNextTransition()
    {
        // Arrange
        var tokenWasCanceled = false;
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithAsynchronousActions()
            .WithInitialState("A")
            .WithState("A", state => state.WithAction(a => a.Invoke(Action)).WithTransition("To B", t => t.To("B")))
            .WithState("B", state => state)
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var transition = stateMachine.Transition("To B", TestContext.Current.CancellationToken);
        var timeout = Task.Delay(TimeSpan.FromSeconds(1), TestContext.Current.CancellationToken);
        var completedTask = await Task.WhenAny(transition, timeout);

        // Assert
        Assert.Same(transition, completedTask);
        Assert.True(transition.IsCompletedSuccessfully);
        Assert.True(tokenWasCanceled);

        return;

        async Task Action(CancellationToken token)
        {
            try
            {
                await Task.Delay(Timeout.Infinite, token);
            }
            catch (OperationCanceledException) when (token.IsCancellationRequested)
            {
                tokenWasCanceled = true;
            }
        }
    }

    [Fact]
    public async Task Activate_WithActionThrowingException_ShouldTransitionSuccessfully()
    {
        // Arrange
        var activateCompleted = new TaskCompletionSource();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithAsynchronousActions()
            .WithInitialState("A")
            .WithState("A", state => state.WithAction(a => a.Invoke(Action)).WithTransition("To B", t => t.To("B")))
            .WithState("B", state => state)
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);
        activateCompleted.SetResult();

        // Act
        var transition = stateMachine.Transition("To B", TestContext.Current.CancellationToken);
        var timeout = Task.Delay(TimeSpan.FromSeconds(1), TestContext.Current.CancellationToken);
        var completedTask = await Task.WhenAny(transition, timeout);

        // Assert
        Assert.Same(transition, completedTask);
        Assert.True(transition.IsCompletedSuccessfully);

        return;

        async Task Action(CancellationToken _)
        {
            await activateCompleted.Task;
            throw new Exception("Test Exception");
        }
    }

    [Fact]
    public async Task Activate_WithAsyncAction_ShouldBeCanceledDuringDeactivation()
    {
        // Arrange
        var tokenWasCanceled = false;
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithAsynchronousActions()
            .WithInitialState("A")
            .WithState("A", state => state.WithAction(a => a.Invoke(Action)))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var deactivate = stateMachine.Deactivate(TestContext.Current.CancellationToken);
        var timeout = Task.Delay(TimeSpan.FromSeconds(1), TestContext.Current.CancellationToken);
        var completedTask = await Task.WhenAny(deactivate, timeout);

        // Assert
        Assert.Same(deactivate, completedTask);
        Assert.True(deactivate.IsCompletedSuccessfully);
        Assert.True(tokenWasCanceled);

        return;

        async Task Action(CancellationToken token)
        {
            try
            {
                await Task.Delay(Timeout.Infinite, token);
            }
            catch (OperationCanceledException) when (token.IsCancellationRequested)
            {
                tokenWasCanceled = true;
            }
        }
    }

    [Fact]
    public async Task Transition_WithAsyncAction_ShouldNotAwaitAction()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithAsynchronousActions()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition("To B", t => t.To("B")))
            .WithState(
                "B",
                state =>
                    state.WithAction(a =>
                        a.Invoke(_ => Task.Delay(Timeout.Infinite, TestContext.Current.CancellationToken))
                    )
            )
            .Build();
        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var transition = stateMachine.Transition("To B", TestContext.Current.CancellationToken);
        var timeout = Task.Delay(TimeSpan.FromSeconds(1), TestContext.Current.CancellationToken);
        var completedTask = await Task.WhenAny(transition, timeout);

        // Assert
        Assert.Same(transition, completedTask);
        Assert.True(transition.IsCompletedSuccessfully);
    }

    [Fact]
    public async Task Transition_WithSyncAction_ShouldNotAwaitAction()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithAsynchronousActions()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition("To B", t => t.To("B")))
            .WithState(
                "B",
                state =>
                    state.WithAction(a =>
                        a.Invoke(() =>
                        {
                            Thread.Sleep(TimeSpan.FromSeconds(5));
                            Console.WriteLine("Done sleeping");
                        })
                    )
            )
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var transition = stateMachine.Transition("To B", TestContext.Current.CancellationToken);
        var timeout = Task.Delay(TimeSpan.FromSeconds(1), TestContext.Current.CancellationToken);
        var completedTask = await Task.WhenAny(transition, timeout);

        // Assert
        Assert.Same(transition, completedTask);
        Assert.True(transition.IsCompletedSuccessfully);
    }

    [Fact]
    public async Task Transition_WithCompletedAction_ShouldNotAwaitAction()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithAsynchronousActions()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition("To B", t => t.To("B")))
            .WithState(
                "B",
                state => state.WithAction(a => a.Invoke(_ => Task.CompletedTask)).WithTransition("To A", t => t.To("A"))
            )
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);
        await stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Act
        var transition = stateMachine.Transition("To A", TestContext.Current.CancellationToken);
        var timeout = Task.Delay(TimeSpan.FromSeconds(1), TestContext.Current.CancellationToken);
        var completedTask = await Task.WhenAny(transition, timeout);

        // Assert
        Assert.Same(transition, completedTask);
        Assert.True(transition.IsCompletedSuccessfully);
    }

    [Fact]
    public async Task Transition_WithAsyncAction_ShouldBeCanceledDuringNextTransition()
    {
        // Arrange
        var tokenWasCanceled = false;
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithAsynchronousActions()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition("To B", t => t.To("B")))
            .WithState("B", state => state.WithAction(a => a.Invoke(Action)).WithTransition("To A", t => t.To("A")))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);
        await stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Act
        var transition = stateMachine.Transition("To A", TestContext.Current.CancellationToken);
        var timeout = Task.Delay(TimeSpan.FromSeconds(1), TestContext.Current.CancellationToken);
        var completedTask = await Task.WhenAny(transition, timeout);

        // Assert
        Assert.Same(transition, completedTask);
        Assert.True(transition.IsCompletedSuccessfully);
        Assert.True(tokenWasCanceled);

        return;

        async Task Action(CancellationToken token)
        {
            try
            {
                await Task.Delay(Timeout.Infinite, token);
            }
            catch (OperationCanceledException) when (token.IsCancellationRequested)
            {
                tokenWasCanceled = true;
            }
        }
    }

    [Fact]
    public async Task Transition_WithActionThrowingException_ShouldTransitionSuccessfully()
    {
        // Arrange
        var transitionCompleted = new TaskCompletionSource();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithAsynchronousActions()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition("To B", t => t.To("B")))
            .WithState("B", state => state.WithAction(a => a.Invoke(Action)).WithTransition("To A", t => t.To("A")))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);
        await stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Act
        var transition = stateMachine.Transition("To A", TestContext.Current.CancellationToken);
        transitionCompleted.SetResult();
        var timeout = Task.Delay(TimeSpan.FromSeconds(1), TestContext.Current.CancellationToken);
        var completedTask = await Task.WhenAny(transition, timeout);

        // Assert
        Assert.Same(transition, completedTask);
        Assert.True(transition.IsCompletedSuccessfully);

        return;

        async Task Action(CancellationToken _)
        {
            await transitionCompleted.Task;
            throw new Exception("Test Exception");
        }
    }

    [Fact]
    public async Task Transition_WithAsyncAction_ShouldBeCanceledDuringDeactivation()
    {
        // Arrange
        var tokenWasCanceled = false;
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithAsynchronousActions()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition("To B", t => t.To("B")))
            .WithState("B", state => state.WithAction(a => a.Invoke(Action)))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);
        await stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Act
        var deactivate = stateMachine.Deactivate(TestContext.Current.CancellationToken);
        var timeout = Task.Delay(TimeSpan.FromSeconds(1), TestContext.Current.CancellationToken);
        var completedTask = await Task.WhenAny(deactivate, timeout);

        // Assert
        Assert.Same(deactivate, completedTask);
        Assert.True(deactivate.IsCompletedSuccessfully);
        Assert.True(tokenWasCanceled);

        return;

        async Task Action(CancellationToken token)
        {
            try
            {
                await Task.Delay(Timeout.Infinite, token);
            }
            catch (OperationCanceledException) when (token.IsCancellationRequested)
            {
                tokenWasCanceled = true;
            }
        }
    }
}
