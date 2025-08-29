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

        using (var fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Read, FileShare.Read))
        {
            // Detect UTF-8 BOM
            int bomLen = 0;
            if (fs.Length >= 3)
            {
                int b0 = fs.ReadByte();
                int b1 = fs.ReadByte();
                int b2 = fs.ReadByte();
                if (b0 == 0xEF && b1 == 0xBB && b2 == 0xBF)
                    bomLen = 3;
            }

            // Start scanning after BOM (if present)
            fs.Seek(bomLen, SeekOrigin.Begin);

            // If there's any content after BOM, first line starts at bomLen
            if (fs.Length > bomLen)
                lineOffsets.Add(bomLen);

            long position = bomLen;
            int b;
            while ((b = fs.ReadByte()) != -1)
            {
                if (b == '\n')
                {
                    long nextPos = position + 1;
                    if (nextPos < fs.Length) // don't add a phantom line at EOF
                        lineOffsets.Add(nextPos);
                }
                position++;
            }
        }

        isIndexed = true;
        Debug.Log($"Indexed {lineOffsets.Count} blocks in \"blocks.jsonl\" (BOM-aware).");
    }

    // Append a new block JSON line and update offsets
    public int AddBlock(BlockData block)
    {
        string json = JsonUtility.ToJson(block, false);

        var utf8NoBom = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

        using (FileStream fs = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.Read))
        using (StreamWriter sw = new StreamWriter(fs, utf8NoBom))
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

    public void DeleteBlock(int index)
    {
        if (!isIndexed) IndexFile();

        if (index < 0 || index >= lineOffsets.Count)
        {
            return;
        }

        string tempPath = filePath + ".tmp";

        var utf8NoBom = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

        using (var reader = new StreamReader(filePath))
        using (var writer = new StreamWriter(tempPath, false, utf8NoBom))
        {
            int currentIndex = 0;
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                if (!string.IsNullOrWhiteSpace(line) && currentIndex != index)
                    writer.WriteLine(line);

                currentIndex++;
            }
        }

        File.Delete(filePath);
        File.Move(tempPath, filePath);

        Debug.Log($"Deleted block at index {index}. Remaining blocks: {lineOffsets.Count}");

        // Rebuild offsets
        IndexFile();
    }
}
