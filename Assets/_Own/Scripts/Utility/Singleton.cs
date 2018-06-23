using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Assertions;

public abstract class Singleton<T> : MyBehaviour where T : Singleton<T>
{
    private static T instance;
    
    // TODO Figure out resetting this on scene reload. Maybe just keep the singletons alive with DontDestroyOnLoad?
    private static bool applicationIsQuitting;

    public static T Instance
    {
        get
        {
            if (applicationIsQuitting) 
            {
                Debug.LogWarning($"[Singleton] Instance '{typeof(T)}' already destroyed on application quit. Won't create again - returning null.");
                return null;
            }
            
            if (instance == null)
            {
                instance = FindInstance();
                if (instance == null) CreateInstance();
            }

            return instance;
        }
    }

    private static T FindInstance()
    {
        return FindObjectOfType<T>();
    }

    private static T CreateInstance()
    {
        var gameObject = new GameObject(typeof(T).ToString());
        gameObject.transform.SetAsFirstSibling();
        return gameObject.AddComponent<T>();
    }

    protected override void Awake()
    {
        Assert.IsNull(instance, $"Can't instantiate {this}: There can only be one instance of {typeof(T)}, but one already exists: {instance}.");
        instance = (T)this;
        base.Awake();
    }

    protected override void OnDestroy()
    {
        applicationIsQuitting = true;
    }
}
