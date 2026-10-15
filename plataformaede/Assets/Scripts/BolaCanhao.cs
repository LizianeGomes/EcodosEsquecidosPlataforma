using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class BolaCanhao : MonoBehaviour
{
    [Header("Configurações")]
    public int dano = 1;
    public float tempoDeVida = 5f;
    public float escalaGravidade = 1.5f; // quanto maior, mais fechado e rápido o arco

    private Rigidbody2D rb;
    private bool lancada;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = escalaGravidade;
    }

    public void Lancar(Vector2 direcao, float velocidade)
    {
        lancada = true;
        rb.linearVelocity = direcao * velocidade;
    }
    private void Start()
    {
        Destroy(gameObject, tempoDeVida);
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