using UnityEngine;
using UnityEngine.Tilemaps;

public class PassagemSecreta : MonoBehaviour
{
    [Header("Fade")]
    [Range(0f, 1f)] public float alphaRevelado = 0.15f; // transparência quando o player está dentro
    public float velocidadeFade = 4f;                    // maior = transição mais rápida

    private SpriteRenderer sr;
    private Tilemap tilemap;

    private float alphaAtual = 1f;
    private float alphaAlvo = 1f;
    private int contatos = 0; // conta quantos colliders do player estão dentro

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        tilemap = GetComponent<Tilemap>();
    }

    void Update()
    {
        if (Mathf.Approximately(alphaAtual, alphaAlvo))
            return;

        alphaAtual = Mathf.MoveTowards(alphaAtual, alphaAlvo, velocidadeFade * Time.deltaTime);
        AplicarAlpha(alphaAtual);
    }

    void AplicarAlpha(float alpha)
    {
        if (tilemap != null)
        {
            Color c = tilemap.color;
            c.a = alpha;
            tilemap.color = c;
        }
        else if (sr != null)
        {
            Color c = sr.color;
            c.a = alpha;
            sr.color = c;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponentInParent<PlayerMovement>() == null)
            return;

        contatos++;
        alphaAlvo = alphaRevelado;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponentInParent<PlayerMovement>() == null)
            return;

        contatos = Mathf.Max(0, contatos - 1);

        if (contatos == 0)
            alphaAlvo = 1f;
    }
}