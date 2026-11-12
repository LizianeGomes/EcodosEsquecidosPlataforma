using UnityEngine;

public class PlataformaElevatoria : MonoBehaviour
{
    [Header("Pontos")]
    public Transform pontoInicial;
    public Transform pontoFinal;

    [Header("Movimento")]
    public float velocidadeSubida = 2f;
    public float velocidadeDescida = 3f;

    private Rigidbody2D rb;
    private bool jogadorNaArea = false;
    private Vector2 destino;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        rb.bodyType = RigidbodyType2D.Kinematic;

        rb.position = pontoInicial.position;
        destino = pontoInicial.position;
    }

    void FixedUpdate()
    {
        // Decide para onde a plataforma deve ir.
        if (jogadorNaArea)
        {
            destino = pontoFinal.position;
        }
        else
        {
            destino = pontoInicial.position;
        }

        // Escolhe a velocidade conforme a direção.
        float velocidade = jogadorNaArea
            ? velocidadeSubida
            : velocidadeDescida;

        // Move a plataforma até o destino.
        Vector2 novaPosicao = Vector2.MoveTowards(
            rb.position,
            destino,
            velocidade * Time.fixedDeltaTime
        );

        rb.MovePosition(novaPosicao);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            jogadorNaArea = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            jogadorNaArea = false;
        }
    }
}