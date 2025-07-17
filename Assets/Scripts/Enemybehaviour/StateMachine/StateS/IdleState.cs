using UnityEngine.AI;

public class IdleState : BaseState
{
    NavMeshAgent navMeshAgent;
    public IdleState(DIContainer diContainer) : base(diContainer)
    {
        navMeshAgent = diContainer.GetDIValue<NavMeshAgent>();
    }

    public override void EnterState()
    {
        navMeshAgent.speed = 0;
    }

    public override void ExitState()
    {

    }

    public override void UpdateState()
    {

    }
}
