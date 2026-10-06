using UnityEngine;
using System.Collections;

public class PlataformaCai : MonoBehaviour
{
    [Header("Tempos")]
    public float tempoTremer = 1f;
    public float intensidadeTremor = 0.05f;
    public float tempoAteSumir = 1.5f; // quanto tempo caindo até desaparecer
    public float tempoReaparecer = 3f;

    private Vector3 posicaoInicial;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Collider2D col;
    private bool ativada = false;

    void Start()
    {
        posicaoInicial = transform.position;
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();

        rb.bodyType = RigidbodyType2D.Kinematic; // fixa até o player pisar
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (ativada) return;
        if (!collision.gameObject.CompareTag("Player")) return;

        bool pisouEmCima = false;

        foreach (ContactPoint2D contato in collision.contacts)
        {
            if (contato.normal.y < -0.5f)
            {
                pisouEmCima = true;
                break;
            }
        }

        if (!pisouEmCima) return;

        ativada = true;
        StartCoroutine(TremerCairEReaparecer());
    }

    IEnumerator TremerCairEReaparecer()
    {
        // Tremor
        float tempoDecorrido = 0f;
        while (tempoDecorrido < tempoTremer)
        {
            float offsetX = Random.Range(-intensidadeTremor, intensidadeTremor);
            transform.position = posicaoInicial + new Vector3(offsetX, 0, 0);
            tempoDecorrido += Time.deltaTime;
            yield return null;
        }
        transform.position = posicaoInicial;

        // Cai (física assume)
        rb.bodyType = RigidbodyType2D.Dynamic;

        yield return new WaitForSeconds(tempoAteSumir);

        // Some
        sr.enabled = false;
        col.enabled = false;

        yield return new WaitForSeconds(tempoReaparecer);

        // Reset
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.linearVelocity = Vector2.zero;
        transform.position = posicaoInicial;
        sr.enabled = true;
        col.enabled = true;
        ativada = false;
    }
}