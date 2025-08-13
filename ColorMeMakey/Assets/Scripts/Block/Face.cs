using UnityEngine;

public class Face : MonoBehaviour
{
    public GameObject pixelPrefab;       // Assign a quad prefab in the Inspector
    public Color pixelColor = Color.white;

    int GRIDSIZE = 16;

    void Start()
    {
        float spacing = 1 / (float)GRIDSIZE;        // Adjust spacing between quads
        for (int i = 0; i < GRIDSIZE * GRIDSIZE; i++)
        {
            int x = i % GRIDSIZE;
            int y = i / GRIDSIZE;

            Vector3 position = new Vector3(x * spacing - (0.5f - (spacing / 2)), y * spacing - (0.5f - (spacing / 2)), 0);
            GameObject pixel = Instantiate(pixelPrefab, Vector3.zero, Quaternion.identity, this.transform);
            pixel.transform.localPosition = position;
            pixel.transform.localRotation = Quaternion.identity;
            pixel.name = $"Pixel_{x}_{y}";
            pixel.tag = "Pixel";

            Renderer renderer = pixel.GetComponent<Renderer>();

            MaterialPropertyBlock mpb = new MaterialPropertyBlock();
            renderer.GetPropertyBlock(mpb);
            mpb.SetColor("_Color", pixelColor); // or any color you want
            //mpb.SetColor("_Color", new Color(Random.Range(0F, 1F), Random.Range(0, 1F), Random.Range(0, 1F))); // or any color you want
            renderer.SetPropertyBlock(mpb);
        }
    }
}
