using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BossZombie : MonoBehaviour
{
    [Header("Referências")]
    public Transform player;          // Player que ele vai perseguir
    public Animator anim;             // Animator do zumbi

    [Header("Movimento")]
    public float speed = 2f;          // Velocidade do zumbi
    public float distanciaAtaque = 1.5f; // Distância para atacar

    [Header("attack")]
    public float danoAtaque = 10f;    // Dano que zumbi causa
    public float cooldownAtaque = 1f; // Tempo entre ataques

    [Header("Vida")]
    public float maxVida = 100f;
    
    private float vidaAtual;
    private Rigidbody2D rb;
    private float tempoUltimoAtaque;
    private AudioSource audioSource;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (anim == null) anim = GetComponent<Animator>();
        
        vidaAtual = maxVida;

        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (player == null) return;

        float distancia = Vector2.Distance(transform.position, player.position);

        // Se o zumbi morreu, não faz nada
        if (vidaAtual <= 0) return;

        // Ataque
        if (distancia <= distanciaAtaque)
        {
            rb.linearVelocity = Vector2.zero;
            anim.SetBool("run", false);

            if (Time.time - tempoUltimoAtaque >= cooldownAtaque)
            {
                anim.SetTrigger("attack");

                // Causa dano ao player
                Health vidaPlayer = player.GetComponent<Health>();
                if (vidaPlayer != null)
                {
                    vidaPlayer.TomarDano(danoAtaque);
                }

                tempoUltimoAtaque = Time.time;
            }
        }
        else // Segue o player
        {
            Vector2 direcao = (player.position - transform.position).normalized;
            rb.linearVelocity = new Vector2(direcao.x * speed, rb.linearVelocity.y);

            anim.SetBool("run", true);

            // Virar sprite para olhar o jogador
            if (direcao.x > 0)
                transform.localScale = new Vector3(1, 1, 1);
            else
                transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    // Função para receber dano do player
    public void TomarDano(float quantidade)
    {
        vidaAtual -= quantidade;
        if (vidaAtual <= 0)
        {
            Morrer();
        }
    }

    private void Morrer()
    {
        anim.SetTrigger("death_01");
        rb.linearVelocity = Vector2.zero;

        // Destrói o zumbi após 1 segundo para permitir animação de morte
        Destroy(gameObject, 1f);
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Health vida = collision.gameObject.GetComponent<Health>();
            if (vida != null)
            {
                vida.TomarDano(danoAtaque);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        // Área de ataque
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, distanciaAtaque);
    }
}
