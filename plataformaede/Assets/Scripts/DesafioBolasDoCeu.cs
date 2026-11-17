
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesafioBolasDoCeu : MonoBehaviour
{
    [Header("Referências")]
    public Transform jogador;
    public GameObject prefabBola;
    public GameObject prefabIndicadorImpacto;

    [Header("Quantidade e intervalo da chuva")]
    public int quantidadeBolas = 30;
    public float intervaloMinimoEntreBolas = 0.2f;
    public float intervaloMaximoEntreBolas = 0.7f;
    public float tempoAviso = 1.2f;

    [Header("Queda")]
    public float velocidadeQueda = 12f;
    public float alturaQueda = 8f;
    public int danoBola = 1;

    [Header("Área dos impactos")]
    public float distanciaMaximaDisparo = 8f;
    public float distanciaMinimaQueda = 1.5f;
    public float distanciaMaximaQueda = 5f;
    public float distanciaMinimaEntreQuedas = 1.5f;
    public bool cairDosDoisLados = true;

    [Header("Detecção do chão")]
    public LayerMask camadaGround;
    public float alturaRaycast = 15f;
    public float distanciaRaycast = 30f;
    public float alturaIndicador = 0.03f;
    
    [Header("Detecção do jogador")]
    public float distanciaDeteccao = 10f;

    private bool jogadorNoRaio;
    private Coroutine rotinaChuva;

    private List<float> destinosUsados = new List<float>();

    private void Start()
    {
        if (jogador == null)
        {
            GameObject objetoJogador =
                GameObject.FindGameObjectWithTag("Player");

            if (objetoJogador != null)
                jogador = objetoJogador.transform;
        }

        if (jogador == null)
        {
            Debug.LogError("Jogador não encontrado!");
            return;
        }
    }
    private void Update()
    {
        if (jogador == null)
            return;

        float distancia = Vector2.Distance(
            transform.position,
            jogador.position
        );

        bool dentroDoRaio = distancia <= distanciaDeteccao;

        if (dentroDoRaio && !jogadorNoRaio)
        {
            jogadorNoRaio = true;

            if (rotinaChuva == null)
                rotinaChuva = StartCoroutine(IniciarChuva());
        }
        else if (!dentroDoRaio && jogadorNoRaio)
        {
            jogadorNoRaio = false;

            if (rotinaChuva != null)
            {
                StopCoroutine(rotinaChuva);
                rotinaChuva = null;
            }
        }
    }

    private IEnumerator IniciarChuva()
    {
        destinosUsados.Clear();

        for (int i = 0; i < quantidadeBolas; i++)
        {
            if (!jogadorNoRaio)
                yield break;

            float destinoX = EscolherDestinoX();

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

            if (hit.collider != null)
            {
                Vector2 impacto = hit.point +
                                  Vector2.up * alturaIndicador;

                GameObject indicador = Instantiate(
                    prefabIndicadorImpacto,
                    impacto,
                    Quaternion.identity
                );

                Vector2 origemBola = new Vector2(
                    destinoX,
                    hit.point.y + alturaQueda
                );

                StartCoroutine(
                    PrepararQueda(origemBola, indicador)
                );
            }
            else
            {
                Debug.LogWarning(
                    "Não foi encontrado chão para uma bola."
                );
            }

            float intervalo = Random.Range(
                intervaloMinimoEntreBolas,
                intervaloMaximoEntreBolas
            );

            yield return new WaitForSeconds(intervalo);
        }

        rotinaChuva = null;
    }
    
    private float EscolherDestinoX()
    {
        float limiteEsquerdo =
            transform.position.x - distanciaMaximaDisparo;

        float limiteDireito =
            transform.position.x + distanciaMaximaDisparo;

        float melhorCandidato = transform.position.x;
        float melhorDistancia = -1f;

        for (int tentativa = 0; tentativa < 100; tentativa++)
        {
            float candidato = Random.Range(
                limiteEsquerdo,
                limiteDireito
            );

            float menorDistancia = float.MaxValue;

            foreach (float destino in destinosUsados)
            {
                float diferenca = Mathf.Abs(candidato - destino);

                if (diferenca < menorDistancia)
                    menorDistancia = diferenca;
            }

            if (destinosUsados.Count == 0)
                menorDistancia = float.MaxValue;

            if (menorDistancia >= distanciaMinimaEntreQuedas)
            {
                destinosUsados.Add(candidato);
                return candidato;
            }

            if (menorDistancia > melhorDistancia)
            {
                melhorDistancia = menorDistancia;
                melhorCandidato = candidato;
            }
        }

        destinosUsados.Add(melhorCandidato);
        return melhorCandidato;
    }

    private void OnDrawGizmos()
    {
        // Limites horizontais da chuva
        float limiteEsquerdo =
            transform.position.x - distanciaMaximaDisparo;

        float limiteDireito =
            transform.position.x + distanciaMaximaDisparo;

        // Desenha a área de queda na Scene
        Gizmos.color = Color.red;

        Vector3 centro = new Vector3(
            transform.position.x,
            transform.position.y,
            0f
        );

        Vector3 tamanho = new Vector3(
            distanciaMaximaDisparo * 2f,
            1f,
            0.1f
        );

        Gizmos.DrawWireCube(centro, tamanho);

        // Desenha os limites verticais
        Gizmos.DrawLine(
            new Vector3(limiteEsquerdo, transform.position.y - 0.5f, 0f),
            new Vector3(limiteEsquerdo, transform.position.y + 0.5f, 0f)
        );

        Gizmos.DrawLine(
            new Vector3(limiteDireito, transform.position.y - 0.5f, 0f),
            new Vector3(limiteDireito, transform.position.y + 0.5f, 0f)
        );
        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(
            transform.position,
            distanciaDeteccao
        );
    }
    
    private IEnumerator PrepararQueda(Vector2 origemBola, GameObject indicador)
    {
        yield return new WaitForSeconds(tempoAviso);

        if (indicador != null)
            Destroy(indicador);

        GameObject bola = Instantiate(
            prefabBola,
            origemBola,
            Quaternion.identity
        );

        BolaQuedaDesafio scriptBola =
            bola.GetComponent<BolaQuedaDesafio>();

        if (scriptBola != null)
        {
            scriptBola.Inicializar(
                velocidadeQueda,
                danoBola,
                camadaGround
            );
        }
        else
        {
            Debug.LogError("O prefab precisa ter BolaQuedaDesafio!");
            Destroy(bola);
        }
    }

}
