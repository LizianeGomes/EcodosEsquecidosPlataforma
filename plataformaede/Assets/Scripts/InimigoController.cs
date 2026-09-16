using UnityEngine;

public class InimigoController : MonoBehaviour
{
    private bool morto = false;

    public void MorrerPisado()
    {
        if (morto) return;

        morto = true;
        Destroy(gameObject); // por enquanto só some, sem animação
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (morto) return;

        if (other.CompareTag("Player"))
        {
            PlayerMovement player = other.GetComponent<PlayerMovement>();
            if (player != null)
                player.PerderVidaDireto();
        }
    }
}