using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class BlockListUI : MonoBehaviour
{
    [SerializeField] private GameObject blockElemPrefab; // Prefab with TMP text + button

    [SerializeField] private BlockManager manager;
    private IReadOnlyList<BlockMeta> metas;
    private List<BlockElement> elements = new List<BlockElement>();

    [SerializeField] private IntVariable selectedIndex;
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
            elements.Clear();
        }

        // Get all metadata
        metas = manager.metaHandler.GetAllMetadata();
        foreach (BlockMeta meta in metas)
        {
            BlockElement item = Instantiate(blockElemPrefab, transform).GetComponent<BlockElement>();
            item.InitiateBlockElem(meta);

            if (meta.index == selectedIndex.Value)
                item.ChangeColor(selectedColor);
            else
                item.ChangeColor(Color.black);

            elements.Add(item);
        }
    }

    public void UpdateList()
    {
        foreach (BlockElement elem in elements)
        {
            if (elem.GetBlockIndex() == selectedIndex.Value)
                elem.ChangeColor(selectedColor);
            else
                elem.ChangeColor(Color.black);
        }
    }
}
