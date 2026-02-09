using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movimento")]
    public float speed = 5f;
    public float jumpForce = 12f;

    [Header("Chão")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.15f;
    public LayerMask groundLayer;

    [Header("Áudio")]
    public AudioClip somPulo;
    public AudioClip somAtaque;
    public AudioClip somAndar;

    [Header("Volumes")]
    [Range(0f, 1f)]
    public float volumePulo = 0.4f;

    [Range(0f, 1f)]
    public float volumeAtaque = 0.7f;

    [Range(0f, 1f)]
    public float volumePasso = 0.5f;

    [Header("Ataque")]
    public float alcanceAtaque = 1.5f;
    public LayerMask inimigoLayer;

    [Header("Vida")]
    public float maxVida = 100f;

    private float vidaAtual;
    private Rigidbody2D rb;
    private Animator anim;
    private AudioSource audioSource;

    private float moveX;
    private bool isGrounded;
    private bool wasGrounded;
    private bool morto = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        vidaAtual = maxVida;
    }

    void Update()
    {
        if (morto) return;

        // ================= MOVIMENTO =================
        moveX = Input.GetAxisRaw("Horizontal");

        if (moveX > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (moveX < 0)
            transform.localScale = new Vector3(-1, 1, 1);

        anim.SetBool("andando", moveX != 0);

        // ================= CHÃO =================
        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );

        // ================= PULO =================
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            anim.SetBool("pulo", true);
            audioSource.PlayOneShot(somPulo, volumePulo);
        }

        if (!wasGrounded && isGrounded)
        {
            anim.SetBool("pulo", false);
        }

        // ================= ATAQUE =================
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Atacar();
        }

        wasGrounded = isGrounded;
    }

    void FixedUpdate()
    {
        if (morto) return;
        rb.linearVelocity = new Vector2(moveX * speed, rb.linearVelocity.y);
    }

    // ================= ATAQUE =================
    void Atacar()
    {
        anim.SetTrigger("atacando");
        audioSource.PlayOneShot(somAtaque, volumeAtaque);

        Collider2D[] inimigos = Physics2D.OverlapCircleAll(
            transform.position,
            alcanceAtaque,
            inimigoLayer
        );

        foreach (Collider2D inimigo in inimigos)
        {
            BossZombie zumbi = inimigo.GetComponent<BossZombie>();
            if (zumbi != null)
            {
                zumbi.ReceberAtaqueDoPlayer();
            }
        }
    }

    // ================= VIDA =================
    public void TomarDano(float dano)
    {
        if (morto) return;

        vidaAtual -= dano;

        if (vidaAtual <= 0)
        {
            Morrer();
        }
    }

    void Morrer()
    {
        morto = true;
        anim.SetTrigger("morto");

        rb.linearVelocity = Vector2.zero;
        rb.simulated = false;

        Invoke(nameof(ReiniciarCena), 1.5f);
    }

    void ReiniciarCena()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // ================= SOM DE PASSO =================
    // Chamado por Animation Event
    public void TocarSomPasso()
    {
        audioSource.PlayOneShot(somAndar, volumePasso);
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, alcanceAtaque);
    }
}
