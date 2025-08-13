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

    private void Awake()
    {
        dataHandler = new BlockDataHandler();
        metaHandler = new BlockMetaHandler();
    }

    private void Update()
    {
        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            ResetBlock();
        }

        if (Keyboard.current.sKey.wasPressedThisFrame)
        {
            SaveCurrentBlock();
        }

        if (Keyboard.current.numpad0Key.wasPressedThisFrame)
        {
            LoadBlockToScene(0);
        }

        if (Keyboard.current.numpad1Key.wasPressedThisFrame)
        {
            LoadBlockToScene(1);
        }

        if (Keyboard.current.numpad2Key.wasPressedThisFrame)
        {
            LoadBlockToScene(2);
        }
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
    }

    public void SaveCurrentBlock()
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

        BlockMeta meta = new BlockMeta(dataHandler.GetBlockCount());

        dataHandler.AddBlock(block);
        metaHandler.AddMetadata(meta);
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
}
