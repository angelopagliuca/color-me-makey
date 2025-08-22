using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Atomic/Events/StringEvent")]
public class StringEvent : ScriptableObject
{
    public StringVariable value;
    private List<Action<string>> listeners = new List<Action<string>>();

    public void Raise()
    {
        for (int i = listeners.Count - 1; i >= 0; i--)
            listeners[i].Invoke(value.Value);
    }

    public void Raise(string evalue)
    {
        for (int i = listeners.Count - 1; i >= 0; i--)
            listeners[i].Invoke(evalue);
    }

    public void RegisterListener(Action<string> listener) => listeners.Add(listener);
    public void UnregisterListener(Action<string> listener) => listeners.Remove(listener);
}