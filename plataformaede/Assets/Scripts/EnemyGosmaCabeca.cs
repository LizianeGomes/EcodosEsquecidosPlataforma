using UnityEngine;

public class EnemyGosmaCabeca : MonoBehaviour
{
    [SerializeField] private EnemyGosma inimigo;

    private Collider2D cabeca;

    void Awake()
    {
        cabeca = GetComponent<Collider2D>();

        if (inimigo == null)
            inimigo = GetComponentInParent<EnemyGosma>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerMovement jogador = other.GetComponentInParent<PlayerMovement>();
        if (jogador == null) return;

        Rigidbody2D rbJogador = jogador.GetComponent<Rigidbody2D>();

        // pés do player acima do centro da cabeça e ele não está subindo
        bool pesAcima = other.bounds.min.y >= cabeca.bounds.center.y;
        bool caindo = rbJogador != null && rbJogador.linearVelocity.y <= 0.1f;

        if (pesAcima && caindo)
            inimigo.Pisado(rbJogador);
    }
}