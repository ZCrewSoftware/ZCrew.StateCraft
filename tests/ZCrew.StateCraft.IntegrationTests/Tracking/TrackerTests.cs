using ZCrew.StateCraft.Tracking;

namespace ZCrew.StateCraft.IntegrationTests.Tracking;

public class TrackerTests
{
    [Fact]
    public async Task Activate_ShouldReportTheLifecycleInOrder()
    {
        // Arrange
        var tracker = new RecordingTracker();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithTracker(() => tracker)
            .WithState("A", state => state.WithAction(a => a.Invoke(() => { })))
            .Build();

        // Act
        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(
            ["Activating", "Activated", "Entering", "Entered", "ActionStarting", "ActionCompleted"],
            tracker.Names
        );
    }

    [Fact]
    public async Task Deactivate_ShouldReportTheLifecycleInOrder()
    {
        // Arrange
        var tracker = new RecordingTracker();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithTracker(() => tracker)
            .WithState("A", state => state)
            .Build();
        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Deactivate(TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(
            ["Exiting", "Exited", "Deactivating", "Deactivated"],
            tracker.Names.Skip(tracker.Names.ToList().IndexOf("Exiting"))
        );
    }

    [Fact]
    public async Task Transition_ShouldReportEveryPhaseInOrder()
    {
        // Arrange
        var tracker = new RecordingTracker();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithTracker(() => tracker)
            .WithState("A", state => state.WithTransition("To B", t => t.To("B")))
            .WithState("B", state => state)
            .Build();
        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(
            [
                "TransitionQuerying",
                "TransitionFound",
                "Exiting",
                "Exited",
                "Transitioning",
                "Transitioned",
                "StateChanging",
                "StateChanged",
                "Entering",
                "Entered",
                "ActionStarting",
                "ActionCompleted",
            ],
            tracker.Names.Skip(tracker.Names.ToList().IndexOf("TransitionQuerying"))
        );
    }

    [Fact]
    public async Task Transition_ShouldReportTheStatesAndTransition()
    {
        // Arrange
        var tracker = new RecordingTracker();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithTracker(() => tracker)
            .WithState("A", state => state.WithTransition("To B", t => t.To("B")))
            .WithState("B", state => state)
            .Build();
        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Assert
        var stateChanged = tracker.Single("StateChanged");
        Assert.Equal("A", stateChanged.State!.StateValue);
        Assert.Equal("To B", stateChanged.Transition!.TransitionValue);
        Assert.Equal("B", stateChanged.Target!.StateValue);
    }

    public static TheoryData<int> ParameterCounts => [0, 1, 2, 3, 4];

    [Fact]
    public async Task Entering_With1Parameter_ShouldCaptureParameter()
    {
        // Arrange
        var tracker = new RecordingTracker();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 1)
            .WithTracker(() => tracker)
            .WithState("A", s => s.WithParameter<int>())
            .Build();

        // Act
        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Assert
        var parameters = tracker.Single("Entered").Parameters;
        Assert.NotNull(parameters);
        Assert.True(parameters.IsSet);
        Assert.Equal(1, parameters.Count);
        Assert.Equal([1], parameters.Values);
        Assert.Equal([typeof(int)], parameters.Types);
    }

    [Fact]
    public async Task Entering_With2Parameters_ShouldCaptureParameters()
    {
        // Arrange
        var tracker = new RecordingTracker();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 1, "two")
            .WithTracker(() => tracker)
            .WithState("A", s => s.WithParameters<int, string>())
            .Build();

