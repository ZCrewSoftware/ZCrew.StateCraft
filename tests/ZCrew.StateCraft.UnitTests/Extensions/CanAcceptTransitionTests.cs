using ZCrew.StateCraft.Extensions;
using ZCrew.StateCraft.StateMachines;

namespace ZCrew.StateCraft.UnitTests.Extensions;

public class CanAcceptTransitionTests
{
    [Theory]
    [InlineData((int)InternalState.Active, true)]
    [InlineData((int)InternalState.Recovery, true)]
    [InlineData((int)InternalState.Entering, true)]
    [InlineData((int)InternalState.Inactive, false)]
    [InlineData((int)InternalState.Idle, false)]
    [InlineData((int)InternalState.Exiting, false)]
    [InlineData((int)InternalState.Exited, false)]
    [InlineData((int)InternalState.Transitioning, false)]
    [InlineData((int)InternalState.Transitioned, false)]
    [InlineData((int)InternalState.Entered, false)]
    public void CanAcceptTransition_WhenGivenState_ShouldReturnExpectedResult(
        int stateValue,
        bool expected
    )
    {
        // Arrange
        var state = (InternalState)stateValue;

        // Act
        var result = state.CanAcceptTransition;

        // Assert
        Assert.Equal(expected, result);
    }
}
