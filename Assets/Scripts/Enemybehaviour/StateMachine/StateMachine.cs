using System;

public class StateMachine
{
    public BaseState currentState { get; private set; }
    public void SwitchState(BaseState newState)
    {
        if(currentState != null) currentState.ExitState();
        currentState = newState;
        newState.EnterState();
    }
    public void Update()
    {
        if(currentState != null) currentState.UpdateState();
    }
}
