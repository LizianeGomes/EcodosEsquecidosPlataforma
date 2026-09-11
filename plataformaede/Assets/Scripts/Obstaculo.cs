using UnityEngine;

public class Obstaculo : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerMovement player = other.GetComponent<PlayerMovement>();

        if (player != null)
        {
            player.PerderVidaDireto();
        }
    }
}