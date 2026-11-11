using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class BolaCanhao : MonoBehaviour
{
    [Header("Configurações")]
    public int dano = 1;
    public float tempoDeVida = 5f;
    public float escalaGravidade = 1.5f;

    private Rigidbody2D rb;
    private bool lancada;

    private GameObject indicador;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = escalaGravidade;

        // as bombas da mesma salva não colidem entre si
        Collider2D meuCollider = GetComponent<Collider2D>();

        foreach (BolaCanhao outra in FindObjectsByType<BolaCanhao>(FindObjectsSortMode.None))
        {
            if (outra != this)
                Physics2D.IgnoreCollision(meuCollider, outra.GetComponent<Collider2D>());
        }
    }

    private void Start()
    {
        Destroy(gameObject, tempoDeVida);
    }

    public void ConfigurarAviso(
        GameObject prefabIndicador,
        Vector2 posicao,
        float tempoVoo)
    {
        if (prefabIndicador == null)
            return;

        indicador = Instantiate(
            prefabIndicador,
            posicao,
            Quaternion.identity
        );

        // Proteção caso a bomba não encontre o chão.
        Destroy(indicador, tempoVoo + 1f);
    }

    public void Lancar(Vector2 direcao, float velocidade)
    {
        lancada = true;
        rb.linearVelocity = direcao * velocidade;
    }

    private void OnCollisionEnter2D(Collision2D colisao)
    {
        if (!lancada)
            return;

        PlayerMovement jogador =
            colisao.collider.GetComponentInParent<PlayerMovement>();

        if (jogador != null)
        {
            jogador.TomarDano(dano);
        }

        DestruirIndicador();
        Destroy(gameObject);
    }

    private void DestruirIndicador()
    {
        if (indicador != null)
        {
            Destroy(indicador);
            indicador = null;
        }
    }

    private void OnDestroy()
    {
        DestruirIndicador();
    }
}