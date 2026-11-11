
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class VilãCanhao : MonoBehaviour
{
    [Header("Referências")]
    public Transform jogador;
    public Transform pontoDisparo;
    public GameObject prefabBomba;
    public Animator anim;

    [Header("Ataque")]
    public bool atacar = true;
    public float cooldown = 3f;
    public int bombasPorSalva = 3;
    public float intervaloEntreBombas = 0.25f;
    public float tempoAnimacaoAtaque = 1.5f;

    [Header("Detecção do jogador")]
    public float distanciaDeteccao = 8f;

    [Header("Mira")]
    public float velocidadeBomba = 7f;
    public float erroMira = 1f;
    public float distanciaMaximaMira = 3f;
    public float distanciaMaximaDisparo = 6f;

    [Header("Vida da Vilã")]
    public int vidaMaxima = 8;
    public int vidaAtual = 8;
    public Image[] coracoes;

    [Header("Duração das animações")]
    public float duracaoAnimacaoDano = 0.5f;
    public float duracaoAnimacaoMorte = 1.5f;

    [Header("Áudios")]
    public AudioClip somAtaque;
    public AudioClip somLancamento;
    public AudioClip somDano;
    public AudioClip somMorte;

    [Range(0f, 1f)]
    public float volumeAtaque = 0.7f;

    [Range(0f, 1f)]
    public float volumeLancamento = 0.7f;

    [Range(0f, 1f)]
    public float volumeDano = 0.7f;

    [Range(0f, 1f)]
    public float volumeMorte = 0.8f;

    private AudioSource audioSource;

    private bool atacando;
    private bool recebendoDano;
    private bool morreu;
    private bool salvaIniciada;

    private Coroutine rotinaDano;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        if (anim == null)
            anim = GetComponent<Animator>();
    }

    private void Start()
    {
        vidaAtual = vidaMaxima;
        AtualizarCoracoes();

        if (anim == null)
        {
            Debug.LogError("Animator da vilã não encontrado!");
            return;
        }

        StartCoroutine(CicloAtaque());
    }

    private IEnumerator CicloAtaque()
    {
        while (!morreu)
        {
            bool jogadorDetectado =
                jogador != null &&
                Vector2.Distance(transform.position, jogador.position)
                <= distanciaDeteccao;

            if (atacar &&
                jogadorDetectado &&
                !recebendoDano &&
                !atacando &&
                pontoDisparo != null &&
                prefabBomba != null)
            {
                atacando = true;
                salvaIniciada = false;

                anim.SetTrigger("Atacar");

                TocarSom(somAtaque, volumeAtaque);

                yield return new WaitForSeconds(
                    tempoAnimacaoAtaque
                );

                atacando = false;

                yield return new WaitForSeconds(
                    Mathf.Max(0.1f, cooldown)
                );
            }
            else
            {
                yield return new WaitForSeconds(0.15f);
            }
        }
    }

    // CHAMADO PELO ANIMATION EVENT
    public void IniciarSalva()
    {
        if (morreu || recebendoDano || !atacando)
            return;

        if (salvaIniciada)
            return;

        salvaIniciada = true;

        StartCoroutine(DispararSalva());
    }

    private IEnumerator DispararSalva()
    {
        for (int i = 0; i < bombasPorSalva; i++)
        {
            if (morreu || recebendoDano || !atacando)
                yield break;

            if (jogador == null)
                yield break;

            DispararBomba();

            if (i < bombasPorSalva - 1)
            {
                yield return new WaitForSeconds(
                    intervaloEntreBombas
                );
            }
        }
    }

    private void DispararBomba()
    {
        GameObject bomba = Instantiate(
            prefabBomba,
            pontoDisparo.position,
            Quaternion.identity
        );

        BolaCanhao projetil = bomba.GetComponent<BolaCanhao>();
        Rigidbody2D rbBomba = bomba.GetComponent<Rigidbody2D>();

        if (projetil == null || rbBomba == null)
        {
            Debug.LogError(
                "O prefab da bomba precisa ter BolaCanhao e Rigidbody2D!"
            );

            Destroy(bomba);
            return;
        }

        rbBomba.bodyType = RigidbodyType2D.Dynamic;

        if (rbBomba.gravityScale <= 0f)
            rbBomba.gravityScale = 1f;

        Vector2 posicaoJogador = jogador.position;

        Vector2 variacao =
            Random.insideUnitCircle * erroMira;

        if (variacao.magnitude > distanciaMaximaMira)
        {
            variacao =
                variacao.normalized * distanciaMaximaMira;
        }

        Vector2 destino = posicaoJogador + variacao;

        Vector2 origem = pontoDisparo.position;
        Vector2 delta = destino - origem;

        if (delta.magnitude > distanciaMaximaDisparo)
        {
            destino = origem +
                delta.normalized * distanciaMaximaDisparo;

            delta = destino - origem;
        }

        float g =
            Physics2D.gravity.y * rbBomba.gravityScale;

        float tempoVoo = Mathf.Clamp(
            delta.magnitude / Mathf.Max(0.1f, velocidadeBomba),
            0.4f,
            2.5f
        );

        Vector2 velocidadeInicial = new Vector2(
            delta.x / tempoVoo,
            delta.y / tempoVoo - 0.5f * g * tempoVoo
        );

        projetil.Lancar(
            velocidadeInicial.normalized,
            velocidadeInicial.magnitude
        );

        TocarSom(somLancamento, volumeLancamento);
    }

    public void TomarDano(int dano)
    {
        if (morreu || dano <= 0)
            return;

        vidaAtual -= dano;
        vidaAtual = Mathf.Max(0, vidaAtual);

        AtualizarCoracoes();

        if (vidaAtual <= 0)
        {
            Morrer();
            return;
        }

        recebendoDano = true;

        if (rotinaDano != null)
            StopCoroutine(rotinaDano);

        anim.ResetTrigger("Atacar");
        anim.SetTrigger("LevarDano");

        TocarSom(somDano, volumeDano);

        rotinaDano = StartCoroutine(FinalizarDano());
    }

    private IEnumerator FinalizarDano()
    {
        yield return new WaitForSeconds(
            duracaoAnimacaoDano
        );

        recebendoDano = false;
        rotinaDano = null;
    }

    private void Morrer()
    {
        morreu = true;
        atacando = false;
        recebendoDano = false;

        StopAllCoroutines();

        Collider2D[] colliders =
            GetComponentsInChildren<Collider2D>();

        foreach (Collider2D col in colliders)
            col.enabled = false;

        anim.ResetTrigger("Atacar");
        anim.ResetTrigger("LevarDano");
        anim.SetTrigger("Morrer");

        TocarSom(somMorte, volumeMorte);

        StartCoroutine(DestruirAposMorte());
    }

    private IEnumerator DestruirAposMorte()
    {
        yield return new WaitForSeconds(
            duracaoAnimacaoMorte
        );

        Destroy(gameObject);
    }

    private void AtualizarCoracoes()
    {
        if (coracoes == null)
            return;

        for (int i = 0; i < coracoes.Length; i++)
        {
            if (coracoes[i] != null)
                coracoes[i].enabled = i < vidaAtual;
        }
    }

    private void TocarSom(AudioClip som, float volume)
    {
        if (som != null && audioSource != null)
        {
            audioSource.PlayOneShot(som, volume);
        }
    }
    private void OnDrawGizmosSelected()
    {
        // Distância em que a vilã detecta o jogador
        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(
            transform.position,
            distanciaDeteccao
        );
    }
}

