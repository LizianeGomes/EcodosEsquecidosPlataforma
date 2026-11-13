using UnityEngine;

public class GosmaCorpoDano : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerMovement jogador =
            other.GetComponentInParent<PlayerMovement>();

        if (jogador != null)
        {
            jogador.PerderVidaDireto();
        }
    }
}