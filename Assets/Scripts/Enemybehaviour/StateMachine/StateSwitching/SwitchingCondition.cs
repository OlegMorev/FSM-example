using System;

public class SwitchingCondition
{
    public int priorityLevel { get; private set; }
    public Func<BaseState, bool> SwitchCondition { get; private set; }
    public Func<BaseState, BaseState> NextState { get; private set; }
    public SwitchingCondition(Func<BaseState, bool> switchCondition, Func<BaseState, BaseState> nextState, int priorityLevel)
    {
        SwitchCondition = switchCondition;
        this.priorityLevel = priorityLevel;
        NextState = nextState;
    }
}