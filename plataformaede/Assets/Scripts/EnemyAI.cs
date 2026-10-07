using UnityEngine;
using System.Collections;

public class EnemyAI : MonoBehaviour, IDanificavel
{
    [Header("Referências")]
    public Transform player;
    public Transform visual;
    public Transform pontoDisparo;

    [Header("Movimento")]
    public float velocidadeMovimento = 2f;
    public float distanciaDeteccao = 8f;

    [Header("Limites da Arena")]
    public Transform limiteEsquerdo;
    public Transform limiteDireito;

    [Header("Vida")]
    public int vidaMaxima = 10;
    private int vidaAtual;

    [Header("Ataque")]
    public GameObject projetilPrefab;
    public float velocidadeProjetil = 6f;
    public int disparosPorRajada = 3;
    public float intervaloEntreDisparos = 0.8f;
    public float cooldownLongo = 3f;

    private Animator anim;
    private Rigidbody2D rb;

    private bool morto = false;
    private bool voltadoDireita = true;

    private float posicaoY;
    private float posicaoZ;

    void Start()
    {
        vidaAtual = vidaMaxima;

        anim = visual.GetComponent<Animator>();

        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;

        posicaoY = transform.position.y;
        posicaoZ = transform.position.z;

        // Começa parado olhando para a direita
        DefinirIdleDirecao(true);

        StartCoroutine(CicloDeCombate());
    }

    IEnumerator CicloDeCombate()
    {
        while (!morto)
        {
            // Espera o player entrar no raio de detecção
            while (player == null || DistanciaDoPlayer() > distanciaDeteccao)
            {
                DefinirIdleDirecao(voltadoDireita);
                yield return null;
            }

            // Anda até o lado do player
            yield return AndarAtePosicaoDeAtaque();

            if (morto)
                yield break;

            // Rajada de 3 tiros
            yield return DispararRajada();

            if (morto)
                yield break;

            // Fica parado durante o cooldown
            DefinirIdleDirecao(voltadoDireita);

            yield return new WaitForSeconds(cooldownLongo);
        }
    }

    IEnumerator AndarAtePosicaoDeAtaque()
    {
        if (player == null)
            yield break;

        bool playerEstaADireita = player.position.x > transform.position.x;

        // Escolhe o limite correspondente ao lado do player
        float destinoX;

        if (playerEstaADireita)
            destinoX = limiteDireito.position.x;
        else
            destinoX = limiteEsquerdo.position.x;

        // Define para qual lado o boss está olhando
        DefinirDirecao(playerEstaADireita);

        // Ativa caminhada
        anim.SetBool("Andando", true);

        while (!morto)
        {
            float distancia = Mathf.Abs(transform.position.x - destinoX);

            if (distancia <= 0.05f)
                break;

            float novoX = Mathf.MoveTowards(
                transform.position.x,
                destinoX,
                velocidadeMovimento * Time.deltaTime
            );

            transform.position = new Vector3(
                novoX,
                posicaoY,
                posicaoZ
            );

            yield return null;
        }

        // Garante que nunca ultrapasse os limites
        float xSeguro = Mathf.Clamp(
            transform.position.x,
            limiteEsquerdo.position.x,
            limiteDireito.position.x
        );

        transform.position = new Vector3(
            xSeguro,
            posicaoY,
            posicaoZ
        );

        // Para de andar
        anim.SetBool("Andando", false);

        // Quando parar, olha para o player
        if (player != null)
        {
            bool olharDireita = player.position.x > transform.position.x;
            DefinirIdleDirecao(olharDireita);
        }
    }

    IEnumerator DispararRajada()
    {
        for (int i = 0; i < disparosPorRajada; i++)
        {
            if (morto)
                yield break;

            // Olha para o player antes de cada disparo
            bool olharDireita = player.position.x > transform.position.x;

            DefinirDirecao(olharDireita);

            // Ataque normal ou espelhado
            if (olharDireita)
            {
                anim.SetTrigger("atacando");
            }
            else
            {
                anim.SetTrigger("atacando2");
            }

            // Cria o projétil
            if (projetilPrefab != null && pontoDisparo != null)
            {
                GameObject projetil = Instantiate(
                    projetilPrefab,
                    pontoDisparo.position,
                    Quaternion.identity
                );

                float dirX = Mathf.Sign(
                    player.position.x - pontoDisparo.position.x
                );

                Rigidbody2D rbProjetil =
                    projetil.GetComponent<Rigidbody2D>();

                if (rbProjetil != null)
                {
                    rbProjetil.linearVelocity =
                        new Vector2(dirX * velocidadeProjetil, 0f);
                }
            }

            yield return new WaitForSeconds(intervaloEntreDisparos);
        }
    }

    // Define apenas o lado que o boss está olhando
    void DefinirDirecao(bool direita)
    {
        voltadoDireita = direita;

        // Andando = movimento
        // Andando2 = direção

        anim.SetBool("Andando2", !direita);
    }

    // Coloca o boss parado olhando para o lado correto
    void DefinirIdleDirecao(bool direita)
    {
        voltadoDireita = direita;

        anim.SetBool("Andando", false);
        anim.SetBool("Andando2", !direita);
    }

    float DistanciaDoPlayer()
    {
        if (player == null)
            return Mathf.Infinity;

        return Vector2.Distance(
            transform.position,
            player.position
        );
    }

    public void ReceberDano(int dano)
    {
        if (morto)
            return;

        vidaAtual -= dano;

        // Dano depende do lado que o boss está olhando
        if (voltadoDireita)
        {
            anim.SetTrigger("tomouDano");
        }
        else
        {
            anim.SetTrigger("tomouDano2");
        }

        if (vidaAtual <= 0)
        {
            Morrer();
        }
    }

    void Morrer()
    {
        if (morto)
            return;

        morto = true;

        StopAllCoroutines();

        // Desliga estados de movimento
        anim.SetBool("Andando", false);
        anim.SetBool("Andando2", false);

        // Limpa possíveis triggers antigos
        anim.ResetTrigger("atacando");
        anim.ResetTrigger("atacando2");
        anim.ResetTrigger("tomouDano");
        anim.ResetTrigger("tomouDano2");

        // Vai DIRETAMENTE para a morte
        if (voltadoDireita)
            anim.Play("Morte", 0, 0f);
        else
            anim.Play("MorteEspelhada", 0, 0f);

        // Desativa colisão
        Collider2D col = GetComponent<Collider2D>();

        if (col != null)
            col.enabled = false;

        // POR ENQUANTO NÃO DESTRUIR
    }
}