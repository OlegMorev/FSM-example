using System;
using System.Collections.Generic;
using UnityEngine;

public class DIContainer
{
    Dictionary<(Type, string), object> container = new Dictionary<(Type, string), object>();
    DIContainer parentContainer;
    private void AddValueToContainer<T>(T value, string name)
    {
        container.Add((typeof(T),name), value);
    }
    public T GetDIValue<T>(string name = null)
    {
        try
        {
            return (T)container[(typeof(T), name)];
        }
        catch 
        {
            
            try
            {
                if (parentContainer != null) return parentContainer.GetDIValue<T>(name);
                else
                {
                    Debug.LogError($"DI container value with key ({typeof(T)}, {name}) was not found, returning default value for Type");
                    return default(T);
                }
            }
            catch
            {
                Debug.LogError($"DI container value with key ({typeof(T)}, {name}) was not found, returning default value for Type");
                return default(T);
            }
        }
    }
    public bool TryGetDIValue<T>(out T value, string name=null)
    {
        if(container.TryGetValue((typeof(T), name), out object obj))
        {
            value = (T)obj;
            return true;
        }
        else
        {
            if (parentContainer == null)
            {
                value = default(T);
                return false; 
            }
            else
            {
                if(parentContainer.TryGetDIValue<T>(out T parentValue, name))
                {
                    value = parentValue;
                    return true;
                }
                else
                {
                    value = default(T);
                    return false;
                }
            }
        }
    }
    public void AddDIValue<T>(T value, string name=null)
    {
        if(container.TryGetValue((typeof(T), name), out object obj))
        {
            return;
        }
        else
        {
            AddValueToContainer(value, name);
        }
    }
    public void AddDIValueForced<T>(T value, string name=null)
    {
        if (container.TryGetValue((typeof(T), name), out object obj))
        {
            container[(typeof(T), name)] = value;
        }
        else
        {
            AddValueToContainer(value, name);
        }
    }    
    public void DebugLogAllDIContainerComponents()
    {
        foreach (var component in container.Keys)
        {
            Debug.Log(component + " : " + container[component]);
        }
    }
    public DIContainer()
    {
        parentContainer = null;
    }
    public DIContainer(DIContainer parentContainer)
    {
        this.parentContainer = parentContainer;
    }
}