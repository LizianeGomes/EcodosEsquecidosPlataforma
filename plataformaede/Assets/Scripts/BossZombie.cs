using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BossZombie : MonoBehaviour
{
    [Header("Referências")]
    public Transform player;
    public Animator anim;

    [Header("Movimento")]
    public float speed = 2f;
    public float distanciaAtaque = 1.5f;

    [Header("Música do Boss")]
    public AudioClip musicaBoss;
    public float musicaMaxDistance = 10f;
    public float fadeSpeed = 1f;

    [Header("Ataque")]
    public float danoAtaque = 10f; // dano ao jogador
    public float cooldownAtaque = 1f;

    [Header("Vida")]
    public float maxVida = 100f;

    private Rigidbody2D rb;
    private AudioSource audioSource;
    private float tempoUltimoAtaque;
    private float vidaAtual;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (anim == null) anim = GetComponent<Animator>();

        // Vida do boss
        vidaAtual = maxVida;

        // Configura AudioSource
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = musicaBoss;
        audioSource.loop = true;
        audioSource.spatialBlend = 1f; // Som 3D
        audioSource.rolloffMode = AudioRolloffMode.Linear;
        audioSource.maxDistance = musicaMaxDistance;
        audioSource.volume = 0f;
        audioSource.Play();
    }

    void Update()
    {
        if (player == null) return;

        float distancia = Vector2.Distance(transform.position, player.position);

        // Fade da música do boss
        if (distancia <= musicaMaxDistance)
            audioSource.volume = Mathf.MoveTowards(audioSource.volume, 1f, fadeSpeed * Time.deltaTime);
        else
            audioSource.volume = Mathf.MoveTowards(audioSource.volume, 0f, fadeSpeed * Time.deltaTime);

        // Se o boss morreu, não faz nada
        if (vidaAtual <= 0) return;

        // Ataque
        if (distancia <= distanciaAtaque)
        {
            rb.velocity = Vector2.zero;
            anim.SetBool("andando", false);

            if (Time.time - tempoUltimoAtaque >= cooldownAtaque)
            {
                anim.SetTrigger("atacando");

                // Causa dano ao jogador
                Health vidaPlayer = player.GetComponent<Health>();
                if (vidaPlayer != null)
                {
                    vidaPlayer.TomarDano(danoAtaque);
                }

                tempoUltimoAtaque = Time.time;
            }
        }
        else // Segue o jogador
        {
            Vector2 direcao = (player.position - transform.position).normalized;
            rb.velocity = new Vector2(direcao.x * speed, rb.velocity.y);

            anim.SetBool("andando", true);

            // Virar sprite para olhar o jogador
            if (direcao.x > 0)
                transform.localScale = new Vector3(1, 1, 1);
            else
                transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    // Função para receber dano
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
        anim.SetTrigger("morto");
        rb.velocity = Vector2.zero;

        // Opcional: destruir após animação de morte
        Destroy(gameObject, 1f);
    }

    void OnDrawGizmosSelected()
    {
        // Área de ataque
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, distanciaAtaque);

        // Área da música
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, musicaMaxDistance);
    }
}
