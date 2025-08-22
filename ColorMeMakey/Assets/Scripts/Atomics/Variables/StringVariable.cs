using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "StringVariable", menuName = "Atomic/Variables/StringVariable")]
public class StringVariable : ScriptableObject
{
    [SerializeField] private string initialValue = "";  // value it starts with
    [SerializeField] private bool resetOnEnable = true;

    [SerializeField] private string value;
    public UnityEvent<string> OnValueChanged; // Event fired when value changes

    public string Value
    {
        get => value;
        set
        {
            if (this.value != value)
            {
                this.value = value;
                OnValueChanged?.Invoke(this.value);
            }
        }
    }

    private void OnEnable()
    {
        if (resetOnEnable)
        {
            Value = initialValue;
        }
    }
}
