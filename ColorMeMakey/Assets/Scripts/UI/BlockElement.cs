using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class BlockElement : MonoBehaviour, IPointerClickHandler
{
    public TMP_Text nameText;

    BlockManager blockManager;
    BlockMeta blockMeta;

    public void InitiateBlockElem(BlockManager manager, BlockMeta meta)
    {
        blockManager = manager;
        blockMeta = meta;

        if (nameText != null)
            nameText.text = blockMeta.name;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            blockManager.LoadBlockToScene(blockMeta.index);
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            blockManager.DeleteBlockByIndex();
            Debug.Log($"Delete block: {blockMeta.name}");
        }
    }
}
