using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "IntVariable", menuName = "Atomic/Variables/IntVariable")]
public class IntVariable : ScriptableObject
{
    [SerializeField] private int initialValue = 0;  // value it starts with
    [SerializeField] private bool resetOnEnable = true;

    [SerializeField] private int value;
    public UnityEvent<int> OnValueChanged; // Event fired when value changes

    public int Value
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
