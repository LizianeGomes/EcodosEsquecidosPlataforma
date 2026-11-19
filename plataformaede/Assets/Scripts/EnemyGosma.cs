using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class EnemyGosma : MonoBehaviour
{
    [Header("Movimento")]
    [SerializeField] private float velocidade = 2f;
    [SerializeField] private bool comecaIndoParaDireita = true;

    [Header("Detecção de parede e borda")]
    [SerializeField] private LayerMask camadaChao;        // Layer do chão/paredes (não inclua a do player nem a do inimigo)
    [SerializeField] private float distanciaParede = 0.1f;
    [SerializeField] private bool virarNaBorda = false;   // marque pra ele não cair de plataformas
    [SerializeField] private float distanciaBorda = 0.6f;
    [SerializeField] private float tempoEntreViradas = 0.2f;

    [Header("Pisão")]
    [SerializeField] private float forcaRebote = 10f;

    [Header("Morte")]
    [SerializeField] private Animator anim;
    [SerializeField] private string nomeEstadoMorte = "";  // deixe vazio se não tiver animação de morte
    [SerializeField] private float tempoAnimacaoMorte = 0.5f;

    [Header("Áudio da morte")]
    [SerializeField] private AudioClip somMorte;

    [Range(0f, 1f)]
    [SerializeField] private float volumeSomMorte = 0.8f;

    private AudioSource audioSource;

    private Rigidbody2D rb;
    private Collider2D col;
    private float direcao;          // 1 = direita, -1 = esquerda
    private float proximaVirada;
    private bool morto;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();

        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;

        if (anim == null)
            anim = GetComponentInChildren<Animator>();

        rb.freezeRotation = true;

        direcao = comecaIndoParaDireita ? 1f : -1f;
        AplicarVisual();
    }

    void FixedUpdate()
    {
        if (morto) return;

        if (Time.time >= proximaVirada && (BateuNaParede() || (virarNaBorda && SemChaoNaFrente())))
            Virar();

        rb.linearVelocity = new Vector2(direcao * velocidade, rb.linearVelocity.y);
    }

    private bool BateuNaParede()
    {
        Bounds b = col.bounds;

        RaycastHit2D hit = Physics2D.BoxCast(
            b.center,
            new Vector2(0.05f, b.size.y * 0.8f),
            0f,
            Vector2.right * direcao,
            b.extents.x + distanciaParede,
            camadaChao
        );

        return hit.collider != null;
    }

    private bool SemChaoNaFrente()
    {
        Bounds b = col.bounds;

        Vector2 origem = new Vector2(
            b.center.x + direcao * (b.extents.x + 0.05f),
            b.center.y
        );

        RaycastHit2D hit = Physics2D.Raycast(
            origem,
            Vector2.down,
            b.extents.y + distanciaBorda,
            camadaChao
        );

        return hit.collider == null;
    }

    private void Virar()
    {
        direcao = -direcao;
        proximaVirada = Time.time + tempoEntreViradas;
        AplicarVisual();
    }

    // O sinal negativo faz o visual funcionar com um sprite que originalmente olha para a ESQUERDA.
    // Se o seu sprite olhar para a direita, tire o sinal negativo.
    private void AplicarVisual()
    {
        Vector3 escala = transform.localScale;
        escala.x = -Mathf.Abs(escala.x) * direcao;
        transform.localScale = escala;
    }

    // Chamado pelo script da cabeça (EnemyGosmaCabeca) quando o player pisa
    public void Pisado(Rigidbody2D rbJogador)
    {
        if (morto) return;

        if (rbJogador != null)
            rbJogador.linearVelocity = new Vector2(rbJogador.linearVelocity.x, forcaRebote);

        Morrer();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (morto) return;

        PlayerMovement jogador = collision.collider.GetComponentInParent<PlayerMovement>();
        if (jogador == null) return;

        StartCoroutine(DanoSeNaoForPisado(jogador));
    }

    private IEnumerator DanoSeNaoForPisado(PlayerMovement jogador)
    {
        // Espera o fim do passo de física. Se a cabeça foi pisada nesse toque,
        // 'morto' já terá virado true e o dano é cancelado.
        yield return new WaitForFixedUpdate();

        if (!morto && jogador != null)
            jogador.PerderCoracaoNoLocal();
    }

    private void Morrer()
    {
        if (morto) return;
        morto = true;

        // Reproduz o som da morte
        if (somMorte != null && audioSource != null)
            audioSource.PlayOneShot(somMorte, volumeSomMorte);

        foreach (Collider2D c in GetComponentsInChildren<Collider2D>())
            c.enabled = false;

        rb.linearVelocity = Vector2.zero;
        rb.simulated = false;

        if (anim != null && !string.IsNullOrEmpty(nomeEstadoMorte))
        {
            anim.Play(nomeEstadoMorte, 0, 0f);
            Destroy(gameObject, tempoAnimacaoMorte);
        }
        else
        {
            Destroy(gameObject, 1f);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Collider2D c = col != null ? col : GetComponent<Collider2D>();
        if (c == null) return;

        float dir = Application.isPlaying ? direcao : (comecaIndoParaDireita ? 1f : -1f);
        Bounds b = c.bounds;

        // parede (vermelho)
        Gizmos.color = Color.red;
        Gizmos.DrawLine(b.center, b.center + Vector3.right * dir * (b.extents.x + distanciaParede));

        // borda (verde)
        if (virarNaBorda)
        {
            Vector3 origem = new Vector3(b.center.x + dir * (b.extents.x + 0.05f), b.center.y, 0f);
            Gizmos.color = Color.green;
            Gizmos.DrawLine(origem, origem + Vector3.down * (b.extents.y + distanciaBorda));
        }
    }
}