using System;
using System.Collections.Generic;
using UnityEngine;

public class StatesSwitchingData : IDisposable
{
    StateMachine stateMachine;
    EnemyBootstrap bootstrap;
    Dictionary<Type, List<SwitchingCondition>> SwitchesMatrix = new Dictionary<Type, List<SwitchingCondition>>();
    public BaseState startState { get; private set; }
    public StatesSwitchingData AddSwitch<T>(List<SwitchingCondition> stateConditions) where T : BaseState
    {
        if(!typeof(BaseState).IsAssignableFrom(typeof(T)))
        {
            throw new ArgumentException($"{typeof(T)} не является наследником BaseState.");
        }
        else
        {
            SwitchesMatrix.Add(typeof(T), stateConditions);
        }
        return this;
    }
    public List<SwitchingCondition> GetStateSwitches(BaseState state)
    {
        if (SwitchesMatrix.TryGetValue(state.GetType(), out List<SwitchingCondition> list))
        {
            return list;
        }
        Debug.LogWarning($"No state called '{state.GetType()}' found in List");
        return new List<SwitchingCondition>();
    }
    public SwitchingCondition CurrentBestOption(BaseState targetState)
    {
        SwitchingCondition result = null;
        int maxPriority = int.MinValue;
        foreach (SwitchingCondition state in GetStateSwitches(targetState))
        {
            if (state.SwitchCondition.Invoke(targetState))
            {
                if (state.priorityLevel > maxPriority)
                {
                    maxPriority = state.priorityLevel;
                    result = state;
                }
            }
        }
        return result;
    }
    public StatesSwitchingData(BaseState startState)
    {
        this.startState = startState;
    }

    private void CheckIfSystemRequiresSwitchState(BaseState state)
    {
        SwitchingCondition possibleSwitch = CurrentBestOption(stateMachine.currentState);
        if (possibleSwitch != null)
        {
            stateMachine.SwitchState(possibleSwitch.NextState.Invoke(state));
        }
    }
    private void UpdateSwitchCheck()
    {
        CheckIfSystemRequiresSwitchState(stateMachine.currentState);
    }
    public void Build(StateMachine stateMachine, EnemyBootstrap enemyBootstrap)
    {
        this.stateMachine = stateMachine;
        this.stateMachine.SwitchState(startState);

        bootstrap = enemyBootstrap;
        bootstrap.OnUpdate += UpdateSwitchCheck;
    }

    public void Dispose()
    {
        if(bootstrap != null) bootstrap.OnUpdate -= UpdateSwitchCheck;
    }
    ~StatesSwitchingData()
    {
        Dispose();
    }
}
