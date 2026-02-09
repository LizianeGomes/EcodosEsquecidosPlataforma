using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{
    public Transform target;        // Personagem
    public float smoothSpeed = 5f;   // Suavidade
    public Vector3 offset;           // Distância da câmera

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;
        desiredPosition.z = transform.position.z; // mantém o Z da câmera

        Vector3 smoothPosition = Vector3.Lerp(
            transform.position,
            desiredPosition,
            smoothSpeed * Time.deltaTime
        );

        transform.position = smoothPosition;
    }
}