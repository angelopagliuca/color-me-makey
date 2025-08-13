using UnityEngine;

public class Pixel : MonoBehaviour
{
    private Renderer _renderer;
    private MaterialPropertyBlock _mpb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _renderer = GetComponent<Renderer>();
        _mpb = new MaterialPropertyBlock();
    }

    public void SetPixelColor(Color color)
    {
        _renderer.GetPropertyBlock(_mpb);
        _mpb.SetColor("_Color", color);
        _renderer.SetPropertyBlock(_mpb);
    }
}
