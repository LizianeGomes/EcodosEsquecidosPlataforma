
using System.Collections;
using UnityEngine;

public class VilãCanhao : MonoBehaviour
{
    [Header("Referências")]
    public Transform jogador;
    public Transform pontoDisparo;
    public GameObject prefabBola;
    public Animator anim;

    [Header("Ataque")]
    public float cooldown = 3f;
    public int bolasPorSalva = 3;
    public float intervaloEntreBolas = 0.25f;
    public float tempoAntesDoPrimeiroTiro = 0.3f;

    [Header("Mira")]
    public float velocidadeBola = 7f;
    public float erroMira = 1f;
    public float distanciaMaximaMira = 3f;

    [Header("Estado")]
    public bool atacar = true;
    
    [Header("Detecção do jogador")]
    public float distanciaDeteccao = 8f;
    public float distanciaMaximaDisparo = 6f;

    private void Start()
    {
        if (anim == null)
            anim = GetComponent<Animator>();

        StartCoroutine(CicloAtaque());
    }

    private IEnumerator CicloAtaque()
    {
        while (true)
        {
            bool jogadorDetectado =
                jogador != null &&
                Vector2.Distance(transform.position, jogador.position)
                <= distanciaDeteccao;

            if (atacar &&
                jogadorDetectado &&
                pontoDisparo != null &&
                prefabBola != null)
            {
                if (anim != null)
                    anim.SetTrigger("Atacar");

                yield return new WaitForSeconds(
                    tempoAntesDoPrimeiroTiro
                );

                for (int i = 0; i < bolasPorSalva; i++)
                {
                    if (!atacar)
                        break;

                    // Se o jogador sair do alcance, interrompe a salva.
                    if (jogador == null ||
                        Vector2.Distance(
                            transform.position,
                            jogador.position
                        ) > distanciaDeteccao)
                    {
                        break;
                    }

                    DispararBola();

                    if (i < bolasPorSalva - 1)
                    {
                        yield return new WaitForSeconds(
                            intervaloEntreBolas
                        );
                    }
                }
            }

            yield return new WaitForSeconds(
                Mathf.Max(0.1f, cooldown)
            );
        }
    }

    private void DispararBola()
    {
        GameObject bola = Instantiate(
            prefabBola,
            pontoDisparo.position,
            Quaternion.identity
        );

        BolaCanhao projetil = bola.GetComponent<BolaCanhao>();
        Rigidbody2D rbBola = bola.GetComponent<Rigidbody2D>();

        if (projetil == null || rbBola == null)
        {
            Debug.LogError(
                "O prefab da bola precisa ter BolaCanhao e Rigidbody2D!"
            );

            Destroy(bola);
            return;
        }

        // garante física de verdade
        rbBola.bodyType = RigidbodyType2D.Dynamic;

        if (rbBola.gravityScale <= 0f)
            rbBola.gravityScale = 1f;

        // alvo com erro de mira (igual ao seu)
        Vector2 posicaoJogador = jogador.position;

        Vector2 variacao = Random.insideUnitCircle * erroMira;

        if (variacao.magnitude > distanciaMaximaMira)
            variacao = variacao.normalized * distanciaMaximaMira;

        Vector2 destino = posicaoJogador + variacao;
        
        Vector2 origem = pontoDisparo.position;
        Vector2 delta = destino - origem;


        if (delta.magnitude > distanciaMaximaDisparo)
        {
            destino = origem +
                      delta.normalized * distanciaMaximaDisparo;

            delta = destino - origem;
        }

        float g = Physics2D.gravity.y * rbBola.gravityScale; // negativo

        float tempoVoo = Mathf.Clamp(
            delta.magnitude / velocidadeBola,
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
    }
}
