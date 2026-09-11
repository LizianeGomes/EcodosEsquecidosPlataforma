using UnityEngine;

public class Parallax : MonoBehaviour
{
    public Transform camera;
    public float parallaxFactor = 0.5f; // 0 = fica parado, 1 = segue igual à câmera

    private Vector3 lastCameraPosition;

    void Start()
    {
        lastCameraPosition = camera.position;
    }

    void LateUpdate()
    {
        Vector3 delta = camera.position - lastCameraPosition;
        transform.position += new Vector3(delta.x * parallaxFactor, delta.y * parallaxFactor, 0);
        lastCameraPosition = camera.position;
    }
}