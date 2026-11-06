using UnityEngine;

public class VidaExtra : MonoBehaviour
{
    [SerializeField] private AudioClip somColeta; // opcional
    [SerializeField] [Range(0f, 1f)] private float volume = 0.7f;

    private bool coletada;

    // Stay (e não Enter) para que, se o player estiver em cima do item com a
    // vida cheia e perder um coração ali, ele possa coletar sem sair e voltar.
    private void OnTriggerStay2D(Collider2D other)
    {
        if (coletada) return;

        PlayerMovement jogador = other.GetComponentInParent<PlayerMovement>();
        if (jogador == null) return;

        // Vida cheia: ignora e deixa o item onde está
        if (!jogador.GanharVida()) return;

        coletada = true;

        if (somColeta != null)
            AudioSource.PlayClipAtPoint(somColeta, transform.position, volume);

        Destroy(gameObject);
    }
}