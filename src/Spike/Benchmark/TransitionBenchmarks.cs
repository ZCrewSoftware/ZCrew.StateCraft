using BenchmarkDotNet.Attributes;
using Nito.AsyncEx;
using Spike.Configuration;
using Spike.Contracts;
using ZCrew.StateCraft;
using ZCrew.StateCraft.StateMachines.Contracts;

namespace Spike.Benchmark;

[MemoryDiagnoser]
[GroupBenchmarksBy(BenchmarkDotNet.Configs.BenchmarkLogicalGroupRule.ByCategory)]
[CategoriesColumn]
public class TransitionBenchmarks
{
    private const int LoopCount = 100;

    private enum State
    {
        A,
    }

    private enum Trigger
    {
        Go,
    }

    // AsyncLock to match StateCraft's async lock behavior
    private readonly AsyncLock spikeLock = new AsyncLock();

    // === Spike RawImpl transitions (self-transitions) ===
    private ITransition<State, Trigger> spikeStandard0 = null!;
    private ITransition<State, Trigger> spikeStandard1 = null!;
    private ITransition<State, Trigger> spikeStandard2 = null!;
    private ITransition<State, Trigger> spikeStandard3 = null!;
    private ITransition<State, Trigger> spikeStandard4 = null!;
    private ITransition<State, Trigger> spikeMapped1to1 = null!;
    private ITransition<State, Trigger> spikeMapped2to2 = null!;
    private ITransition<State, Trigger> spikeMapped3to3 = null!;
    private ITransition<State, Trigger> spikeMapped4to4 = null!;

    // === StateCraft library state machines (self-transitions) ===
    private IStateMachine<State, Trigger> stateCraftStandard0 = null!;
    private IStateMachine<State, Trigger> stateCraftStandard1 = null!;
    private IStateMachine<State, Trigger> stateCraftMapped1to1 = null!;

    [GlobalSetup]
    public void Setup()
    {
        // === Spike RawImpl Setup (all self-transitions to State.A) ===

        // Standard 0 parameters (parameterless -> parameterless)
        this.spikeStandard0 = new ParameterlessTransitionConfiguration<State, Trigger>(
            new TransitionPreviousStateConfiguration<State>(State.A),
            Trigger.Go
        )
            .To(State.A)
            .Build();

        // Standard 1 parameter (int -> int)
        this.spikeStandard1 = new InitialTransitionConfiguration<State, Trigger, int>(State.A, Trigger.Go)
            .WithParameter<int>()
            .To(State.A)
            .Build();

        // Standard 2 parameters (int -> (int, string))
        this.spikeStandard2 = new InitialTransitionConfiguration<State, Trigger, int>(State.A, Trigger.Go)
            .WithParameters<int, string>()
            .To(State.A)
            .Build();

        // Standard 3 parameters (int -> (int, string, double))
        this.spikeStandard3 = new InitialTransitionConfiguration<State, Trigger, int>(State.A, Trigger.Go)
            .WithParameters<int, string, double>()
            .To(State.A)
            .Build();

        // Standard 4 parameters (int -> (int, string, double, bool))
        this.spikeStandard4 = new InitialTransitionConfiguration<State, Trigger, int>(State.A, Trigger.Go)
            .WithParameters<int, string, double, bool>()
            .To(State.A)
            .Build();

        // Mapped 1->1 (int -> int, self-transition)
        this.spikeMapped1to1 = new InitialTransitionConfiguration<State, Trigger, int>(State.A, Trigger.Go)
            .WithMappedParameter(x => x + 1)
            .To(State.A)
            .Build();

        // Mapped 1->2 (int -> (int, string))
        this.spikeMapped2to2 = new InitialTransitionConfiguration<State, Trigger, int>(State.A, Trigger.Go)
            .WithMappedParameters<int, string>(x => (x + 1, "mapped!"))
            .To(State.A)
            .Build();

        // Mapped 1->3 (int -> (int, string, double))
        this.spikeMapped3to3 = new InitialTransitionConfiguration<State, Trigger, int>(State.A, Trigger.Go)
            .WithMappedParameters<int, string, double>(x => (x + 1, "mapped!", x * 3.14))
            .To(State.A)
            .Build();

        // Mapped 1->4 (int -> (int, string, double, bool))
        this.spikeMapped4to4 = new InitialTransitionConfiguration<State, Trigger, int>(State.A, Trigger.Go)
            .WithMappedParameters<int, string, double, bool>(x => (x + 1, "mapped!", x * 3.14, x > 0))
            .To(State.A)
            .Build();

        // === StateCraft Library Setup (all self-transitions) ===

        // Standard 0 parameters (parameterless self-transition)
        this.stateCraftStandard0 = StateMachine
            .Configure<State, Trigger>()
            .WithInitialState(State.A)
            .WithState(State.A, state => state.WithTransition(Trigger.Go, State.A))
            .Build();

        // Standard 1 parameter (int -> int self-transition using WithSameParameter)
        this.stateCraftStandard1 = StateMachine
            .Configure<State, Trigger>()
            .WithInitialState(State.A, 42)
            .WithState(State.A, state => state.WithParameter<int>().WithTransition(Trigger.Go, State.A))
            .Build();

        // Mapped 1->1 (int -> int self-transition with mapping)
        this.stateCraftMapped1to1 = StateMachine
            .Configure<State, Trigger>()
            .WithInitialState(State.A, 0)
            .WithState(
                State.A,
                state =>
                    state
                        .WithParameter<int>()
                        .WithTransition(Trigger.Go, t => t.WithMappedParameter(x => x + 1).To(State.A))
            )
            .Build();
    }

