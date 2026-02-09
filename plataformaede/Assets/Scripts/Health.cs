public class Health : MonoBehaviour
{
    public float maxVida = 100f;
    private float vidaAtual;

    void Start() { vidaAtual = maxVida; }

    public void TomarDano(float quantidade)
    {
        vidaAtual -= quantidade;
        if (vidaAtual <= 0)
        {
            // Player morreu
            Destroy(gameObject);
        }
    }
}

