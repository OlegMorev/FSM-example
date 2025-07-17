using System;
using System.Collections.Generic;
using UnityEngine;

public class StatesSwitchingData
{
    StateMachine stateMachine;
    Dictionary<Type, List<SwitchingCondition>> SwitchesMatrix = new Dictionary<Type, List<SwitchingCondition>>();
    private event Action OnUpdate;
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
    public void Update()
    {
        OnUpdate?.Invoke();
    }
    public void Build(StateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
        this.stateMachine.SwitchState(startState);


        OnUpdate += () => CheckIfSystemRequiresSwitchState(this.stateMachine.currentState);
    }
}
