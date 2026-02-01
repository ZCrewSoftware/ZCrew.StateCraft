namespace Spike.Contracts;

public interface IStateMachineParameters
{
    void SetPreviousParameter<T>(int index, T value);
    T GetPreviousParameter<T>(int index);
    void SetNextParameter<T>(int index, T value);
    T GetNextParameter<T>(int index);
}
