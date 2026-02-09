using UnityEngine;

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

    [Header("Ataque")]
    public float danoAtaque = 20f;       
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

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        vidaAtual = maxVida;
    }

    void Update()
    {
        // INPUT HORIZONTAL
        moveX = Input.GetAxisRaw("Horizontal");

        // VIRAR SPRITE
        if (moveX > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (moveX < 0)
            transform.localScale = new Vector3(-1, 1, 1);

        // CHECAGEM DE CHÃO
        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );

        // ANIMAÇÃO ANDAR
        anim.SetBool("andando", moveX != 0);

        // PULO
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            anim.SetBool("pulo", true);
            audioSource.PlayOneShot(somPulo);
        }

        // DESLIGA PULO AO ATERRISSAR
        if (!wasGrounded && isGrounded)
        {
            anim.SetBool("pulo", false);
        }

        // ATAQUE (Z)
        if (Input.GetKeyDown(KeyCode.Z))
        {
            anim.SetTrigger("atacando");
            audioSource.PlayOneShot(somAtaque);

            // Detecta inimigos no alcance
            Collider2D[] inimigos = Physics2D.OverlapCircleAll(transform.position, alcanceAtaque, inimigoLayer);
            foreach (Collider2D inimigo in inimigos)
            {
                Health vidaInimigo = inimigo.GetComponent<Health>();
                if (vidaInimigo != null)
                {
                    vidaInimigo.TomarDano(danoAtaque);
                }
            }
        }

        wasGrounded = isGrounded;
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(moveX * speed, rb.linearVelocity.y);
    }

    
    public void TocarSomPasso()
    {
        audioSource.PlayOneShot(somAndar);
    }

    
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
        rb.linearVelocity = Vector2.zero;
        Destroy(gameObject, 1f);
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
