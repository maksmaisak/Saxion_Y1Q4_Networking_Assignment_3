using System;
using UnityEngine;
using UnityEngine.Assertions;

public class FsmState<T> : MyBehaviour where T : class, IAgent
{
    protected T agent;

    public bool isEntered { get; private set; }

    public void SetAgent(T agent)
    {
        Assert.IsNull(this.agent, this + ": agent is already set.");
        this.agent = agent;
    }

    public virtual void Enter()
    {
        Assert.IsFalse(isEntered, this + " is already entered. Can't Enter state.");
        
        Debug.Log("entered state:" + this);

        enabled = true;
        isEntered = true;
    }

    public virtual void Exit()
    {
        Assert.IsTrue(isEntered, this + " is not entered. Can't Exit state.");
        
        Debug.Log("exited state:" + this);
        
        enabled = false;
        isEntered = false;
    }
}


