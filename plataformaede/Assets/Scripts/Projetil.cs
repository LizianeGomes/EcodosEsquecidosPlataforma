using UnityEngine;

public class Projetil : MonoBehaviour
{
    public int dano = 1;
    public float tempoVida = 5f;

    private bool jaAtingiu = false;

    void Start()
    {
        Destroy(gameObject, tempoVida);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (jaAtingiu)
            return;

        PlayerMovement player =
            other.GetComponentInParent<PlayerMovement>();

        if (player != null)
        {
            jaAtingiu = true;

            player.TomarDano(dano);

            Destroy(gameObject);
        }
    }
}


