using UnityEngine;

public abstract class BaseState
{
    public DIContainer diContainer { get; protected set; }
    public BaseState(DIContainer diContainer)
    {
        this.diContainer = diContainer;
    }
    public abstract void EnterState();
    public abstract void ExitState();
    public abstract void UpdateState();
}
