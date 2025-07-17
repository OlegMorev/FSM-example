using System;
using UnityEngine;
using UnityEngine.AI;

public class GoToTargetState : BaseState
{
    Transform target;
    NavMeshAgent navMeshAgent;
    public GoToTargetState(DIContainer diContainer, Transform target) : base(diContainer)
    {
        this.target = target;
        navMeshAgent = diContainer.GetDIValue<NavMeshAgent>();
    }

    public override void EnterState()
    {
        navMeshAgent.speed = diContainer.GetDIValue<Func<float>>("WalkSpeed").Invoke();
    }

    public override void ExitState()
    {

    }

    public override void UpdateState()
    {
        navMeshAgent.destination = target.position;
    }
}
