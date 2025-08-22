using UnityEngine;
using UnityEngine.Events;

public class IntEventListener : MonoBehaviour
{
    [Tooltip("The IntVariable to listen to.")]
    public IntVariable variable;

    [Tooltip("Response when the IntVariable changes.")]
    public UnityEvent<int> Response;

    private void OnEnable()
    {
        if (variable != null)
            variable.OnValueChanged.AddListener(OnEventRaised);
    }

    private void OnDisable()
    {
        if (variable != null)
            variable.OnValueChanged.RemoveListener(OnEventRaised);
    }

    public void OnEventRaised(int newValue)
    {
        Response.Invoke(newValue);
    }
}