        // Act
        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Assert
        var parameters = tracker.Single("Entered").Parameters;
        Assert.NotNull(parameters);
        Assert.True(parameters.IsSet);
        Assert.Equal(2, parameters.Count);
        Assert.Equal([1, "two"], parameters.Values);
        Assert.Equal([typeof(int), typeof(string)], parameters.Types);
    }

    [Fact]
    public async Task Entering_With3Parameters_ShouldCaptureParameters()
    {
        // Arrange
        var tracker = new RecordingTracker();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 1, "two", 3.0)
            .WithTracker(() => tracker)
            .WithState("A", s => s.WithParameters<int, string, double>())
            .Build();

        // Act
        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Assert
        var parameters = tracker.Single("Entered").Parameters;
        Assert.NotNull(parameters);
        Assert.True(parameters.IsSet);
        Assert.Equal(3, parameters.Count);
        Assert.Equal([1, "two", 3.0], parameters.Values);
        Assert.Equal([typeof(int), typeof(string), typeof(double)], parameters.Types);
    }

    [Fact]
    public async Task Entering_With4Parameters_ShouldCaptureParameters()
    {
        // Arrange
        var tracker = new RecordingTracker();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 1, "two", 3.0, true)
            .WithTracker(() => tracker)
            .WithState("A", s => s.WithParameters<int, string, double, bool>())
            .Build();

        // Act
        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Assert
        var parameters = tracker.Single("Entered").Parameters;
        Assert.NotNull(parameters);
        Assert.True(parameters.IsSet);
        Assert.Equal(4, parameters.Count);
        Assert.Equal([1, "two", 3.0, true], parameters.Values);
        Assert.Equal([typeof(int), typeof(string), typeof(double), typeof(bool)], parameters.Types);
    }

    [Theory]
    [MemberData(nameof(ParameterCounts))]
    public async Task Entering_ShouldCaptureEveryParameter(int count)
    {
        // Arrange
        var tracker = new RecordingTracker();
        var configuration = StateMachine.Configure<string, string>();
        var stateMachine = count switch
        {
            0 => configuration.WithInitialState("A").WithTracker(() => tracker).WithState("A", s => s).Build(),
            1 => configuration
                .WithInitialState("A", 1)
                .WithTracker(() => tracker)
                .WithState("A", s => s.WithParameter<int>())
                .Build(),
            2 => configuration
                .WithInitialState("A", 1, "two")
                .WithTracker(() => tracker)
                .WithState("A", s => s.WithParameters<int, string>())
                .Build(),
            3 => configuration
                .WithInitialState("A", 1, "two", 3.0)
                .WithTracker(() => tracker)
                .WithState("A", s => s.WithParameters<int, string, double>())
                .Build(),
            _ => configuration
                .WithInitialState("A", 1, "two", 3.0, true)
                .WithTracker(() => tracker)
                .WithState("A", s => s.WithParameters<int, string, double, bool>())
                .Build(),
        };

        // Act
        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Assert
        var parameters = tracker.Single("Entered").Parameters;
        Assert.NotNull(parameters);
        Assert.True(parameters.IsSet);
        Assert.Equal(count, parameters.Count);

        object?[] expectedValues = [1, "two", 3.0, true];
        Type[] expectedTypes = [typeof(int), typeof(string), typeof(double), typeof(bool)];
        Assert.Equal(expectedValues.Take(count), parameters.Values);
        Assert.Equal(expectedTypes.Take(count), parameters.Types);
    }

    [Fact]
    public async Task Entered_ShouldReadParametersByType()
    {
        // Arrange
        var tracker = new RecordingTracker();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42, "answer")
            .WithTracker(() => tracker)
            .WithState("A", state => state.WithParameters<int, string>())
            .Build();

        // Act
        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Assert
        var parameters = tracker.Single("Entered").Parameters!;
        Assert.Equal((42, "answer"), parameters.Get<int, string>());
        Assert.Throws<InvalidOperationException>(() => parameters.Get<int>());
        Assert.Throws<InvalidCastException>(() => parameters.Get<string, int>());
    }

    [Fact]
    public async Task Entered_ShouldRenderParameterisedStatesWithTheirTypes()
    {
        // Arrange
        var tracker = new RecordingTracker();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 1, "two")
            .WithTracker(() => tracker)
            .WithState("A", state => state.WithParameters<int, string>())
            .Build();

        // Act
        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal("A<int, string>", tracker.Single("Entered").State!.ToString());
    }

    [Fact]
    public async Task Entering_WhenAnEntryHandlerThrows_ShouldLeaveThePairUnterminated()
    {
        // Arrange
        var tracker = new RecordingTracker();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithTracker(() => tracker)
            .WithState("A", state => state.WithTransition("To B", t => t.To("B")))
            .WithState("B", state => state.OnEntry(() => throw new InvalidOperationException("boom")))
            .Build();
        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var transition = () => stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(transition);
        var entering = tracker.All("Entering").Last();
        Assert.Equal("B", entering.State!.StateValue);
        Assert.DoesNotContain(tracker.Events, e => e.Name == "Entered" && e.State!.StateValue.Equals("B"));
    }

    [Fact]
    public async Task HandlerFailed_ShouldReportTheCallSiteAndException()
    {
        // Arrange
        var tracker = new RecordingTracker();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithTracker(() => tracker)
            .WithState("A", state => state.WithTransition("To B", t => t.To("B")))
            .WithState("B", state => state.OnEntry(() => throw new InvalidOperationException("boom")))
            .Build();
        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var transition = () => stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(transition);
        var failure = tracker.Single("HandlerFailed");
        Assert.Equal(ExceptionCallSite.OnEntry, failure.CallSite);
        Assert.Equal("boom", failure.Exception!.Message);
    }

    [Fact]
    public async Task RolledBack_WhenATransitionFails_ShouldCarryTheException()
    {
        // Arrange
        var tracker = new RecordingTracker();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithTracker(() => tracker)
            .WithState(
                "A",
                state =>
                    state
                        .OnExit(() => throw new InvalidOperationException("boom"))
                        .WithTransition("To B", t => t.To("B"))
            )
            .WithState("B", state => state)
            .Build();
        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var transition = () => stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(transition);
        var rolledBack = tracker.Single("RolledBack");
        Assert.Equal("A", rolledBack.State!.StateValue);
        Assert.Equal("boom", rolledBack.Exception!.Message);
    }

    [Fact]
    public async Task CanTransition_ShouldReportTheDryRunAndNoLifecycleEvents()
    {
        // Arrange
        var tracker = new RecordingTracker();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithTracker(() => tracker)
            .WithState("A", state => state.WithTransition("To B", t => t.To("B")))
            .WithState("B", state => state)
            .Build();
        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var canTransition = await stateMachine.CanTransition("To B", TestContext.Current.CancellationToken);

        // Assert
        Assert.True(canTransition);
        Assert.Equal(
            ["TransitionQuerying", "TransitionFound", "RolledBack"],
            tracker.Names.Skip(tracker.Names.ToList().IndexOf("TransitionQuerying"))
        );
        Assert.Equal(TransitionQueryKind.CanTransition, tracker.Single("TransitionQuerying").Kind);
        Assert.Null(tracker.Single("RolledBack").Exception);
    }

    [Fact]
    public async Task CanTransition_WhenNoMatch_ShouldReportUnresolved()
    {
        // Arrange
        var tracker = new RecordingTracker();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithTracker(() => tracker)
            .WithState("A", state => state.WithTransition("To B", t => t.To("B")))
            .WithState("B", state => state)
            .Build();
        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var canTransition = await stateMachine.CanTransition("To C", TestContext.Current.CancellationToken);

        // Assert
        Assert.False(canTransition);
        var notFound = tracker.Single("TransitionNotFound");
        Assert.Equal(TransitionQueryKind.CanTransition, notFound.Kind);
        Assert.Equal("To C", notFound.Value);
        Assert.Equal("A", notFound.State!.StateValue);
    }

    [Fact]
    public async Task TryTransition_WhenNoMatch_ShouldReportUnresolved()
    {
        // Arrange
        var tracker = new RecordingTracker();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithTracker(() => tracker)
            .WithState("A", state => state.WithTransition("To B", t => t.To("B")))
            .WithState("B", state => state)
            .Build();
        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var transitioned = await stateMachine.TryTransition("To C", TestContext.Current.CancellationToken);

        // Assert
        Assert.False(transitioned);
        var notFound = tracker.Single("TransitionNotFound");
        Assert.Equal(TransitionQueryKind.TryTransition, notFound.Kind);
        Assert.Equal("To C", notFound.Value);
    }

    [Fact]
    public async Task CandidateRejected_WhenTheTransitionValueDiffers_ShouldReportTheReason()
    {
        // Arrange
        var tracker = new RecordingTracker();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithTracker(() => tracker)
            .WithState("A", state => state.WithTransition("To B", t => t.To("B")))
            .WithState("B", state => state)
            .Build();
        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.TryTransition("To C", TestContext.Current.CancellationToken);

        // Assert
        var skipped = tracker.Single("TransitionSkipped");
        Assert.Equal(TransitionSkippedReason.TransitionValueMismatch, skipped.Reason);
        Assert.Equal("To B", skipped.Transition!.TransitionValue);
    }

    [Fact]
    public async Task CandidateRejected_WhenAConditionFails_ShouldReportTheReason()
    {
        // Arrange
        var tracker = new RecordingTracker();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithTracker(() => tracker)
            .WithState("A", state => state.WithTransition("To B", t => t.If(() => false).To("B")))
            .WithState("B", state => state)
            .Build();
        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.TryTransition("To B", TestContext.Current.CancellationToken);

        // Assert
        var skipped = tracker.Single("TransitionSkipped");
        Assert.Equal(TransitionSkippedReason.ConditionFailed, skipped.Reason);
    }

    [Fact]
    public async Task CandidateRejected_WhenTheParameterTypeDiffers_ShouldReportTheReason()
    {
        // Arrange
        var tracker = new RecordingTracker();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithTracker(() => tracker)
            .WithState("A", state => state.WithTransition("To B", t => t.WithParameter<int>().To("B")))
            .WithState("B", state => state.WithParameter<int>())
            .Build();
        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.TryTransition("To B", "not an int", TestContext.Current.CancellationToken);

        // Assert
        var skipped = tracker.Single("TransitionSkipped");
        Assert.Equal(TransitionSkippedReason.ParameterTypeMismatch, skipped.Reason);
    }

    [Fact]
    public async Task TransitionRequested_ShouldCarryTheCallersParameters()
    {
        // Arrange
        var tracker = new RecordingTracker();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithTracker(() => tracker)
            .WithState("A", state => state.WithTransition("To B", t => t.WithParameters<int, string>().To("B")))
            .WithState("B", state => state.WithParameters<int, string>())
            .Build();
        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", 7, "seven", TestContext.Current.CancellationToken);

        // Assert
        var querying = tracker.Single("TransitionQuerying");
        Assert.Equal(TransitionQueryKind.Transition, querying.Kind);
        Assert.Equal("To B", querying.Value);
        Assert.NotNull(querying.Parameters);
        Assert.Equal([7, "seven"], querying.Parameters.Values);
        Assert.Equal([typeof(int), typeof(string)], querying.Parameters.Types);
    }

    [Fact]
    public async Task Build_ShouldGiveEachStateMachineItsOwnTracker()
    {
        // Arrange
        var trackers = new List<RecordingTracker>();
        var configuration = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithTracker(() =>
            {
                var tracker = new RecordingTracker();
                trackers.Add(tracker);
                return tracker;
            })
            .WithState("A", state => state);

        // Act
        var first = configuration.Build();
        var second = configuration.Build();
        await first.Activate(TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(2, trackers.Count);
        Assert.NotEmpty(trackers[0].Events);
        Assert.Empty(trackers[1].Events);
        Assert.NotNull(second);
    }
}
