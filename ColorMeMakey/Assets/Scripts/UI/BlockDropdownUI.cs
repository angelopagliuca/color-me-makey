using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BlockDropdownUI : MonoBehaviour
{
    public BlockManager blockManager;

    private TMP_Dropdown dropdown;
    private IReadOnlyList<BlockMeta> metadatas;

    void Start()
    {
        dropdown = GetComponent<TMP_Dropdown>();

        LoadMetadata();
        PopulateDropdown();
    }

    void LoadMetadata()
    {
        metadatas = blockManager.metaHandler.GetAllMetadata();
    }

    void PopulateDropdown()
    {
        dropdown.ClearOptions();
        List<string> names = new List<string>();

        foreach (var meta in metadatas)
        {
            names.Add(meta.name);
        }

        dropdown.AddOptions(names);

        // Listen for selection changes
        dropdown.onValueChanged.AddListener(OnDropdownChanged);
    }

    void OnDropdownChanged(int index)
    {
        int blockIndex = metadatas[index].index;
        blockManager.LoadBlockToScene(blockIndex);
        //Debug.Log($"Selected block: {metadatas[index].name}");
    }
}
