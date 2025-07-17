using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;
using Unity.VisualScripting;
public class EnemyBootstrap : MonoBehaviour
{
    [Header("Injections")]
    [SerializeField] float _walkSpeed;
    [SerializeField] NavMeshAgent _agent;

    [Header("Test Variables")]
    [SerializeField] bool _isGoingToTarget = false;
    [SerializeField] List<Transform> _possibleTargets = new List<Transform>();
    [SerializeField] int _targetID;
    [SerializeField] bool _isDead = false;

    DIContainer diContainer;
    bool isInitialized = false;
    StateMachine stateMachine;
    StatesSwitchingData switchingData;

    private void Start()
    {
        if (!isInitialized) Initialize();
    }
    private void Update()
    {
        switchingData.Update();
        stateMachine.Update();

        Debug.Log(stateMachine.currentState);
    }

    private void Initialize(DIContainer diContainer = null)
    {
        isInitialized = true;
        this.diContainer = new DIContainer(diContainer);

        // Test Values
        this.diContainer.AddDIValue<Func<List<Transform>>>(() => _possibleTargets, "Targets");
        this.diContainer.AddDIValue<Func<int>>(() => _targetID, "TargetID");
        this.diContainer.AddDIValue<Func<bool>>(() => _isDead, "IsDead");
        this.diContainer.AddDIValue<Func<bool>>(() => _isGoingToTarget, "IsGoing");
        //

        this.diContainer.AddDIValue<Func<float>>(() => _walkSpeed, "WalkSpeed");
        this.diContainer.AddDIValue<EnemyBootstrap>(this, "EnemyBootstrap");
        this.diContainer.AddDIValue<NavMeshAgent>(_agent);

        stateMachine = new StateMachine();
        this.diContainer.AddDIValue<StateMachine>(stateMachine, "SM");


        switchingData = new StatesSwitchingData(new IdleState(this.diContainer))
            .AddSwitch<IdleState>(new List<SwitchingCondition>()
            {
                new SwitchingCondition(
                    (state) =>
                    {
                        if(!state.diContainer.TryGetDIValue<Func<bool>>(out Func<bool> isGoing, "IsGoing")) return false;
                        if(!state.diContainer.TryGetDIValue<Func<List<Transform>>>(out Func<List<Transform>> targetsList, "Targets")) return false;
                        if(!state.diContainer.TryGetDIValue<Func<int>>(out Func<int> targetID, "TargetID")) return false;

                        if(targetsList.Invoke().Count == 0) return false;
                        if((targetID.Invoke() < 0) || (targetID.Invoke() >= targetsList.Invoke().Count)) return false;

                        return isGoing.Invoke();
                    },
                    (state) =>
                    {
                        List<Transform> targets = state.diContainer.GetDIValue<Func<List<Transform>>>("Targets").Invoke();
                        int targetID = state.diContainer.GetDIValue<Func<int>>("TargetID").Invoke();
                        return new GoToTargetState(state.diContainer, targets[targetID]);
                    },
                    1),
                new SwitchingCondition(
                    (state) =>
                    {
                        if(!state.diContainer.TryGetDIValue<Func<bool>>(out Func<bool> isDead, "IsDead")) return false;
                        return isDead.Invoke();
                    },
                    (state) =>
                    {
                        return new DeadState(state.diContainer);
                    },
                    9)
            })
            .AddSwitch<GoToTargetState>(new List<SwitchingCondition>()
            {
                new SwitchingCondition(
                    (state) =>
                    {
                        if(!state.diContainer.TryGetDIValue<Func<bool>>(out Func<bool> isGoing, "IsGoing")) return true;
                        return !isGoing.Invoke();
                    },
                    (state) =>
                    {
                        return new IdleState(state.diContainer);
                    },
                    1),
                new SwitchingCondition(
                    (state) =>
                    {
                        if(!state.diContainer.TryGetDIValue<Func<bool>>(out Func<bool> isDead, "IsDead")) return false;
                        return isDead.Invoke();
                    },
                    (state) =>
                    {
                        return new DeadState(state.diContainer);
                    },
                    9)
            });


        switchingData.Build(stateMachine);
    }
}