using System;

public class StateMachine : IDisposable
{
    public BaseState currentState { get; private set; }
    private EnemyBootstrap enemyBootstrap;
    public void SwitchState(BaseState newState)
    {
        if(currentState != null) currentState.ExitState();
        currentState = newState;
        newState.EnterState();
    }
    private void Update()
    {
        currentState.UpdateState();
    }

    public StateMachine(DIContainer diContainer)
    {
        enemyBootstrap = diContainer.GetDIValue<EnemyBootstrap>("EnemyBootstrap");
        enemyBootstrap.OnUpdate += Update;
    }
    public void Dispose()
    {
        if (enemyBootstrap != null) enemyBootstrap.OnUpdate -= Update;
    }
}
