namespace ZCrew.StateCraft;

public enum ExceptionCallSite
{
    OnEntry,
    OnExit,
    OnStateChange,
    OnActivate,
    OnDeactivate,
    Condition,
    Map,
    Action,
    Trigger,
}