    [IterationSetup]
    public void IterationSetup()
    {
        // Activate StateCraft state machines
        this.stateCraftStandard0.Activate(CancellationToken.None).GetAwaiter().GetResult();
        this.stateCraftStandard1.Activate(CancellationToken.None).GetAwaiter().GetResult();
        this.stateCraftMapped1to1.Activate(CancellationToken.None).GetAwaiter().GetResult();
    }

    [IterationCleanup]
    public void IterationCleanup()
    {
        // Deactivate StateCraft state machines
        this.stateCraftStandard0.Deactivate(CancellationToken.None).GetAwaiter().GetResult();
        this.stateCraftStandard1.Deactivate(CancellationToken.None).GetAwaiter().GetResult();
        this.stateCraftMapped1to1.Deactivate(CancellationToken.None).GetAwaiter().GetResult();
    }

    // === Spike Standard Transition Benchmarks ===

    [Benchmark(Description = "Spike: 0 params")]
    [BenchmarkCategory("Standard", "Spike")]
    public async Task Spike_StandardTransition_0Parameters()
    {
        var parameters = new StateMachineParameters([], []);
        for (var i = 0; i < LoopCount; i++)
        {
            using (await this.spikeLock.LockAsync())
            {
                await this.spikeStandard0.EvaluateConditions(parameters);
                await this.spikeStandard0.Transition(parameters);
            }
        }
    }

    [Benchmark(Description = "Spike: 1 param")]
    [BenchmarkCategory("Standard", "Spike")]
    public async Task Spike_StandardTransition_1Parameter()
    {
        var parameters = new StateMachineParameters([42], [0]);
        for (var i = 0; i < LoopCount; i++)
        {
            using (await this.spikeLock.LockAsync())
            {
                await this.spikeStandard1.EvaluateConditions(parameters);
                await this.spikeStandard1.Transition(parameters);
            }
        }
    }

    [Benchmark(Description = "Spike: 2 params")]
    [BenchmarkCategory("Standard", "Spike")]
    public async Task Spike_StandardTransition_2Parameters()
    {
        var parameters = new StateMachineParameters([42], [0, ""]);
        for (var i = 0; i < LoopCount; i++)
        {
            using (await this.spikeLock.LockAsync())
            {
                await this.spikeStandard2.EvaluateConditions(parameters);
                await this.spikeStandard2.Transition(parameters);
            }
        }
    }

    [Benchmark(Description = "Spike: 3 params")]
    [BenchmarkCategory("Standard", "Spike")]
    public async Task Spike_StandardTransition_3Parameters()
    {
        var parameters = new StateMachineParameters([42], [0, "", 0.0]);
        for (var i = 0; i < LoopCount; i++)
        {
            using (await this.spikeLock.LockAsync())
            {
                await this.spikeStandard3.EvaluateConditions(parameters);
                await this.spikeStandard3.Transition(parameters);
            }
        }
    }

