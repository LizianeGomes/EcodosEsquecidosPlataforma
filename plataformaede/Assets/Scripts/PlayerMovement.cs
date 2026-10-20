using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movimento")]
    public float speed = 5f;
    public float velocidadeNoGelo = 3f;
    public float jumpForce = 12f;

    [Header("Chão")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.15f;
    public LayerMask groundLayer;
    private bool estavaNoChao;
    
    [Header("Gelo")]
    public Collider2D colliderGelo;
    public float aceleracaoNormal = 50f;
    public float desaceleracaoNormal = 50f;
    public float aceleracaoGelo = 8f;
    public float desaceleracaoGelo = 2f;

    private bool noGelo = false;

    [Header("Áudio")]
    public AudioClip somPulo;
    public AudioClip somAtaque;
    public AudioClip somAndar;

    [Header("Volumes")]
    [Range(0f, 1f)] public float volumePulo = 0.4f;
    [Range(0f, 1f)] public float volumeAtaque = 0.7f;
    [Range(0f, 1f)] public float volumePasso = 0.5f;

    [Header("Ataque")]
    public Transform pontoAtaque;
    public float raioAtaque = 0.6f;
    public int danoAtaque = 1;
    public LayerMask Enemy;

    [Header("HP")]
    public int maxVida = 3;
    private int vidaAtual;

    [Header("Vidas")]
    public int vidasRestantes = 3;

    [Header("Respawn")]
    public Transform respawnPoint;
    
    [Header("Animações de dano e morte")]
    [SerializeField] private float tempoAnimacaoDano = 0.5f;
    [SerializeField] private float tempoAnimacaoMorte = 1.5f;

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
    private bool proximoAtaqueEh2 = false;

    [SerializeField]
    private float tempoInvulneravel = 1f;

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
        

        if (morto)
            return;

        moveX = Input.GetAxisRaw("Horizontal");

        if (moveX > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (moveX < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
            
        }

        anim.SetBool("Andando", moveX != 0);

        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );

        // ----- Animações de pulo / queda / impacto -----
        float velocidadeY = rb.linearVelocity.y;

        bool estaSubindo = !isGrounded && velocidadeY > 0.05f;
        bool estaDescendo = !isGrounded && velocidadeY < -0.05f;

        anim.SetBool("pulo", estaSubindo);
        anim.SetBool("aterrissando", estaDescendo);

        if (isGrounded && !estavaNoChao)
        {
            anim.ResetTrigger("impacto");
            anim.SetTrigger("impacto");
        }

        estavaNoChao = isGrounded;

        
        noGelo = false;

        if (isGrounded && colliderGelo != null)
        {
            Collider2D[] chaoAbaixo = Physics2D.OverlapCircleAll(
                groundCheck.position,
                groundCheckRadius
            );

            foreach (Collider2D col in chaoAbaixo)
            {
                if (col == colliderGelo)
                {
                    noGelo = true;
                    break;
                }
            }
        
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                jumpForce
            );

            

            audioSource.PlayOneShot(somPulo, volumePulo);
        }

        

        wasGrounded = isGrounded;

        Atacar();
    }

    void FixedUpdate()
    {
        if (morto)
            return;

        float velocidadeAtual = noGelo
            ? velocidadeNoGelo
            : speed;

        float velocidadeAlvo = moveX * velocidadeAtual;

        float aceleracao = noGelo
            ? aceleracaoGelo
            : aceleracaoNormal;

        float desaceleracao = noGelo
            ? desaceleracaoGelo
            : desaceleracaoNormal;

        float novaVelocidadeX;

        if (moveX != 0)
        {
            novaVelocidadeX = Mathf.MoveTowards(
                rb.linearVelocity.x,
                velocidadeAlvo,
                aceleracao * Time.fixedDeltaTime
            );
        }
        else
        {
            novaVelocidadeX = Mathf.MoveTowards(
                rb.linearVelocity.x,
                0f,
                desaceleracao * Time.fixedDeltaTime
            );
        }

        rb.linearVelocity = new Vector2(
            novaVelocidadeX,
            rb.linearVelocity.y
        );
    }

    void Atacar()
    {
        proximoAtaqueEh2 = !proximoAtaqueEh2;
        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (proximoAtaqueEh2)
            {
                anim.ResetTrigger("atacando");
                anim.SetTrigger("atacando2");
            }
            else
            {
                anim.ResetTrigger("atacando2");
                anim.SetTrigger("atacando");
            }
            audioSource.PlayOneShot(somAtaque, volumeAtaque);
        }
    }

    public void AplicarDanoAtaque()
    {
        Collider2D[] atingidos = Physics2D.OverlapCircleAll(
            pontoAtaque.position,
            raioAtaque,
            Enemy
        );

        foreach (Collider2D c in atingidos)
        {
            IDanificavel alvo = c.GetComponentInParent<IDanificavel>();

            if (alvo != null)
            {
                alvo.ReceberDano(danoAtaque);
            }
        }
    }

    public void PerderVidaDireto()
    {
        if (morto || invulneravel)
            return;

        Morrer();
    }
    public void PerderCoracaoNoLocal()
    {
        if (morto || invulneravel)
            return;

        Morrer();
    }

    public void TomarDano(int dano)
    {
        if (morto || invulneravel)
            return;

        StartCoroutine(ProcessarDano(dano));
    }

    private System.Collections.IEnumerator ProcessarDano(int dano)
    {
        // Impede que outro golpe seja recebido imediatamente.
        invulneravel = true;

        // Aplica o dano uma única vez.
        vidaAtual -= dano;

        Debug.Log("HP restante: " + vidaAtual);

        // Inicia a animação de dano.
        
        anim.SetTrigger("TomouDano");

        // Aguarda a animação terminar.
        yield return new WaitForSeconds(tempoAnimacaoDano);

        // Se o HP acabou, perde um coração ou morre definitivamente.
        if (vidaAtual <= 0)
        {
            Morrer();
            yield break;
        }

        // Se sobreviveu ao golpe, pode voltar a receber dano.
        invulneravel = false;
    }
    void Morrer()
    {
        // Perde um coração.
        vidasRestantes--;

        if (vidaUI != null)
            vidaUI.AtualizarVida(vidasRestantes);

        // Se ainda restam corações, volta ao checkpoint.
        if (vidasRestantes > 0)
        {
            Respawn();
            return;
        }

        // Só chega aqui quando perde o último coração.
        morto = true;
        invulneravel = true;

        rb.linearVelocity = Vector2.zero;

        // Inicia a animação de morte definitiva.
        
        
        anim.SetTrigger("Morte");

        // O Game Over só aparece depois da animação.
        StartCoroutine(GameOverAposAnimacao());
    }

    private System.Collections.IEnumerator GameOverAposAnimacao()
    {
        yield return new WaitForSeconds(tempoAnimacaoMorte);

        rb.linearVelocity = Vector2.zero;
        rb.simulated = false;

        if (GameOverManager.Instance != null)
        {
            GameOverManager.Instance.MostrarGameOver();
        }
    }

    void Respawn()
    {
        StartCoroutine(InvulnerabilidadeTemporaria());

        vidaAtual = maxVida;

        Vector3 posicaoAnterior = transform.position;

        Vector3 novaPosicao = checkpointAtual != null
            ? checkpointAtual.position
            : respawnPoint.position;

        transform.position = novaPosicao;

        rb.linearVelocity = Vector2.zero;
        rb.simulated = true;

        morto = false;

        Unity.Cinemachine.CinemachineCamera vcam =
            FindFirstObjectByType<Unity.Cinemachine.CinemachineCamera>();

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
        Gizmos.DrawWireSphere(
            pontoAtaque != null ? pontoAtaque.position : transform.position,
            raioAtaque
        );
    }
}