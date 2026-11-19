
using UnityEngine;
using UnityEngine.UI;

public class BossController : MonoBehaviour, IDanificavel
{
    [Header("Detecção do jogador")]
    public float distanciaDeteccao = 8f;
    
    [Header("Variação dos disparos")]
    public Vector2 variacaoAlvoBola = new Vector2(1.5f, 0.75f);
    
    [Header("Vida")]
    public int vidaMaxima = 15;
    public int vidaAtual = 15;

    [Header("Segunda Fase")]
    public int vidaParaSegundaFase = 5;
    public float cooldownFase1 = 3f;
    public float cooldownFase2 = 1.5f;
    
    [Header("Visual do Cooldown")]
    public Image imagemCooldown;

    private float duracaoCooldownAtual;

    [Header("Ataque")]
    public float distanciaAtaque = 10f;
    public float velocidadeBola = 7f;
    public int danoBola = 1;

    [Header("Bola de Fogo")]
    public GameObject prefabBolaDeFogo;
    public Transform pontoDisparo;

    [Header("Referências")]
    public Transform jogador;
    public Animator anim;
    public SpriteRenderer sprite;
    
    [Header("Áudio do ataque")]
    public AudioSource audioSource;
    public AudioClip somAtaque;
    [Range(0f, 1f)]
    public float volumeSomAtaque = 0.8f;

    [Header("Morte")]
    public float tempoDestruicao = 2f;
    
    [Header("Vida Visual")]
    public UnityEngine.UI.Image[] coracoesVida;
    
    [Header("Sequência de disparos")]
    public int quantidadeBolas = 3;
    public float intervaloEntreBolas = 0.3f;

    private float proximoAtaque;
    private bool segundaFase;
    private bool morreu;
    private bool sequenciaDisparosAtiva = false;
    
    public static bool bossDerrotado = false;
    private bool ataqueEmAndamento;

    private void Start()
    {
        vidaAtual = vidaMaxima;
        AtualizarCoracoes();

        if (anim == null)
            anim = GetComponent<Animator>();

        if (sprite == null)
            sprite = GetComponent<SpriteRenderer>();
        
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (jogador == null)
        {
            GameObject objetoJogador =
                GameObject.FindGameObjectWithTag("Player");

            if (objetoJogador != null)
                jogador = objetoJogador.transform;
        }

        proximoAtaque = Time.time + cooldownFase1;
    }
    public void ReceberDano(int quantidade)
    {
            // Não recebe dano se já morreu ou se o dano for inválido.
            if (morreu || quantidade <= 0)
                return;

            // Retira vida do boss.
            vidaAtual -= quantidade;

            // Impede que a vida fique negativa.
            vidaAtual = Mathf.Max(vidaAtual, 0);
            AtualizarCoracoes();

            Debug.Log("Vida do Boss: " + vidaAtual + "/" + vidaMaxima);

            // Verifica se o boss morreu.
            if (vidaAtual <= 0)
            {
                Morrer();
                return;
            }

            // Verifica se chegou à segunda fase.
            if (!segundaFase && vidaAtual <= vidaParaSegundaFase)
            {
                segundaFase = true;
                Debug.Log("Boss entrou na segunda fase!");
            }

            if (!ataqueEmAndamento)
            {
                anim.SetTrigger("Dano");
            }

    }

    private void Update()
    {
        AtualizarVisualCooldown();
        if (morreu || jogador == null)
            return;

        float distancia = Vector2.Distance(
            transform.position,
            jogador.position
        );

        // Só ataca se o jogador estiver dentro da área de detecção.
        if (distancia <= distanciaDeteccao)
        {
            OlharParaJogador();

            if (Time.time >= proximoAtaque)
            {
                Atacar();
            }
        }
    }

    private void OlharParaJogador()
    {
        if (sprite == null) return;

        sprite.flipX =
            jogador.position.x < transform.position.x;
    }

