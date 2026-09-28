using UnityEngine;

public class InimigoPontoCabeca : MonoBehaviour
{
    public float forcaPulo = 8f; // dá um pulinho no player ao pisar

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        
        Rigidbody2D playerRb = other.GetComponent<Rigidbody2D>();
        Debug.Log($"pé do player: {other.bounds.min.y} | centro da cabeça: {GetComponent<Collider2D>().bounds.center.y} | vel Y: {playerRb.linearVelocity.y}");
        if (playerRb == null) return;

        // Só conta como pisão se o player estiver caindo (ou parado) e os pés dele estiverem acima do centro da cabeça
        bool caindo = playerRb.linearVelocity.y <= 0.1f;
        bool acima = other.bounds.min.y >= GetComponent<Collider2D>().bounds.center.y;

        if (!caindo || !acima) return;

        playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, forcaPulo);
        GetComponentInParent<InimigoController>().MorrerPisado();
    }

}