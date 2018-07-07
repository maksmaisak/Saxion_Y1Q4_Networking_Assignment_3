using System.Collections.Generic;
using System;
using UnityEngine;

public class FiniteStateMachine<TAgent> where TAgent : Component, IAgent
{
    // Maps the class name of a state to a specific instance of that state
    private readonly Dictionary<Type, FsmState<TAgent>> stateCache;

    // The current state we are in
    private FsmState<TAgent> currentState;

    // Reference to our target so we can pass into our new states.
    private readonly TAgent agent;

    public FiniteStateMachine(TAgent agent)
    {
        this.agent = agent;

        stateCache = new Dictionary<Type, FsmState<TAgent>>();
        DetectExistingStates();
    }

    public void Update()
    {
        //if (currentState != null) Debug.Log("Executing state " + currentState);
    }

    public FsmState<TAgent> GetCurrentState()
    {
        return currentState;
    }

    public void Reset()
    {
        if (currentState == null) return;
        
        currentState.Exit();
        currentState.Enter();
    }

    /**
	 * Tells the FSM to enter a state which is a subclass of AbstractState<T>.
	 * So for exampe for FSM<Bob> the state entered must be a subclass of AbstractState<Bob>
	 */
    public TState ChangeState<TState>() where TState : FsmState<TAgent>
    {
        FsmState<TAgent> state = null;
        // Check if a state like this was already in the cache
        if (!stateCache.TryGetValue(typeof(TState), out state))
        {
            // If not, create it, passing in the agent
            state = agent.gameObject.AddComponent<TState>();
            state.SetAgent(agent);
            stateCache[typeof(TState)] = state;
        }
        
        ChangeState(state);
        return (TState)state;
    }

    private void ChangeState(FsmState<TAgent> newState)
    {
        if (currentState == newState) return;

        if (currentState != null) currentState.Exit();
        currentState = newState;
        if (currentState != null) currentState.Enter();
    }

    private void DetectExistingStates()
    {
        FsmState<TAgent>[] states = agent.GetComponentsInChildren<FsmState<TAgent>>();
        foreach (FsmState<TAgent> state in states)
        {
            state.enabled = false;
            state.SetAgent(agent);
            stateCache.Add(state.GetType(), state);
        }
    }
}
