using ZCrew.StateCraft;
using ZCrew.StateCraft.StateMachines.Contracts;

namespace MarineSample;

public class MarineStateMachineBuilder
{
    private readonly Marine marine;
    private readonly GameDisplay display;
    private readonly Random rng;

    public MarineStateMachineBuilder(Marine marine, GameDisplay display, Random rng)
    {
        this.marine = marine;
        this.display = display;
        this.rng = rng;
    }

    public IStateMachine<MarineState, MarineTransition> CreateStateMachine()
    {
        // csharpier-ignore-start
        var stateMachine = StateMachine
            .Configure<MarineState, MarineTransition>()

            // With asynchronous actions allows these unbounded, long-running actions that don't prevent other transitions
            // These actions can listen to the cancellation token to know when to exit gracefully
            .WithAsynchronousActions()
            .WithInitialState(MarineState.Idle)
            .OnStateChange((from, transition, to) =>
            {
                this.display.UpdateState(to);
                this.display.LogStateChange(from, transition, to);
            })

            // Idle: hub state — can transition to any combat state
            .WithState(MarineState.Idle, state => state
                .OnEntry(() => this.display.LogAction("Orders?"))
                .WithTransition(MarineTransition.Move, MarineState.Moving)
                .WithTransition(MarineTransition.Attack, MarineState.Attacking)
                .WithTransition(MarineTransition.Defend, MarineState.Defending)
                .WithTransition(MarineTransition.Stimpack, t => t
                    .If(() => this.marine.Health > 10)
                    .To(MarineState.Stimpacked)))

            // Moving: periodic random position updates
            .WithState(MarineState.Moving, state => state
                .OnEntry(() => this.display.LogAction("Move it, move it!"))
                .WithAction(action => action.Invoke(async Task (token) =>
                {
                    while (!token.IsCancellationRequested)
                    {
                        await Task.Delay(1500, token);
                        this.marine.MoveRandom(this.rng);
                        this.display.LogAction($"  Position: ({this.marine.Position.X},{this.marine.Position.Y})");
                    }
                }))
                .OnExit(() => this.display.LogAction("Armed and ready!"))
                .WithTransition(MarineTransition.Halt, MarineState.Idle)
                .WithTransition(MarineTransition.Attack, MarineState.Attacking))

            // Attacking: gauss rifle fire with random kills
            .WithState(MarineState.Attacking, state => state
                .OnEntry(() => this.display.LogAction("Get Some!"))
                .WithAction(action => action.Invoke(async Task (token) =>
                {
                    while (!token.IsCancellationRequested)
                    {
                        await Task.Delay(1200, token);

                        // Use a stimpack "charge" to simulate temporarily increased damage
                        var damage = this.marine.AttackPower + this.rng.Next(-2, 3);
                        this.marine.UseStimpackCharge();

                        this.display.LogAction($"  *RATATATATA* Target hit! ({damage} damage)");
                        if (this.rng.Next(100) < 15)
                        {
                            this.marine.Kills++;
                            this.display.LogAction("  ** TARGET ELIMINATED **");
                        }
                    }
                }))
                .WithTransition(MarineTransition.Halt, MarineState.Idle)
                .WithTransition(MarineTransition.Defend, MarineState.Defending))

            // Defending: damage absorption messages
            .WithState(MarineState.Defending, state => state
                .OnEntry(() => this.display.LogAction("We could use some help here!"))
                .WithAction(action => action.Invoke(async Task (token) =>
                {
                    while (!token.IsCancellationRequested)
                    {
                        await Task.Delay(2000, token);
                        var absorbed = this.marine.Armor + this.rng.Next(1, 4);
                        this.display.LogAction($"  *CLANG* Absorbed {absorbed} damage.");
                    }
                }))
                .OnExit(() => this.display.LogAction("Right on."))
                .WithTransition(MarineTransition.Halt, MarineState.Idle)
                .WithTransition(MarineTransition.Attack, MarineState.Attacking))

            // Stimpacked: HP drain with enhanced stats
            .WithState(MarineState.Stimpacked, state => state
                .OnEntry(() =>
                {
                    this.marine.ApplyStimpack();
                    this.display.LogWarning("Awww yeah.");
                })
                .WithTransition(MarineTransition.Halt, MarineState.Idle)
                .WithTransition(MarineTransition.Attack, MarineState.Attacking)
                .WithTransition(MarineTransition.Move, MarineState.Moving))

            .Build();
        // csharpier-ignore-end

        return stateMachine;
    }
}
