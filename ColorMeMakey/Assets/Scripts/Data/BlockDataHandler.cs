using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class BlockDataHandler
{
    private readonly string filePath = Path.Combine(Application.persistentDataPath, "blocks.jsonl");
    private readonly List<long> lineOffsets = new List<long>();
    private bool isIndexed = false;

    public BlockDataHandler()
    {
        EnsureFileExists();
        IndexFile();
    }

    // Ensures the file and directory exist
    private void EnsureFileExists()
    {
        string dir = Path.GetDirectoryName(filePath);
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        if (!File.Exists(filePath))
            File.Create(filePath).Dispose();
    }

    // Build the list of line offsets for lazy loading
    private void IndexFile()
    {
        lineOffsets.Clear();

        // First line always starts at 0
        if (new FileInfo(filePath).Length > 0)
            lineOffsets.Add(0);

        using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
            long position = 0;
            int b;

            while ((b = fs.ReadByte()) != -1)
            {
                if (b == '\n')
                {
                    // Start of next line is right after newline
                    if (position + 1 < fs.Length)
                        lineOffsets.Add(position + 1);
                }
                position++;
            }
        }

        isIndexed = true;
        Debug.Log($"Indexed {lineOffsets.Count} blocks in \"blocks.jsonl\"");
    }

    // Append a new block JSON line and update offsets
    public int AddBlock(BlockData block)
    {
        string json = JsonUtility.ToJson(block, false);

        using (FileStream fs = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.Read))
        using (StreamWriter sw = new StreamWriter(fs, Encoding.UTF8))
        {
            long offset = fs.Position;   // <-- this is where the new line starts

            sw.WriteLine(json);
            sw.Flush();

            lineOffsets.Add(offset);

            int index = lineOffsets.Count - 1;
            Debug.Log($"Appended block at offset {offset} index {index}, total blocks: {lineOffsets.Count}");
            return index;
        }
    }

    // Load a block by its zero-based index
    public BlockData LoadBlockAtIndex(int index)
    {
        if (!isIndexed)
            IndexFile();

        if (index < 0 || index >= lineOffsets.Count)
        {
            Debug.LogError($"Block index {index} out of range.");
            return null;
        }

        using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
            fs.Seek(lineOffsets[index], SeekOrigin.Begin);
            using (StreamReader sr = new StreamReader(fs, Encoding.UTF8, false, 1024, true))
            {
                string line = sr.ReadLine();
                return JsonUtility.FromJson<BlockData>(line);
            }
        }
    }

    public List<BlockData> LoadAllBlocks()
    {
        var blocks = new List<BlockData>();

        using (FileStream fs = File.OpenRead(filePath))
        using (StreamReader sr = new StreamReader(fs, Encoding.UTF8))
        {
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                blocks.Add(JsonUtility.FromJson<BlockData>(line));
            }
        }

        return blocks;
    }

    // Get total number of blocks saved
    public int GetBlockCount()
    {
        if (!isIndexed)
            IndexFile();
        return lineOffsets.Count;
    }
}
