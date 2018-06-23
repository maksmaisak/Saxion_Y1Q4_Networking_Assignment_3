using System;
using UnityEngine;
using UnityEngine.Assertions;

public class FSMState<T> : MyBehaviour where T : class, IAgent
{
    protected T agent;

    public void SetAgent(T agent)
    {
        Assert.IsNull(this.agent, this + ": agent is already set.");
        this.agent = agent;
    }

    public virtual void Enter()
    {
        Assert.IsFalse(enabled, this + " is already enabled.");
        
        Debug.Log("entered state:" + this);
        enabled = true;
    }

    public virtual void Exit()
    {
        Assert.IsTrue(enabled, this + " is already disabled.");
        
        Debug.Log("exited state:" + this);
        enabled = false;
    }
}


