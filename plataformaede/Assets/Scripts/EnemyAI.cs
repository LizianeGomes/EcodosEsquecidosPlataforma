using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyAI : MonoBehaviour
{
    [Header("Player")]
    public Transform player;

    [Header("Movimento")]
    public float speed = 3f;
    public float distanciaAtaque = 1.2f;

    [Header("Vida")]
    public int vida = 3;

    [Header("Ataque")]
    public int dano = 1;
    public float cooldownAtaque = 1f;

    private Rigidbody2D rb;
    private float tempoAtaque;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (player == null) return;

        float distancia = Vector2.Distance(
            transform.position,
            player.position
        );

        // Seguir player
        if (distancia > distanciaAtaque)
        {
            Vector2 direcao =
                (player.position - transform.position).normalized;

            rb.linearVelocity = new Vector2(
                direcao.x * speed,
                rb.linearVelocity.y
            );
        }
        else
        {
            rb.linearVelocity = Vector2.zero;

            // Ataque
            if (Time.time > tempoAtaque)
            {
                PlayerMovement p =
                    player.GetComponent<PlayerMovement>();

                if (p != null)
                {
                    p.TomarDano(dano);
                }

                tempoAtaque = Time.time + cooldownAtaque;
            }
        }
    }

    // Dano recebido
    public void ReceberDano(int danoRecebido)
    {
        vida -= danoRecebido;

        Debug.Log("Inimigo tomou dano!");

        if (vida <= 0)
        {
            Destroy(gameObject);
        }
    }
}