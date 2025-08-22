using UnityEngine;
using UnityEngine.Events;

public class StringEventListener : MonoBehaviour
{
    [Tooltip("The StringVariable to listen to.")]
    public StringVariable variable;

    [Tooltip("Response when the StringVariable changes.")]
    public UnityEvent<string> Response;

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

    public void OnEventRaised(string newValue)
    {
        Response.Invoke(newValue);
    }
}
