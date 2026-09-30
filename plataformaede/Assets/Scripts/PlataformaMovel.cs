using UnityEngine;

public class PlataformaMovel : MonoBehaviour
{
    [Header("Pontos")]
    public Transform pontoA;
    public Transform pontoB;

    [Header("Movimento")]
    public float velocidade = 2f;

    private Vector3 destino;
    private Rigidbody2D rb;
    private Vector3 ultimaPosicao;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        transform.position = pontoA.position;
        destino = pontoB.position;
        ultimaPosicao = transform.position;
    }

    void FixedUpdate()
    {
        Vector3 novaPos = Vector3.MoveTowards(transform.position, destino, velocidade * Time.fixedDeltaTime);
        rb.MovePosition(novaPos);

        if (Vector3.Distance(transform.position, destino) < 0.05f)
        {
            destino = (destino == pontoA.position) ? pontoB.position : pontoA.position;
        }

        ultimaPosicao = transform.position;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        
        foreach (ContactPoint2D contato in collision.contacts)
        {
            if (contato.normal.y > 0.5f)
            {
                Vector3 delta = transform.position - ultimaPosicao;
                collision.transform.position += delta;
                break;
            }
        }
    }
    void OnDrawGizmos()
    {
        if (pontoA == null || pontoB == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(pontoA.position, pontoB.position);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(pontoA.position, 0.2f);
        Gizmos.DrawWireSphere(pontoB.position, 0.2f);
    }
}