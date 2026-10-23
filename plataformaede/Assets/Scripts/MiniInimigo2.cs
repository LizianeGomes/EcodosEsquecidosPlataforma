
using System.Collections;
using UnityEngine;

public class MiniInimigo2 : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private Transform player;

    [Range(0, 10)]
    [SerializeField] private float distanciaDetecção = 5f;

    [Header("Pulo do Inimigo")]
    [SerializeField] private float forçaHorizontal = 5f;
    [SerializeField] private float forçaVertical = 8f;
    [SerializeField] private float intervalo = 2f;

    [Header("Limites")]
    [SerializeField] private GameObject limiteEsquerda;
    [SerializeField] private GameObject limiteDireita;

    [Header("Animação")]
    [SerializeField] private string nomeEstadoPulo = "Caminhada";
    [SerializeField] private string nomeEstadoMorte = "morte";

    [Header("Pisão do Jogador")]
    [SerializeField] private float forçaImpulsoJogador = 10f;
    [SerializeField] private float alturaMinimaPisão = 0.1f;
    [SerializeField] private float tempoAnimacaoMorte = 1f;

    private Rigidbody2D rb;
    private Animator anim;
    private float cronometroPulo;

    private bool estaNoChao = true;
    private bool preparandoPulo = false;
    private bool morreu = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();

        if (player == null)
        {
            GameObject objetoPlayer = GameObject.FindGameObjectWithTag("Player");

            if (objetoPlayer != null)
                player = objetoPlayer.transform;
        }

        if (rb == null)
            Debug.LogError("MiniInimigo2 precisa de Rigidbody2D!");

        if (anim == null)
        {
            Debug.LogError("MiniInimigo2 não encontrou o Animator!");
            return;
        }

        anim.speed = 1f;
        anim.Play(nomeEstadoPulo, 0, 0f);
    }

    void Update()
    {
        if (morreu || player == null || anim == null)
            return;

        VirarParaOJogador();

        float distancia = Vector2.Distance(
            player.position,
            transform.position
        );

        if (estaNoChao && !preparandoPulo &&
            distancia <= distanciaDetecção)
        {
            cronometroPulo += Time.deltaTime;

            if (cronometroPulo >= intervalo)
            {
                cronometroPulo = 0f;
                preparandoPulo = true;

                anim.speed = 1f;
                anim.Play(nomeEstadoPulo, 0, 0f);
            }
        }
    }

    void VirarParaOJogador()
    {
        if (player == null)
            return;

        Vector3 escala = transform.localScale;

        if (player.position.x < transform.position.x)
            escala.x = -Mathf.Abs(escala.x);
        else if (player.position.x > transform.position.x)
            escala.x = Mathf.Abs(escala.x);

        transform.localScale = escala;
    }

    public void ExecutarPulo()
    {
        if (morreu || !preparandoPulo || player == null)
            return;

        float direcao = Mathf.Sign(
            player.position.x - transform.position.x
        );

        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

        rb.AddForce(
            new Vector2(direcao * forçaHorizontal, forçaVertical),
            ForceMode2D.Impulse
        );

        estaNoChao = false;
        preparandoPulo = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (morreu)
            return;

        PlayerMovement jogador =
            collision.collider.GetComponentInParent<PlayerMovement>();

        if (jogador != null)
        {
            // Confere se o jogador está caindo sobre a cabeça.
            bool contatoPorCima = false;

            foreach (ContactPoint2D contato in collision.contacts)
            {
                if (contato.normal.y < -0.5f)
                {
                    contatoPorCima = true;
                    break;
                }
            }

            Rigidbody2D rbJogador =
                jogador.GetComponent<Rigidbody2D>();

            if (rbJogador == null)
                rbJogador = jogador.GetComponentInParent<Rigidbody2D>();

            bool jogadorDescendo =
                rbJogador != null &&
                rbJogador.linearVelocity.y <= 0.5f;

            Collider2D colInimigo = GetComponent<Collider2D>();

            if (colInimigo == null)
                colInimigo = GetComponentInChildren<Collider2D>();

            Collider2D colJogador = collision.collider;

            bool jogadorAcima =
                colInimigo != null &&
                colJogador.bounds.min.y >=
                colInimigo.bounds.center.y - alturaMinimaPisão;

            if (contatoPorCima && jogadorDescendo && jogadorAcima)
            {
                // Impulso vertical no jogador.
                if (rbJogador != null)
                {
                    rbJogador.linearVelocity = new Vector2(
                        rbJogador.linearVelocity.x,
                        0f
                    );

                    rbJogador.AddForce(
                        Vector2.up * forçaImpulsoJogador,
                        ForceMode2D.Impulse
                    );
                }

                Morrer();
                return;
            }

            // Encostar de lado causa dano ao jogador.
            jogador.PerderCoracaoNoLocal();
            return;
        }

        if (collision.gameObject.CompareTag("Chao"))
        {
            if (!estaNoChao)
            {
                estaNoChao = true;
                preparandoPulo = false;

                if (anim != null)
                {
                    anim.speed = 1f;
                    anim.Play(nomeEstadoPulo, 0, 0f);
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (morreu)
            return;

        PlayerMovement jogador =
            other.GetComponentInParent<PlayerMovement>();

        if (jogador == null)
            return;

        // Evita aplicar dano por um trigger se o jogador
        // estiver tocando o inimigo por cima.
        Collider2D colInimigo = GetComponent<Collider2D>();

        if (colInimigo == null)
            colInimigo = GetComponentInChildren<Collider2D>();

        Rigidbody2D rbJogador =
            jogador.GetComponent<Rigidbody2D>();

        if (rbJogador == null)
            rbJogador = jogador.GetComponentInParent<Rigidbody2D>();

        bool jogadorAcima =
            colInimigo != null &&
            other.bounds.min.y >=
            colInimigo.bounds.center.y - alturaMinimaPisão;

        bool jogadorDescendo =
            rbJogador != null &&
            rbJogador.linearVelocity.y <= 0.5f;

        if (jogadorAcima && jogadorDescendo)
            return;

        jogador.TomarDano(1);
    }

    private void Morrer()
    {
        if (morreu)
            return;

        morreu = true;

        // Interrompe o comportamento de pulo.
        StopAllCoroutines();

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.simulated = false;
        }

        // Desativa todos os colliders do inimigo.
        Collider2D[] colliders =
            GetComponentsInChildren<Collider2D>();

        foreach (Collider2D col in colliders)
            col.enabled = false;

        if (anim != null)
        {
            anim.speed = 1f;
            anim.Play(nomeEstadoMorte, 0, 0f);
            StartCoroutine(DestruirDepoisDaMorte());
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator DestruirDepoisDaMorte()
    {
        yield return new WaitForSeconds(tempoAnimacaoMorte);
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;

        Gizmos.DrawWireSphere(
            transform.position,
            distanciaDetecção
        );
    }
}
