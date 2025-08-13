using UnityEngine;

public class UXUtil : MonoBehaviour
{
    private Camera _mainCam;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _mainCam = Camera.main;
    }

    private float addminus = 1;
    public void FlipCamera()
    {
        addminus *= -1;
        _mainCam.transform.position = new Vector3(_mainCam.transform.position.x, _mainCam.transform.position.y + (addminus * 0.05f), _mainCam.transform.position.z);
        _mainCam.transform.eulerAngles = new Vector3(_mainCam.transform.eulerAngles.x, _mainCam.transform.eulerAngles.y, _mainCam.transform.eulerAngles.z + 180);
    }
}
