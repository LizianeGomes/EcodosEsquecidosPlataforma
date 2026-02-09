using UnityEngine;

public class Health : MonoBehaviour
{
    public float maxVida = 100f;
    public float vidaAtual;

    void Awake()
    {
        vidaAtual = maxVida;
    }

    public void TomarDano(float dano)
    {
        vidaAtual -= dano;

        if (vidaAtual <= 0)
        {
            Morrer();
        }
    }

    void Morrer()
    {
        Destroy(gameObject);
    }
}