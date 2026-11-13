using UnityEngine;

public class EnemyGosma : MonoBehaviour
{
    [Header("Movimento")]
    public float velocidade = 2f;

    [Header("Detecção de Parede/Borda")]
    public Transform checkParede;
    public Transform checkChao;
    public float raioCheck = 0.1f;
    public LayerMask groundLayer;

    [Header("Gosma")]
    public GameObject gosmaPrefab;
    public Transform pontoGosma;
    public float intervaloGosma = 3f;

    private Rigidbody2D rb;
    private bool indoDireita = false;
    private float tempoGosma;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        tempoGosma = intervaloGosma;
    }

    void Update()
    {
        bool bateuParede = Physics2D.OverlapCircle(
            checkParede.position, raioCheck, groundLayer);

        bool temChaoNaFrente = Physics2D.OverlapCircle(
            checkChao.position, raioCheck, groundLayer);

        if (bateuParede || !temChaoNaFrente)
            Virar();

        tempoGosma -= Time.deltaTime;

        if (tempoGosma <= 0)
        {
            SoltarGosma();
            tempoGosma = intervaloGosma;
        }
    }

    void FixedUpdate()
    {
        float direcao = indoDireita ? 1f : -1f;
        rb.linearVelocity = new Vector2(
            direcao * velocidade, rb.linearVelocity.y);
    }

    void Virar()
    {
        indoDireita = !indoDireita;

        Vector3 escala = transform.localScale;
        escala.x = Mathf.Abs(escala.x) * (indoDireita ? 1 : -1);

        transform.localScale = escala;
    }

    void SoltarGosma()
    {
        if (gosmaPrefab != null && pontoGosma != null)
            Instantiate(gosmaPrefab, pontoGosma.position, Quaternion.identity);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}