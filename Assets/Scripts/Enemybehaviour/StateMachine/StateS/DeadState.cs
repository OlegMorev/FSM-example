using UnityEngine;
using UnityEngine.AI;

public class DeadState : BaseState
{
    NavMeshAgent navMeshAgent;
    public DeadState(DIContainer diContainer) : base(diContainer)
    {
        navMeshAgent = diContainer.GetDIValue<NavMeshAgent>();
    }

    public override void EnterState()
    {
        navMeshAgent.speed = 0;

        Debug.Log("Оно больше не дышит");

        GameObject.Destroy(navMeshAgent.gameObject, 3);
    }

    public override void ExitState()
    {

    }

    public override void UpdateState()
    {

    }
}
