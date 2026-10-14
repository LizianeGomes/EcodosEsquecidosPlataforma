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
    public int vidaMaxima = 5;
    public VidaUI vidaUI; 
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
        if (vidaUI != null)
            vidaUI.AtualizarVida(vidaAtual);

        anim = visual.GetComponent<Animator>();

        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;

        posicaoY = transform.position.y;
        posicaoZ = transform.position.z;

        
        DefinirIdleDirecao(true);

        StartCoroutine(CicloDeCombate());
    }

    IEnumerator CicloDeCombate()
    {
        while (!morto)
        {
            
            while (player == null || DistanciaDoPlayer() > distanciaDeteccao)
            {
                DefinirIdleDirecao(voltadoDireita);
                yield return null;
            }

            
            yield return AndarAtePosicaoDeAtaque();

            if (morto)
                yield break;

            
            yield return DispararRajada();

            if (morto)
                yield break;

            
            DefinirIdleDirecao(voltadoDireita);

            yield return new WaitForSeconds(cooldownLongo);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        PlayerMovement player =
            other.GetComponentInParent<PlayerMovement>();

        if (other.CompareTag("Player"))
        {
            player.TomarDano(1);
            
        }
    }

    IEnumerator AndarAtePosicaoDeAtaque()
    {
        if (player == null)
            yield break;

        bool playerEstaADireita = player.position.x > transform.position.x;

       
        float destinoX;

        if (playerEstaADireita)
            destinoX = limiteDireito.position.x;
        else
            destinoX = limiteEsquerdo.position.x;

        
        DefinirDirecao(playerEstaADireita);

        
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

        
        anim.SetBool("Andando", false);

        
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

            
            bool olharDireita = player.position.x > transform.position.x;

            DefinirDirecao(olharDireita);

           
            if (olharDireita)
            {
                anim.SetTrigger("atacando");
            }
            else
            {
                anim.SetTrigger("atacando2");
            }

            
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

    
    void DefinirDirecao(bool direita)
    {
        voltadoDireita = direita;

        // Andando = movimento
        // Andando2 = direção

        anim.SetBool("Andando2", !direita);
    }

    
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
        vidaAtual = Mathf.Max(vidaAtual, 0);

        if (vidaUI != null)
            vidaUI.AtualizarVida(vidaAtual);

        
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
        if (vidaUI != null)
            vidaUI.gameObject.SetActive(false); 

        if (ProgressoFase.Instance != null)
            ProgressoFase.Instance.RegistrarBossDerrotado();

        StopAllCoroutines();

        
        anim.SetBool("Andando", false);
        anim.SetBool("Andando2", false);

        
        anim.ResetTrigger("atacando");
        anim.ResetTrigger("atacando2");
        anim.ResetTrigger("tomouDano");
        anim.ResetTrigger("tomouDano2");

        
        if (voltadoDireita)
            anim.Play("Morte", 0, 0f);
        else
            anim.Play("MorteEspelhada", 0, 0f);

        
        Collider2D col = GetComponent<Collider2D>();

        if (col != null)
            col.enabled = false;

      
    }
}