using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class BolaCanhao : MonoBehaviour
{
    [Header("Configurações")]
    public int dano = 1;
    public float tempoDeVida = 5f;

    private Rigidbody2D rb;
    private bool lancada;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        Destroy(gameObject, tempoDeVida);
    }

    public void Lancar(Vector2 direcao, float velocidade)
    {
        lancada = true;

        rb.gravityScale = 0f;
        rb.linearVelocity = direcao * velocidade;
    }

    private void OnCollisionEnter2D(Collision2D colisao)
    {
        if (!lancada)
            return;

        PlayerMovement jogador =
            colisao.collider.GetComponentInParent<PlayerMovement>();

        if (jogador != null)
        {
            jogador.TomarDano(dano);
        }

        Destroy(gameObject);
    }
}