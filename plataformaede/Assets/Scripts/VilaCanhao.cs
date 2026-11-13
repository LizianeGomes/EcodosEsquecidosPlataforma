using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

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

    [Header("Mira e locais de queda")]
    public float velocidadeBomba = 7f;
    public float distanciaMaximaDisparo = 6f;

    public float distanciaMinimaQueda = 1.5f;
    public float distanciaMaximaQueda = 5f;

    public bool cairDosDoisLados = true;

    [Header("Aviso de impacto")]
    public GameObject prefabIndicadorImpacto;
    public LayerMask camadaGround;
    public float alturaRaycast = 3f;
    public float distanciaRaycast = 10f;
    public float alturaIndicador = 0.03f;

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

    [Range(0f, 1f)] public float volumeAtaque = 0.7f;
    [Range(0f, 1f)] public float volumeLancamento = 0.7f;
    [Range(0f, 1f)] public float volumeDano = 0.7f;
    [Range(0f, 1f)] public float volumeMorte = 0.8f;

    [Header("Distância entre impactos")]
    public float distanciaMinimaEntreQuedas = 2f;

    private AudioSource audioSource;

    private bool atacando;
    private bool recebendoDano;
    private bool morreu;
    private bool salvaIniciada;

    private Coroutine rotinaDano;

    private List<float> destinosUsados = new List<float>();

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

                yield return new WaitForSeconds(tempoAnimacaoAtaque);

                atacando = false;

                yield return new WaitForSeconds(Mathf.Max(0.1f, cooldown));
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
        destinosUsados.Clear();

        for (int i = 0; i < bombasPorSalva; i++)
        {
            if (recebendoDano || morreu)
                yield break;

            DispararBomba();

            yield return new WaitForSeconds(intervaloEntreBombas);
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
            Debug.LogError("O prefab precisa ter BolaCanhao e Rigidbody2D!");
            Destroy(bomba);
            return;
        }

        rbBomba.linearDamping = 0f;

        // a bomba não pode colidir com a própria vilã
        Collider2D colBomba = bomba.GetComponent<Collider2D>();

        if (colBomba != null)
        {
            foreach (Collider2D col in GetComponentsInChildren<Collider2D>())
                Physics2D.IgnoreCollision(colBomba, col);
        }

        Vector2 origem = pontoDisparo.position;

        // Escolhe um destino aleatório diferente dos anteriores.
        float destinoX = EscolherDestinoX();

        // Procura o chão abaixo do destino, partindo perto da altura do jogador.
        Vector2 inicioRaycast = new Vector2(
            destinoX,
            jogador.position.y + alturaRaycast
        );

        RaycastHit2D hit = Physics2D.Raycast(
            inicioRaycast,
            Vector2.down,
            distanciaRaycast,
            camadaGround
        );

        Debug.DrawRay(inicioRaycast, Vector2.down * distanciaRaycast, Color.red, 3f);

        // Se não achar chão (buraco), mira na altura do jogador em vez de destruir a bomba.
        Vector2 destino = hit.collider != null
            ? hit.point
            : new Vector2(destinoX, jogador.position.y);

        // Calcula a trajetória.
        Vector2 delta = destino - origem;

        float g = Physics2D.gravity.y * rbBomba.gravityScale;

        float tempoVoo = Mathf.Clamp(
            delta.magnitude / Mathf.Max(0.1f, velocidadeBomba),
            0.4f,
            2.5f
        );

        Vector2 velocidadeInicial = new Vector2(
            delta.x / tempoVoo,
            delta.y / tempoVoo - 0.5f * g * tempoVoo
        );

        // Cria o indicador vermelho no local de impacto (só se houver chão).
        if (hit.collider != null && prefabIndicadorImpacto != null)
        {
            Vector2 posicaoIndicador =
                hit.point + Vector2.up * alturaIndicador;

            projetil.ConfigurarAviso(
                prefabIndicadorImpacto,
                posicaoIndicador,
                tempoVoo
            );
        }

        Debug.DrawLine(origem, destino, Color.yellow, 3f);

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
        yield return new WaitForSeconds(duracaoAnimacaoDano);

        recebendoDano = false;
        rotinaDano = null;
    }

    private void Morrer()
    {
        morreu = true;
        if (ProgressoFase.Instance != null)
        {
            ProgressoFase.Instance.RegistrarBossDerrotado();
        }
        atacando = false;
        recebendoDano = false;

        StopAllCoroutines();

        Collider2D[] colliders = GetComponentsInChildren<Collider2D>();

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
        yield return new WaitForSeconds(duracaoAnimacaoMorte);

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
            audioSource.PlayOneShot(som, volume);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(transform.position, distanciaDeteccao);
    }

    private float EscolherDestinoX()
    {
        float limiteEsquerdo =
            pontoDisparo.position.x - distanciaMaximaDisparo;

        float limiteDireito =
            pontoDisparo.position.x + distanciaMaximaDisparo;

        float melhorCandidato = jogador.position.x;
        float melhorDistancia = -1f;

        for (int tentativa = 0; tentativa < 100; tentativa++)
        {
            float lado = Random.value < 0.5f ? -1f : 1f;

            if (!cairDosDoisLados)
            {
                lado = jogador.position.x >= pontoDisparo.position.x
                    ? 1f : -1f;
            }

            float distancia = Random.Range(
                distanciaMinimaQueda,
                distanciaMaximaQueda
            );

            float candidato = jogador.position.x + lado * distancia;

            if (candidato < limiteEsquerdo ||
                candidato > limiteDireito)
                continue;

            float menorDistancia = float.MaxValue;

            foreach (float destino in destinosUsados)
            {
                float diferenca = Mathf.Abs(candidato - destino);

                if (diferenca < menorDistancia)
                    menorDistancia = diferenca;
            }

            // Na primeira bomba, qualquer posição é válida.
            if (destinosUsados.Count == 0)
                menorDistancia = float.MaxValue;

            if (menorDistancia >= distanciaMinimaEntreQuedas)
            {
                destinosUsados.Add(candidato);
                return candidato;
            }

            // Guarda a melhor posição caso não encontre
            // uma que respeite a distância mínima.
            if (menorDistancia > melhorDistancia)
            {
                melhorDistancia = menorDistancia;
                melhorCandidato = candidato;
            }
        }

        Debug.LogWarning(
            "Não foi possível respeitar a distância mínima. Usando a melhor posição encontrada."
        );

        destinosUsados.Add(melhorCandidato);

        return melhorCandidato;
    }
}