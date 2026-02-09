using UnityEngine;

public class BackgroundFollowCamera : MonoBehaviour
{
    private Transform cam;
    private Vector3 startPos;

    void Start()
    {
        cam = Camera.main.transform;
        startPos = transform.position;
    }

    void LateUpdate()
    {
        transform.position = new Vector3(
            cam.position.x,
            startPos.y,
            startPos.z
        );
    }
}