using UnityEngine;
using UnityEngine.InputSystem;

public class Paint : MonoBehaviour
{
    private Color currentColor;

    void Start()
    {
        ColorUtility.TryParseHtmlString("#FFFFFF", out currentColor); // or any color you want
    }

    // Update is called once per frame
    void Update()
    {
        if (Mouse.current.leftButton.isPressed)
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                GameObject hitObject = hit.collider.gameObject;

                if (hitObject.CompareTag("Pixel"))
                {
                    Pixel pixel = hitObject.GetComponent<Pixel>();
                    if (pixel != null)
                    {
                        Color randomColor = new Color(Random.value, Random.value, Random.value);
                        pixel.SetPixelColor(randomColor);
                    }
                }
            }
        }
    }
}
