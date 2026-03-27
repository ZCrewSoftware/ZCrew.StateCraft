using NSubstitute;

namespace ZCrew.StateCraft.IntegrationTests.ExceptionHandling;

public class MappedTransitionExceptionTests
{
    [Fact]
    public async Task Transition_WhenMappingThrows_ShouldRouteExceptionThroughOnExceptionHandler()
    {
        // Arrange
        var mappingException = new InvalidOperationException("Mapping failed");
        var onException = Substitute.For<Action<ExceptionContext>>();

        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42)
            .OnException(onException)
            .WithState(
                "A",
                state =>
                    state
                        .WithParameter<int>()
                        .WithTransition(
                            "Go",
                            t =>
                                t.WithMappedParameter<string>(x =>
                                    {
                                        if (x == 42)
                                        {
                                            throw mappingException;
                                        }

                                        return x.ToString();
                                    })
                                    .To("B")
                        )
            )
            .WithState("B", state => state.WithParameter<string>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stateMachine.Transition("Go", TestContext.Current.CancellationToken)
        );

        // Assert
        onException.Received().Invoke(Arg.Is<ExceptionContext>(ctx => ctx.Exception == mappingException));
    }
}
