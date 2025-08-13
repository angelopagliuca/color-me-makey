
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[Serializable]
public struct PixelData
{
    public byte r, g, b, a;
    public PixelData(Color32 color)
    {
        r = color.r; g = color.g; b = color.b; a = color.a;
    }
    public Color32 ToColor32() => new Color32(r, g, b, a);
}

[Serializable]
public class FaceData
{
    public PixelData[] pixels; // length = 256 for 16×16

    public FaceData(int pixelCount)
    {
        pixels = new PixelData[pixelCount];
    }
}

[Serializable]
public class BlockData
{
    public FaceData[] faces; // length = 6

    public BlockData(int facesCount, int pixelCountPerFace)
    {
        faces = new FaceData[facesCount];
        for (int i = 0; i < facesCount; i++)
        {
            faces[i] = new FaceData(pixelCountPerFace);
        }
    }
}

[Serializable]
public class BlockMeta
{
    public int index;           // Line number in blocks.jsonl
    public string name;         
    public string createdAt;    // ISO 8601 timestamp

    public BlockMeta(int index, string name = "")
    {
        this.index = index;
        createdAt = DateTime.UtcNow.ToString("o");
        if (name == "") this.name = createdAt;
        else this.name = name;

    }
}

[Serializable]
public class BlockMetas
{
    public List<BlockMeta> blocks = new List<BlockMeta>();
}