using UnityEngine;

public class Projetil : MonoBehaviour
{
    public int dano = 1;
    public float tempoVida = 5f;

    void Start()
    {
        Destroy(gameObject, tempoVida);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerMovement player = other.GetComponent<PlayerMovement>();

        if (player != null)
        {
            player.TomarDano(dano);
            Destroy(gameObject);
        }
    }
}