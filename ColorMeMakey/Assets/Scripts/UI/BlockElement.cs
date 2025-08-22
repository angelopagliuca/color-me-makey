using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BlockElement : MonoBehaviour, IPointerClickHandler
{
    public TMP_Text nameText;

    BlockMeta blockMeta;

    [SerializeField] IntEvent LoadBlockEvent;
    [SerializeField] IntEvent DeleteBlockEvent;

    private Graphic btnImg;

    private void Awake()
    {
        btnImg = GetComponent<Button>().targetGraphic;
    }

    public void InitiateBlockElem(BlockMeta meta)
    {
        blockMeta = meta;

        if (nameText != null)
            nameText.text = blockMeta.name;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            LoadBlockEvent.Raise(blockMeta.index);
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            DeleteBlockEvent.Raise(blockMeta.index);
        }
    }

    public void ChangeColor(Color newColor)
    {
        if (btnImg != null)
            btnImg.color = newColor;
    }

    public int GetBlockIndex() { return blockMeta.index; }
}
