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
    [Range(0f, 1f)] public float volumePulo = 0.4f;
    [Range(0f, 1f)] public float volumeAtaque = 0.7f;
    [Range(0f, 1f)] public float volumePasso = 0.5f;

    [Header("Ataque")]
    public float alcanceAtaque = 1.5f;
    public LayerMask inimigoLayer;

    [Header("HP")]
    public int maxVida = 3;

    private int vidaAtual;

    [Header("Vidas")]
    public int vidasRestantes = 3;

    [Header("Respawn")]
    public Transform respawnPoint;
    

    [Header("UI")]
    public VidaUI vidaUI;

    private Rigidbody2D rb;
    private Animator anim;
    private AudioSource audioSource;
    public Transform checkpointAtual;

    private float moveX;
    private bool isGrounded;
    private bool wasGrounded;
    private bool morto = false;
    private bool invulneravel = false;
    [SerializeField] private float tempoInvulneravel = 1f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        vidaAtual = maxVida;

        if (vidaUI != null)
            vidaUI.AtualizarVida(vidasRestantes);
    }

    void Update()
    {
        anim.SetBool("pulo", !isGrounded);
        if (morto) return;

        // Movimento
        moveX = Input.GetAxisRaw("Horizontal");

        if (moveX > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (moveX < 0)
            transform.localScale = new Vector3(-1, 1, 1);

        anim.SetBool("Andando", moveX != 0);

        // Chão
        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );

        // Pulo
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                jumpForce
            );

            anim.SetBool("pulo", true);

            audioSource.PlayOneShot(
                somPulo,
                volumePulo
            );
        }

        anim.SetBool("pulo", !isGrounded);

        // Ataque
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Atacar();
        }

        wasGrounded = isGrounded;
    }

    void FixedUpdate()
    {
        if (morto) return;

        rb.linearVelocity = new Vector2(
            moveX * speed,
            rb.linearVelocity.y
        );
    }

    void Atacar()
    {
        anim.SetTrigger("atacando");

        audioSource.PlayOneShot(
            somAtaque,
            volumeAtaque
        );

        Collider2D[] inimigos = Physics2D.OverlapCircleAll(
            transform.position,
            alcanceAtaque,
            inimigoLayer
        );

        foreach (Collider2D enemyCollider in inimigos)
        {
            EnemyAI enemy =
                enemyCollider.GetComponent<EnemyAI>();

            if (enemy != null)
            {
                enemy.ReceberDano(1);
            }
        }
    }
    public void PerderVidaDireto()
    {
        if (morto || invulneravel) return;

        Morrer();
    }

    public void TomarDano(int dano)
    {
        if (morto || invulneravel) return;

        vidaAtual -= dano;

        if (vidaAtual <= 0)
        {
            Morrer();
        }
    }

    void Morrer()
{
    morto = true;

    vidasRestantes--;

    if (vidaUI != null)
        vidaUI.AtualizarVida(vidasRestantes);

    if (vidasRestantes <= 0)
    {
        rb.linearVelocity = Vector2.zero;
        rb.simulated = false;

        ReiniciarCena();
        return;
    }

    Respawn();
}
    void Respawn()
    {
        StartCoroutine(InvulnerabilidadeTemporaria());

        vidaAtual = maxVida;

        Vector3 posicaoAnterior = transform.position;
        Vector3 novaPosicao = checkpointAtual != null ? checkpointAtual.position : respawnPoint.position;

        transform.position = novaPosicao;

        rb.linearVelocity = Vector2.zero;
        rb.simulated = true;
        morto = false;

        Unity.Cinemachine.CinemachineCamera vcam = FindFirstObjectByType<Unity.Cinemachine.CinemachineCamera>();
        if (vcam != null)
        {
            vcam.OnTargetObjectWarped(transform, novaPosicao - posicaoAnterior);
        }

        Parallax[] camadasParallax = FindObjectsByType<Parallax>(FindObjectsSortMode.None);
        foreach (Parallax camada in camadasParallax)
        {
            camada.ResetLastCameraPosition();
        }
    }
    

    void ReiniciarCena()
    {
        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }

    // Chamado por Animation Event
    public void TocarSomPasso()
    {
        audioSource.PlayOneShot(
            somAndar,
            volumePasso
        );
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(
                groundCheck.position,
                groundCheckRadius
            );
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(
            transform.position,
            alcanceAtaque
        );
    }
    public void SetCheckpoint(Transform novoCheckpoint)
{
    checkpointAtual = novoCheckpoint;
     Debug.Log("Checkpoint salvo: " + novoCheckpoint.name);
}
    private System.Collections.IEnumerator InvulnerabilidadeTemporaria()
    {
        invulneravel = true;
        yield return new WaitForSeconds(tempoInvulneravel);
        invulneravel = false;
    }
}