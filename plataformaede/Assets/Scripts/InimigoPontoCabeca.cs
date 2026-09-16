using UnityEngine;

public class InimigoPontoCabeca : MonoBehaviour
{
    public float forcaPulo = 8f; // dá um pulinho no player ao pisar

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        Rigidbody2D playerRb = other.GetComponent<Rigidbody2D>();
        if (playerRb != null)
            playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, forcaPulo);

        GetComponentInParent<InimigoController>().MorrerPisado();
    }
}