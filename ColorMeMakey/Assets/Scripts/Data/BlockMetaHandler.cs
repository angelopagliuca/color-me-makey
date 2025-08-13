using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class BlockMetaHandler
{
    private readonly string filePath = Path.Combine(Application.persistentDataPath, "blocks_meta.json");

    BlockMetas metadataList;

    public BlockMetaHandler()
    {
        LoadMetadata();
    }

    // Load metadata from file or create empty list if missing
    public void LoadMetadata()
    {
        if (!File.Exists(filePath))
        {
            metadataList = new BlockMetas();
            SaveMetadata();  // Create empty file to avoid future errors
            return;
        }

        string json = File.ReadAllText(filePath);
        if (string.IsNullOrWhiteSpace(json))
        {
            metadataList = new BlockMetas();
            return;
        }

        metadataList = JsonUtility.FromJson<BlockMetas>(json);
        if (metadataList == null)
            metadataList = new BlockMetas();
    }

    // Save the current metadata list to disk
    public void SaveMetadata()
    {
        string json = JsonUtility.ToJson(metadataList, true);
        File.WriteAllText(filePath, json);
    }

    // Add a new metadata entry
    public void AddMetadata(BlockMeta newMetadata)
    {
        if (metadataList == null)
            LoadMetadata();

        metadataList.blocks.Add(newMetadata);
        SaveMetadata();
    }

    // Find metadata by block name (returns first match or null)
    public BlockMeta GetMetadataByName(string name)
    {
        if (metadataList == null)
            LoadMetadata();

        return metadataList.blocks.Find(meta => string.Equals(meta.name, name, StringComparison.OrdinalIgnoreCase));
    }

    // Get all metadata entries (read-only)
    public IReadOnlyList<BlockMeta> GetAllMetadata()
    {
        if (metadataList == null)
            LoadMetadata();

        return metadataList.blocks.AsReadOnly();
    }
}