    [Benchmark(Description = "Spike: 4 params")]
    [BenchmarkCategory("Standard", "Spike")]
    public async Task Spike_StandardTransition_4Parameters()
    {
        var parameters = new StateMachineParameters([42], [0, "", 0.0, false]);
        for (var i = 0; i < LoopCount; i++)
        {
            using (await this.spikeLock.LockAsync())
            {
                await this.spikeStandard4.EvaluateConditions(parameters);
                await this.spikeStandard4.Transition(parameters);
            }
        }
    }

    // === Spike Mapped Transition Benchmarks ===

    [Benchmark(Description = "Spike: 1->1")]
    [BenchmarkCategory("Mapped", "Spike")]
    public async Task Spike_MappedTransition_1to1()
    {
        var parameters = new StateMachineParameters([0], [0]);
        for (var i = 0; i < LoopCount; i++)
        {
            using (await this.spikeLock.LockAsync())
            {
                await this.spikeMapped1to1.EvaluateConditions(parameters);
                await this.spikeMapped1to1.Transition(parameters);
                // Swap next -> previous for next iteration
                parameters.SetPreviousParameter(0, parameters.GetNextParameter<int>(0));
            }
        }
    }

    [Benchmark(Description = "Spike: 1->2")]
    [BenchmarkCategory("Mapped", "Spike")]
    public async Task Spike_MappedTransition_1to2()
    {
        var parameters = new StateMachineParameters([0], [0, ""]);
        for (var i = 0; i < LoopCount; i++)
        {
            using (await this.spikeLock.LockAsync())
            {
                await this.spikeMapped2to2.EvaluateConditions(parameters);
                await this.spikeMapped2to2.Transition(parameters);
                // Swap next -> previous for next iteration (only first param used for mapping)
                parameters.SetPreviousParameter(0, parameters.GetNextParameter<int>(0));
            }
        }
    }

    [Benchmark(Description = "Spike: 1->3")]
    [BenchmarkCategory("Mapped", "Spike")]
    public async Task Spike_MappedTransition_1to3()
    {
        var parameters = new StateMachineParameters([0], [0, "", 0.0]);
        for (var i = 0; i < LoopCount; i++)
        {
            using (await this.spikeLock.LockAsync())
            {
                await this.spikeMapped3to3.EvaluateConditions(parameters);
                await this.spikeMapped3to3.Transition(parameters);
                parameters.SetPreviousParameter(0, parameters.GetNextParameter<int>(0));
            }
        }
    }

    [Benchmark(Description = "Spike: 1->4")]
    [BenchmarkCategory("Mapped", "Spike")]
    public async Task Spike_MappedTransition_1to4()
    {
        var parameters = new StateMachineParameters([0], [0, "", 0.0, false]);
        for (var i = 0; i < LoopCount; i++)
        {
            using (await this.spikeLock.LockAsync())
            {
                await this.spikeMapped4to4.EvaluateConditions(parameters);
                await this.spikeMapped4to4.Transition(parameters);
                parameters.SetPreviousParameter(0, parameters.GetNextParameter<int>(0));
            }
        }
    }

    // === StateCraft Library Standard Transition Benchmarks ===

    [Benchmark(Description = "StateCraft: 0 params")]
    [BenchmarkCategory("Standard", "StateCraft")]
    public async Task StateCraft_StandardTransition_0Parameters()
    {
        for (var i = 0; i < LoopCount; i++)
        {
            await this.stateCraftStandard0.Transition(Trigger.Go, CancellationToken.None);
        }
    }

    [Benchmark(Description = "StateCraft: 1 param")]
    [BenchmarkCategory("Standard", "StateCraft")]
    public async Task StateCraft_StandardTransition_1Parameter()
    {
        for (var i = 0; i < LoopCount; i++)
        {
            await this.stateCraftStandard1.Transition(Trigger.Go, CancellationToken.None);
        }
    }

    // === StateCraft Library Mapped Transition Benchmarks ===

    [Benchmark(Description = "StateCraft: 1->1")]
    [BenchmarkCategory("Mapped", "StateCraft")]
    public async Task StateCraft_MappedTransition_1to1()
    {
        for (var i = 0; i < LoopCount; i++)
        {
            await this.stateCraftMapped1to1.Transition(Trigger.Go, CancellationToken.None);
        }
    }
}
