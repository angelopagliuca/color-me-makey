using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class BlockListUI : MonoBehaviour
{
    [SerializeField] private GameObject blockElemPrefab; // Prefab with TMP text + button

    [SerializeField] private BlockManager manager;

    [SerializeField] private Color selectedColor = Color.white;

    private void Start()
    {
        PopulateList();
    }

    public void PopulateList()
    {
        // Clear old items
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        // Get all metadata
        IReadOnlyList<BlockMeta> allBlocks = manager.metaHandler.GetAllMetadata();
        foreach (BlockMeta meta in allBlocks)
        {
            BlockElement item = Instantiate(blockElemPrefab, transform).GetComponent<BlockElement>();
            item.InitiateBlockElem(manager, meta);

            if (meta.index == manager.selectedIndex)
                item.ChangeColor(selectedColor);
            else
                item.ChangeColor(Color.black);
        }
    }
}
