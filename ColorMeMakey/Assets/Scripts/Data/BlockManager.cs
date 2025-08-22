using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class BlockManager : MonoBehaviour
{
    public GameObject[] faces; // Assign the 6 face objects in the Inspector
    int GRIDSIZE = 16;

    [HideInInspector]
    public BlockDataHandler dataHandler;
    [HideInInspector]
    public BlockMetaHandler metaHandler;

    [SerializeField] IntVariable selectedIndex;
    [SerializeField] StringVariable selectedName;

    [SerializeField] IntEvent LoadBlockEvent;
    [SerializeField] StringEvent SaveBlockEvent;
    [SerializeField] IntEvent DeleteBlockEvent;

    private void Awake()
    {
        dataHandler = new BlockDataHandler();
        metaHandler = new BlockMetaHandler();

        selectedIndex.Value = -1;
        selectedName.Value = "New Block";

        LoadBlockEvent.RegisterListener(LoadBlockToScene);
        SaveBlockEvent.RegisterListener(SaveCurrentBlock);
        DeleteBlockEvent.RegisterListener(DeleteBlock);
    }

    public void ResetBlock()
    {
        for (int faceIndex = 0; faceIndex < faces.Length; faceIndex++)
        {
            Transform face = faces[faceIndex].transform;
            for (int i = 0; i < face.childCount; i++)
            {
                if (face.GetChild(i).CompareTag("Pixel"))
                {
                    Pixel pixel = face.GetChild(i).GetComponent<Pixel>();
                    if (pixel != null)
                    {
                        Color resetColor = Color.white; // new Color(Random.value, Random.value, Random.value);
                        pixel.SetPixelColor(resetColor);
                    }
                }
            }
        }

        selectedIndex.Value = -1;
        selectedName.Value = "New Block";
    }

    public void SaveCurrentBlock(string name)
    {
        BlockData block = new BlockData(6, GRIDSIZE*GRIDSIZE);

        for (int faceIndex = 0; faceIndex < faces.Length; faceIndex++)
        {
            Transform face = faces[faceIndex].transform;
            int ipixel = 0;
            for (int i = 0; i < face.childCount; i++)
            {
                if (face.GetChild(i).CompareTag("Pixel"))
                {
                    Renderer renderer = face.GetChild(i).GetComponent<Renderer>();
                    MaterialPropertyBlock mpb = new MaterialPropertyBlock();
                    renderer.GetPropertyBlock(mpb);

                    Color color = mpb.GetColor("_Color");
                    block.faces[faceIndex].pixels[ipixel] = new PixelData((Color32)color);
                    ipixel++;
                }
            }
        }

        int blockIndex = dataHandler.AddBlock(block);
        metaHandler.AddMetadata(new BlockMeta(blockIndex, name));

        LoadBlockToScene(blockIndex);
    }

    public void LoadBlockToScene(int index)
    {
        BlockData block = dataHandler.LoadBlockAtIndex(index);

        if (block != null)
        {
            for (int faceIndex = 0; faceIndex < faces.Length; faceIndex++)
            {
                Transform face = faces[faceIndex].transform;
                int ipixel = 0;
                for (int i = 0; i < face.childCount; i++)
                {
                    if (face.GetChild(i).CompareTag("Pixel"))
                    {
                        Renderer renderer = face.GetChild(i).GetComponent<Renderer>();
                        MaterialPropertyBlock mpb = new MaterialPropertyBlock();
                        mpb.SetColor("_Color", block.faces[faceIndex].pixels[ipixel].ToColor32());
                        renderer.SetPropertyBlock(mpb);
                        ipixel++;
                    }
                }
            }
        }

        selectedIndex.Value = index;
        selectedName.Value = metaHandler.GetMetadataByIndex(index).name;
    }

    public void LoadBlockToScene(string blockName)
    {
        var metadata = metaHandler.GetMetadataByName(blockName);
        if (metadata == null)
        {
            Debug.LogError($"Block named '{blockName}' not found.");
            return;
        }
        LoadBlockToScene(metadata.index);
    }

    public void DeleteBlock(int index) 
    {
        dataHandler.DeleteBlock(index);
        metaHandler.DeleteMetadata(index);

        if (index < selectedIndex.Value) selectedIndex.Value -= 1;
        if (index == selectedIndex.Value) ResetBlock();
    }
}
