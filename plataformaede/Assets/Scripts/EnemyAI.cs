using UnityEngine;

public class EnemyAI : MonoBehaviour, IDanificavel
{
    [Header("Player")]
    public Transform player;

    [Header("Movimento")]
    public float velocidadeMovimento = 1.5f;
    public float distanciaDeteccao = 8f;
    public float distanciaMaximaMovimento = 2f;
    public float tolerancia = 0.1f;

    [Header("Vida")]
    public int vidaMaxima = 10;
    private int vidaAtual;

    [Header("Ataque")]
    public GameObject projetilPrefab;
    public Transform pontoDisparo;
    public float velocidadeProjetil = 6f;
    public int disparosAntesDoDescanso = 3;
    public float cooldownEntreDisparos = 1f;
    public float cooldownLongo = 3f;

    private Vector3 posicaoInicial;
    private Animator anim;
    private Rigidbody2D rb;
    private bool morto = false;
    private int disparosFeitos = 0;
    private float tempoProximoDisparo = 0f;
    private bool emCooldownLongo = false;

    void Start()
    {
        posicaoInicial = transform.position;
        vidaAtual = vidaMaxima;
        anim = GetComponent<Animator>();

        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
    }

    void Update()
    {
        if (morto || player == null) return;

        float distancia = Vector2.Distance(transform.position, player.position);

        if (distancia > distanciaDeteccao)
        {
            anim.SetBool("Andando", false);
            return;
        }

        // Só eixo X, limitado pela corrente. Y sempre fixo na posição original.
        float difX = player.position.x - posicaoInicial.x;
        float xClamp = Mathf.Clamp(difX, -distanciaMaximaMovimento, distanciaMaximaMovimento);
        Vector3 destino = new Vector3(posicaoInicial.x + xClamp, posicaoInicial.y, posicaoInicial.z);

        float distanciaAteDestino = Mathf.Abs(transform.position.x - destino.x);

        if (distanciaAteDestino > tolerancia)
        {
            transform.position = Vector3.MoveTowards(transform.position, destino, velocidadeMovimento * Time.deltaTime);
            anim.SetBool("Andando", true);

            if (destino.x > transform.position.x)
                transform.localScale = new Vector3(1, 1, 1);
            else
                transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            anim.SetBool("Andando", false);

            // Vira pro lado do player mesmo parado, pra mirar certo
            if (player.position.x > transform.position.x)
                transform.localScale = new Vector3(1, 1, 1);
            else
                transform.localScale = new Vector3(-1, 1, 1);

            TentarAtacar();
        }
    }

    void TentarAtacar()
    {
        if (emCooldownLongo) return;
        if (Time.time < tempoProximoDisparo) return;

        Disparar();

        disparosFeitos++;
        tempoProximoDisparo = Time.time + cooldownEntreDisparos;

        if (disparosFeitos >= disparosAntesDoDescanso)
        {
            disparosFeitos = 0;
            emCooldownLongo = true;
            Invoke(nameof(TerminarCooldownLongo), cooldownLongo);
        }
    }

    void TerminarCooldownLongo()
    {
        emCooldownLongo = false;
    }

    void Disparar()
    {
        anim.SetTrigger("atacando");

        if (projetilPrefab != null && pontoDisparo != null)
        {
            GameObject projetil = Instantiate(projetilPrefab, pontoDisparo.position, Quaternion.identity);

            Vector2 direcaoTiro = (player.position - pontoDisparo.position).normalized;

            Rigidbody2D rbProjetil = projetil.GetComponent<Rigidbody2D>();
            if (rbProjetil != null)
            {
                rbProjetil.linearVelocity = direcaoTiro * velocidadeProjetil;
            }
        }
    }

    public void ReceberDano(int dano)
    {
        if (morto) return;

        vidaAtual -= dano;
        anim.SetTrigger("tomouDano");

        if (vidaAtual <= 0)
        {
            Morrer();
        }
    }

    void Morrer()
    {
        morto = true;
        anim.SetTrigger("morreu");

        GetComponent<Collider2D>().enabled = false;

        Destroy(gameObject, 2f);
    }
}