    private void Atacar()
    {
        if (morreu || sequenciaDisparosAtiva || ataqueEmAndamento)
            return;

        duracaoCooldownAtual = segundaFase
            ? cooldownFase2
            : cooldownFase1;

        proximoAtaque = Time.time + duracaoCooldownAtual;

        ataqueEmAndamento = true;

        anim.SetTrigger("Atacar");

        if (imagemCooldown != null)
            imagemCooldown.fillAmount = 0f;
    }
    
    private void LancarBolaDeFogo()
    {
        
        Debug.Log("EVENTO DE DISPARO CHAMADO!");
        
        if (sequenciaDisparosAtiva || morreu)
            return;
        
        StartCoroutine(DispararSequencia());
        

    }

    
    private System.Collections.IEnumerator DispararSequencia()
    {
        sequenciaDisparosAtiva = true;

        for (int i = 0; i < quantidadeBolas; i++)
        {
            if (morreu)
            {
                sequenciaDisparosAtiva = false;
                ataqueEmAndamento = false;
                yield break;
            }

            Vector2 variacao = new Vector2(
                Random.Range(-variacaoAlvoBola.x, variacaoAlvoBola.x),
                Random.Range(-variacaoAlvoBola.y, variacaoAlvoBola.y)
            );

            Vector2 alvoAleatorio = (Vector2)jogador.position + variacao;

            Vector2 direcao =
                (alvoAleatorio - (Vector2)pontoDisparo.position).normalized;
            if (audioSource != null && somAtaque != null)
            {
                audioSource.PlayOneShot(somAtaque, volumeSomAtaque);
            }
            
            GameObject bola = Instantiate(
                prefabBolaDeFogo,
                pontoDisparo.position,
                Quaternion.identity
            );
            Destroy(bola, 2f);
            

            Collider2D[] collidersBoss = GetComponentsInChildren<Collider2D>();
            Collider2D[] collidersBola = bola.GetComponentsInChildren<Collider2D>();

            foreach (Collider2D bossCollider in collidersBoss)
            {
                foreach (Collider2D bolaCollider in collidersBola)
                {
                    Physics2D.IgnoreCollision(bossCollider, bolaCollider);
                }
            }

            BolaDeFogoBoss projetil = bola.GetComponent<BolaDeFogoBoss>();

            if (projetil != null)
            {
                projetil.Inicializar(direcao, velocidadeBola, danoBola);
            }

            yield return new WaitForSeconds(intervaloEntreBolas);
        }

        sequenciaDisparosAtiva = false;
        ataqueEmAndamento = false;
    }

    
    private void Morrer()
    {
        if (morreu)
            return;

        morreu = true;

        // Registra a derrota do boss apenas uma vez.
        if (ProgressoFase.Instance != null)
        {
            ProgressoFase.Instance.RegistrarBossDerrotado();
        }

        bossDerrotado = true;

        anim.ResetTrigger("Atacar");
        anim.ResetTrigger("Dano");

        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;

            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        Collider2D[] colliders =
            GetComponentsInChildren<Collider2D>();

        foreach (Collider2D col in colliders)
            col.enabled = false;

        anim.SetTrigger("Morte");
    }

    private void OnDrawGizmosSelected()
    {
        
        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(
            transform.position,
            distanciaDeteccao
        );
    }
    
    private void AtualizarCoracoes()
    {
        for (int i = 0; i < coracoesVida.Length; i++)
        {
            if (coracoesVida[i] == null)
                continue;

            coracoesVida[i].gameObject.SetActive(i < vidaAtual);
        }
    }
    private void AtualizarVisualCooldown()
    {
        if (imagemCooldown == null)
            return;

        float tempoRestante = proximoAtaque - Time.time;

        if (duracaoCooldownAtual <= 0)
        {
            imagemCooldown.fillAmount = 1f;
            return;
        }

        imagemCooldown.fillAmount = 1f -
                                    Mathf.Clamp01(tempoRestante / duracaoCooldownAtual);
    }
}
