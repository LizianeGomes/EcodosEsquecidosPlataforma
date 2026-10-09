using UnityEngine;
using System.Collections.Generic;

public class PlataformaMovel : MonoBehaviour
{
    [Header("Pontos")]
    public Transform pontoA;
    public Transform pontoB;

    [Header("Movimento")]
    public float velocidade = 2f;

    private Vector3 destino;
    private Rigidbody2D rb;
    private HashSet<Rigidbody2D> playersEmCima = new HashSet<Rigidbody2D>();

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        transform.position = pontoA.position;
        destino = pontoB.position;
    }

    void FixedUpdate()
    {
        Vector3 posicaoAntes = transform.position;

        Vector3 novaPos = Vector3.MoveTowards(transform.position, destino, velocidade * Time.fixedDeltaTime);
        rb.MovePosition(novaPos);

        Vector3 delta = novaPos - posicaoAntes;

        foreach (Rigidbody2D playerRb in playersEmCima)
        {
            if (playerRb != null)
            {
                playerRb.position += (Vector2)delta;
            }
        }

        if (Vector3.Distance(novaPos, destino) < 0.05f)
        {
            destino = (destino == (Vector3)pontoA.position) ? pontoB.position : pontoA.position;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("[PLAT] ENTER com: " + collision.gameObject.name + " | tag: " + collision.gameObject.tag);

        if (!collision.gameObject.CompareTag("Player")) return;

        foreach (ContactPoint2D contato in collision.contacts)
        {
            Debug.Log("[PLAT] normal.y = " + contato.normal.y);

            if (contato.normal.y < -0.5f)
            {
                if (collision.rigidbody != null)
                {
                    playersEmCima.Add(collision.rigidbody);
                    Debug.Log("[PLAT] ADICIONADO. Total na lista: " + playersEmCima.Count);
                }
                break;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        if (collision.rigidbody != null)
        {
            playersEmCima.Remove(collision.rigidbody);
            Debug.Log("[PLAT] REMOVIDO. Total na lista: " + playersEmCima.Count);
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