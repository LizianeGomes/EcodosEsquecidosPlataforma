
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
            if (atacar && jogador != null &&
                pontoDisparo != null && prefabBola != null)
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

        
        Vector2 posicaoJogador = jogador.position;

        Vector2 variacaoAleatoria =
            Random.insideUnitCircle * erroMira;

        Vector2 destino = posicaoJogador + variacaoAleatoria;

        
        Vector2 diferenca = destino - posicaoJogador;

        if (diferenca.magnitude > distanciaMaximaMira)
        {
            destino = posicaoJogador +
                diferenca.normalized * distanciaMaximaMira;
        }

        
        Vector2 direcao =
            ((Vector2)destino -
            (Vector2)pontoDisparo.position).normalized;

        BolaCanhao projetil = bola.GetComponent<BolaCanhao>();

        if (projetil != null)
        {
            projetil.Lancar(direcao, velocidadeBola);
        }
        else
        {
            Debug.LogError(
                "O prefab da bola precisa ter o script BolaCanhao!"
            );

            Destroy(bola);
        }
    }
}
