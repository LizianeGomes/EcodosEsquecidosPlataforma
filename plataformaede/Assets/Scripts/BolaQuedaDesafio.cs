using UnityEngine;

public class BolaQuedaDesafio : MonoBehaviour
{
    private Rigidbody2D rb;

    private float velocidade;
    private int dano;
    private LayerMask camadaGround;

    private bool jaAtingiu;

    public void Inicializar(
        float velocidadeQueda,
        int danoBola,
        LayerMask ground
    )
    {
        rb = GetComponent<Rigidbody2D>();

        velocidade = velocidadeQueda;
        dano = danoBola;
        camadaGround = ground;

        if (rb != null)
        {
            rb.gravityScale = 0f;
            rb.linearVelocity = Vector2.down * velocidade;
        }

        Destroy(gameObject, 8f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (jaAtingiu)
            return;

        PlayerMovement jogador =
            other.GetComponentInParent<PlayerMovement>();

        if (jogador != null)
        {
            jaAtingiu = true;

            jogador.TomarDano(dano);

            Destroy(gameObject);
            return;
        }

        int camadaObjeto = 1 << other.gameObject.layer;

        if ((camadaGround.value & camadaObjeto) != 0)
        {
            jaAtingiu = true;
            Destroy(gameObject);
        }
    }
}