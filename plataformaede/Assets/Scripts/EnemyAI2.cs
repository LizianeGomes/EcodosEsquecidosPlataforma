using UnityEngine;

public class EnemyAI2 : MonoBehaviour
{
    [Header("Movimento")]
    public float velocidade = 2f;

    [Header("Detecção de Parede/Borda")]
    public Transform checkParede;
    public Transform checkChao;
    public float raioCheck = 0.1f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private bool indoDireita = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        bool bateuParede = Physics2D.OverlapCircle(checkParede.position, raioCheck, groundLayer);
        bool temChaoNaFrente = Physics2D.OverlapCircle(checkChao.position, raioCheck, groundLayer);

        // Vira se bater numa parede OU se não tiver chão na frente (evita cair de plataformas)
        if (bateuParede || !temChaoNaFrente)
        {
            Virar();
        }
    }

    void FixedUpdate()
    {
        float direcao = indoDireita ? 1f : -1f;
        rb.linearVelocity = new Vector2(direcao * velocidade, rb.linearVelocity.y);
    }

    void Virar()
    {
        Debug.Log("Virou! Frame: " + Time.frameCount);
        indoDireita = !indoDireita;
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }
}
