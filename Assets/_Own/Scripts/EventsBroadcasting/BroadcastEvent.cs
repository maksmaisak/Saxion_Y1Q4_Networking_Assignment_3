using System;
using UnityEngine.Assertions;

public interface IBroadcastEvent
{
    void PostEvent();
    void DeliverEvent();
}

public abstract class BroadcastEvent<T> : IBroadcastEvent where T : BroadcastEvent<T>
{
    public delegate void Handler(T eventData);
    public static event Handler handlers;

    private bool isPosted;
    private bool isDelivered;

    public void PostEvent()
    {
        Assert.IsFalse(isPosted, $"{this} has already been posted!");
        EventsManager.Instance.Post(this);
        isPosted = true;
    }

    void IBroadcastEvent.DeliverEvent()
    {
        Assert.IsFalse(isDelivered, $"{this} has already been delivered!");
        handlers?.Invoke((T)this);
        isDelivered = true;
    }
}
