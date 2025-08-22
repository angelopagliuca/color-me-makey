using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Atomic/Events/IntEvent")]
public class IntEvent : ScriptableObject
{
    public IntVariable value;
    private List<Action<int>> listeners = new List<Action<int>>();

    public void Raise()
    {
        for (int i = listeners.Count - 1; i >= 0; i--)
            listeners[i].Invoke(value.Value);
    }

    public void Raise(int evalue)
    {
        for (int i = listeners.Count - 1; i >= 0; i--)
            listeners[i].Invoke(evalue);
    }

    public void RegisterListener(Action<int> listener) => listeners.Add(listener);
    public void UnregisterListener(Action<int> listener) => listeners.Remove(listener);
}