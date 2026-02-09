using UnityEngine;

public class Health : MonoBehaviour
{
    public float maxVida = 100f;
    private float vidaAtual;

    void Start()
    {
        vidaAtual = maxVida;
    }

    // Chamada quando o personagem sofre dano
    public void TomarDano(float quantidade)
    {
        vidaAtual -= quantidade;
        if (vidaAtual <= 0)
        {
            Morrer();
        }
    }

    // Função para matar o personagem
    private void Morrer()
    {
        // Aqui você pode colocar animação de morte
        Animator anim = GetComponent<Animator>();
        if (anim != null) anim.SetTrigger("morto");

        // Para movimento
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = Vector2.zero;

        // Desativa o GameObject após alguns segundos
        Destroy(gameObject, 1f);
    }

    public float GetVida()
    {
        return vidaAtual;
    }
}