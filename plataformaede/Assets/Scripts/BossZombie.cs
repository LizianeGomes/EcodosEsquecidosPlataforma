using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BossZombie : MonoBehaviour
{
    [Header("Referências")]
    public Transform player;
    public Animator anim;

    [Header("Movimento")]
    public float speed = 2f;
    public float distanciaDeteccao = 6f;
    public float distanciaAtaque = 1.5f;

    [Header("Ataque do Zumbi")]
    public float danoAtaque = 10f;
    public float cooldownAtaque = 1f;

    [Header("Vida do Zumbi")]
    public float maxVida = 100f;
    [Range(0f, 100f)]
    public float danoRecebidoPercentual = 25f;

    private float vidaAtual;
    private Rigidbody2D rb;
    private float tempoUltimoAtaque;
    private bool morto = false;

    private AudioSource musicaZumbi;
    private bool musicaTocando = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (anim == null) anim = GetComponent<Animator>();
        vidaAtual = maxVida;
            rb = GetComponent<Rigidbody2D>();
            anim = GetComponent<Animator>();

            musicaZumbi = GetComponent<AudioSource>();
            vidaAtual = maxVida;
    }

    void Update()
    {
        if (player == null || morto) return;
        
        float distancia = Vector2.Distance(transform.position, player.position);

// ===== CONTROLE DA MÚSICA DO ZUMBI =====
        if (distancia <= distanciaDeteccao && !musicaTocando)
        {
            musicaZumbi.Play();
            musicaTocando = true;
        }
        else if (distancia > distanciaDeteccao && musicaTocando)
        {
            musicaZumbi.Stop();
            musicaTocando = false;
        }

        if (distancia > distanciaDeteccao)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            anim.SetBool("andando", false);
            return;
        }

        if (distancia > distanciaAtaque)
        {
            Vector2 dir = (player.position - transform.position).normalized;
            rb.linearVelocity = new Vector2(dir.x * speed, rb.linearVelocity.y);
            anim.SetBool("andando", true);

            transform.localScale = new Vector3(dir.x > 0 ? 1 : -1, 1, 1);
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
            anim.SetBool("andando", false);

            if (Time.time - tempoUltimoAtaque >= cooldownAtaque)
            {
                anim.SetTrigger("attack");

                PlayerMovement vidaPlayer = player.GetComponent<PlayerMovement>();
                if (vidaPlayer != null)
                {
                    vidaPlayer.TomarDano(danoAtaque);
                }

                tempoUltimoAtaque = Time.time;
            }
        }
    }

    // 🔥 CHAMADO PELO PLAYER
    public void ReceberAtaqueDoPlayer()
    {
        if (morto) return;

        float dano = maxVida * (danoRecebidoPercentual / 100f);
        vidaAtual -= dano;

        anim.SetTrigger("hit");

        if (vidaAtual <= 0)
        {
            Morrer();
        }
    }

    private void Morrer()
    {
        morto = true;
        anim.SetTrigger("death_01");
        rb.linearVelocity = Vector2.zero;
        rb.simulated = false;
        Destroy(gameObject, 1.3f);
        morto = true;

        if (musicaZumbi.isPlaying)
            musicaZumbi.Stop();

        anim.SetTrigger("death_01");
        rb.linearVelocity = Vector2.zero;
        rb.simulated = false;

        Destroy(gameObject, 1.3f);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, distanciaDeteccao);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, distanciaAtaque);
    }
}
