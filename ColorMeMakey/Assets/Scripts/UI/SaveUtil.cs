using UnityEngine;
using TMPro;

public class SaveUtil : MonoBehaviour
{
    [SerializeField] GameObject popups;
    [SerializeField] GameObject savepop;
    [SerializeField] StringVariable selectedName;
    [SerializeField] TMP_InputField nameInput;
    [SerializeField] StringVariable savedName;

    public void SavePopupOnOff(bool on)
    {
        popups.SetActive(on);
        savepop.SetActive(on);
    }

    public void ResetSaveInput()
    {
        savedName.Value = string.Empty;

        if (selectedName.Value != "New Block")
        {
            nameInput.text = selectedName.Value;
            return;
        }

        nameInput.text = string.Empty;
    }
}
