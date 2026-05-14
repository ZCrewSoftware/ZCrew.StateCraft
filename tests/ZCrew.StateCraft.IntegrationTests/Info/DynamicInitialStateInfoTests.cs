namespace ZCrew.StateCraft.IntegrationTests.Info;

public class DynamicInitialStateInfoTests
{
    [Fact]
    public void InitialState_WhenDynamicProviderConfigured_ShouldBeDynamicVariantWithCapturedDescriptor()
    {
        // Arrange
        var configuration = StateMachine
            .Configure<string, string>()
            .WithInitialState(() => "A")
            .WithState("A", state => state);

        // Act
        var info = configuration.GetInfo();

        // Assert
        Assert.NotNull(info.InitialState);
        var initialState = Assert.IsAssignableFrom<IDynamicInitialStateInfo<string>>(info.InitialState);
        Assert.Equal("() => \"A\"", initialState.Descriptor);
        Assert.Empty(initialState.InitialParameterTypes);
    }

    [Fact]
    public void InitialState_WhenDynamicProvider_ShouldBeDynamicVariantWithCapturedDescriptor()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState(() => "A")
            .WithState("A", state => state)
            .Build();

        // Act
        var info = stateMachine.GetInfo();

        // Assert
        Assert.NotNull(info.InitialState);
        var initialState = Assert.IsAssignableFrom<IDynamicInitialStateInfo<string>>(info.InitialState);
        Assert.Equal("() => \"A\"", initialState.Descriptor);
        Assert.Empty(initialState.InitialParameterTypes);
    }

    [Fact]
    public void InitialState_WhenDynamicProviderConfiguredWithExplicitDescriptor_ShouldUseProvidedDescriptor()
    {
        // Arrange
        var configuration = StateMachine
            .Configure<string, string>()
            .WithInitialState(() => "A", "explicit-descriptor")
            .WithState("A", state => state);

        // Act
        var info = configuration.GetInfo();

        // Assert
        Assert.NotNull(info.InitialState);
        var initialState = Assert.IsAssignableFrom<IDynamicInitialStateInfo<string>>(info.InitialState);
        Assert.Equal("explicit-descriptor", initialState.Descriptor);
    }

    [Fact]
    public void InitialState_WhenDynamicProviderWithExplicitDescriptor_ShouldUseProvidedDescriptor()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState(() => "A", "explicit-descriptor")
            .WithState("A", state => state)
            .Build();

        // Act
        var info = stateMachine.GetInfo();

        // Assert
        Assert.NotNull(info.InitialState);
        var initialState = Assert.IsAssignableFrom<IDynamicInitialStateInfo<string>>(info.InitialState);
        Assert.Equal("explicit-descriptor", initialState.Descriptor);
    }

    [Fact]
    public void InitialParameterTypes_WhenDynamicProviderConfiguredToReturnFourParameters_ShouldReflectAllTypes()
    {
        // Arrange
        var configuration = StateMachine
            .Configure<string, string>()
            .WithInitialState(() => ("A", 1, "two", true, 4.0))
            .WithState("A", state => state.WithParameters<int, string, bool, double>());

        // Act
        var info = configuration.GetInfo();

        // Assert
        Assert.NotNull(info.InitialState);
        var initialState = Assert.IsAssignableFrom<IDynamicInitialStateInfo<string>>(info.InitialState);
        Assert.Equal([typeof(int), typeof(string), typeof(bool), typeof(double)], initialState.InitialParameterTypes);
    }

    [Fact]
    public void InitialParameterTypes_WhenDynamicProviderReturnsFourParameters_ShouldReflectAllTypes()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState(() => ("A", 1, "two", true, 4.0))
            .WithState("A", state => state.WithParameters<int, string, bool, double>())
            .Build();

        // Act
        var info = stateMachine.GetInfo();

        // Assert
        Assert.NotNull(info.InitialState);
        var initialState = Assert.IsAssignableFrom<IDynamicInitialStateInfo<string>>(info.InitialState);
        Assert.Equal([typeof(int), typeof(string), typeof(bool), typeof(double)], initialState.InitialParameterTypes);
    }
}
