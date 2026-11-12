using UnityEngine;
using System.Collections.Generic;

public class PlataformaMovel : MonoBehaviour
{
    [Header("Pontos")]
    public Transform pontoA;
    public Transform pontoB;

    [Header("Movimento")]
    public float velocidade = 2f;

    [Header("Passageiros")]
    [SerializeField] private float margemTopo = 0.15f; // tolerância pra considerar "em cima"

    private Rigidbody2D rb;
    private bool indoParaB = true;

    // detectados = quem tocou o topo no último passo da física
    private HashSet<Rigidbody2D> detectados = new HashSet<Rigidbody2D>();
    private List<Rigidbody2D> passageiros = new List<Rigidbody2D>();

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;

        rb.position = pontoA.position;
        indoParaB = true;
    }

    void FixedUpdate()
    {
        // passageiros = quem estava em cima no passo anterior
        passageiros.Clear();
        passageiros.AddRange(detectados);
        detectados.Clear();

        Vector2 alvo = indoParaB ? (Vector2)pontoB.position : (Vector2)pontoA.position;

        Vector2 posicaoAntes = rb.position;
        Vector2 novaPos = Vector2.MoveTowards(posicaoAntes, alvo, velocidade * Time.fixedDeltaTime);
        Vector2 delta = novaPos - posicaoAntes;

        rb.MovePosition(novaPos);

        // leva quem está em cima junto com a plataforma
        foreach (Rigidbody2D passageiro in passageiros)
        {
            if (passageiro != null)
                passageiro.position += delta;
        }

        if (Vector2.Distance(novaPos, alvo) < 0.05f)
            indoParaB = !indoParaB;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        PlayerMovement jogador = collision.collider.GetComponentInParent<PlayerMovement>();
        if (jogador == null) return;

        // os pés do jogador precisam estar acima do topo da plataforma
        float peDoJogador = collision.collider.bounds.min.y;
        float topoDaPlataforma = collision.otherCollider.bounds.max.y;

        if (peDoJogador >= topoDaPlataforma - margemTopo)
            detectados.Add(jogador.GetComponent<Rigidbody2D>());
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