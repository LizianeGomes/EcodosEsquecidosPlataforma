using UnityEngine;

public class BolaDeFogoBoss : MonoBehaviour
{
    private Rigidbody2D rb;
    private float velocidade;
    private int dano;

    private bool jaAtingiuJogador;

    public void Inicializar(
        Vector2 direcao,
        float velocidadeBola,
        int danoBola)
    {
        rb = GetComponent<Rigidbody2D>();

        velocidade = velocidadeBola;
        dano = danoBola;

        rb.gravityScale = 0f;
        rb.linearVelocity = direcao.normalized * velocidade;

        // Evita que a bola fique esquecida na cena.
        Destroy(gameObject, 6f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Ignora o próprio boss.
        if (other.GetComponentInParent<BossController>() != null)
            return;

        // Verifica se atingiu o jogador.
        PlayerMovement jogador =
            other.GetComponentInParent<PlayerMovement>();

        if (jogador != null && !jaAtingiuJogador)
        {
            jaAtingiuJogador = true;

            jogador.TomarDano(dano);

            Destroy(gameObject);
        }

        // Outros objetos, como chão e plataformas,
        // não destroem a bola neste teste.
    }
}