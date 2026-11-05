using UnityEngine;
using TMPro;

[RequireComponent(typeof(Collider2D))]
public class TutorialTexto : MonoBehaviour
{
    [Header("Referência")]
    public TextMeshPro texto;

    [Header("Fade")]
    public float velocidadeFade = 2f;

    [Header("Flutuação suave (0 = desligado)")]
    public float amplitudeFlutuacao = 0.05f;
    public float velocidadeFlutuacao = 2f;

    private float alvo = 0f;
    private Vector3 posicaoBase;

    void Awake()
    {
        GetComponent<Collider2D>().isTrigger = true;

        if (texto == null)
            texto = GetComponentInChildren<TextMeshPro>();

        if (texto == null)
        {
            Debug.LogWarning("[TutorialTexto] Nenhum TextMeshPro encontrado em '" + gameObject.name + "'.");
            enabled = false;
            return;
        }

        posicaoBase = texto.transform.localPosition;
        texto.alpha = 0f;
    }

    void Update()
    {
        texto.alpha = Mathf.MoveTowards(texto.alpha, alvo, velocidadeFade * Time.deltaTime);

        if (amplitudeFlutuacao > 0f)
        {
            float onda = Mathf.Sin(Time.time * velocidadeFlutuacao) * amplitudeFlutuacao;
            texto.transform.localPosition = posicaoBase + Vector3.up * onda * texto.alpha;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            alvo = 1f;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            alvo = 0f;
    }
}